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
using System.Net;
using Newtonsoft.Json;
using System.IO;
using VehicleGPS.Views.Control.ReportCentre.Reports.Common;
using Visifire.Charts;

namespace VehicleGPS.Views.Control.ReportCentre.Reports
{
    /// <summary>
    /// OfflineDetails.xaml 的交互逻辑
    /// </summary>
    public partial class OfflineDetails : Window
    {

        private List<InfoOffline> infoOfflineList = new List<InfoOffline>();
        private List<InfoOffline> infoOfflineListCur = new List<InfoOffline>();
        private List<InfoOffline> listInfoOfflineToday = new List<InfoOffline>();
        private List<InfoOffline> listInfoOfflineFive = new List<InfoOffline>();
        private List<InfoOffline> listInfoOfflineMore = new List<InfoOffline>();
        private List<InfoOffline> listInfoOfflineTodayCur = new List<InfoOffline>();
        private List<InfoOffline> listInfoOfflineFiveCur = new List<InfoOffline>();
        private List<InfoOffline> listInfoOfflineMoreCur = new List<InfoOffline>();
        private List<CVBasicInfo> selectedVehicleList;
        private InfoOffline selectedOffline;
        private int pageSize = 20;
        private int flag = 0;//0全部，1,2,3表示今天，五天，多于五天
        private int pieFlag = 0;//饼图状态
        private DataSeries dataseries;//图标显示
        public OfflineDetails(List<CVBasicInfo> selectedList)
        {
            InitializeComponent();
            this.InitBaiduMap();
            this.selectedVehicleList = selectedList;

            //初始化图标属性
            this.dataseries = new DataSeries();
            dataseries.RenderAs = RenderAs.Pie;
            dataseries.BorderThickness = new Thickness(0);
            dataseries.Background = new SolidColorBrush(Colors.Transparent);
            dataseries.LabelEnabled = true;
            this.chartTest1.View3D = true;//3D显示
            Title title = new Visifire.Charts.Title();
            title.Text = "离线车辆按天统计图";
            this.chartTest1.Titles.Add(title);

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
            this.infoOfflineList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/GetOfflineVehicle.ashx";
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
                foreach (CVBasicInfo info in this.selectedVehicleList)
                {
                    tmpList.Add(info.ID);
                }

                string jsonStr = "";// JavaScriptConvert.SerializeObject(tmpList);
                HttpWebRequest request = (HttpWebRequest)(asyncResult.AsyncState);
                StreamWriter postStream = new StreamWriter(request.EndGetRequestStream(asyncResult));
                postStream.Write("{0}={1}", "Request", jsonStr);
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
                List<LastTimeJson> offJson = null;// (List<LastTimeJson>)JavaScriptConvert.DeserializeObject(responseStr, typeof(List<LastTimeJson>));

                List<double> lngList = new List<double>();
                List<double> latList = new List<double>();
                int count = 0;
                for (int i = 0; i < offJson.Count; i++)
                {
                    if (string.IsNullOrEmpty(offJson[i].lastTime))
                    {//无车辆信息
                        continue;
                    }
                    
                    foreach (CVBasicInfo selectedInfo in this.selectedVehicleList)
                    {
                        if (selectedInfo.ID == offJson[i].VehicleNum)
                        {
                            InfoOffline infoOff = new InfoOffline();
                            infoOff.sequence = ++count;
                            infoOff.vehicleNum = selectedInfo.Name;
                            infoOff.sim = selectedInfo.SIM;
                            infoOff.lastTime = offJson[i].lastTime;
                            infoOff.offTime = this.GetOffTimeSum(infoOff.lastTime);
                            this.AllocateOffInfo(infoOff.lastTime, infoOff);
                            string parentTmp = selectedInfo.ParentID;
                            /*找到所属单位*/
                            List<CVBasicInfo> tmpList = StaticBasicInfo.GetInstance().ListClientBasicInfo;
                            foreach (CVBasicInfo info in tmpList)
                            {
                                if (info.ID == parentTmp)
                                {
                                    infoOff.parentDepart = info.Name;
                                    break;
                                }
                            }
                            this.infoOfflineList.Add(infoOff);
                            lngList.Add(Convert.ToDouble(offJson[i].lng));
                            latList.Add(Convert.ToDouble(offJson[i].lat));
                            break;
                        }
                    }
                }
                ParseAddress parseAddr = new ParseAddress(lngList, latList);
                List<PointAddr> pointAddrList = parseAddr.BeginParse();
                for (int i = 0; i < pointAddrList.Count; i++)
                {
                    PointAddr pa = pointAddrList[i];
                    this.infoOfflineList[i].lat = pa.lat;
                    this.infoOfflineList[i].lng = pa.lng;
                    this.infoOfflineList[i].address = pa.addr;
                }

                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    this.PageIndexChanging(0, null);
                    Pager pager = new Pager(this.infoOfflineList.Count, this.pageSize);
                    pager.PageIndexChanging += new PagerIndexChangingEvent(this.PageIndexChanging);
                    this.pagerContainer.Children.Clear();
                    this.pagerContainer.Children.Add(pager);


                    DataPoint pointToady;
                    pointToady = new DataPoint();
                    pointToady.AxisXLabel = "今天离线";
                    pointToady.YValue = this.listInfoOfflineToday.Count;
                    pointToady.MouseLeftButtonUp += new MouseButtonEventHandler(point_MouseLeftButtonUp);
                    this.dataseries.DataPoints.Add(pointToady);

                    DataPoint pointFive;
                    pointFive = new DataPoint();
                    pointFive.AxisXLabel = "离线1-5天";
                    pointFive.YValue = this.listInfoOfflineFive.Count;
                    pointFive.MouseLeftButtonUp += new MouseButtonEventHandler(point_MouseLeftButtonUp);
                    this.dataseries.DataPoints.Add(pointFive);

                    DataPoint pointMore;
                    pointMore = new DataPoint();
                    pointMore.AxisXLabel = "离线5天以上";
                    pointMore.YValue = this.listInfoOfflineMore.Count;
                    pointMore.MouseLeftButtonUp += new MouseButtonEventHandler(point_MouseLeftButtonUp);
                    this.dataseries.DataPoints.Add(pointMore);

                    this.chartTest1.Series.Add(this.dataseries);

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

        //饼图点击事件 
        void point_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            DataPoint point = sender as DataPoint;
            if (pieFlag == 0)
            {
                point.Exploded = true;
                pieFlag = 1;
            }
            else
            {
                point.Exploded = false;
                pieFlag = 0;
            }
            int countTmp = 0;
            if (point.AxisXLabel.Equals("今天离线"))
            {
                flag = 1;
                countTmp = this.listInfoOfflineToday.Count;
            }
            else if (point.AxisXLabel.Equals("离线1-5天"))
            {
                flag = 2; ;
                countTmp = this.listInfoOfflineFive.Count;
            }
            else if (point.AxisXLabel.Equals("离线5天以上"))
            {
                flag = 3;
                countTmp = this.listInfoOfflineMore.Count;
            }
            this.PageIndexChanging(0, null);
            Pager pager = new Pager(countTmp, this.pageSize);
            pager.PageIndexChanging += new PagerIndexChangingEvent(this.PageIndexChanging);
            this.pagerContainer.Children.Clear();
            this.pagerContainer.Children.Add(pager);
        }

