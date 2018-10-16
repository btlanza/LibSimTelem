/* Benjamin Lanza
 * TelemReader.cs
 * October 11th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSimTelem
{
    public interface TelemReader
    {
        bool Start(params object[] args);
        bool Stop();
    }
}
