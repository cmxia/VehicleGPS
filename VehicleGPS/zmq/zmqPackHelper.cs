using System;
using System.Text;
using Newtonsoft.Json.Linq;
using ZeroMQ;
using VehicleGPS.Services;
using VehicleGPS.Models;

namespace VehicleGPS.zmq
{
    class zmqPackHelper
    {
        //public static string PubSubRemoutEndPoint = "tcp://61.183.9.107:4011";//推送监听地址
        public static string PubSubRemoutEndPoint = "tcp://" + VehicleConfig.GetInstance().BUSINESSIP + ":" + VehicleConfig.GetInstance().BUSINESSPORT;//推送监听地址
        //public static string DoubleInfoRemoutEndPoint = "tcp://61.183.9.107:4012";//双向通信连接地址
        public static string DoubleInfoRemoutEndPoint = "tcp://" + VehicleConfig.GetInstance().INSTRUCTIONIP + ":" + VehicleConfig.GetInstance().INSTRUCTIONPORT;//双向通信连接地址
        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="sim">sim卡号</param>
        /// <param name="insBody">指令包体</param>
        /// <returns>返回值 true 代表发送成功 false 代表发送失败
        /// </returns>
        public static bool zmqInstructionsPack(string sim, string insBody)
        {
            bool reFlag = false;
            if (sim != null && insBody != null)
            {
                using (var InsCTX = ZmqContext.Create())
                {
                    using (var InsSockt = InsCTX.CreateSocket(SocketType.REQ))
                    {
                        InsSockt.Connect(DoubleInfoRemoutEndPoint);
                        //  Log.WriteLog("ins", sim);
                        JObject jo = new JObject();
                        jo.Add("GType", "dispatch");
                        jo.Add("GpsBasic", insBody);
                        jo.Add("GpsAttatch", sim);
                        ZmqMessage msg = new ZmqMessage();
                        TimeSpan timeOut = new TimeSpan(0, 0, 0, 5, 0);//接收等待超时为1分钟
                        msg.Append(Encoding.UTF8.GetBytes(jo.ToString()));
                        InsSockt.SendMessage(msg);
                        string callBack = null;
                        try
                        {
                            callBack = InsSockt.Receive(Encoding.UTF8, timeOut);
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("message:" + jo.ToString() + "\n", "dispatch");
                            Log.WriteLog("status message:" + e.Message + "\n", "dispatch");
                            throw;
                        }

                        if (callBack == null)
                        {
                            Log.WriteLog("message:" + jo.ToString() + "\n", "dispatch");
                            Log.WriteLog("status message:send failure" + "\n", "dispatch");
                            reFlag = false;
                        }
                        else
                        {
                            if (callBack.Equals("1"))
                            {
                                reFlag = false;
                                Log.WriteLog("message:" + jo.ToString() + "\n", "dispatch");
                                Log.WriteLog("status message:send waiting" + "\n", "dispatch");
                            }
                            else
                            {
                                reFlag = true;
                                Log.WriteLog("message:" + jo.ToString() + "\n", "dispatch");
                                Log.WriteLog("status message:send succeed" + "\n", "dispatch");
                            }
                        }
                        InsSockt.Disconnect(DoubleInfoRemoutEndPoint);
                    }
                }
            }
            return reFlag;
        }
        /// <summary>
        /// 基站定位
        /// </summary>
        /// <param name="sim">sim卡号</param>
        /// <param name="insBody">指令内容</param>
        /// <returns>发送状态</returns>
        public static bool StationLocate_zmq(string sim, string insBody)
        {

            bool reFlag = false;
            if (sim != null && insBody != null)
            {
                using (var InsCTX = ZmqContext.Create())
                {
                    using (var InsSockt = InsCTX.CreateSocket(SocketType.REQ))
                    {
                        InsSockt.Connect(DoubleInfoRemoutEndPoint);
                        //  Log.WriteLog("ins", sim);
                        JObject jo = new JObject();
                        jo.Add("GType", "cellLocation");
                        jo.Add("GpsBasic", insBody);
                        jo.Add("GpsAttatch", sim);
                        ZmqMessage msg = new ZmqMessage();
                        TimeSpan timeOut = new TimeSpan(0, 0, 0, 5, 0);//接收等待超时为1分钟
                        msg.Append(Encoding.UTF8.GetBytes(jo.ToString()));
                        InsSockt.SendMessage(msg);
                        string callBack = null;
                        try
                        {
                            callBack = InsSockt.Receive(Encoding.UTF8, timeOut);
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("message:" + jo.ToString() + "\n", "cellLocation");
                            Log.WriteLog("status message:" + e.Message + "\n", "cellLocation");
                            throw;
                        }

                        if (callBack == null)
                        {
                            Log.WriteLog("message:" + jo.ToString() + "\n", "cellLocation");
                            Log.WriteLog("status message:send failure" + "\n", "cellLocation");
                            reFlag = false;
                        }
                        else
                        {
                            if (callBack.Equals("1"))
                            {
                                reFlag = true;
                                Log.WriteLog("message:" + jo.ToString() + "\n", "cellLocation");
                                Log.WriteLog("status message:send waiting" + "\n", "cellLocation");
                            }
                            else
                            {
                                reFlag = false;
                                Log.WriteLog("message:" + jo.ToString() + "\n", "cellLocation");
                                Log.WriteLog("status message:send succeed" + "\n", "cellLocation");
                            }
                        }
                        InsSockt.Disconnect(DoubleInfoRemoutEndPoint);
                    }
                }
            }
            return reFlag;
        }
    }
}

