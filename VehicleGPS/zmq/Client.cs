using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VehicleGPS.zmq;
using ZeroMQ;

using System.Windows.Threading;
using VehicleGPS.Models;
using System.Threading;
using VehicleGPS.Services;




namespace VehicleGPS.zmq
{
    public class ReceiveArgs : EventArgs
    {
        private string _msg = null;
        public ReceiveArgs(string msg)
        {
            this._msg = msg;
        }
        public string Result
        {
            get { return _msg; }
        }
    }
    public class Client
    {

        ZmqContext testSubZmqContext;
        ZmqSocket testSubZmqSocket;
        private string TestPubSubPoint = "tcp://" + VehicleConfig.GetInstance().BUSINESSIP + ":" + VehicleConfig.GetInstance().BUSINESSPORT;
        private System.Threading.Thread TestPubSubThread;
        private bool vehicleDataIsOk = false;
        private DispatcherTimer dispatcherTimer;
        private DispatcherTimer threadMonitor;//监控接受数据的线程是否停止

        private DispatcherTimer dispatcherRefresh;//监控接受数据的线程是否停止
        public object QueueMonitor;//互斥量
        public object ZmqMonitor;//zmq互斥量
        public List<string> MsgQueue;//消息队列


        TimeSpan timeOut = new TimeSpan(0, 0, 0, 0, 100);

        public Client()
        {
            QueueMonitor = new object();
            ZmqMonitor = new object();
            initThread();
            this.MsgQueue = new List<string>();
        }
        public void initThread()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (StaticTreeState.VehicleGPSInfo == LoadingState.LOADCOMPLETE && this.vehicleDataIsOk == false)
            {
                this.vehicleDataIsOk = true;
            }

            if (this.vehicleDataIsOk == true)
            {
                this.dispatcherTimer.Stop();
                RefreshZMQ();
                threadMonitor = new DispatcherTimer();
                threadMonitor.Interval = TimeSpan.FromSeconds(30);
                threadMonitor.Tick += new EventHandler(threadMonitor_Tick);
                threadMonitor.Start();

                dispatcherRefresh = new DispatcherTimer();
                dispatcherRefresh.Interval = TimeSpan.FromMinutes(10);
                dispatcherRefresh.Tick += new EventHandler(dispatcherRefresh_Tick);
                dispatcherRefresh.Start();
            }

        }

        /// <summary>
        /// 刷新ZeroMQ
        /// </summary>
        void RefreshZMQ()
        {
            if (TestPubSubThread != null)
            {
                TestPubSubThread.Abort();
                Thread.Sleep(500);
            }
            testSubZmqContext = null;
            if (testSubZmqSocket!=null)
            {
                testSubZmqSocket.Disconnect(TestPubSubPoint);
                testSubZmqSocket.Close();
            }
            testSubZmqSocket = null;
            Thread.Sleep(500);
            testSubZmqContext = ZmqContext.Create();
            testSubZmqSocket = testSubZmqContext.CreateSocket(SocketType.SUB);
            testSubZmqSocket.Connect(TestPubSubPoint);
            testSubZmqSocket.ReceiveHighWatermark = 10240000;
            testSubZmqSocket.SubscribeAll();
            TestPubSubThread = new Thread(delegate() { this.PushListening2(); });
            TestPubSubThread.Start();
        }
        void dispatcherRefresh_Tick(object sender, EventArgs e)
        {
            RefreshZMQ();
        }

        void threadMonitor_Tick(object sender, EventArgs e)
        {
            if (TestPubSubThread != null)
            {
                //跟踪实时信息获取线程的状态
                Log.WriteLog("thread status: " + TestPubSubThread.ThreadState, "threadStatus");
            }
        }
        private static string matchStr = "{*{*}*}";

        private void PushListening2()
        {
            #region
            StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
            //testSubZmqSocket.Subscribe(Encoding.UTF8.GetBytes("0136357199814"));
            string hb;
            while (true)
            {
                try
                {
                    string message = testSubZmqSocket.Receive(Encoding.UTF8, timeOut);
                    if (message != null)
                    {
                        hb = testSubZmqSocket.Receive(Encoding.UTF8, timeOut);
                        if (hb != null)
                        {
                            System.Threading.Monitor.Enter(this.QueueMonitor);
                            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(matchStr);
                            if (re.IsMatch(hb))
                            {
                                Log.WriteLog(hb);
                                MsgQueue.Add(hb);
                            }
                            else if (re.IsMatch(message))
                            {
                                Log.WriteLog(message);
                                MsgQueue.Add(message);
                            }

                            System.Threading.Monitor.Exit(this.QueueMonitor);
                        }
                        else
                        {
                            Log.WriteLog("2", "getFromWebNull");

                        }
                    }
                    else
                    {
                        Log.WriteLog("1", "getFromWebNull");
                    }
                    Log.WriteLog("2", "getFromWeb");
                }
                catch (Exception e)
                {
                    Log.WriteLog("error data by :" + e.Message, "error");
                }
                System.Threading.Thread.Sleep(50);
            }
            #endregion
        }

        private void PushListening()
        {
            StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
            testSubZmqSocket.SubscribeAll();
            while (true)
            {
                try
                {
                    string message = testSubZmqSocket.Receive(Encoding.UTF8);
                    string hb = testSubZmqSocket.Receive(Encoding.UTF8);
                    if (message != "")
                    {
                        if (hb != null && hb != "")
                        {
                            System.Threading.Monitor.Enter(this.QueueMonitor);
                            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(matchStr);
                            if (re.IsMatch(hb))
                            {
                                Log.WriteLog(hb);
                                MsgQueue.Add(hb);
                            }
                            else if (re.IsMatch(message))
                            {
                                Log.WriteLog(hb);
                                MsgQueue.Add(message);
                            }

                            System.Threading.Monitor.Exit(this.QueueMonitor);
                        }
                    }
                    else
                    {
                        Log.WriteLog("error:" + hb + "————" + message, "error");
                    }
                    if (message == null)
                    {

                        Log.WriteLog("error:" + hb + "————" + message, "error");
                    }
                }
                catch (Exception e)
                {
                    Log.WriteLog("error data by :" + e.Message, "error");
                }
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
