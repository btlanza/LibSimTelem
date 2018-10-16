/* Benjamin Lanza
 * TelemData.cs
 * October 11th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LibSimTelem.EventDefs;

namespace LibSimTelem
{
    public interface TelemData
    {
        event PowertrainReceived PowertrainReceivedEvent;
        event GForceReceived GForceReceivedEvent;
        event PositionReceived PositionReceivedEvent;
        event AxesReceived AxesReceivedEvent;

        bool SubscribeToReader(TelemReader reader);

        int GetMaxRPM();
    }
}
