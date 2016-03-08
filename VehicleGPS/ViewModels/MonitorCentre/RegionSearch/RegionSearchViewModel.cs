using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls;
using VehicleGPS.Models;
using System.Windows;
using System.Windows.Navigation;
using VehicleGPS.Models.Login;
using System.Data;
using VehicleGPS.Services;

namespace VehicleGPS.ViewModels.MonitorCentre.RegionSearch
{
    class RegionSearchViewModel : NotificationObject
    {
        public WebBrowser WebMap { get; set; }
        public RegionSearchViewModel(WebBrowser web)
        {
            this.QueryCommand = new DelegateCommand(new Action(this.QueryCommandExecute));
            this.GetRegionInfoCommand = new DelegateCommand(new Action(this.GetRegionInfoCommandExecute));
            this.ClearRegionInfoCommand = new DelegateCommand(new Action(this.ClearRegionInfoCommandExecute));
            this.WebMap = web;
            InitBaiduMap();//初始化地图
        }
        //绑定数据源
        private List<RegionSearchOneViewModel> vehiclelist;

        public List<RegionSearchOneViewModel> VehicleList
        {
            get { return vehiclelist; }
            set
            {
                vehiclelist = value;
                this.RaisePropertyChanged("VehicleList");
            }
        }

        private double top_left_lng { get; set; }
        private double top_left_lat { get; set; }
        private double down_right_lng { get; set; }
        private double down_right_lat { get; set; }

        #region 绑定字段
        private string square;

        public string Square
        {
            get { return square; }
            set
            {
                square = value;
                this.RaisePropertyChanged("Square");
            }
        }


        private string top_left;

        public string Top_Left
        {
            get { return top_left; }
            set
            {
                top_left = value;
                this.RaisePropertyChanged("Top_Left");
            }
        }
        private string down_right;

        public string Down_Right
        {
            get { return down_right; }
            set
            {
                down_right = value;
                this.RaisePropertyChanged("Down_Right");
            }
        }
        private DateTime begintime = DateTime.Now.AddDays(-2);

        public DateTime BeginTime
        {
            get { return begintime; }
            set
            {
                begintime = value;
                this.RaisePropertyChanged("BeginTime");
            }
        }
        private DateTime endtime = DateTime.Now;

        public DateTime EndTime
        {
            get { return endtime; }
            set
            {
                endtime = value;
                this.RaisePropertyChanged("EndTime");
            }
        }
        #endregion

        #region 绑定操作
        public DelegateCommand GetRegionInfoCommand { get; set; }
        private void GetRegionInfoCommandExecute()
        {
            this.GetRegionInfo();
        }
        public DelegateCommand ClearRegionInfoCommand { get; set; }
        private void ClearRegionInfoCommandExecute()
        {
            this.ClearMarkers();
            this.VehicleList = null;
            this.Top_Left = "";
            this.Square = null;
            this.Down_Right = "";
            this.OpenDrawRectManager();
        }
        public DelegateCommand QueryCommand { get; set; }
        private void Datatable2List(DataTable dt)
        {
            this.VehicleList = new List<RegionSearchOneViewModel>();
            int i = 1;

            foreach (DataRow row in dt.Rows)
            {
                string vehicleId = row["vehicleId"].ToString();
                foreach (CVDetailInfo vehicle in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                {
                    if (vehicleId.Equals(vehicle.VehicleId))
                    {
                        RegionSearchOneViewModel vm = new RegionSearchOneViewModel();
                        vm.vehicleNum = row["vehicleNum"].ToString();
                        vm.Sequence = (i++) + "";
                        vm.VehicleId = vehicleId;
                        vm.starttime = this.BeginTime;
                        vm.endtime = this.EndTime;
                        vm.SIM = row["simId"].ToString();
                        this.VehicleList.Add(vm);
                    }
                }
            }
        }
        private void QueryCommandExecute()
        {
            if (this.Square == null)
            {
                MessageBox.Show("请先获取区域信息！");
                return;
            }
            if (this.Square == null || this.Square.Equals("null"))
            {
                MessageBox.Show("区域面积不能为0！");
            }
            if (BeginTime == null)
            {
                MessageBox.Show("请选择开始时间！");
                return;
            }
            if (EndTime == null)
            {
                MessageBox.Show("请选择结束时间！");
                return;
            }
            string sql = @"select distinct vehicleNum,simId,vehicleId from GpsBasic where ";
            if (top_left_lat < down_right_lat)
            {
                sql += " latitude < " + down_right_lat + " and latitude > " + top_left_lat;
            }
            else
            {
                sql += " latitude > " + down_right_lat + " and latitude < " + top_left_lat;
            }
            if (top_left_lng < down_right_lng)
            {
                sql += " and longitude < " + down_right_lng + " and longitude > " + top_left_lng;
            }
            else
            {
                sql += " and longitude > " + down_right_lng + " and longitude < " + top_left_lng;
            }
            sql += " and recordtime < '" + EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and recordtime > '" + BeginTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.YExecuteReportSql(StaticLoginInfo.GetInstance().UserName, BeginTime, EndTime, sql, "0");
            if (jsonStr == "error")
            {
                MessageBox.Show("查询数据库失败！请重试");
                return;
            }
            DataTable dt = new DataTable();
            dt = JsonHelper.JsonToDataTable(jsonStr);
            this.VehicleList = null;
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("没有查到符合条件的数据");
                return;
            }
            Datatable2List(dt);
        }
        #endregion

        #region 地图操作

        private void InitBaiduMap()
        {
            try
            {
                Uri uri = new Uri(VehicleConfig.GetInstance().regionsearchMapPath, UriKind.RelativeOrAbsolute);
                this.WebMap.Navigate(uri);
            }
            catch (Exception)
            {
                MessageBox.Show("地图加载失败！");
                throw;
            }
        }
        private void OpenDrawRectManager()
        {
            this.WebMap.InvokeScript("OpenDrawRectManager");
        }
        private void ClearMarkers()
        {
            this.WebMap.InvokeScript("RemoveAllOverlays");
        }
        private void GetRegionInfo()
        {
            string regioninfo = this.WebMap.InvokeScript("getRect").ToString();
            if (regioninfo == null || regioninfo == "")
            {
                MessageBox.Show("请先在地图上划好区域");
                return;
            }
            string[] region = regioninfo.Split(';');
            this.Top_Left = region[0];
            this.top_left_lng = Convert.ToDouble((region[0].Split(','))[0]);
            this.top_left_lat = Convert.ToDouble((region[0].Split(','))[1]);
            this.Down_Right = region[1];
            this.down_right_lng = Convert.ToDouble((region[1].Split(','))[0]);
            this.down_right_lat = Convert.ToDouble((region[1].Split(','))[1]);

            this.Square = region[2];
        }
        #endregion
    }
}
