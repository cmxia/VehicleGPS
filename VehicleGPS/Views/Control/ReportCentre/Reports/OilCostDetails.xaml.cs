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
using VehicleGPS.Models;
using VehicleGPS.Views.Control.ReportCentre.Reports.Common;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace VehicleGPS.Views.Control.ReportCentre.Reports
{
    /// <summary>
    /// OilCostDetails.xaml 的交互逻辑
    /// </summary>
    public partial class OilCostDetails : Window
    {
        private List<Oil> oilList;
        private List<Oil> currentOilList;
        private CVBasicInfo selectedVehicle;
        private Oil selectedOil;
        private DateTime startTime;
        private DateTime endTime;
        private int pageSize = 20;

        public OilCostDetails(List<CVBasicInfo> selectedList, DateTime sTime, DateTime eTime)
        {
            InitializeComponent();
            this.InitBaiduMap();
            this.selectedVehicle = selectedList[0];
            this.startTime = sTime;
            this.endTime = eTime;
            this.oilList = new List<Oil>();
            this.currentOilList = new List<Oil>();
            this.InitData();
        }

        public void InitBaiduMap()
        {
            string currentUri = Environment.CurrentDirectory;
            Uri uri = new Uri(VehicleConfig.GetInstance().reportMapPath, UriKind.RelativeOrAbsolute);
            this.MyWeb.Navigate(uri);
        }

        private void InitData()
        {
            this.MyWeb.Visibility = Visibility.Hidden;
            this.Indicator.IsBusy = true;
            this.oilList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/getOilList.ashx";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(new AsyncCallback(this.RequestReady), request);
        }
        private void RequestReady(IAsyncResult asyncResult)
        {
            try
            {
                List<string> tmpList = new List<string>();
                tmpList.Add(this.selectedVehicle.ID);
                string jsonStr = "";// JavaScriptConvert.SerializeObject(tmpList);
                HttpWebRequest request = (HttpWebRequest)(asyncResult.AsyncState);
                StreamWriter postStream = new StreamWriter(request.EndGetRequestStream(asyncResult));
                postStream.Write("{0}={1}&{2}={3}&{4}={5}", "Request", jsonStr,
                    "startTime", this.startTime.ToString("yyyy-MM-dd HH:mm"), "endTime", this.endTime.ToString("yyyy-MM-dd HH:mm"));
                postStream.Close();
                postStream.Dispose();
                request.BeginGetResponse(new AsyncCallback(this.ResponseReady), request);
            }
            catch
            {
                MessageBox.Show("获取数据失败,请重新尝试", "获取数据", MessageBoxButton.OK);
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    this.MyWeb.Visibility = Visibility.Visible;
                    this.Indicator.IsBusy = false;
                    this.Close();
                });
            }
        }
        private void ResponseReady(IAsyncResult asyncResult)
        {
            try
            {
                HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
                WebResponse response = request.EndGetResponse(asyncResult) as WebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string responseStr = reader.ReadToEnd();
                    this.DataOperate(responseStr);
                }
            }
            catch
            {
                MessageBox.Show("获取数据失败,请关闭重新尝试", "获取数据", MessageBoxButton.OK);
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    this.MyWeb.Visibility = Visibility.Visible;
                    this.Indicator.IsBusy = false;
                    this.Close();
                });
            }
        }
        private void DataOperate(string responseStr)
        {
            if (!string.IsNullOrEmpty(responseStr))
            {
                OilJson oilJson = null;// (OilJson)JavaScriptConvert.DeserializeObject(responseStr, typeof(OilJson));

                if (oilJson.oilVolume.Count == 0 && oilJson.datetime.Count == 0
                    && oilJson.oilVolume.Count == 0 && oilJson.longitude.Count == 0
                    && oilJson.oilVolume.Count == 0 && oilJson.latitude.Count == 0)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        MessageBox.Show("没有找到数据", "提示", MessageBoxButton.OK);
                        this.MyWeb.Visibility = Visibility.Visible;
                        this.Indicator.IsBusy = false;
                        this.Close();
                        return;
                    });
                }
                if (oilJson.oilVolume.Count == oilJson.datetime.Count
                    && oilJson.oilVolume.Count == oilJson.longitude.Count
                    && oilJson.oilVolume.Count == oilJson.latitude.Count)
                {
                    ParseAddress parseAddr = new ParseAddress(oilJson.longitude, oilJson.latitude);
                    List<PointAddr> pointAddrList = parseAddr.BeginParse();

                    for (int i = 0; i < pointAddrList.Count; i++)
                    {
                        PointAddr pa = pointAddrList[i];
                        Oil oil = new Oil();
                        oil.sequence = i + 1;
                        /*找到所属单位*/
                        List<CVBasicInfo> tmpList = StaticBasicInfo.GetInstance().ListClientBasicInfo;
                        foreach (CVBasicInfo info in tmpList)
                        {
                            if (info.ID == this.selectedVehicle.ParentID)
                            {
                                oil.vehicleNum = this.selectedVehicle.Name;
                                oil.sim = this.selectedVehicle.SIM;
                                oil.parentDepart = info.Name;
                                break;
                            }
                        }
                        oil.time = oilJson.datetime[i];
                        oil.oil = oilJson.oilVolume[i].ToString();
                        oil.lat = pa.lat;
                        oil.lng = pa.lng;
                        oil.address = pa.addr;
                        this.oilList.Add(oil);
                    }
                }
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    this.PageIndexChanging(0, null);
                    Pager pager = new Pager(this.oilList.Count, this.pageSize);
                    pager.PageIndexChanging += new PagerIndexChangingEvent(this.PageIndexChanging);
                    this.pagerContainer.Children.Clear();
                    this.pagerContainer.Children.Add(pager);
                    this.MyWeb.Visibility = Visibility.Visible;
                    this.Indicator.IsBusy = false; 
                });
            }
            else
            {
                MessageBox.Show("没有找到数据", "提示", MessageBoxButton.OK);
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    this.MyWeb.Visibility = Visibility.Visible;
                    this.Indicator.IsBusy = false;
                    this.Close();
                });
            }
        }
        private void PageIndexChanging(int pageIndex, EventArgs e)
        {
            int startIndex, endIndex;
            startIndex = this.pageSize * pageIndex;
            if (startIndex + this.pageSize > this.oilList.Count)
            {
                endIndex = this.oilList.Count - 1;
            }
            else
            {
                endIndex = startIndex + this.pageSize - 1;
            }
            this.currentOilList.Clear();
            for (; startIndex <= endIndex; startIndex++)
            {
                this.currentOilList.Add(this.oilList[startIndex]);
            }
            this.OilDetail.ItemsSource = null;
            this.OilDetail.ItemsSource = this.currentOilList;
        }
        private void export_static_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OilDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Oil tmp = (Oil)((DataGrid)sender).SelectedItem;
            if (tmp != null)
            {
                this.selectedOil = tmp;
                string cont = this.GetHtml(this.selectedOil.vehicleNum, this.selectedOil.sim,
                    this.selectedOil.time, this.selectedOil.parentDepart, this.selectedOil.oil, this.selectedOil.address);
                this.MyWeb.InvokeScript("SetMarker", new object[] { this.selectedOil.lng, this.selectedOil.lat, cont});
            }
        }

        public string GetHtml(string licenseNum, string SIM, string uploadTime, string parentDepart, string oil, string curlocation)
        {
            string ret = "";
            ret += "<table style='font-family:verdana;font-size:12;color:black'>";
            ret += "<tr><td>车牌号：</td><td>";
            ret += licenseNum + "</td></tr>";
            ret += "<tr><td>SIM卡号：</td><td>";
            ret += SIM + "</td></tr>";
            ret += "<tr><td>所属单位：</td><td>";
            ret += parentDepart + "</td></tr>";
            ret += "<tr><td>上传时间：</td><td>";
            ret += uploadTime + "</td></tr>";
            ret += "<tr><td>油量：</td><td>";
            ret += oil + "</td></tr>";
            ret += "<tr><td>当前位置：</td><td>";
            ret += curlocation + "</td></tr>";
            ret += "</table>";
            return ret;
        }
    }
}
