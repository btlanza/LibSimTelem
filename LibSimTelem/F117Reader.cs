/* Benjamin Lanza
 * F117Reader.cs
 * October 11th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using static LibSimTelem.F117Defs;
using System.Runtime.InteropServices;

namespace LibSimTelem
{
    public class F117Reader : TelemReader
    {

        /* Event handler declarations */
        public delegate void PacketReceived(object sender, F117ReceivedEventArgs args);
        public event PacketReceived PacketReceivedEvent;

        /* Fields */
        private int port;
        private string ip;
        private int timeout;

        private volatile bool isRunning = false;
        private static Mutex flagMtx = new Mutex();
        private Thread receiverThread;

        /* Constructor */
        public F117Reader(int port = DEFAULT_PORT, string ip = DEFAULT_IP,
            int timeout = DEFAULT_TIMEOUT)
        {
            this.port = port;
            this.ip = ip;
            this.timeout = timeout;
        }

        /* Starts listening for data */
        public bool Start(params object[] args)
        {
            /* Returns false if Start() has previously been called */
            flagMtx.WaitOne();
            if (isRunning) { flagMtx.ReleaseMutex(); return false; }
            else { isRunning = true; flagMtx.ReleaseMutex(); }

            /* Starts the receiving thread and returns true */
            receiverThread = new Thread(ThreadRoutine);
            receiverThread.Start();

            return true;
        }

        /* Signals the receiver thread to stop and cleans up */
        public bool Stop()
        {
            /* Returns false if the program isn't already running */
            flagMtx.WaitOne();
            if (!isRunning) { flagMtx.ReleaseMutex(); return false; }
            else { isRunning = false; flagMtx.ReleaseMutex(); }

            /* Waits for the receiver thread to join and returns true */
            receiverThread.Join();

            return true;
        }

        /* Receives packets and raises events continuously on a thread */
        private void ThreadRoutine()
        {
            /* Sets up UDP connection objects */
            UdpClient client = new UdpClient(port);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            client.Client.ReceiveTimeout = timeout;

            /* Loops until broken by the isRunning flag */
            while (true)
            {
                /* Break if isRunning is false */
                flagMtx.WaitOne();
                if(!isRunning) { flagMtx.ReleaseMutex(); break; }
                else { flagMtx.ReleaseMutex(); }

                byte[] data;

                /* Try to get data from the socket and restart the loop on timeout */
                try { data = client.Receive(ref ep); }
                catch (SocketException e) { continue; }

                /* Restart the loop if the packet is invalid */
                if(!IsValidPacketData(ref data)) continue;

                /* Create arguments object for triggering event */
                F117ReceivedEventArgs args = new F117ReceivedEventArgs(ParsePacket(ref data));

                /* Raise event */
                RaisePacketReceived(args);

            }

            client.Close();

        }

        /* Raises a PacketReceivedEvent if someone has subscribed to it */
        private void RaisePacketReceived(F117ReceivedEventArgs args)
        {
            PacketReceivedEvent?.Invoke(this, args);
        }

        /* Returns true if the passed byte array is the same size as a UDPPacket */
        private bool IsValidPacketData(ref byte[] bytes)
        {
            return (bytes.Length == PACKET_SIZE);
        }

        /* Parses a packet from the given bytes */
        private UDPPacket ParsePacket(ref byte[] bytes)
        {
            UDPPacket ret;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try { ret = Marshal.PtrToStructure<UDPPacket>(handle.AddrOfPinnedObject()); }
            finally { handle.Free(); }

            return ret;
        }

    }

    /* F117ReceivedEventArgs class definition */
    public class F117ReceivedEventArgs : EventArgs
    {
        public UDPPacket packet;

        public F117ReceivedEventArgs(UDPPacket packet)
        {
            this.packet = packet;
        }
    }
}
