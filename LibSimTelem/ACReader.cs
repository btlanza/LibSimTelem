/* Benjamin Lanza
 * ACReader.cs
 * October 11th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using static LibSimTelem.ACDefs;
using System.Runtime.InteropServices;

namespace LibSimTelem
{
    public class ACReader : TelemReader
    {

        /* Event handler declarations */
        public delegate void PacketReceived(object sender, ACReceivedEventArgs e);
        public event PacketReceived PacketReceivedEvent;

        /* Fields */
        private int serverPort;
        private int clientPort;
        private string serverIP;
        private int timeout;

        private PacketID expectedID = PacketID.UPDATE;

        private volatile bool isRunning = false;
        private static Mutex flagMtx = new Mutex();
        private Thread receiverThread;

        /* Constructor */
        public ACReader(int serverPort = DEFAULT_SERVER_PORT, int clientPort = DEFAULT_CLIENT_PORT,
            string serverIP = DEFAULT_IP, int timeout = DEFAULT_TIMEOUT)
        {
            this.serverPort = serverPort;
            this.clientPort = clientPort;
            this.serverIP = serverIP;
            this.timeout = timeout;
        }

        /* Starts listening for data */
        public bool Start(params object[] args)
        {
            /* Returns false if Start() has previously been called */
            flagMtx.WaitOne();
            if (isRunning) { flagMtx.ReleaseMutex(); return false; }
            else { isRunning = true; flagMtx.ReleaseMutex(); }

            /* Accepts an optional SubscriptionID parameter (defaults to UPDATE) */
            if (args.Length > 0)
            {
                if (args[0] is SubscriptionType) this.expectedID = (PacketID)args[0];
            }

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
            Console.WriteLine("Test");

            /* Sets up UDP connection objects */
            UdpClient client = new UdpClient(clientPort);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
            client.Client.ReceiveTimeout = timeout;

            /* Sends required connection handshake and gets back static data */
            byte[] responseData = new byte[1];
            while (true)
            {
                flagMtx.WaitOne();
                if (!isRunning) { flagMtx.ReleaseMutex(); expectedID = (PacketID)(-1); break; }
                else { flagMtx.ReleaseMutex(); }

                client.Send(CONNECT_HANDSHAKE, HANDSHAKE_PACKET_SIZE, ep);
                try {
                    responseData = client.Receive(ref ep);
                    if (IsValidPacketData(ref responseData, PacketID.RESPONSE)) break;
                    else { Console.WriteLine("Received response data of unexpected size!"); continue; }
                }
                catch (SocketException e) { continue; }
            }

            /* Parse the data into a packet, wrap it inside an EventArgs object and raise the received event */
            ACReceivedEventArgs responseArgs = new ACReceivedEventArgs(ParsePacket<HandshakeResponse>(ref responseData), PacketID.RESPONSE);
            RaisePacketReceived(responseArgs);

            /* Loop to receive data depending on set subscription type */
            if(expectedID == PacketID.UPDATE)
            {

                /* Send a request to start updating */
                client.Send(UPDATE_HANDSHAKE, HANDSHAKE_PACKET_SIZE, ep);

                while (true)
                {
                    /* Break if isRunning is false */
                    flagMtx.WaitOne();
                    if (!isRunning) { flagMtx.ReleaseMutex(); break; }
                    else { flagMtx.ReleaseMutex(); }

                    byte[] data;

                    /* Try to get data from the socket and restart on timeout */
                    try { data = client.Receive(ref ep); }
                    catch (SocketException e) { continue; }

                    // Console.WriteLine(data.Length);

                    /* Restart the loop if the received data is not of the correct length */
                    if (!IsValidPacketData(ref data, PacketID.UPDATE)) continue;

                    /* Parse the data into a packet, wrap it inside an EventArgs object and raise the received event */
                    ACReceivedEventArgs updateArgs = new ACReceivedEventArgs(ParsePacket<RTCarInfo>(ref data), PacketID.UPDATE);
                    RaisePacketReceived(updateArgs);
                    // Console.WriteLine(Marshal.SizeOf(ParsePacket<RTCarInfo>(ref data)));

                }
            }
            else if(expectedID == PacketID.SPOT)
            {
                /* Send a request to receive spot updates */
                client.Send(SPOT_HANDSHAKE, HANDSHAKE_PACKET_SIZE, ep);

                while (true)
                {
                    /* Break if isRunning is false */
                    flagMtx.WaitOne();
                    if (!isRunning) { flagMtx.ReleaseMutex(); break; }
                    else { flagMtx.ReleaseMutex(); }

                    byte[] data;

                    /* Try to get data from the socket and restart on timeout */
                    try { data = client.Receive(ref ep); }
                    catch (SocketException e) { continue; }

                    /* Restart the loop of the received data is not of the correct length */
                    if (!IsValidPacketData(ref data, PacketID.SPOT)) continue;

                    /* Parse the data into a pakcet, wrap it inside an EventArgs object and raise the received event */
                    ACReceivedEventArgs updateArgs = new ACReceivedEventArgs(ParsePacket<RTCarInfo>(ref data), PacketID.SPOT);
                    RaisePacketReceived(updateArgs);

                }
            }

            /* Send a request to stop updates and close the client */
            client.Send(DISMISS_HANDSHAKE, HANDSHAKE_PACKET_SIZE, ep);
            client.Close();

        }

        /* Raises a PacketReceivedEvent if someone has subscribed to it */
        private void RaisePacketReceived(ACReceivedEventArgs args)
        {
            PacketReceivedEvent?.Invoke(this, args);
        }

        /* Returns true if the passed byte array is the same size as the expected packet */
        private bool IsValidPacketData(ref byte[] bytes, PacketID id)
        {   
            return (bytes.Length == PACKET_SIZES[id]);
        }

        /* Parses a packet from the given bytes */
        private T ParsePacket<T>(ref byte[] bytes)
        {
            T ret;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try { ret = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject()); }
            finally { handle.Free(); }

            return ret;
        }

    }

    /* ACReceivedEventArgs class definition */
    public class ACReceivedEventArgs : EventArgs
    {
        public object packet;
        public PacketID id;

        public ACReceivedEventArgs(object packet, PacketID id)
        {
            this.packet = packet;
            this.id = id;
        }
    }

}
