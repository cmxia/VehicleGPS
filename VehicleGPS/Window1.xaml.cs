using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.zmq;
using ZeroMQ;

namespace VehicleGPS
{
    /// <summary>
    /// Window.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {

        ZmqContext testSubZmqContext;
        ZmqSocket testSubZmqSocket;
        private string TestPubSubPoint = "tcp://59.69.105.80:5554";
        System.Threading.Thread TestPubSubThread;

        ZmqContext testWebGSZmqContext;
        ZmqSocket testWebGSZmqSocket;
        private string TestWebGSPoint = "tcp://59.69.105.80:5553";
        System.Threading.Thread TestWebGSThread;

        public Window1()
        {
            InitializeComponent();

            testSubZmqContext = ZmqContext.Create();
            testSubZmqSocket = testSubZmqContext.CreateSocket(SocketType.SUB);
            testSubZmqSocket.Connect(TestPubSubPoint);
            TestPubSubThread = new System.Threading.Thread(TestWebPubListener);

            testWebGSZmqContext = ZmqContext.Create();
            testWebGSZmqSocket = testWebGSZmqContext.CreateSocket(SocketType.REQ);
            testWebGSZmqSocket.Connect(TestWebGSPoint);
            testWebGSZmqSocket.Identity = Encoding.UTF8.GetBytes("testSendTag1");
            TestWebGSThread = new System.Threading.Thread(TestWebGSListener);
            
        }

        private void TestWebGSListener()
        {
            string tag = "testSendTag1";

            while (true)
            {
                Log.WriteLog("SendToRouterStart", "Start");
                ZmqMessage msg = new ZmqMessage();
                msg.Append(Encoding.UTF8.GetBytes(tag));
                testWebGSZmqSocket.SendMessage(msg);
                Log.WriteLog("SendToRouter", tag);
                testWebGSZmqSocket.Receive(Encoding.UTF8);
            }
        }
        private void TestWebPubListener()
        {

            testSubZmqSocket.Subscribe(Encoding.UTF8.GetBytes("013477328651"));
            while (true)
            {
                try
                {

                    string message = testSubZmqSocket.Receive(Encoding.UTF8);
                    string hb = testSubZmqSocket.Receive(Encoding.UTF8);
                    if (message != "")
                    {
                        Log.WriteLog("subcs1", ":" + message + "|" + hb);
                    }
                }
                catch (Exception)
                {

                }
                System.Threading.Thread.Sleep(200);
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            TestPubSubThread.Start();
            button1.IsEnabled = false;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            button2.IsEnabled = false;
            Log.WriteLog("GetFromRouter", "start");
            TestWebGSThread.Start();
        }
    }
}
