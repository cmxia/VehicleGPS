using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using VehicleGPS.Models.Login;
using VehicleGPS.Models;
using System.Data;
using VehicleGPS.Services;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.ViewModels.MessCenter
{
    class GetNewMessages : NotificationObject
    {
        private DispatcherTimer Timer = new DispatcherTimer();
        private string mesCount;
        public string MesCount
        {
            get { return mesCount; }
            set
            {
                if (mesCount != value)
                {
                    mesCount = value;
                    this.RaisePropertyChanged("MesCount");
                }
            }
        }

        public GetNewMessages()
        {
            Timer.Interval = new TimeSpan(0, 0, 0, 5);
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Start();
            Timer_Tick(null, null);
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            string sqlstr = "select COUNT(*) from MsgShip where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and Isenable=0 and zt=1";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);

            string sqlStr = "select COUNT(*) from MsgDetails where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and IsRead=0 and zt=1";
            string JsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlStr);
            DataTable RetDt = JsonHelper.JsonToDataTable(JsonStr);

            if (retDt.Rows[0][0].ToString() == "0" && RetDt.Rows[0][0].ToString() == "0")
            {   
                this.MesCount = "";
            }
            else
            {
                this.MesCount = (int.Parse(retDt.Rows[0][0].ToString()) + int.Parse(RetDt.Rows[0][0].ToString())).ToString();
            }
        }

    }
}
