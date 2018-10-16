/* Benjamin Lanza
 * F118Defs.cs
 * October 10th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibSimTelem
{
    public class F118Defs
    {

        /* Connection constants */
        public const int DEFAULT_PORT = 20777;
        public const string DEFAULT_IP = "127.0.0.1";
        public const int DEFAULT_TIMEOUT = 1000;
        public const int TIMEOUT_CODE = 10060;
        public const string GAME_PROCESS_NAME = "F1_2018";

        /* Data block size constants */
        public const int HEADER_DATA_SIZE = 21;
        public const int MOTION_DATA_SIZE = 60;
        public const int MARSHAL_ZONE_DATA_SIZE = 5;
        public const int LAP_DATA_SIZE = 41;
        public const int PARTICIPANT_DATA_SIZE = 53;
        public const int SETUP_DATA_SIZE = 41;
        public const int TELEMETRY_DATA_SIZE = 53;
        public const int STATUS_DATA_SIZE = 52;

        /* Packet size constants */
        public const int MOTION_PACKET_SIZE = 1341;
        public const int SESSION_PACKET_SIZE = 147;
        public const int LAP_PACKET_SIZE = 841;
        public const int EVENT_PACKET_SIZE = 25;
        public const int PARTICIPANT_PACKET_SIZE = 1082;
        public const int SETUP_PACKET_SIZE = 841;
        public const int TELEMETRY_PACKET_SIZE = 1085;
        public const int STATUS_PACKET_SIZE = 1061;

        /* Enums */
        public enum PacketID : int
        {
            MOTION = 0,
            SESSION = 1,
            LAPDATA = 2,
            EVENT = 3,
            PARTICIPANTS = 4,
            SETUPS = 5,
            TELEMETRY = 6,
            STATUS = 7
        }

        /* Other constants and static readonly fields */
        public const int PACKET_ID_OFFSET = 3;
        public static readonly int MIN_PACKET_ID = (int)Enum.GetValues(typeof(PacketID)).GetLowerBound(0);
        public static readonly int MAX_PACKET_ID = (int)Enum.GetValues(typeof(PacketID)).GetUpperBound(0);

        /* Helper collections */
        public static readonly Dictionary<PacketID, int> PACKET_SIZES = new Dictionary<PacketID, int>
        {
            {PacketID.MOTION, MOTION_PACKET_SIZE},
            {PacketID.SESSION, SESSION_PACKET_SIZE},
            {PacketID.LAPDATA, LAP_PACKET_SIZE},
            {PacketID.EVENT, EVENT_PACKET_SIZE},
            {PacketID.PARTICIPANTS, PARTICIPANT_PACKET_SIZE},
            {PacketID.SETUPS, SETUP_PACKET_SIZE},
            {PacketID.TELEMETRY, TELEMETRY_PACKET_SIZE},
            {PacketID.STATUS, STATUS_PACKET_SIZE}
        };

        /* Data block structures */
        [StructLayout(LayoutKind.Sequential, Size = HEADER_DATA_SIZE, Pack = 1)]
        public struct PacketHeader
        {
            public ushort m_packetFormat;
            public byte m_packetVersion;
            public byte m_packetId;
            public ulong m_sessionUID;
            public float m_sessionTime;
            public uint m_frameIdentifier;
            public byte m_playerCarIndex;
        }

        [StructLayout(LayoutKind.Sequential, Size = MOTION_DATA_SIZE, Pack = 1)]
        public struct CarMotionData
        {
            public float m_worldPositionX;
            public float m_worldPositionY;
            public float m_worldPositionZ;
            public float m_worldVelocityX;
            public float m_worldVelocityY;
            public float m_worldVelocityZ;
            public short m_worldForwardDirX;
            public short m_worldForwardDirY;
            public short m_worldForwardDirZ;
            public short m_worldRightDirX;
            public short m_worldRightDirY;
            public short m_worldRightDirZ;
            public float m_gForceLateral;
            public float m_gForceLongitudinal;
            public float m_gForceVertical;
            public float m_yaw;
            public float m_pitch;
            public float m_roll;
        }

        [StructLayout(LayoutKind.Sequential, Size = MARSHAL_ZONE_DATA_SIZE, Pack = 1)]
        public struct MarshalZone
        {
            public float m_zoneStart;
            public sbyte m_zoneFlag;
        }

        [StructLayout(LayoutKind.Sequential, Size = LAP_DATA_SIZE, Pack = 1)]
        public struct LapData
        {
            public float m_lastLapTime;
            public float m_currentLapTime;
            public float m_bestLapTime;
            public float m_sector1Time;
            public float m_sector2Time;
            public float m_lapDistance;
            public float m_totalDistance;
            public float m_safetyCarDelta;
            public byte m_carPosition;
            public byte m_currentLapNum;
            public byte m_pitStatus;
            public byte m_sector;
            public byte m_currentLapInvalid;
            public byte m_penalties;
            public byte m_gridPosition;
            public byte m_driverStatus;
            public byte m_resultStatus;
        }

        [StructLayout(LayoutKind.Sequential, Size = PARTICIPANT_DATA_SIZE, Pack = 1)]
        public struct ParticipantData
        {
            public byte m_aiControlled;
            public byte m_driverId;
            public byte m_teamId;
            public byte m_raceNumber;
            public byte m_nationality;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] m_name;
        }

        [StructLayout(LayoutKind.Sequential, Size = SETUP_DATA_SIZE, Pack = 1)]
        public struct CarSetupData
        {
            public byte m_frontWing;
            public byte m_rearWing;
            public byte m_onThrottle;
            public byte m_offThrottle;
            public float m_frontCamber;
            public float m_rearCamber;
            public float m_frontToe;
            public float m_rearToe;
            public byte m_frontSuspension;
            public byte m_rearSuspension;
            public byte m_frontAntiRollBar;
            public byte m_rearAntiRollBar;
            public byte m_frontSuspensionHeight;
            public byte m_rearSuspensionHeight;
            public byte m_brakePressure;
            public byte m_brakeBias;
            public float m_frontTyrePressure;
            public float m_rearTyrePressure;
            public byte m_ballast;
            public float m_fuelLoad;
        }

        [StructLayout(LayoutKind.Sequential, Size = TELEMETRY_DATA_SIZE, Pack = 1)]
        public struct CarTelemetryData
        {
            public ushort m_speed;
            public byte m_throttle;
            public sbyte m_steer;
            public byte m_brake;
            public byte m_clutch;
            public sbyte m_gear;
            public ushort m_engineRPM;
            public byte m_drs;
            public byte m_revLightsPercent;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] m_brakesTemperature;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] m_tyresSurfaceTemperature;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] m_tyresInnerTemperature;
            public ushort m_engineTemperature;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_tyresPressure;
        }

        [StructLayout(LayoutKind.Sequential, Size = STATUS_DATA_SIZE, Pack = 1)]
        public struct CarStatusData
        {
            public byte m_tractionControl;
            public byte m_antiLockBrakes;
            public byte m_fuelMix;
            public byte m_frontBrakeBias;
            public byte m_pitLimiterStatus;
            public float m_fuelInTank;
            public float m_fuelCapacity;
            public ushort m_maxRPM;
            public ushort m_idleRPM;
            public byte m_maxGears;
            public byte m_drsAllowed;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresWear;
            public byte m_tyreCompound;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresDamage;
            public byte m_frontLeftWingDamage;
            public byte m_frontRightWingDamage;
            public byte m_rearWingDamage;
            public byte m_engineDamage;
            public byte m_gearBoxDamage;
            public byte m_exhaustDamage;
            public sbyte m_vehicleFiaFlags;
            public float m_ersStoreEnergy;
            public byte m_ersDeployMode;
            public float m_ersHarvestedThisLapMGUK;
            public float m_eraHarvestedThisLapMGUH;
            public float m_ersDeployedThisLap;
        }

        /* Packet structures */
        /* Sent at the frequency defined in the telemetry settings menu */
        [StructLayout(LayoutKind.Sequential, Size = MOTION_PACKET_SIZE, Pack = 1)]
        public struct PacketMotionData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public PacketHeader m_header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public CarMotionData[] m_carMotionData;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionPosition;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionVelocity;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionAcceleration;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSpeed;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSlip;
            public float m_localVelocityX;
            public float m_localVelocityY;
            public float m_localVelocityZ;
            public float m_angularVelocityX;
            public float m_angularVelocityY;
            public float m_angularVelocityZ;
            public float m_angularAccelerationX;
            public float m_angularAccelerationY;
            public float m_angularAccelerationZ;
            public float m_frontWheelsAngle;
        }

        /* Sent twice per second */
        [StructLayout(LayoutKind.Sequential, Size = SESSION_PACKET_SIZE, Pack = 1)]
        public struct PacketSessionData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public PacketHeader m_header;
            public byte m_weather;
            public sbyte m_trackTemperature;
            public sbyte m_airTemperature;
            public byte m_totalLaps;
            public ushort m_trackLength;
            public byte m_sessionType;
            public sbyte m_trackId;
            public byte m_era;
            public ushort m_sessionTimeLeft;
            public ushort m_sessionDuration;
            public byte m_pitSpeedLimit;
            public byte m_gamePaused;
            public byte m_isSpectating;
            public byte m_spectatorCarIndex;
            public byte m_sliProNativeSupport;
            public byte m_numMarshalZones;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            public MarshalZone[] m_marshalZones;
            public byte m_safetyCarStatus;
            public byte m_networkGame;
        }

        /* Sent at the frequency defined in the telemetry settings menu */
        [StructLayout(LayoutKind.Sequential, Size = LAP_PACKET_SIZE, Pack = 1)]
        public struct PacketLapData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public PacketHeader m_header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public LapData[] m_lapData;
        }

        /* Sent when an event occurs */
        [StructLayout(LayoutKind.Sequential, Size = EVENT_PACKET_SIZE, Pack = 1)]
        public struct PacketEventData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public PacketHeader m_header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_eventStringCode;
        }

        /* Sent once every 5 seconds */
        [StructLayout(LayoutKind.Sequential, Size = PARTICIPANT_PACKET_SIZE, Pack = 1)]
        public struct PacketParticipantsData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public PacketHeader m_header;
            public byte m_numCars;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public ParticipantData[] m_participants;
        }

        /* Sent once every 5 seconds */
        [StructLayout(LayoutKind.Sequential, Size = SETUP_PACKET_SIZE, Pack = 1)]
        public struct PacketCarSetupData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public PacketHeader m_header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public CarSetupData[] m_carSetups;
        }

        /* Sent at the frequency defined in the telemetry settings menu */
        [StructLayout(LayoutKind.Sequential, Size = TELEMETRY_PACKET_SIZE, Pack = 1)]
        public struct PacketCarTelemetryData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public PacketHeader m_header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public CarTelemetryData[] m_carTelemetryData;
            public uint m_buttonStatus;
        }

        /* Sent twice per second */
        [StructLayout(LayoutKind.Sequential, Size = STATUS_PACKET_SIZE, Pack = 1)]
        public struct PacketCarStatusData
        {
            [MarshalAs(UnmanagedType.Struct)]
            public PacketHeader m_header;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public CarStatusData[] m_carStatusData;
        }

        /* Helper functions */
        /* Returns whether or not the passed byte is within the value range of the PacketID enum */
        public static bool IsValidPacketID(int id) { return (id >= MIN_PACKET_ID && id <= MAX_PACKET_ID); }

    }
}
