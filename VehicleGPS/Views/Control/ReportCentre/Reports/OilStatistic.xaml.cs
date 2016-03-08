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
using Visifire.Charts;
using VehicleGPS.Models;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using VehicleGPS.Views.Control.ReportCentre.Reports.Common;

namespace VehicleGPS.Views.Control.ReportCentre
{
    /// <summary>
    /// OilStatistic.xaml 的交互逻辑
    /// </summary>
    public partial class OilStatistic : Window
    {
        private CVBasicInfo selectedVehicle;
        private DataSeries dataseries;//图标显示
        private DateTime startTime;
        private DateTime endTime;

        public OilStatistic(List<CVBasicInfo> selectedList, DateTime sTime, DateTime eTime)
        {
            InitializeComponent();
            this.selectedVehicle = selectedList[0];
            this.startTime = sTime;
            this.endTime = eTime;

            //初始化图标属性
            this.dataseries = new DataSeries();
            dataseries.RenderAs = RenderAs.Line;
            dataseries.BorderThickness = new Thickness(0);
            dataseries.Background = new SolidColorBrush(Colors.Transparent);
            dataseries.LabelEnabled = true;
            chartTest1.View3D = true;//3D显示
            
            this.InitData();
        }
        private void InitData()
        {
            this.Indicator.IsBusy = true;
            this.dataseries.DataPoints.Clear();

            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/getOilStatistic.ashx";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(new AsyncCallback(this.RequestReady), request);
        }
        private void RequestReady(IAsyncResult asyncResult)
        {
            try
            {
                string jsonStr = "";// JavaScriptConvert.SerializeObject(this.selectedVehicle.ID);

                HttpWebRequest request = (HttpWebRequest)(asyncResult.AsyncState);
                StreamWriter postStream = new StreamWriter(request.EndGetRequestStream(asyncResult));
                postStream.Write("{0}={1}&{2}={3}&{4}={5}&", "Request", jsonStr,
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
                oilstatis oilList = null; // JavaScriptConvert.DeserializeObject(responseStr,
                    // typeof(oilstatis)) as oilstatis;
                List<string> listDate = new List<string>();
                List<double> listVolmun = new List<double>();
                if (oilList.oilVolume.Count > 0)
                {
                    listDate = oilList.datetime;
                    listVolmun = oilList.oilVolume;

                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        string showtime = this.startTime.ToString("yyyy/MM/dd");
                        this.tbTitle.Text = "油耗统计图(" + showtime + ")   " + this.selectedVehicle.Name;
                        for (int i = 0; i < listVolmun.Count; i++)
                        {
                            DataPoint dp = new DataPoint();
                            dp.YValue = listVolmun[i];
                            dp.AxisXLabel = listDate[i].Split(' ')[1];
                            dataseries.DataPoints.Add(dp);
                        }
                        //重新绑定数据
                        this.chartTest1.Series.Clear();
                        this.chartTest1.Series.Add(dataseries);
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
    }
}
