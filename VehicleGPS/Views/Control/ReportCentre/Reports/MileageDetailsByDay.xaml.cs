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
using VehicleGPS.Views.Control.ReportCentre.Reports.Common;
using VehicleGPS.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Visifire.Charts;

namespace VehicleGPS.Views.Control.ReportCentre.Reports
{
    /// <summary>
    /// MileageDetailsByDay.xaml 的交互逻辑
    /// </summary>
    public partial class MileageDetailsByDay : Window
    {
        private List<Mileage> mileageList;
        private List<Mileage> currentMileageList;
        private List<Mileage> dayMileageList;
        private List<CVBasicInfo> selectedVehicleList;
        private DataSeries dataseries;//图标显示
        private DateTime startTime;
        private DateTime endTime;
        private int pageSize = 20;
        private Mileage selectedMileage;

        public MileageDetailsByDay(List<CVBasicInfo> selectedList, DateTime sTime, DateTime eTime)
        {
            InitializeComponent();
            this.selectedVehicleList = selectedList;
            this.startTime = sTime;
            this.endTime = eTime;
            this.mileageList = new List<Mileage>();
            this.currentMileageList = new List<Mileage>();
            this.dayMileageList = new List<Mileage>();

            //初始化图标属性
            this.dataseries = new DataSeries();
            this.dataseries.RenderAs = RenderAs.Column;
            this.dataseries.BorderThickness = new Thickness(0);
            this.dataseries.Background = new SolidColorBrush(Colors.Transparent);
            this.dataseries.LabelEnabled = true;
            chartTest.View3D = true;//3D显示

            this.InitData();
        }
        private void InitData()
        {
            this.Indicator.IsBusy = true;
            this.mileageList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/getVehicleMileageByDay.ashx";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(new AsyncCallback(this.RequestReady), request);
        }
        private void RequestReady(IAsyncResult asyncResult)
        {
            try
            {
                List<string> jsonList = new List<string>();
                foreach (CVBasicInfo info in this.selectedVehicleList)
                {
                    jsonList.Add(info.ID);
                }
                string jsonStr = "";// JavaScriptConvert.SerializeObject(jsonList);

                HttpWebRequest request = (HttpWebRequest)(asyncResult.AsyncState);
                StreamWriter postStream = new StreamWriter(request.EndGetRequestStream(asyncResult));
                postStream.Write("{0}={1}&{2}={3}&{4}={5}&{6}={7}&", "Request", jsonStr,
                    "startTime", this.startTime.ToString("yyyy-M-d"), "endTime", this.endTime.ToString("yyyy-M-d"), "flag", "Total");//flag为"Total"就是请求的总里程。 
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
                    this.Dispatcher.BeginInvoke((Action)delegate() 
                    { 
                        this.Indicator.IsBusy = false; 
                    });
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
                List<MileageJson> jsonList = null;// (List<MileageJson>)JavaScriptConvert.DeserializeObject(responseStr, typeof(List<MileageJson>));
                int count = 0;
                foreach (MileageJson json in jsonList)
                {
                    foreach (var valuekey in json.mileages)
                    {
                        Mileage m = new Mileage();
                        m.sequence = ++count;
                        m.id = json.vehicleNum;
                        /*找到所属单位*/
                        List<CVBasicInfo> tmpList = StaticBasicInfo.GetInstance().ListClientBasicInfo;
                        foreach (CVBasicInfo selectedInfo in this.selectedVehicleList)
                        {
                            if (selectedInfo.ID == json.vehicleNum)
                            {
                                foreach (CVBasicInfo info in tmpList)
                                {
                                    if (info.ID == selectedInfo.ParentID)
                                    {
                                        m.vehicleNum = selectedInfo.Name;
                                        m.sim = selectedInfo.SIM;
                                        m.parentDepart = info.Name;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        m.mileage = valuekey.Value.ToString();
                        m.time = valuekey.Key.ToString();
                        this.mileageList.Add(m);
                    }
                }
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    //this.MileageList.ItemsSource = null;
                    //this.MileageList.ItemsSource = this.mileageList;
                    this.PageIndexChanging(0, null);
                    Pager pager = new Pager(this.mileageList.Count, this.pageSize);
                    pager.PageIndexChanging += new PagerIndexChangingEvent(this.PageIndexChanging);
                    this.pagerContainer.Children.Clear();
                    this.pagerContainer.Children.Add(pager);
                });
            }
        }
        private void PageIndexChanging(int pageIndex, EventArgs e)
        {
            int startIndex, endIndex;
            startIndex = this.pageSize * pageIndex;
            if (startIndex + this.pageSize > this.mileageList.Count)
            {
                endIndex = this.mileageList.Count - 1;
            }
            else
            {
                endIndex = startIndex + this.pageSize - 1;
            }
            this.currentMileageList.Clear();
            for (; startIndex <= endIndex; startIndex++)
            {
                this.currentMileageList.Add(this.mileageList[startIndex]);
            }
            this.TotalMileageList.ItemsSource = null;
            this.TotalMileageList.ItemsSource = this.currentMileageList;
        }
        #region 获取每一天的数据
        private void InitData_Day()
        {
            this.Indicator.IsBusy = true;
            this.dayMileageList.Clear();
            this.dataseries.DataPoints.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/getVehicleMileageByDay.ashx";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(new AsyncCallback(this.RequestReady_Day), request);
        }
        private void RequestReady_Day(IAsyncResult asyncResult)
        {
            try
            {
                string jsonStr = "";// JavaScriptConvert.SerializeObject(this.selectedMileage.id);

                HttpWebRequest request = (HttpWebRequest)(asyncResult.AsyncState);
                StreamWriter postStream = new StreamWriter(request.EndGetRequestStream(asyncResult));
                postStream.Write("{0}={1}&{2}={3}&{4}={5}&{6}={7}&", "Request", jsonStr,
                    "startTime", this.startTime.ToString("yyyy-M-d"), "endTime", this.endTime.ToString("yyyy-M-d"), "flag", "days");//flag为"days"就是请求的每天的里程。 
                postStream.Close();
                postStream.Dispose();
                request.BeginGetResponse(new AsyncCallback(this.ResponseReady_Day), request);
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
        private void ResponseReady_Day(IAsyncResult asyncResult)
        {
            try
            {
                HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
                WebResponse response = request.EndGetResponse(asyncResult) as WebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string responseStr = reader.ReadToEnd();
                    this.DataOperate_Day(responseStr);
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
        private void DataOperate_Day(string responseStr)
        {
            if (!string.IsNullOrEmpty(responseStr))
            {
                List<MileageJson> jsonList = null;// (List<MileageJson>)JavaScriptConvert.DeserializeObject(responseStr, typeof(List<MileageJson>));
                int count = 0;
                foreach (MileageJson json in jsonList)
                {
                    foreach (var valuekey in json.mileages)
                    {
                        Mileage m = new Mileage();
                        m.sequence = ++count;
                        m.id = json.vehicleNum;
                        /*找到所属单位*/
                        List<CVBasicInfo> tmpList = StaticBasicInfo.GetInstance().ListClientBasicInfo;
                        foreach (CVBasicInfo selectedInfo in this.selectedVehicleList)
                        {
                            if (selectedInfo.ID == json.vehicleNum)
                            {
                                foreach (CVBasicInfo info in tmpList)
                                {
                                    if (info.ID == selectedInfo.ParentID)
                                    {
                                        m.vehicleNum = selectedInfo.Name;
                                        m.sim = selectedInfo.SIM;
                                        m.parentDepart = info.Name;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        m.mileage = valuekey.Value.ToString();
                        m.time = valuekey.Key.ToString();
                        this.dayMileageList.Add(m); 
                    }
                }
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    this.DayMileageList.ItemsSource = null;
                    this.DayMileageList.ItemsSource = this.dayMileageList;
                    this.tbTitle.Text = this.selectedMileage.vehicleNum + " " + this.selectedMileage.time + "里程柱状图";
                    this.gdTitle.Text = this.selectedMileage.vehicleNum + " " + this.selectedMileage.time + "里程明细表";
                    //添加图标点
                    foreach (Mileage m in this.dayMileageList)
                    {
                        DataPoint dp = new DataPoint();
                        dp.YValue = Convert.ToDouble(m.mileage);
                        dp.AxisXLabel = m.time;
                        this.dataseries.DataPoints.Add(dp);
                    }
                    this.chartTest.Series.Clear();
                    this.chartTest.Series.Add(dataseries);
                    this.Indicator.IsBusy = false;
                });
            }
        }
        #endregion
        private void export_static_Click(object sender, RoutedEventArgs e)
        {

        }

        private void export_detail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TotalMileageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((Mileage)((DataGrid)sender).SelectedItem == null)
            {
                return;
            }
            this.selectedMileage = (Mileage)((DataGrid)sender).SelectedItem;
            this.InitData_Day();
        }
    }
}
