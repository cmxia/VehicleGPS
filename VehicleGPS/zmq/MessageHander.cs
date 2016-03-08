using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroMQ;
using WpfApplication1.zmq;
using Newtonsoft.Json;

namespace VehicleGPS.zmq
{
    class MessageHander
    {
        ZmqContext SubZmqContext;
        ZmqSocket SubZmqSocket;
        System.Threading.Thread PubSubThread;

        ZmqContext ReqRepContext;
        ZmqSocket ReqRepSocket;
        System.Threading.Thread HeartThread;

        private Subscriber msgPub;

        public MessageHander()
        {
            initSocket();
        }

        private void initSocket()
        {
            ReqRepContext = ZmqContext.Create();
            ReqRepSocket = ReqRepContext.CreateSocket(SocketType.REQ);
            ReqRepSocket.Connect(WpfApplication1.zmq.GlobalConfig.ReqRepRemoutPoint);
            HeartThread = new System.Threading.Thread(heartTheadHander);
        }

        public void startListen()
        {
            HeartThread.Name = "HeartThread";
            HeartThread.IsBackground = true;
            HeartThread.Start();
            //Register();
        }
        private void PubSubListener()
        {
            //SubZmqSocket.Subscribe(Encoding.UTF8.GetBytes("013477328651"));//权限内消息订阅  根据权限内的sim卡号订阅
            List<string> l = new List<string>();
            l.Add("013477328651");
            l.Add("013477328653");
            foreach(string sim in l)
            {
                
                SubZmqSocket.Subscribe(Encoding.UTF8.GetBytes(sim));//权限内消息订阅  根据权限内的sim卡号订阅
            }
            while (true)
            {
                try
                {
                    string message = SubZmqSocket.Receive(Encoding.UTF8);//信封
                    string hb = SubZmqSocket.Receive(Encoding.UTF8);//内容
                    if (message != "")
                    {
                        msgPub.RecieveMsg(hb);
                        Log.WriteLog("subcs1", ":" + message + "|" + hb);
                    }
                }
                catch (Exception)
                {

                }
                System.Threading.Thread.Sleep(200);
            }

        }

        private void heartTheadHander()
        {
            MsgHB mhb = new MsgHB();
            mhb.GType = "Heart";
            mhb.GpsBasic = JsonConvert.SerializeObject(Subscriber.heartPack()).ToString();
            mhb.GpsAttatch = "";
            string callBack = "";
            string tag = JsonConvert.SerializeObject(mhb);
            while (true)
            {
                Log.WriteLog("SendToRouterStart", "Start");
                ZmqMessage msg = new ZmqMessage();
                msg.Append(Encoding.UTF8.GetBytes(tag));
                ReqRepSocket.SendMessage(msg);
                Log.WriteLog("SendToRouter", tag);
                callBack = ReqRepSocket.Receive(Encoding.UTF8);
                Log.WriteLog("GetFromRouter", callBack);
                System.Threading.Thread.Sleep(10000);
            }

        }

        private void Register()
        {
            using (var randCTX = ZmqContext.Create())
            {
                using (var randSockt = randCTX.CreateSocket(SocketType.REQ))
                {
                    randSockt.Connect(WpfApplication1.zmq.GlobalConfig.ReqRepRemoutPoint);
                    MsgHB mhb = new MsgHB();
                    mhb.GType = "Register";
                    mhb.GpsBasic = JsonConvert.SerializeObject(Subscriber.heartPack()).ToString();
                    mhb.GpsAttatch = "";

                    string tag = JsonConvert.SerializeObject(mhb); ;

                    Log.WriteLog("SendToRouterRegisterStart", "RegisterStart");//登陆鉴权
                    ZmqMessage msg = new ZmqMessage();
                    msg.Append(Encoding.UTF8.GetBytes(tag));
                    randSockt.SendMessage(msg);
                    Log.WriteLog("SendToRouterRegister", tag);
                    string sg = randSockt.Receive(Encoding.UTF8);


                    //sg
                    Log.WriteLog("GetFromRouterRegister", sg);
                    randSockt.Disconnect(WpfApplication1.zmq.GlobalConfig.ReqRepRemoutPoint);
                }
            }
        
        }

        public delegate void SublishEventHander(object sender, PubEventArgs e);
        public event SublishEventHander SublishToPub;

        private void msgPub_Publish(object sender, PubEventArgs e)
        {
            OnPublish(e);
        }
        private void OnPublish(PubEventArgs e)
        {
            if (SublishToPub != null)
            {
                this.SublishToPub(this, e);
            }
        }
    }
}
