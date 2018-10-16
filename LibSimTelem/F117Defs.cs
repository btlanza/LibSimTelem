/* Benjamin Lanza
 * F117Defs.cs
 * October 11th, 2018 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibSimTelem
{
    public class F117Defs
    {

        /* Connection constants */
        public const int DEFAULT_PORT = 20777;
        public const string DEFAULT_IP = "127.0.0.1";
        public const int DEFAULT_TIMEOUT = 1000;
        public const int TIMEOUT_CODE = 10060;
        public const string GAME_PROCESS_NAME = "F1_2017";

        /* Data block size constants */
        public const int CAR_DATA_SIZE = 45;

        /* Packet size constants */
        public const int PACKET_SIZE = 1289;

        /* Data block structures */
        [StructLayout(LayoutKind.Sequential, Size = CAR_DATA_SIZE, Pack = 1)]
        public struct CarUDPData
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] m_worldPosition;
            public float m_lastLapTime;
            public float m_currentLapTime;
            public float m_bestLapTime;
            public float m_sector1Time;
            public float m_sector2Time;
            public float m_lapDistance;
            public byte m_driverId;
            public byte m_teamId;
            public byte m_carPosition;
            public byte m_currentLapNum;
            public byte m_tyreCompound;
            public byte m_inPits;
            public byte m_sector;
            public byte m_currentLapInvalid;
            public byte m_penalties;
        }

        /* Packet structures */
        [StructLayout(LayoutKind.Sequential, Size = PACKET_SIZE, Pack = 1)]
        public struct UDPPacket
        {
            public float m_time;
            public float m_lapTime;
            public float m_lapDistance;
            public float m_totalDistance;
            public float m_x;
            public float m_y;
            public float m_z;
            public float m_speed;
            public float m_xv;
            public float m_yv;
            public float m_zv;
            public float m_xr;
            public float m_yr;
            public float m_zr;
            public float m_xd;
            public float m_yd;
            public float m_zd;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_susp_pos;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_susp_vel;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheel_speed;
            public float m_throttle;
            public float m_steer;
            public float m_brake;
            public float m_clutch;
            public float m_gear;
            public float m_gforce_lat;
            public float m_gforce_lon;
            public float m_lap;
            public float m_engineRate;
            public float m_sli_pro_native_support;
            public float m_car_position;
            public float m_kers_level;
            public float m_kers_max_level;
            public float m_drs;
            public float m_traction_control;
            public float m_anti_lock_brakes;
            public float m_fuel_in_tank;
            public float m_fuel_capacity;
            public float m_in_pits;
            public float m_sector;
            public float m_sector1_time;
            public float m_sector2_time;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_brakes_temp;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_tyres_pressure;
            public float m_team_info;
            public float m_total_laps;
            public float m_track_size;
            public float m_last_lap_time;
            public float m_max_rpm;
            public float m_idle_rpm;
            public float m_max_gears;
            public float m_sessionType;
            public float m_drsAllowed;
            public float m_track_number;
            public float m_vehicleFIAFlags;
            public float m_era;
            public float m_engine_temperature;
            public float m_gforce_vert;
            public float m_ang_vel_x;
            public float m_ang_vel_y;
            public float m_ang_vel_z;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyres_temperature;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyres_wear;
            public byte m_tyre_compound;
            public byte m_front_brake_bias;
            public byte m_fuel_mix;
            public byte m_currentLapInvalid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyres_damage;
            public byte m_front_left_wing_damage;
            public byte m_front_right_wing_damage;
            public byte m_rear_wing_damage;
            public byte m_engine_damage;
            public byte m_gear_box_damage;
            public byte m_exhaust_damage;
            public byte m_pit_limiter_status;
            public byte m_pit_speed_limit;
            public float m_session_time_left;
            public byte m_rev_lights_percent;
            public byte m_is_spectating;
            public byte m_spectator_car_index;
            public byte m_num_cars;
            public byte m_player_car_index;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public CarUDPData[] m_car_data;
            public float m_yaw;
            public float m_pitch;
            public float m_roll;
            public float m_x_local_velocity;
            public float m_y_local_velocity;
            public float m_z_local_velocity;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_susp_acceleration;
            public float m_ang_acc_x;
            public float m_ang_acc_y;
            public float m_ang_acc_z;
        }

    }
}
