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
    /// HistoryTrack.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryTrack : Window
    {
        private List<InfoTrack> trackInfoList = new List<InfoTrack>();
        private List<InfoTrack> currentTrackInfoList = new List<InfoTrack>();
        private List<CVBasicInfo> selectedVehicleList;
        private DateTime startTime;
        private DateTime endTime;
        private string speed;
        private int pageSize = 20;
        private bool firstLoadColumn = false;//第一次加载选择列

        public HistoryTrack(List<CVBasicInfo> selectedList, DateTime sTime, DateTime eTime, string speed)
        {
            InitializeComponent();

            this.selectedVehicleList = selectedList;
            this.startTime = sTime;
            this.endTime = eTime;
            this.speed = speed;
            this.InitData();
        }
        private void InitData()
        {
            this.Indicator.IsBusy = true;
            this.trackInfoList.Clear();
            string url = VehicleConfig.GetInstance().concrete_webServer_url + "Reports/GetHistoryTracks.ashx";
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
                    "StartTime", this.startTime.ToString("yyyy-MM-dd HH:mm"), 
                    "EndTime", this.endTime.ToString("yyyy-MM-dd HH:mm"),
                    "Speed", speed);
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
                List<Dictionary<string, object>> jsonList = null;// (List<Dictionary<string, object>>)JavaScriptConvert.DeserializeObject(responseStr, typeof(List<Dictionary<string, object>>));

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
                    foreach (Dictionary<string, object> item in jsonList)
                    {
                        InfoTrack tmp = new InfoTrack();
                        tmp.serial = ++count;
                        tmp.longitude = Double.Parse(item["longitude"].ToString());
                        tmp.latitude = Double.Parse(item["latitude"].ToString());
                        tmp.direction = VehicleCommon.translateDirection(item["direction"].ToString());//changed by shiwei 2014/01/19
                        tmp.gpsStatus = item["gpsStatus"].ToString();
                        tmp.altitude = Double.Parse(item["altitude"].ToString());
                        tmp.speed = Double.Parse(item["speed"].ToString());
                        tmp.devSpeed = Double.Parse(item["devSpeed"].ToString());
                        tmp.mileage = Double.Parse(item["mileage"].ToString());
                        tmp.oilVolumn = Double.Parse(item["oilVolumn"].ToString());
                        tmp.recordtime = item["recordtime"].ToString();
                        tmp.idtime = item["idtime"].ToString();
                        tmp.inserttime =Convert.ToDateTime(item["inserttime"]).ToString("yyyy/MM/dd HH:mm:ss");
                        tmp.accstatus = item["accstatus"].ToString();
                        tmp.workstatus = item["workstatus"].ToString();
                        tmp.llsecret = item["llsecret"].ToString();
                        tmp.gpsmode = item["gpsmode"].ToString();
                        tmp.oilwaystatus = item["oilwaystatus"].ToString();
                        tmp.vcstatus = item["vcstatus"].ToString();
                        tmp.vdstatus = item["vdstatus"].ToString();
                        tmp.fdstatus = item["fdstatus"].ToString();
                        tmp.bdstatus = item["bdstatus"].ToString();
                        tmp.enginestatus = item["enginestatus"].ToString();
                        tmp.conditionerstatus = item["conditionerstatus"].ToString();
                        tmp.brakestatus = item["brakestatus"].ToString();
                        tmp.ltstatus = item["ltstatus"].ToString();
                        tmp.rtstatus = item["rtstatus"].ToString();
                        tmp.farlstatus = item["farlstatus"].ToString();
                        tmp.nearlstatus = item["nearlstatus"].ToString();
                        tmp.pnstatus = item["pnstatus"].ToString();
                        tmp.shakestatus = item["shakestatus"].ToString();
                        tmp.hornstatus = item["hornstatus"].ToString();
                        tmp.protectstatus = item["protectstatus"].ToString();
                        tmp.loadstatus = item["loadstatus"].ToString();
                        tmp.busstatus = item["busstatus"].ToString();
                        tmp.gsmstatus = item["gsmstatus"].ToString();
                        tmp.lcstatus = item["lcstatus"].ToString();
                        tmp.ffstatus = item["ffstatus"].ToString();
                        tmp.bfstatus = item["bfstatus"].ToString();
                        tmp.gpsantstatus = item["gpsantstatus"].ToString();
                        tmp.soswarn = item["soswarn"].ToString();
                        tmp.overspeedwarn = item["overspeedwarn"].ToString();
                        tmp.tiredwarn = item["tiredwarn"].ToString();
                        tmp.prewarn = item["prewarn"].ToString();
                        tmp.gnssfatal = item["gnssfatal"].ToString();
                        tmp.gnssantwarn = item["gnssantwarn"].ToString();
                        tmp.lowvolwarn = item["lowvolwarn"].ToString();
                        tmp.highvolwarn = item["highvolwarn"].ToString();
                        tmp.outagewarn = item["outagewarn"].ToString();
                        tmp.lcdfatalwarn = item["lcdfatalwarn"].ToString();
                        tmp.ttsfatalwarn = item["ttsfatalwarn"].ToString();
                        tmp.camerafatalwarn = item["camerafatalwarn"].ToString();
                        tmp.vediolosewarn = item["vediolosewarn"].ToString();
                        tmp.accumtimeout = item["accumtimeout"].ToString();
                        tmp.stoptimeout = item["stoptimeout"].ToString();
                        tmp.inoutareawarn = item["inoutareawarn"].ToString();
                        tmp.inoutlinewarn = item["inoutlinewarn"].ToString();
                        tmp.drivingtimewarn = item["drivingtimewarn"].ToString();
                        tmp.deviatewarn = item["deviatewarn"].ToString();
                        tmp.vssfatalwarn = item["vssfatalwarn"].ToString();
                        tmp.oilexceptionwarn = item["oilexceptionwarn"].ToString();
                        tmp.vehiclestolenwarn = item["vehiclestolenwarn"].ToString();
                        tmp.illignitewarn = item["illignitewarn"].ToString();
                        tmp.illmovewarn = item["illmovewarn"].ToString();
                        tmp.crashwarn = item["crashwarn"].ToString();
                        tmp.sdexceptionwarn = item["sdexceptionwarn"].ToString();
                        tmp.robwarn = item["robwarn"].ToString();
                        tmp.sleeptimewarn = item["sleeptimewarn"].ToString();
                        tmp.illtimedrivingwarn = item["illtimedrivingwarn"].ToString();
                        tmp.overstationwarn = item["overstationwarn"].ToString();
                        tmp.ilopendoorwarn = item["ilopendoorwarn"].ToString();
                        tmp.protectwarn = item["protectwarn"].ToString();
                        tmp.trimmingwarn = item["trimmingwarn"].ToString();
                        tmp.passwdwarn = item["passwdwarn"].ToString();
                        tmp.prohibitmovewarn = item["prohibitmovewarn"].ToString();
                        tmp.illstopwarn = item["illstopwarn"].ToString();
                        tmp.gnssshortwarn = item["gnssshortwarn"].ToString();
                        tmp.vedioshelterwarn = item["vedioshelterwarn"].ToString();

                        this.trackInfoList.Add(tmp);
                    }
                    /*找到所属单位*/
                    List<CVBasicInfo> tmpList = StaticBasicInfo.GetInstance().ListClientBasicInfo;
                    foreach (CVBasicInfo selectedInfo in this.selectedVehicleList)
                    {
                        foreach (CVBasicInfo info in tmpList)
                        {
                            if (info.ID == selectedInfo.ParentID)
                            {
                                foreach(InfoTrack track in this.trackInfoList)
                                {
                                    track.vehicleNum = selectedInfo.Name;
                                    track.id = selectedInfo.ID;
                                    track.sim = selectedInfo.SIM;
                                    track.parentDepart = info.Name;
                                }
                                break;
                            }
                        }
                    }
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        this.PageIndexChanging(0, null);
                        Pager pager = new Pager(this.trackInfoList.Count, this.pageSize);
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
            if (startIndex + this.pageSize > this.trackInfoList.Count)
            {
                endIndex = this.trackInfoList.Count - 1;
            }
            else
            {
                endIndex = startIndex + this.pageSize - 1;
            }
            this.currentTrackInfoList.Clear();
            for (; startIndex <= endIndex; startIndex++)
            {
                this.currentTrackInfoList.Add(this.trackInfoList[startIndex]);
            }
            this.dg_HistoryTrackList.ItemsSource = null;
            this.dg_HistoryTrackList.ItemsSource = this.currentTrackInfoList;
        }

        private void ColumnsList_Click(object sender, RoutedEventArgs e)
        {
            if (this.firstLoadColumn == false)
            {
                this.dg_ColumnsList.ItemsSource = null;
                this.dg_ColumnsList.ItemsSource = this.dg_HistoryTrackList.Columns;
                this.firstLoadColumn = true;
            }
            if (this.btn_ColumnsList.Content.Equals("选列"))
            {
                this.btn_ColumnsList.Content = "确定";
                this.border_ColumnList.Visibility = Visibility.Visible;
            }
            else
            {
                this.btn_ColumnsList.Content = "选列";
                this.border_ColumnList.Visibility = Visibility.Collapsed;
            }
        }

        private void export_static_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class DisplayConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((bool)value) ? (Visibility.Visible) : (Visibility.Collapsed);
        }
    }
}
