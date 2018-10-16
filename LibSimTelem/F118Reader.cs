/* Benjamin Lanza
 * F118Reader.cs
 * October 10th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using static LibSimTelem.F118Defs;
using System.Runtime.InteropServices;

namespace LibSimTelem
{
    /* F118Reader class definition */
    public class F118Reader : TelemReader
    {

        /* Event handler declarations */
        public delegate void PacketReceived(object sender, F118ReceivedEventArgs args);
        public event PacketReceived PacketReceivedEvent;

        /* Fields */
        private int port;
        private string ip;
        private int timeout;

        private volatile bool isRunning = false;
        private static Mutex flagMtx = new Mutex();
        private Thread receiverThread;

        /* Constructor */
        public F118Reader(int port = DEFAULT_PORT, string ip = DEFAULT_IP,
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

            /* Waits for the reciever thread to join and returns true */
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
                if (!isRunning) { flagMtx.ReleaseMutex(); break; }
                else { flagMtx.ReleaseMutex(); }

                byte[] data;
 
                /* Try to get data from the socket and restart the loop on timeout */
                try { data = client.Receive(ref ep); }
                catch(SocketException e) { continue; }

                /* Get the packet type id */
                int idInt = GetValidPacketID(ref data);
                if(idInt == -1) continue;
                PacketID id = (PacketID)idInt;

                /* Create arguments object for triggering event */
                F118ReceivedEventArgs args;

                /* Parse identified packet and construct PacketReceivedEventArgs object */
                switch (id)
                {
                    case PacketID.MOTION:
                        args = new F118ReceivedEventArgs(ParsePacket<PacketMotionData>(ref data), id);
                        break;
                    case PacketID.SESSION:
                        args = new F118ReceivedEventArgs(ParsePacket<PacketSessionData>(ref data), id);
                        break;
                    case PacketID.LAPDATA:
                        args = new F118ReceivedEventArgs(ParsePacket<PacketLapData>(ref data), id);
                        break;
                    case PacketID.EVENT:
                        args = new F118ReceivedEventArgs(ParsePacket<PacketEventData>(ref data), id);
                        break;
                    case PacketID.PARTICIPANTS:
                        args = new F118ReceivedEventArgs(ParsePacket<PacketParticipantsData>(ref data), id);
                        break;
                    case PacketID.SETUPS:
                        args = new F118ReceivedEventArgs(ParsePacket<PacketCarSetupData>(ref data), id);
                        break;
                    case PacketID.TELEMETRY:
                        args = new F118ReceivedEventArgs(ParsePacket<PacketCarTelemetryData>(ref data), id);
                        break;
                    case PacketID.STATUS:
                        args = new F118ReceivedEventArgs(ParsePacket<PacketCarStatusData>(ref data), id);
                        break;
                    default:
                        continue;
                }

                /* Raise event */
                RaisePacketReceived(args);

            }

            client.Close();

        }

        /* Raises a PacketReceivedEvent if someone has subscibed to it */
        private void RaisePacketReceived(F118ReceivedEventArgs args)
        {
            PacketReceivedEvent?.Invoke(this, args);
        }

        /* Gets the integer corresponding to the packet id from the passed bytes, or -1 if invalid */
        private int GetValidPacketID(ref byte[] bytes)
        {
            int len = bytes.Length;
            int id = -1;

            /* Checks the identifier byte if the array is long enough */
            if(len > PACKET_ID_OFFSET) id = (int)bytes[PACKET_ID_OFFSET];
            else { return -1; }

            /* Checks if the identifier is valid */
            if(!IsValidPacketID(id)) return -1;

            /* Makes sure the packet is the expected size */
            if(len != PACKET_SIZES[(PacketID)id]) return -1;

            return id;
        }

        /* Parses a packet of the given generic type and returns it */
        private T ParsePacket<T>(ref byte[] bytes)
        {
            T ret;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try { ret = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject()); }
            finally { handle.Free(); }

            return ret;
        }

    }

    /* F118ReceivedEventArgs class definition */
    public class F118ReceivedEventArgs : EventArgs
    {
        public object packet;
        public PacketID id;

        public F118ReceivedEventArgs(object packet, PacketID id)
        {
            this.packet = packet;
            this.id = id;
        }
    }
}
