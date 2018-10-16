/* Benjamin Lanza
 * EventDefs.cs
 * October 10th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSimTelem
{
    public class EventDefs
    {
        /* EventArgs class definitions */
        public class PowertrainEventArgs : EventArgs
        {
            public float mph;
            public float kmh;
            public int rpm;
            public char gear;

            public PowertrainEventArgs() { }

            public PowertrainEventArgs(float mph, float kmh, int rpm, char gear)
            {
                this.mph = mph;
                this.kmh = kmh;
                this.rpm = rpm;
                this.gear = gear;
            }
        }

        public class GForceEventArgs : EventArgs
        {
            public float lat;
            public float lon;

            public GForceEventArgs() { }

            public GForceEventArgs(float lat, float lon)
            {
                this.lat = lat;
                this.lon = lon;
            }
        }

        public class PositionEventArgs : EventArgs
        {
            public float x;
            public float y;
            public float z;

            public PositionEventArgs() { }

            public PositionEventArgs(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }

        public class AxesEventArgs : EventArgs
        {
            public float throttle;
            public float brake;
            public float clutch;
            public float steering;

            public AxesEventArgs() { }

            public AxesEventArgs(float throttle, float brake,
                float clutch, float steering)
            {
                this.throttle = throttle;
                this.brake = brake;
                this.clutch = clutch;
                this.steering = steering;
            }
        }

        /* Delegate declarations */
        public delegate void PowertrainReceived(object sender, PowertrainEventArgs args);
        public delegate void GForceReceived(object sender, GForceEventArgs args);
        public delegate void PositionReceived(object sender, PositionEventArgs args);
        public delegate void AxesReceived(object sender, AxesEventArgs args);
    }
}
