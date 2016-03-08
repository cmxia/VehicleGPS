using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using Newtonsoft.Json.Linq;
using VehicleGPS.Models.Login;
using VehicleGPS.Models;
using VehicleGPS.Services;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    class Socket
    {
        public static string PubSubRemoutEndPoint = "tcp://" + VehicleConfig.GetInstance().BUSINESSIP + ":" + VehicleConfig.GetInstance().BUSINESSPORT;//推送监听地址
        public static string DoubleInfoRemoutEndPoint = "tcp://" + VehicleConfig.GetInstance().INSTRUCTIONIP + ":" + VehicleConfig.GetInstance().INSTRUCTIONPORT;//双向通信连接地址
        //public static string DoubleInfoRemoutEndPoint = "tcp://172.24.0.50:4011";//双向通信连接地址
        //public static string DoubleInfoRemoutEndPoint = "tcp://61.183.9.107:4012";//双向通信连接地址
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sim">sim卡号</param>
        /// <param name="insBody">指令包体</param>
        /// <returns>返回值 true 代表发送成功 false 代表发送失败
        /// </returns>
        public static string zmqInstructionsPack(string sim, JObject insBody)
        {
            string reFlag = "";
            if (sim != null && insBody != null)
            {
                using (var InsCTX = ZmqContext.Create())
                {
                    using (var InsSockt = InsCTX.CreateSocket(SocketType.REQ))
                    {
                        InsSockt.Connect(DoubleInfoRemoutEndPoint);
                        //Log.WriteLog("ins", sim);
                        JObject jo = new JObject();
                        jo.Add("GType", "ins");
                        jo.Add("GpsBasic", insBody);
                        jo.Add("GpsAttatch", sim);
                        ZmqMessage msg = new ZmqMessage();
                        TimeSpan timeOut = new TimeSpan(0, 0, 0, 20, 0);//接收等待超时为1分钟
                        msg.Append(Encoding.UTF8.GetBytes(jo.ToString()));
                        InsSockt.SendMessage(msg);
                        string callBack = InsSockt.Receive(Encoding.UTF8, timeOut);

                        if (callBack == null)
                        {
                            reFlag = "发送失败";
                            Log.WriteLog("status message:send failure" + "\n ;" + "message:" + jo.ToString() + "\n", "instruction");
                        }
                        else
                        {
                            if (callBack.Equals("1"))
                            {
                                reFlag = "指令处理中，请稍后发送！";
                                Log.WriteLog("status message:send waiting" + "\n ;" + "message:" + jo.ToString() + "\n", "instruction");
                            }
                            else if (callBack.Equals("2"))
                            {
                                reFlag = "指令已发出，正在处理！";
                                Log.WriteLog("status message:send succeed" + "\n ;" + "message:" + jo.ToString() + "\n", "instruction");
                            }
                            else if (callBack.Equals("0"))
                            {
                                reFlag = "指令解析数据出错！";
                                Log.WriteLog("status message:send failure,wrong data" + "\n ;" + "message:" + jo.ToString() + "\n", "instruction");
                            }
                            else
                            {
                                reFlag = "发送失败";
                                Log.WriteLog("status message:send failure" + "\n ;" + "message:" + jo.ToString() + "\n", "instruction");
                            }
                        }
                        InsSockt.Disconnect(DoubleInfoRemoutEndPoint);
                    }
                }
            }
            return reFlag;
        }

        public static string Texttest(string sim, string termType, string tb_text, string id, string cmdid)
        {
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "SETTERMPARAMCMD_TYPE");
            jo.Add("simId", sim);
            jo.Add("termType", termType);
            jo.Add("paramNum", "1");
            jo.Add("data", id + ";" + tb_text);
            jo.Add("cmdid", cmdid);
            rebool = zmqInstructionsPack(sim, jo);
            return rebool;
        }

        public static string GetDateTimeStrAnd4BitsRandom()
        {
            Random rad = new Random();
            return DateTime.Now.ToString("yyyyMMddhh24mmss") + rad.Next(1000, 10000).ToString();
        }

        public static void ExcuteSql(string cmdname, string username, string cmdcontent, string cmdresult, string cmdsim)
        {
            string CmdID = "cmd" + GetDateTimeStrAnd4BitsRandom();
            string sql = "INSERT INTO CmdLogPara(CmdID,CmdName,CmdTime,UserName,CmdContent,CmdResult,CmdSim) VALUES ('" +
                CmdID + "','" + cmdname + "','" + DateTime.Now.ToString() + "','" + username + "','" + cmdcontent + "','" + cmdresult + "','" + cmdsim + "')";
            string jsonstr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
        }

        public static void ExcuteSqlRegion(int id, string type, string username, string sim)
        {
            string sql = "INSERT INTO RegionIDInfo(id,type,inserttime,username,sim) VALUES ('" +
                id + "','" + type + "','" + DateTime.Now.ToString() + "','" + username + "','" + sim + "')";
            string jsonstr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
        }

        public static void ExcuteSqlDelRegion(List<string> ListId, string type, string sim)
        {
            foreach (string item in ListId)
            {
                string sql = "delete RegionIDInfo where id='" + int.Parse(item) + "' and type='" + type + "'" + " and sim='" + sim + "'";
                string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            }
        }
    }
}
