/* Benjamin Lanza
 * ACDefs.cs
 * October 11th, 2018 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LibSimTelem
{
    public class ACDefs
    {

        /* Connection constants */
        public const int DEFAULT_SERVER_PORT = 9996;
        public const int DEFAULT_CLIENT_PORT = 25565;
        public const string DEFAULT_IP = "127.0.0.1";
        public const int DEFAULT_TIMEOUT = 1000;
        public const string GAME_PROCESS_NAME = "acs";
        public const string LAUNCHER_PROCESS_NAME = "AssettoCorsa";
        public const string CM_PROCESS_NAME = "Content Manager";

        /* Packet size constants */
        public const int HANDSHAKE_PACKET_SIZE = 12;
        public const int RESPONSE_PACKET_SIZE = 408;
        public const int SPOT_PACKET_SIZE = 212;
        public const int UPDATE_PACKET_SIZE = 328;

        /* Enums */
        public enum PacketID
        {
            RESPONSE = 0,
            UPDATE = 1,
            SPOT = 2
        }
        
        public enum SubscriptionType
        {
            UPDATE = 1,
            SPOT = 2
        }

        /* Helper collections */
        public static readonly Dictionary<PacketID, int> PACKET_SIZES = new Dictionary<PacketID, int>
        {
            {PacketID.RESPONSE, RESPONSE_PACKET_SIZE},
            {PacketID.UPDATE, UPDATE_PACKET_SIZE},
            {PacketID.SPOT, SPOT_PACKET_SIZE}
        };

        /* Handshake packet data */
        public static readonly byte[] CONNECT_HANDSHAKE = new byte[]
        {
            0x01, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00
        };

        public static readonly byte[] DISMISS_HANDSHAKE = new byte[]
        {
            0x01, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00
        };

        public static readonly byte[] UPDATE_HANDSHAKE = new byte[]
        {
            0x01, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00
        };

        public static readonly byte[] SPOT_HANDSHAKE = new byte[]
        {
            0x01, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00
        };

        /* Packet structures */
        /* Response packet */
        [StructLayout(LayoutKind.Sequential, Size = RESPONSE_PACKET_SIZE, Pack = 4, CharSet = CharSet.Unicode)]
        public struct HandshakeResponse
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string carName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string driverName;
            public int identifier;
            public int version;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string trackName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string trackConfig;
        }

        /* Spot packet */
        [StructLayout(LayoutKind.Sequential, Size = SPOT_PACKET_SIZE, Pack = 4, CharSet = CharSet.Unicode)]
        public struct RTLap
        {
            public int carIdentificationNumber;
            public int lap;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string driverName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string carName;
            public int time;
        }

        /* Update packet */
        [StructLayout(LayoutKind.Sequential, Size = UPDATE_PACKET_SIZE, Pack = 4, CharSet = CharSet.Unicode)]
        public struct RTCarInfo
        {
            public char identifier;
            public int size;
            public float speed_Kmh;
            public float speed_Mph;
            public float speed_Ms;
            public byte isAbsEnabled;
            public byte isAbsInAction;
            public byte isTcInAction;
            public byte isTcEnabled;
            public byte isInPit;
            public byte isEngineLimiterOn;
            public float accG_vertical;
            public float accG_horizontal;
            public float accG_frontal;
            public int lapTime;
            public int lastLap;
            public int bestLap;
            public int lapCount;
            public float gas;
            public float brake;
            public float clutch;
            public float engineRPM;
            public float steer;
            public int gear;
            public float cgHeight;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] wheelAngularSpeed;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] slipAngle;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] slipAngle_ContactPatch;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] slipRatio;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] tyreSlip;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] ndSlip;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] load;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] Dy;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] Mz;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] tyreDirtyLevel;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] camberRAD;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] tyreRadius;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] tyreLoadedRadius;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] suspensionHeight;
            public float carPositionNormalized;
            public float carSlope;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] carCoordinates;
        }

    }
}
