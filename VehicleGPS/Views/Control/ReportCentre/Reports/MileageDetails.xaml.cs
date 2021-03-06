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
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;


namespace VehicleGPS.Views.Control.ReportCentre.Reports
{
    /// <summary>
    /// MileageDetails.xaml 的交互逻辑
    /// </summary>
    public partial class MileageDetails : Window
    {
        private List<Mileage> mileageList;
        private List<Mileage> currentMileageList;
        private List<CVBasicInfo> selectedVehicleList;
        private DateTime startTime;
        private DateTime endTime;
        private int pageSize = 20;

        public MileageDetails(List<CVBasicInfo> selectedList,DateTime sTime,DateTime eTime)
        {
            InitializeComponent();
            this.selectedVehicleList = selectedList;
            this.startTime = sTime;
            this.endTime = eTime;
            this.mileageList = new List<Mileage>();
            this.currentMileageList = new List<Mileage>();
            //this.currentMileageList
            this.InitData();
        }

        private void InitData()
        {
            this.Indicator.IsBusy = true;
            this.mileageList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/getVehicleMileageDetail.ashx";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(new AsyncCallback(this.RequestReady), request);
        }
        private void RequestReady(IAsyncResult asyncResult)
        {
            try
            {
                List<MileageJson> jsonList = new List<MileageJson>();
                foreach (CVBasicInfo info in this.selectedVehicleList)
                {
                    MileageJson json = new MileageJson();
                    json.vehicleNum = info.ID;
                    Dictionary<string, double> timeDic = new Dictionary<string, double>();
                    timeDic.Add("startTime", this.startTime.ToOADate());
                    timeDic.Add("endTime", this.endTime.ToOADate());
                    //timeDic.Add("startTime", DateTime.Now.ToOADate());
                    //timeDic.Add("endTime", DateTime.Now.ToOADate());
                    json.mileages = timeDic;
                    jsonList.Add(json);
                }
                string jsonStr = "";// JavaScriptConvert.SerializeObject(jsonList);
                

                HttpWebRequest request = (HttpWebRequest)(asyncResult.AsyncState);
                StreamWriter postStream = new StreamWriter(request.EndGetRequestStream(asyncResult));
                postStream.Write("{0}={1}&", "Request", jsonStr);
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
                    this.Dispatcher.BeginInvoke((Action)delegate() { this.Indicator.IsBusy = false; });
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
                int count = 0;
                List<MileageJson> jsonList = null;// (List<MileageJson>)JavaScriptConvert.DeserializeObject(responseStr, typeof(List<MileageJson>));
                foreach (MileageJson json in jsonList)
                {
                    foreach (var valuekey in json.mileages)
                    {
                        Mileage m = new Mileage();
                        m.sequence = ++count;
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
                        m.time = valuekey.Key;
                        m.mileage = valuekey.Value.ToString();
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
            this.MileageList.ItemsSource = null;
            this.MileageList.ItemsSource = this.currentMileageList;
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
