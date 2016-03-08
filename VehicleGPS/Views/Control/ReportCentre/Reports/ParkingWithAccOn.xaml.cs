﻿using System;
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
    /// ParkingWithAccOn.xaml 的交互逻辑
    /// </summary>
    public partial class ParkingWithAccOn : Window
    {
        private List<ParkingInfo> parkingList = new List<ParkingInfo>();
        private List<ParkingInfo> currentParkingList = new List<ParkingInfo>();
        private List<CVBasicInfo> selectedVehicleList;
        private DateTime startTime;
        private DateTime endTime;
        private string timeLimit;
        private int pageSize = 20;

        public ParkingWithAccOn(List<CVBasicInfo> selectedList, DateTime sTime, DateTime eTime, string timelimit)
        {
            InitializeComponent();

            this.selectedVehicleList = selectedList;
            this.startTime = sTime;
            this.endTime = eTime;
            this.timeLimit = timelimit;
            this.InitData();
        }

        private void InitData()
        {
            this.Indicator.IsBusy = true;
            this.parkingList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/ParkingWithAccOnData.ashx";
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
                postStream.Write("{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}&", "SIM", jsonStr,
                    "startTime", this.startTime.ToString("yyyy-MM-dd HH:mm"),
                    "endTime", this.endTime.ToString("yyyy-MM-dd HH:mm"),
                    "time", this.timeLimit, "speed", "1");
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
                List<RunningAnalysisJson> jsonList = null;// (List<RunningAnalysisJson>)JavaScriptConvert.DeserializeObject(responseStr, typeof(List<RunningAnalysisJson>));

                if (jsonList.Count == 0)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        MessageBox.Show("没有找到数据", "提示", MessageBoxButton.OK);
                        this.Indicator.IsBusy = false;
                        this.Close();
                        return;
                    });
                }
                else
                {
                    int count = 0;
                    List<double> sLngList = new List<double>();
                    List<double> sLatList = new List<double>();
                    foreach (RunningAnalysisJson raj in jsonList)
                    {
                        ParkingInfo pInfo = new ParkingInfo();
                        if (string.IsNullOrEmpty(raj.sLatitude.ToString()) || string.IsNullOrEmpty(raj.sLongituse.ToString()))
                        {
                            continue;
                        }
                        pInfo.sequence = ++count;
                        pInfo.startTime = raj.StartTime;
                        pInfo.endTime = raj.EndTime;
                        pInfo.sim = raj.SIM;
                        pInfo.interval = raj.TimeLength == "" ? "0" : raj.TimeLength;
                        /*找到所属单位*/
                        List<CVBasicInfo> tmpList = StaticBasicInfo.GetInstance().ListClientBasicInfo;
                        foreach (CVBasicInfo selectedInfo in this.selectedVehicleList)
                        {
                            if (selectedInfo.SIM == pInfo.sim)
                            {
                                pInfo.vehicleNum = selectedInfo.Name;
                                foreach (CVBasicInfo info in tmpList)
                                {
                                    if (info.ID == selectedInfo.ParentID)
                                    {
                                        pInfo.parentDepart = info.Name;
                                        break;
                                    }
                                }
                                break;
                            }
                        }

                        sLngList.Add(raj.sLongituse);
                        sLatList.Add(raj.sLatitude);
                        this.parkingList.Add(pInfo);
                    }
                    ParseAddress parseAddr = new ParseAddress(sLngList, sLatList);
                    List<PointAddr> sAddrList = parseAddr.BeginParse();
                    for (int i = 0; i < sAddrList.Count; i++)
                    {
                        PointAddr pa = sAddrList[i];
                        this.parkingList[i].lat = pa.lat;
                        this.parkingList[i].lng = pa.lng;
                        this.parkingList[i].address = pa.addr;
                    }

                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        this.PageIndexChanging(0, null);
                        Pager pager = new Pager(this.parkingList.Count, this.pageSize);
                        pager.PageIndexChanging += new PagerIndexChangingEvent(this.PageIndexChanging);
                        this.pagerContainer.Children.Clear();
                        this.pagerContainer.Children.Add(pager);
                        this.Indicator.IsBusy = false;
                    });
                }
            }
            else
            {
                MessageBox.Show("没有找到数据", "提示", MessageBoxButton.OK);
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    this.Indicator.IsBusy = false;
                    this.Close();
                });
            }
        }
        private void PageIndexChanging(int pageIndex, EventArgs e)
        {
            int startIndex, endIndex;
            startIndex = this.pageSize * pageIndex;
            if (startIndex + this.pageSize > this.parkingList.Count)
            {
                endIndex = this.parkingList.Count - 1;
            }
            else
            {
                endIndex = startIndex + this.pageSize - 1;
            }
            this.currentParkingList.Clear();
            for (; startIndex <= endIndex; startIndex++)
            {
                this.currentParkingList.Add(this.parkingList[startIndex]);
            }
            this.dg_Parkinglist.ItemsSource = null;
            this.dg_Parkinglist.ItemsSource = this.currentParkingList;
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