        private void PageIndexChanging(int pageIndex, EventArgs e)
        {
            int startIndex, endIndex;
            startIndex = this.pageSize * pageIndex;
            List<InfoOffline> tmp, tmpCur;
            if (this.flag == 0)
            {
                tmp = this.infoOfflineList;
                tmpCur = this.infoOfflineListCur;
            }
            else if (this.flag == 1)
            {
                tmp = this.listInfoOfflineToday;
                tmpCur = this.listInfoOfflineTodayCur;
            }
            else if (this.flag == 2)
            {
                tmp = this.listInfoOfflineFive;
                tmpCur = this.listInfoOfflineFiveCur;
            }
            else
            {
                tmp = this.listInfoOfflineMore;
                tmpCur = this.listInfoOfflineMoreCur;
            }
            if (startIndex + this.pageSize > tmp.Count)
            {
                endIndex = tmp.Count - 1;
            }
            else
            {
                endIndex = startIndex + this.pageSize - 1;
            }
            tmpCur.Clear();
            for (; startIndex <= endIndex; startIndex++)
            {
                tmpCur.Add(tmp[startIndex]);
            }
            this.InfoOfflineList.ItemsSource = null;
            this.InfoOfflineList.ItemsSource = tmpCur;
        }

        //获取离线时长
        private string GetOffTimeSum(string lastTime)
        {
            string SumOffTime = "";
            DateTime dt = DateTime.ParseExact(lastTime, "yyyy/M/d HH:mm:ss", null);
            TimeSpan sp = new TimeSpan(); 
            sp = DateTime.Now - dt;
            if (sp.Days > 0)
            {
                SumOffTime += sp.Days + "天";
            }
            if (sp.Hours > 0)
            {
                SumOffTime += sp.Hours + "小时";
            }
            if (sp.Minutes > 0)
            {
                SumOffTime += sp.Minutes + "分";
            }
            if (sp.Seconds > 0)
            {
                SumOffTime += sp.Seconds + "秒";
            }
            return SumOffTime;
        }

