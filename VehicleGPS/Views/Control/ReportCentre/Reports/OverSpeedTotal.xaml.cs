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

namespace VehicleGPS.Views.Control.ReportCentre
{
    /// <summary>
    /// OverSpeedTotal.xaml 的交互逻辑
    /// </summary>
    public partial class OverSpeedTotal : Window
    {
        private List<InfoOverSpeed> overSpeedTotal = new List<InfoOverSpeed>();
        private List<InfoOverSpeed> overSpeedList = new List<InfoOverSpeed>();
        //private List<InfoOverSpeed> currentOverSpeedList = new List<InfoOverSpeed>();
        //private List<InfoOverSpeedDetail> overSpeedDetailList = new List<InfoOverSpeedDetail>();
        //private List<InfoOverSpeedDetail> currentOverSpeedDetailList = new List<InfoOverSpeedDetail>();
        private int selectedIndex;
        private int selectedDetailIndex;
        private List<CVBasicInfo> selectedVehicleList;
        private DateTime startTime;
        private DateTime endTime;
        private string timeLimit;
        private int pageSize = 20;

        public OverSpeedTotal(List<CVBasicInfo> selectedList, DateTime sTime, DateTime eTime, string timelimit)
        {
            InitializeComponent();
            this.InitBaiduMap();
            this.selectedVehicleList = selectedList;
            this.startTime = sTime;
            this.endTime = eTime;
            this.timeLimit = timelimit;
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
            this.overSpeedList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/GetOverSpeedVehicle.ashx";
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
                postStream.Write("{0}={1}&{2}={3}&{4}={5}&{6}={7}&", "Request", jsonStr,
                    "startTime", this.startTime.ToString("yyyy-MM-dd HH:mm"), "endTime",
                    this.endTime.ToString("yyyy-MM-dd HH:mm"), "continuedTime", this.timeLimit);
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
                Dictionary<string, List<OverSpeedJson>> jsonDic = null;// (Dictionary<string, List<OverSpeedJson>>)JavaScriptConvert.DeserializeObject(responseStr, typeof(Dictionary<string, List<OverSpeedJson>>));

                if (jsonDic.Count == 0)
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
                else
                {
                    int count = 0;
                    foreach (KeyValuePair<string, List<OverSpeedJson>> kvp in jsonDic)
                    {
                        InfoOverSpeed infoOverSpeed = new InfoOverSpeed();
                        infoOverSpeed.sequence = ++count;
                        infoOverSpeed.overSpeedTimes = kvp.Value.Count;
                        infoOverSpeed.startTime = this.startTime.ToString("yyyy-MM-dd HH:mm");
                        infoOverSpeed.endTime = this.endTime.ToString("yyyy-MM-dd HH:mm");
                        
                        /*找到所属单位*/
                        List<CVBasicInfo> tmpList = StaticBasicInfo.GetInstance().ListClientBasicInfo;
                        foreach (CVBasicInfo selectedInfo in this.selectedVehicleList)
                        {
                            if (selectedInfo.ID == kvp.Key)
                            {
                                infoOverSpeed.sim = selectedInfo.SIM;
                                infoOverSpeed.id = selectedInfo.ID;
                                infoOverSpeed.vehicleNum = selectedInfo.Name;
                                foreach (CVBasicInfo info in tmpList)
                                {
                                    if (info.ID == selectedInfo.ParentID)
                                    {
                                        infoOverSpeed.parentDepart = info.Name;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        double totalMilage = 0.0;
                        List<double> lngList = new List<double>();
                        List<double> latList = new List<double>();
                        for (int i = 0; i < kvp.Value.Count; i++)
                        {
                            
                            InfoOverSpeedDetail infoOverSpeedDetail = new InfoOverSpeedDetail();
                            infoOverSpeedDetail.sequence = i + 1;
                            infoOverSpeedDetail.parentDepart = infoOverSpeed.parentDepart;
                            infoOverSpeedDetail.id = infoOverSpeed.id;
                            infoOverSpeedDetail.vehicleNum = infoOverSpeed.vehicleNum;
                            infoOverSpeedDetail.sim = infoOverSpeed.sim;
                            infoOverSpeedDetail.startTime = kvp.Value[i].startTime;
                            infoOverSpeedDetail.endTime = kvp.Value[i].endTime;
                            infoOverSpeedDetail.mileage = kvp.Value[i].mileage;
                            infoOverSpeedDetail.lng = kvp.Value[i].lng;
                            infoOverSpeedDetail.lat = kvp.Value[i].lat;
                            infoOverSpeedDetail.listLngLat = kvp.Value[i].listLngLat;
                            infoOverSpeedDetail.maxSpeed = Convert.ToDouble(kvp.Value[i].maxSpeed).ToString("0.000");
                            infoOverSpeedDetail.mileage = kvp.Value[i].mileage;
                            infoOverSpeedDetail.continuedTime = kvp.Value[i].continusTime;
                            /*解析地址和坐标转换*/
                            latList.Clear();
                            lngList.Clear();
                            foreach (LngLat ll in infoOverSpeedDetail.listLngLat)
                            {
                                lngList.Add(Convert.ToDouble(ll.lng));
                                latList.Add(Convert.ToDouble(ll.lat));
                            }
                            ParseAddress parseDetailAddr = new ParseAddress(lngList, latList);
                            List<PointAddr> detailAddrList = parseDetailAddr.BeginParse();
                            for (int j = 0; j < detailAddrList.Count; j++)
                            {
                                PointAddr pa = detailAddrList[j];
                                if (j == 0)
                                {
                                    infoOverSpeedDetail.lat = pa.lat;
                                    infoOverSpeedDetail.lng = pa.lng;
                                    infoOverSpeedDetail.address = pa.addr;
                                }
                                infoOverSpeedDetail.listLngLat[j].lat = pa.lat;
                                infoOverSpeedDetail.listLngLat[j].lng = pa.lng;
                                infoOverSpeedDetail.listLngLat[j].addr = pa.addr;
                            }

                            infoOverSpeed.overSpeedDetailList.Add(infoOverSpeedDetail);
                            totalMilage += kvp.Value[i].mileage;
                        }
                        infoOverSpeed.mileage = totalMilage.ToString();
                        this.overSpeedList.Add(infoOverSpeed);
                    }

                    /*数据整合*/
                    foreach (InfoOverSpeed info in this.overSpeedList)
                    {
                        int i = 0;
                        for (; i < this.overSpeedTotal.Count; i++)
                        {
                            if (info.parentDepart == this.overSpeedTotal[i].parentDepart)
                            {
                                this.overSpeedTotal[i].mileage += info.mileage;
                                this.overSpeedTotal[i].overSpeedTimes += info.overSpeedTimes;
                                foreach (InfoOverSpeedDetail detailInfo in info.overSpeedDetailList)
                                {
                                    detailInfo.sequence = this.overSpeedTotal[i].overSpeedDetailList.Count + 1;
                                    this.overSpeedTotal[i].overSpeedDetailList.Add(detailInfo);
                                }
                                break;
                            }
                        }
                        //没有找到该单位
                        if (i == this.overSpeedTotal.Count)
                        {
                            InfoOverSpeed newInfo = new InfoOverSpeed();
                            newInfo.sequence = this.overSpeedTotal.Count + 1;
                            newInfo.parentDepart = info.parentDepart;
                            newInfo.startTime = info.startTime;
                            newInfo.endTime = info.endTime;
                            newInfo.overSpeedTimes = info.overSpeedTimes;
                            newInfo.mileage = info.mileage;
                            foreach (InfoOverSpeedDetail detailInfo in info.overSpeedDetailList)
                            {
                                detailInfo.sequence = newInfo.overSpeedDetailList.Count + 1;
                                newInfo.overSpeedDetailList.Add(detailInfo);
                            }
                            this.overSpeedTotal.Add(newInfo);
                        }
                    }
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        this.dg_OverSpeedData.ItemsSource = null;
                        this.dg_OverSpeedData.ItemsSource = this.overSpeedTotal;
                        this.MyWeb.Visibility = Visibility.Visible;
                        this.Indicator.IsBusy = false;
                    });
                }
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
        private void export_static_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*查看超速明细*/
        private void OverSpeedData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedIndex = ((DataGrid)sender).SelectedIndex;
            this.dg_OverSpeedDetailList.ItemsSource = null;
            this.dg_OverSpeedDetailList.ItemsSource = this.overSpeedTotal[this.selectedIndex].overSpeedDetailList;
        }

        private void dg_OverSpeedDetailList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedDetailIndex = ((DataGrid)sender).SelectedIndex;
            string latList = "", lngList = "", contList = "";
            InfoOverSpeedDetail detailInfo = this.overSpeedTotal[this.selectedIndex].overSpeedDetailList[this.selectedDetailIndex];
            if (detailInfo.listLngLat.Count > 0)
            {
                LngLat firstLngLat = detailInfo.listLngLat[0];
                latList = firstLngLat.lat.ToString();
                lngList = firstLngLat.lng.ToString();
                contList = this.GetHtml(detailInfo.vehicleNum, detailInfo.sim, detailInfo.parentDepart, firstLngLat.addr);
            }
            for (int i = 1; i < detailInfo.listLngLat.Count; i++)
            {
                LngLat ll = detailInfo.listLngLat[i];
                latList += "$" + ll.lat.ToString();
                lngList += "$" + ll.lng.ToString();
                contList += "$" + this.GetHtml(detailInfo.vehicleNum, detailInfo.sim, detailInfo.parentDepart, ll.addr);
            }
            this.MyWeb.InvokeScript("OverSpeedTrack", new object[] { lngList, latList, contList });
        }

        public string GetHtml(string licenseNum, string SIM, string parentDepart, string location)
        {
            string ret = "";
            ret += "<table style='font-family:verdana;font-size:12;color:black'>";
            ret += "<tr><td>车牌号：</td><td>";
            ret += licenseNum + "</td></tr>";
            ret += "<tr><td>SIM卡号：</td><td>";
            ret += SIM + "</td></tr>";
            ret += "<tr><td>所属单位：</td><td>";
            ret += parentDepart + "</td></tr>";
            ret += "<tr><td>所在位置：</td><td>";
            ret += location + "</td></tr>";
            ret += "</table>";
            return ret;
        }
    }
}
