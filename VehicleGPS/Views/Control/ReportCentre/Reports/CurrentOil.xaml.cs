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
using VehicleGPS.Views;
using VehicleGPS.Views.Control.ReportCentre.Reports.Common;
using VehicleGPS.Models;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace VehicleGPS.Views.Control.ReportCentre
{
    /// <summary>
    /// CurrentOil.xaml 的交互逻辑
    /// </summary>
    public partial class CurrentOil : Window
    {
        private List<Oil> oilList;
        private List<Oil> currentOilList;
        private List<CVBasicInfo> selectedVehicleList;
        private int pageSize = 20;

        public CurrentOil(List<CVBasicInfo> selectedList)
        {
            InitializeComponent();
            this.oilList = new List<Oil>();
            this.currentOilList = new List<Oil>();
            this.selectedVehicleList = selectedList;

            this.InitData();
        }
        private void InitData()
        {
            this.Indicator.IsBusy = true;
            this.oilList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/getCurrentOil.ashx";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(new AsyncCallback(this.RequestReady), request);
        }
        private void RequestReady(IAsyncResult asyncResult)
        {
            try
            {
                List<string> selectdIDList = new List<string>();
                foreach (CVBasicInfo info in this.selectedVehicleList)
                {
                    selectdIDList.Add(info.ID);
                }
                string jsonStr = "";// JavaScriptConvert.SerializeObject(selectdIDList);

                HttpWebRequest request = (HttpWebRequest)(asyncResult.AsyncState);
                StreamWriter postStream = new StreamWriter(request.EndGetRequestStream(asyncResult));
                postStream.Write("{0}={1}&{2}={3}&{4}={5}&", "Request", jsonStr, "startTime", 
                    DateTime.Now.AddMonths(0).ToString("yyyy-M-d"), "endTime", DateTime.Now.AddMonths(0).ToString("yyyy-M-d"));
                postStream.Close();
                postStream.Dispose();
                request.BeginGetResponse(new AsyncCallback(this.ResponseReady), request);
            }
            catch
            {
                MessageBox.Show("获取数据失败,请重新尝试", "获取数据", MessageBoxButton.OK);
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
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
                    this.Indicator.IsBusy = false;
                    this.Close();
                });
            }
        }
        private void DataOperate(string responseStr)
        {
            if (!string.IsNullOrEmpty(responseStr))
            {
                List<OilJson> oilList = null;//(List<OilJson>)JavaScriptConvert.DeserializeObject(responseStr, typeof(List<OilJson>));

                if (oilList.Count > 0)
                {
                    List<double> lng = new List<double>();
                    List<double> lat = new List<double>();
                    List<double> oilVolume = new List<double>();
                    List<string> time = new List<string>();
                    foreach (OilJson json in oilList)
                    {
                        lng.Add(json.location.Keys.FirstOrDefault<double>());
                        lat.Add(json.location.Values.FirstOrDefault<double>());
                        oilVolume.Add(json.oil.Values.FirstOrDefault<double>());
                        time.Add(json.oil.Keys.FirstOrDefault<string>());
                    }

                    ParseAddress parseAddr = new ParseAddress(lng, lat);
                    List<PointAddr> pointAddrList = parseAddr.BeginParse();

                    for (int i = 0; i < pointAddrList.Count; i++)
                    {
                        PointAddr pa = pointAddrList[i];
                        Oil oil = new Oil();
                        oil.sequence = i + 1;
                        oil.vehicleNum = this.selectedVehicleList[i].Name;
                        oil.sim = this.selectedVehicleList[i].SIM;
                        string parentID = this.selectedVehicleList[i].ParentID;
                        /*找到所属单位*/
                        List<CVBasicInfo> tmpList = StaticBasicInfo.GetInstance().ListClientBasicInfo;
                        foreach (CVBasicInfo info in tmpList)
                        {
                            if (info.ID == parentID)
                            {
                                oil.parentDepart = info.Name;
                                break;
                            }
                        }
                        oil.time = time[i];
                        oil.oil = oilVolume[i].ToString();
                        oil.lat = pa.lat;
                        oil.lng = pa.lng;
                        oil.address = pa.addr;
                        this.oilList.Add(oil);
                    }
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        this.PageIndexChanging(0, null);
                        Pager pager = new Pager(this.oilList.Count, this.pageSize);
                        pager.PageIndexChanging += new PagerIndexChangingEvent(this.PageIndexChanging);
                        this.pagerContainer.Children.Clear();
                        this.pagerContainer.Children.Add(pager);
                        this.Indicator.IsBusy = false;
                    });
                }
                else
                {
                    MessageBox.Show("找不到数据", "提示", MessageBoxButton.OK);
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        this.Indicator.IsBusy = false;
                        this.Close();
                    });
                }
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
            this.OilList.ItemsSource = null;
            this.OilList.ItemsSource = this.currentOilList;
        }

        private void export_static_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
