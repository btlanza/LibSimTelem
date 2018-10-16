/* Benjamin Lanza
 * F117Data.cs
 * October 11th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LibSimTelem.EventDefs;
using static LibSimTelem.F117Defs;

namespace LibSimTelem
{
    public class F117Data : TelemData
    {
        /* Event declarations */
        public event PowertrainReceived PowertrainReceivedEvent;
        public event GForceReceived GForceReceivedEvent;
        public event PositionReceived PositionReceivedEvent;
        public event AxesReceived AxesReceivedEvent;

        /* Fields */
        private static Mutex dataMtx = new Mutex();

        /* Commonly used data fields */
        private int maxRpm = 1;

        /* Default contructor */
        public F117Data() { }

        /* Constructor to automatically subscibe to the passed F117Reader's event */
        public F117Data(ref F117Reader reader)
        {
            SubscribeToReader(reader);
        }

        /* Subscribes to the passed F117Reader's event */
        public bool SubscribeToReader(TelemReader reader)
        {
            if(reader is F117Reader) { ((F117Reader)reader).PacketReceivedEvent += OnPacketReceived; return true; }
            else { return false; }
        }

        /* Runs when triggered by an F117Reader object */
        private void OnPacketReceived(object sender, F117ReceivedEventArgs args)
        {
            UDPPacket packet = args.packet;

            dataMtx.WaitOne();
            maxRpm = (int)packet.m_max_rpm;
            dataMtx.ReleaseMutex();

            PositionEventArgs newPos = new PositionEventArgs();
            newPos.x = packet.m_x;
            newPos.y = packet.m_y;
            newPos.z = packet.m_z;

            GForceEventArgs newGs = new GForceEventArgs();
            newGs.lat = packet.m_gforce_lat;
            newGs.lon = packet.m_gforce_lon;

            PowertrainEventArgs newPow = new PowertrainEventArgs();
            newPow.rpm = (int)packet.m_engineRate;
            newPow.kmh = packet.m_speed * 3.6f;
            newPow.mph = packet.m_speed * 2.23694f;

            AxesEventArgs newAxes = new AxesEventArgs();
            newAxes.throttle = packet.m_throttle;
            newAxes.brake = packet.m_brake;
            newAxes.clutch = packet.m_clutch;
            newAxes.steering = packet.m_steer * -1.0f;

            RaisePositionReceived(newPos);
            RaiseGForceReceived(newGs);
            RaisePowertrainReceived(newPow);
            RaiseAxesReceived(newAxes);
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
