using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;

namespace WpfApplication1.zmq
{
    public class LatestGpsInfo
    {
        private string altitude;

        public string Altitude
        {
            get { return altitude; }
            set { altitude = value; }
        }
        private string devSpeed;

        public string DevSpeed
        {
            get { return devSpeed; }
            set { devSpeed = value; }
        }
        private string direction;

        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        private string distance;

        public string Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        private string gpsDistance;

        public string GpsDistance
        {
            get { return gpsDistance; }
            set { gpsDistance = value; }
        }
        private string latitude;

        public string Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        private string longitude;

        public string Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }
        private string nlatitude;

        public string Nlatitude
        {
            get { return nlatitude; }
            set { nlatitude = value; }
        }

        private string nlongitude;

        public string Nlongitude
        {
            get { return nlongitude; }
            set { nlongitude = value; }
        }
        private string oil;

        public string Oil
        {
            get { return oil; }
            set { oil = value; }
        }
        private string satelliteNum;

        public string SatelliteNum
        {
            get { return satelliteNum; }
            set { satelliteNum = value; }
        }
        private string signalStrength;

        public string SignalStrength
        {
            get { return signalStrength; }
            set { signalStrength = value; }
        }
        private string simId;

        public string SimId
        {
            get { return simId; }
            set { simId = value; }
        }
        private string speed;

        public string Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        private string upTime;

        public string UpTime
        {
            get { return upTime; }
            set { upTime = value; }
        }
    }
}
