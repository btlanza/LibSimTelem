/* Benjamin Lanza
 * ACData.cs
 * October 11th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static LibSimTelem.EventDefs;
using static LibSimTelem.ACDefs;

namespace LibSimTelem
{
    public class ACData : TelemData
    {
        /* Event declarations */
        public event PowertrainReceived PowertrainReceivedEvent;
        public event GForceReceived GForceReceivedEvent;
        public event PositionReceived PositionReceivedEvent;
        public event AxesReceived AxesReceivedEvent;

        /* Fields */
        private HandshakeResponse responsePacket = new HandshakeResponse();
        private bool responsePacketAvailable = false;
        private static Mutex responseMtx = new Mutex();
        private static Mutex dataMtx = new Mutex();

        /* Commonly used data fields */
        private int maxRpm = 1;

        /* Default constructor */
        public ACData() { }

        /* Constructor to automatically subscibe to the passed ACReader's event */
        public ACData(ref ACReader reader)
        {
            SubscribeToReader(reader);
        }

        /* Subscribes to the passed ACReader's event */
        public bool SubscribeToReader(TelemReader reader)
        {
            if(reader is ACReader) { ((ACReader)reader).PacketReceivedEvent += OnPacketReceived; return true; }
            else { return false; }
        }

        /* Runs when triggered by an ACReader object */
        private void OnPacketReceived(object sender, ACReceivedEventArgs args)
        {
            PacketID id = args.id;

            if(id == PacketID.RESPONSE)
            {

                responseMtx.WaitOne();
                responsePacket = (HandshakeResponse)args.packet;
                responseMtx.ReleaseMutex();

                dataMtx.WaitOne();
                maxRpm = 0;
                dataMtx.ReleaseMutex();

            }else if(id == PacketID.UPDATE)
            {
                RTCarInfo packet = (RTCarInfo)args.packet;

                dataMtx.WaitOne();
                if(packet.engineRPM > maxRpm) maxRpm = (int)packet.engineRPM;
                dataMtx.ReleaseMutex();

                PositionEventArgs newPos = new PositionEventArgs();
                newPos.x = packet.carCoordinates[0];
                newPos.y = packet.carCoordinates[1];
                newPos.z = packet.carCoordinates[2];

                GForceEventArgs newGs = new GForceEventArgs();
                newGs.lat = packet.accG_horizontal;
                newGs.lon = packet.accG_frontal;

                PowertrainEventArgs newPow = new PowertrainEventArgs();
                newPow.rpm = (int)packet.engineRPM;
                newPow.kmh = packet.speed_Kmh;
                newPow.mph = packet.speed_Mph;

                AxesEventArgs newAxes = new AxesEventArgs();
                newAxes.throttle = packet.gas;
                newAxes.brake = packet.brake;
                newAxes.clutch = 1.0f - packet.clutch;
                newAxes.steering = packet.steer;

                RaisePositionReceived(newPos);
                RaiseGForceReceived(newGs);
                RaisePowertrainReceived(newPow);
                RaiseAxesReceived(newAxes);
            }
        }

        /* Checks if the response packet has been set by OnPacketReceived */
        public bool IsResponseAvailable()
        {
            bool ret;
            responseMtx.WaitOne();
            ret = responsePacketAvailable;
            responseMtx.ReleaseMutex();
            return ret;
        }

        /* Returns the response packet */
        public HandshakeResponse GetResponse()
        {
            HandshakeResponse ret;
            responseMtx.WaitOne();
            ret = responsePacket;
            responseMtx.ReleaseMutex();
            return ret;
        }

        /* Commonly used data accessors */
        /* Returns the greatest known rpm value */
        public int GetMaxRPM()
        {
            dataMtx.WaitOne();
            int temp = maxRpm;
            dataMtx.ReleaseMutex();
            return temp;
        }

        /* Event raisers */
        private void RaisePowertrainReceived(PowertrainEventArgs args)
        {
            PowertrainReceivedEvent?.Invoke(this, args);
        }

        private void RaiseGForceReceived(GForceEventArgs args)
        {
            GForceReceivedEvent?.Invoke(this, args);
        }

        private void RaisePositionReceived(PositionEventArgs args)
        {
            PositionReceivedEvent?.Invoke(this, args);
        }

        private void RaiseAxesReceived(AxesEventArgs args)
        {
            AxesReceivedEvent?.Invoke(this, args);
        }

    }
}
