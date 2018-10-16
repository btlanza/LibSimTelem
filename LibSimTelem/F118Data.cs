/* Benjamin Lanza
 * F118Data.cs
 * October 10th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LibSimTelem.EventDefs;
using static LibSimTelem.F118Defs;

namespace LibSimTelem
{
    public class F118Data : TelemData
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

        /* Default constructor */
        public F118Data() { }

        /* Constructor to automatically subscribe to the passed F118Reader's event */
        public F118Data(ref F118Reader reader)
        {
            SubscribeToReader(reader);
        }

        /* Subscribes to the passed F118Reader's event */
        public bool SubscribeToReader(TelemReader reader)
        {
            if(reader is F118Reader) { ((F118Reader)reader).PacketReceivedEvent += OnPacketReceived; return true; }
            else { return false; }
        }

        /* Runs when triggered by an F118Reader object */
        private void OnPacketReceived(object sender, F118ReceivedEventArgs args)
        {
            PacketID id = args.id;
            int carIndex = 0;

            switch (id)
            {
                case PacketID.MOTION:

                    PacketMotionData motion = (PacketMotionData)args.packet;
                    carIndex = motion.m_header.m_playerCarIndex;

                    PositionEventArgs newPos = new PositionEventArgs();

                    newPos.x = motion.m_carMotionData[carIndex].m_worldPositionX;
                    newPos.y = motion.m_carMotionData[carIndex].m_worldPositionY;
                    newPos.z = motion.m_carMotionData[carIndex].m_worldPositionZ;

                    GForceEventArgs newGs = new GForceEventArgs();

                    newGs.lat = motion.m_carMotionData[carIndex].m_gForceLateral;
                    newGs.lon = motion.m_carMotionData[carIndex].m_gForceLongitudinal;

                    RaisePositionReceived(newPos);
                    RaiseGForceReceived(newGs);

                    break;

                case PacketID.TELEMETRY:

                    PacketCarTelemetryData telemetry = (PacketCarTelemetryData)args.packet;
                    carIndex = telemetry.m_header.m_playerCarIndex;

                    PowertrainEventArgs newPow = new PowertrainEventArgs();

                    sbyte gearByte = telemetry.m_carTelemetryData[carIndex].m_gear;
                    if(gearByte == -1) newPow.gear = 'R';
                    else if(gearByte == 0) newPow.gear = 'N';
                    else newPow.gear = (char)('0' + gearByte);

                    newPow.rpm = telemetry.m_carTelemetryData[carIndex].m_engineRPM;
                    newPow.kmh = telemetry.m_carTelemetryData[carIndex].m_speed;
                    newPow.mph = telemetry.m_carTelemetryData[carIndex].m_speed * 0.621371f;

                    AxesEventArgs newAxes = new AxesEventArgs();

                    newAxes.throttle = telemetry.m_carTelemetryData[carIndex].m_throttle / 100.0f;
                    newAxes.brake = telemetry.m_carTelemetryData[carIndex].m_brake / 100.0f;
                    newAxes.clutch = telemetry.m_carTelemetryData[carIndex].m_clutch / 100.0f;
                    newAxes.steering = telemetry.m_carTelemetryData[carIndex].m_steer / 100.0f;

                    RaisePowertrainReceived(newPow);
                    RaiseAxesReceived(newAxes);

                    break;
                
                case PacketID.STATUS:

                    PacketCarStatusData status = (PacketCarStatusData)args.packet;
                    dataMtx.WaitOne();
                    maxRpm = status.m_carStatusData[carIndex].m_maxRPM;
                    dataMtx.ReleaseMutex();

                    break;

                default:

                    return;
            }
        }

        /* Commonly used data accessors */
        /* Returns the last known maxRpm value */
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