        //以离线时长分别管理信息
        private void AllocateOffInfo(string lastTime,InfoOffline infoOff)
        {
            TimeSpan sp = new TimeSpan();
            DateTime dt = DateTime.ParseExact(lastTime, "yyyy/M/d HH:mm:ss", null);
            sp = DateTime.Now - dt;

            if (sp.Days == 0)
            {
                infoOff.sequence = this.listInfoOfflineToday.Count + 1;
                this.listInfoOfflineToday.Add(infoOff);
            }
            if (sp.Days > 0)
            {
                if (sp.Days < 6 && sp.Days > 0)
                {
                    infoOff.sequence = this.listInfoOfflineFive.Count + 1;
                    this.listInfoOfflineFive.Add(infoOff);
                }
                else if (sp.Days > 5)
                {
                    infoOff.sequence = this.listInfoOfflineMore.Count + 1;
                    this.listInfoOfflineMore.Add(infoOff);
                }
            }
        }
        private void export_static_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Offline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InfoOffline tmp = (InfoOffline)((DataGrid)sender).SelectedItem;
            if (tmp != null)
            {
                this.selectedOffline = tmp;
                string cont = this.GetHtml(this.selectedOffline.vehicleNum, this.selectedOffline.sim,
                    this.selectedOffline.lastTime, this.selectedOffline.parentDepart, this.selectedOffline.offTime, this.selectedOffline.address);
                this.MyWeb.InvokeScript("SetMarker", new object[] { this.selectedOffline.lng, this.selectedOffline.lat, cont });
            }
        }

        public string GetHtml(string licenseNum, string SIM, string uploadTime, string parentDepart, string offTime, string location)
        {
            string ret = "";
            ret += "<table style='font-family:verdana;font-size:12;color:black'>";
            ret += "<tr><td>车牌号：</td><td>";
            ret += licenseNum + "</td></tr>";
            ret += "<tr><td>SIM卡号：</td><td>";
            ret += SIM + "</td></tr>";
            ret += "<tr><td>所属单位：</td><td>";
            ret += parentDepart + "</td></tr>";
            ret += "<tr><td>上报时间：</td><td>";
            ret += uploadTime + "</td></tr>";
            ret += "<tr><td>离线时长：</td><td>";
            ret += offTime + "</td></tr>";
            ret += "<tr><td>离线位置：</td><td>";
            ret += location + "</td></tr>";
            ret += "</table>";
            return ret;
        }
    }
}

