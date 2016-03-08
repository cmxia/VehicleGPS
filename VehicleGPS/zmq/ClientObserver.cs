using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models.Login;
using System.Timers;
using VehicleGPS.Services;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models;

namespace VehicleGPS.zmq
{
    class ClientObserver
    {
        private Client client;
        private BusinessDataServiceWEB webService;
        public ClientObserver()
        {
            init_Socket_Factory();
        }

        private void init_Socket_Factory()
        {
            if (client == null)
            {
                this.webService = new BusinessDataServiceWEB();
                this.client = new Client();
                System.Threading.Thread handleThread = new System.Threading.Thread(this.HandleMesageThread);//消息队列处理线程
                handleThread.Start();
            }
        }
        //消息处理线程
        private void HandleMesageThread()
        {
            while (true)
            {
                if (client.MsgQueue.Count != 0 && StaticTreeState.VehicleGPSInfo == LoadingState.LOADCOMPLETE)
                {
                    System.Threading.Monitor.Enter(client.QueueMonitor);
                    string msg = client.MsgQueue[0];
                    client.MsgQueue.RemoveAt(0);
                    System.Threading.Monitor.Exit(client.QueueMonitor);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        Log.WriteLog("1", "DeserializeGpsInfo");
                        CVDetailInfo sockDetailInfo = this.webService.DeserializeSockGpsInfo(msg);
                        if (sockDetailInfo != null)
                        {
                            if (StaticTreeState.RealTimeViewContruct)
                            {
                                RealTimeViewModel.GetInstance().ModMarker(sockDetailInfo);
                            }
                            if (StaticTreeState.VehicleDispatchViewContruct)
                            {
                                // VehicleDispatchViewModel.GetInstance().SockRefreshDispatchInfo(sockDetailInfo);
                            }
                        }
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }
        }
    }
}
