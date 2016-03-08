using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using Newtonsoft.Json.Linq;
using VehicleGPS.Services;
using Newtonsoft.Json;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;

namespace WpfApplication1.zmq
{
    #region 事件参数
    public class PubEventArgs : EventArgs
    {
        public string type;
        public string sim;
        public string body;
        public PubEventArgs()
        {

        }

        public PubEventArgs(string mtype, string msim, string msghb)
        {
            this.type = mtype;
            this.sim = msim;
            this.body = msghb;
        }
    }
    #endregion
    public class Subscriber
    {
        public delegate void SublishEventHander(object sender, PubEventArgs e);
        public event SublishEventHander SublishToPub;

        public Subscriber()
        {
        }
        public void RecieveMsg(string objStr)
        {
            if (objStr == "" || objStr == null)
                return;
            JObject jo = JObject.Parse(objStr);
            string type = "";
            try
            {
                type = jo["gType"].ToString().ToLower();
            }
            catch (Exception)
            {
                throw;
            }
            switch (type)
            {
                case "gpslocation":
                    LatestGpsInfo lgi = new LatestGpsInfo();
                    lgi = JsonConvert.DeserializeObject<LatestGpsInfo>(jo["gpsBasic"].ToString());
                    if (lgi.SimId == null)
                    {
                        Log.WriteLog("LatestGpsInfoHanderError", ":" + jo["gpsBasic"].ToString());
                    }
                    else
                    {
                        //lock (ListLatestGpsInfo)
                        //{
                        //    ListLatestGpsInfo.Remove(ListLatestGpsInfo.Where(p => p.SimId.Equals(lgi.SimId)).FirstOrDefault());
                        //    ListLatestGpsInfo.Add(lgi);
                        //}
                        startToPublish(type, lgi.SimId, jo["gpsBasic"].ToString());
                    }
                    //Log.WriteLog("gpsLocation", ":" + objStr);

                    break;
                default:
                    Log.WriteLog("unkowntype", ":" + objStr);
                    break;
            }

        }

        #region 静态变量
        private static readonly string guid = System.Guid.NewGuid().ToString() + "-" + DateTime.Now.ToFileTimeUtc().ToString(); //直接返回字符串类型
        public static List<LatestGpsInfo> ListLatestGpsInfo = new List<LatestGpsInfo>();

        public static VehicleGPS.zmq.HeartBody heartPack()
        {
            VehicleGPS.zmq.HeartBody hb = new VehicleGPS.zmq.HeartBody();
            hb.Guid = guid;
            hb.Userid = StaticLoginInfo.GetInstance().UserName;//登陆的用户名,带修改
            hb.LoginType = "1";//LoginType:1——客户端；2——web；3——android；4——IOS
            return hb;
        }
        #endregion
        #region publishhander
        private void startToPublish(string mtype, string msim, string mbody)
        {
            if (mtype != null && msim != null && mbody != null)
            {
                OnPublish(new PubEventArgs(mtype, msim, mbody));
            }
        }
        private void OnPublish(PubEventArgs e)
        {
            if (SublishToPub != null)
            {
                this.SublishToPub(this, e);
            }
        }
        #endregion
    }

    public class GlobalConfig
    {
        //public static readonly string PubSubRemoutPoint = "tcp://61.183.9.107:4011";//发布版5554      
        //public static readonly string ReqRepRemoutPoint = "tcp://61.183.9.107:4012";//发布版5553
        public static readonly string PubSubRemoutPoint = "tcp://" + VehicleConfig.GetInstance().BUSINESSIP + ":" + VehicleConfig.GetInstance().BUSINESSPORT;//发布版5554      
        public static readonly string ReqRepRemoutPoint = "tcp://" + VehicleConfig.GetInstance().INSTRUCTIONIP + ":" + VehicleConfig.GetInstance().INSTRUCTIONPORT;//发布版5553
    }
}
