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
using System.Windows.Shapes;
using VehicleGPS.Models;
using VehicleGPS.Views.Control.ReportCentre.Reports.Common;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace VehicleGPS.Views.Control.ReportCentre.Reports
{
    /// <summary>
    /// 违规作业报表 夏创铭
    /// IllegalWork.xaml 的交互逻辑
    /// </summary>
    public partial class IllegalWork : Window
    {
        private List<InfoLeaveWithoutTask> infoillegalList;
        private List<InfoLeaveWithoutTask> currentillegalList;
        private List<CVBasicInfo> selectedVehicleList;
        private int pageSize = 20;
        private DateTime starttime;
        private DateTime endtime;
        public IllegalWork(List<CVBasicInfo> selectedList, DateTime startime, DateTime endtime)
        {
            InitializeComponent();
            this.infoillegalList = new List<InfoLeaveWithoutTask>();
            this.currentillegalList = new List<InfoLeaveWithoutTask>();
            this.selectedVehicleList = selectedList;
            this.starttime = startime;
            this.endtime = endtime;
            this.InitData();
        }
        private void InitData()
        {
            this.Indicator.IsBusy = true;
            this.infoillegalList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/getWarnTask.ashx";
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

                postStream.Write("{0}={1}&{2}={3}&{4}={5}&{6}={7}", "Request", jsonStr, "startTime",
                    this.starttime.ToString("yyyy-M-d"), "endTime", this.endtime.ToString("yyyy-M-d"),
                    "taskType", "wt0002#wt0004");
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
                List<InfoLeaveWithoutTask> InfoIllegalList = null;// (List<InfoLeaveWithoutTask>)JavaScriptConvert.DeserializeObject(responseStr, typeof(List<InfoLeaveWithoutTask>));

                if (InfoIllegalList.Count > 0)
                {
                    for (int i = 0; i < InfoIllegalList.Count; i++)
                    {
                        InfoLeaveWithoutTask infoLeave = new InfoLeaveWithoutTask();
                        infoLeave.Customer = InfoIllegalList[i].Customer;
                        infoLeave.Entertime = infoillegalList[i].Entertime;
                        infoLeave.Inserttime = InfoIllegalList[i].Inserttime;
                        infoLeave.Leavetime = infoillegalList[i].Leavetime;
                        infoLeave.PlanId = InfoIllegalList[i].PlanId;
                        infoLeave.PlanName = InfoIllegalList[i].PlanName;
                        infoLeave.SIM = InfoIllegalList[i].SIM;
                        infoLeave.VehicleId = InfoIllegalList[i].VehicleId;
                        infoLeave.VehicleNum = InfoIllegalList[i].VehicleNum;
                        infoLeave.Warntype = InfoIllegalList[i].Warntype;
                        this.infoillegalList.Add(infoLeave);
                    }
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        this.PageIndexChanging(0, null);
                        Pager pager = new Pager(this.infoillegalList.Count, this.pageSize);
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
            if (startIndex + this.pageSize > this.infoillegalList.Count)
            {
                endIndex = this.infoillegalList.Count - 1;
            }
            else
            {
                endIndex = startIndex + this.pageSize - 1;
            }
            this.currentillegalList.Clear();
            for (; startIndex <= endIndex; startIndex++)
            {
                this.currentillegalList.Add(this.infoillegalList[startIndex]);
            }
            this.InfoIllegalList.ItemsSource = null;
            this.InfoIllegalList.ItemsSource = this.currentillegalList;
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
