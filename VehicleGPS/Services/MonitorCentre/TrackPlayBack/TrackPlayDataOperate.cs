using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack;
using VehicleGPS.Models.MonitorCentre;
using VehicleGPS.Models;
using System.Net;
using System.Xml;
using System.IO;

namespace VehicleGPS.Services.MonitorCentre.TrackPlayBack
{
    class TrackPlayDataOperate
    {
        /*获取历史轨迹信息*/
        private BusinessDataServiceWEB ServiceWeb;
        private TrackPlayViewModel trackPlayVM;
        public TrackPlayDataOperate(TrackPlayViewModel trackVM)
        {
            this.trackPlayVM = trackVM;
            this.ServiceWeb = new BusinessDataServiceWEB();
        }
        //默认构造函数，用于其他地方定义该类的对象
        public TrackPlayDataOperate()
        {

        }
        #region 获取历史轨迹数据
        /// <summary>
        /// 获取车辆轨迹数据
        /// </summary>
        public void GetTrackPlayInfo()
        {
            this.ServiceWeb.GetTrackPlayVehicleGPSInfo(trackPlayVM);
        }
        #endregion

        #region 根据历史轨迹数据获取其他数据源
        public void InitOtherDataInfo()
        {
            StaticTreeState.StopInfoLoad = LoadingState.LOADCOMPLETE;
            StaticTreeState.OverSpeedInfoLoad = LoadingState.LOADCOMPLETE;
            Thread stopThread = new Thread(new ThreadStart(this.InitVehicleStopInfo));
            stopThread.Start();
            Thread warnThread = new Thread(new ThreadStart(this.GetWarnInfo));
            warnThread.Start();
            Thread overSpeedThread = new Thread(new ThreadStart(this.GetOverSpeedDada));
            overSpeedThread.Start();
            Thread OnlineThread = new Thread(new ThreadStart(this.InitVehicleOnlineInfo));
            OnlineThread.Start();
            this.InitStatisticData();
        }
        #endregion

        #region 获取上下线明细数据
        public void InitVehicleOnlineInfo()
        {
            if (trackPlayVM.ListVehicleInfo != null && trackPlayVM.ListVehicleInfo.Count != 0)
            {
                List<GpsOnlineInfo> tmpList = new List<GpsOnlineInfo>();
                int sequence = 0;
                GpsOnlineInfo onlineInfo = new GpsOnlineInfo();
                bool isNewStopInfo = true;//是否是新来的信息
                bool hasAddedStopInfo = false;//是否已经添加了一条新的信息
                foreach (TrackBackGpsInfo info in trackPlayVM.ListVehicleInfo)
                {
                    if (info.GpsInfo.OnlineStates == "上线" && isNewStopInfo)
                    {
                        isNewStopInfo = false;
                        hasAddedStopInfo = true;
                        onlineInfo.OnlineTime = info.GpsInfo.Datetime;
                        onlineInfo.Slng = info.GpsInfo.Longitude;
                        onlineInfo.Slat = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.OnlineStates != "上线" && hasAddedStopInfo)
                    {
                        isNewStopInfo = true;
                        hasAddedStopInfo = false;
                        onlineInfo.Sequence = ++sequence;
                        onlineInfo.OfflineTime = info.GpsInfo.Datetime;
                        onlineInfo.Elng = info.GpsInfo.Longitude;
                        onlineInfo.Elat = info.GpsInfo.Latitude;
                        tmpList.Add(onlineInfo);
                        onlineInfo = new GpsOnlineInfo();
                    }
                }
                trackPlayVM.ListVehicleOnlineInfo = tmpList;
            }
        }
        #endregion

        #region 获取停车数据
        public void InitVehicleStopInfo()
        {
            if (trackPlayVM.ListVehicleInfo != null && trackPlayVM.ListVehicleInfo.Count != 0)
            {
                TimeSpan tstotal = new TimeSpan(0, 0, 0, 0, 0);
                List<GPSStopInfo> tmpList = new List<GPSStopInfo>();
                int sequence = 0;
                GPSStopInfo stopInfo = new GPSStopInfo();
                bool isNewStopInfo = true;//是否是新来的停车信息
                bool hasAddedStopInfo = false;//是否已经添加了一条新的停车信息
                foreach (TrackBackGpsInfo info in trackPlayVM.ListVehicleInfo)
                {
                    if (info.GpsInfo.Speed == "0" && isNewStopInfo)
                    {
                        isNewStopInfo = false;
                        hasAddedStopInfo = true;
                        stopInfo.Sequence = ++sequence;
                        stopInfo.StartTime = info.GpsInfo.Datetime;
                        stopInfo.lng = info.GpsInfo.Longitude;
                        stopInfo.lat = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Speed != "0" && hasAddedStopInfo)
                    {
                        isNewStopInfo = true;
                        hasAddedStopInfo = false;
                        stopInfo.EndTime = info.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(stopInfo.EndTime) - DateTime.Parse(stopInfo.StartTime);
                        tstotal += ts;
                        stopInfo.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        tmpList.Add(stopInfo);
                        stopInfo = new GPSStopInfo();
                    }
                }
                trackPlayVM.ListVehicleStopInfo = tmpList;
                trackPlayVM.RestTime = tstotal.Days + "天" + tstotal.Hours + "时" + tstotal.Minutes + "分" + tstotal.Seconds + "秒";
            }
            StaticTreeState.StopInfoLoad = LoadingState.LOADING;
        }
        #endregion

        #region 获取报警信息
        private void GetWarnInfo()
        {
            if (trackPlayVM.ListVehicleInfo != null && trackPlayVM.ListVehicleInfo.Count != 0)
            {
                TimeSpan tstotal = new TimeSpan(0, 0, 0, 0, 0);
                List<GPSWarnInfo> tmpList = new List<GPSWarnInfo>();
                List<TrackBackGpsInfo> originInfo = new List<TrackBackGpsInfo>();
                int sequence = 0;
                GPSWarnInfo warnInfo0 = new GPSWarnInfo();
                GPSWarnInfo warnInfo1 = new GPSWarnInfo();
                GPSWarnInfo warnInfo2 = new GPSWarnInfo();
                GPSWarnInfo warnInfo3 = new GPSWarnInfo();
                GPSWarnInfo warnInfo4 = new GPSWarnInfo();
                GPSWarnInfo warnInfo5 = new GPSWarnInfo();
                GPSWarnInfo warnInfo6 = new GPSWarnInfo();
                GPSWarnInfo warnInfo7 = new GPSWarnInfo();
                GPSWarnInfo warnInfo8 = new GPSWarnInfo();
                GPSWarnInfo warnInfo9 = new GPSWarnInfo();
                GPSWarnInfo warnInfo10 = new GPSWarnInfo();
                GPSWarnInfo warnInfo11 = new GPSWarnInfo();
                GPSWarnInfo warnInfo12 = new GPSWarnInfo();
                GPSWarnInfo warnInfo13 = new GPSWarnInfo();
                GPSWarnInfo warnInfo14 = new GPSWarnInfo();
                GPSWarnInfo warnInfo15 = new GPSWarnInfo();
                GPSWarnInfo warnInfo16 = new GPSWarnInfo();
                GPSWarnInfo warnInfo17 = new GPSWarnInfo();
                GPSWarnInfo warnInfo18 = new GPSWarnInfo();
                GPSWarnInfo warnInfo19 = new GPSWarnInfo();
                GPSWarnInfo warnInfo20 = new GPSWarnInfo();
                GPSWarnInfo warnInfo21 = new GPSWarnInfo();
                GPSWarnInfo warnInfo22 = new GPSWarnInfo();
                GPSWarnInfo warnInfo23 = new GPSWarnInfo();
                GPSWarnInfo warnInfo24 = new GPSWarnInfo();
                GPSWarnInfo warnInfo25 = new GPSWarnInfo();
                GPSWarnInfo warnInfo26 = new GPSWarnInfo();
                GPSWarnInfo warnInfo27 = new GPSWarnInfo();
                GPSWarnInfo warnInfo28 = new GPSWarnInfo();
                GPSWarnInfo warnInfo29 = new GPSWarnInfo();
                GPSWarnInfo warnInfo30 = new GPSWarnInfo();
                GPSWarnInfo warnInfo31 = new GPSWarnInfo();
                GPSWarnInfo warnInfo32 = new GPSWarnInfo();
                GPSWarnInfo warnInfo33 = new GPSWarnInfo();
                GPSWarnInfo warnInfo34 = new GPSWarnInfo();
                GPSWarnInfo warnInfo35 = new GPSWarnInfo();
                GPSWarnInfo warnInfo36 = new GPSWarnInfo();
                GPSWarnInfo warnInfo37 = new GPSWarnInfo();
                GPSWarnInfo warnInfo38 = new GPSWarnInfo();
                GPSWarnInfo warnInfo39 = new GPSWarnInfo();
                GPSWarnInfo warnInfo40 = new GPSWarnInfo();
                GPSWarnInfo warnInfo41 = new GPSWarnInfo();
                GPSWarnInfo warnInfo42 = new GPSWarnInfo();
                bool[] isNewStopInfo = { true, true, true, true, true, true, true, true, true, true, 
                                           true, true, true, true, true, true, true, true, true, true, 
                                           true, true, true, true, true, true, true, true, true, true, 
                                           true, true, true, true, true, true, true, true, true, true, 
                                           true, true, true};//是否是新来的报警信息
                bool[] hasAddedStopInfo = { false, false, false, false, false, false, false, false, false, false,
                                          false, false, false, false, false, false, false, false, false, false,
                                          false, false, false, false, false, false, false, false, false, false,
                                          false, false, false, false, false, false, false, false, false, false,
                                          false, false, false};//是否已经添加了一条新的报警信息

                TrackBackGpsInfo tmp0 = new TrackBackGpsInfo();//报警临时变量
                foreach (TrackBackGpsInfo info in trackPlayVM.ListVehicleInfo)
                {
                    if (info.GpsInfo.Soswarn == "报警" && isNewStopInfo[0])
                    {//紧急报警
                        isNewStopInfo[0] = false;
                        hasAddedStopInfo[0] = true;
                        warnInfo0.WarnType = "紧急报警";
                        warnInfo0.WarnTime = info.GpsInfo.Datetime;
                        warnInfo0.Longitude = info.GpsInfo.Longitude;
                        warnInfo0.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[0])
                    {
                        isNewStopInfo[0] = true;
                        hasAddedStopInfo[0] = false;
                        warnInfo0.Sequence = ++sequence;
                        warnInfo0.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo0.EndTime) - DateTime.Parse(warnInfo0.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo0.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo0.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo0);
                        warnInfo0 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Soswarn == "报警" && isNewStopInfo[1])
                    {//超速报警
                        isNewStopInfo[1] = false;
                        hasAddedStopInfo[1] = true;
                        warnInfo1.WarnType = "超速报警";
                        warnInfo1.WarnTime = info.GpsInfo.Datetime;
                        warnInfo1.Longitude = info.GpsInfo.Longitude;
                        warnInfo1.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[1])
                    {
                        isNewStopInfo[1] = true;
                        hasAddedStopInfo[1] = false;
                        warnInfo1.Sequence = ++sequence;
                        warnInfo1.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo1.EndTime) - DateTime.Parse(warnInfo1.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo1.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo1.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo1);
                        warnInfo1 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[2])
                    {//疲劳驾驶
                        isNewStopInfo[2] = false;
                        hasAddedStopInfo[2] = true;
                        warnInfo2.WarnType = "疲劳驾驶";
                        warnInfo2.WarnTime = info.GpsInfo.Datetime;
                        warnInfo2.Longitude = info.GpsInfo.Longitude;
                        warnInfo2.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[2])
                    {
                        isNewStopInfo[2] = true;
                        hasAddedStopInfo[2] = false;
                        warnInfo2.Sequence = ++sequence;
                        warnInfo2.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo2.EndTime) - DateTime.Parse(warnInfo2.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo2.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo2.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo2);
                        warnInfo2 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[3])
                    {//危险预警
                        isNewStopInfo[3] = false;
                        hasAddedStopInfo[3] = true;
                        warnInfo3.WarnType = "危险预警";
                        warnInfo3.WarnTime = info.GpsInfo.Datetime;
                        warnInfo3.Longitude = info.GpsInfo.Longitude;
                        warnInfo3.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[3])
                    {
                        isNewStopInfo[3] = true;
                        hasAddedStopInfo[3] = false;
                        warnInfo3.Sequence = ++sequence;
                        warnInfo3.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo3.EndTime) - DateTime.Parse(warnInfo3.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo3.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo3.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo3);
                        warnInfo3 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[4])
                    {//GNSS模块发生故障
                        isNewStopInfo[4] = false;
                        hasAddedStopInfo[4] = true;
                        warnInfo4.WarnType = "GNSS模块发生故障";
                        warnInfo4.WarnTime = info.GpsInfo.Datetime;
                        warnInfo4.Longitude = info.GpsInfo.Longitude;
                        warnInfo4.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[4])
                    {
                        isNewStopInfo[4] = true;
                        hasAddedStopInfo[4] = false;
                        warnInfo4.Sequence = ++sequence;
                        warnInfo4.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo4.EndTime) - DateTime.Parse(warnInfo4.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo4.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo4.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo4);
                        warnInfo4 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[5])
                    {//超速报警
                        isNewStopInfo[5] = false;
                        hasAddedStopInfo[5] = true;
                        warnInfo5.WarnType = "GNSS天线未接或被剪断";
                        warnInfo5.WarnTime = info.GpsInfo.Datetime;
                        warnInfo5.Longitude = info.GpsInfo.Longitude;
                        warnInfo5.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[5])
                    {
                        isNewStopInfo[5] = true;
                        hasAddedStopInfo[5] = false;
                        warnInfo5.Sequence = ++sequence;
                        warnInfo5.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo5.EndTime) - DateTime.Parse(warnInfo5.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo5.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo5.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo5);
                        warnInfo5 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[6])
                    {//GNSS天线短路
                        isNewStopInfo[6] = false;
                        hasAddedStopInfo[6] = true;
                        warnInfo6.WarnType = "GNSS天线短路";
                        warnInfo6.WarnTime = info.GpsInfo.Datetime;
                        warnInfo6.Longitude = info.GpsInfo.Longitude;
                        warnInfo6.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[6])
                    {
                        isNewStopInfo[6] = true;
                        hasAddedStopInfo[6] = false;
                        warnInfo6.Sequence = ++sequence;
                        warnInfo6.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo6.EndTime) - DateTime.Parse(warnInfo6.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo6.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo6.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo6);
                        warnInfo6 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[7])
                    {//终端主电源欠压
                        isNewStopInfo[7] = false;
                        hasAddedStopInfo[7] = true;
                        warnInfo7.WarnType = "终端主电源欠压";
                        warnInfo7.WarnTime = info.GpsInfo.Datetime;
                        warnInfo7.Longitude = info.GpsInfo.Longitude;
                        warnInfo7.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[7])
                    {
                        isNewStopInfo[7] = true;
                        hasAddedStopInfo[7] = false;
                        warnInfo7.Sequence = ++sequence;
                        warnInfo7.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo7.EndTime) - DateTime.Parse(warnInfo7.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo7.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo7.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo7);
                        warnInfo7 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[8])
                    {//终端主电源掉电
                        isNewStopInfo[8] = false;
                        hasAddedStopInfo[8] = true;
                        warnInfo8.WarnType = "终端主电源掉电";
                        warnInfo8.WarnTime = info.GpsInfo.Datetime;
                        warnInfo8.Longitude = info.GpsInfo.Longitude;
                        warnInfo8.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[8])
                    {
                        isNewStopInfo[8] = true;
                        hasAddedStopInfo[8] = false;
                        warnInfo8.Sequence = ++sequence;
                        warnInfo8.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo8.EndTime) - DateTime.Parse(warnInfo8.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo8.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo8.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo8);
                        warnInfo8 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[9])
                    {//终端LCD或显示器故障
                        isNewStopInfo[9] = false;
                        hasAddedStopInfo[9] = true;
                        warnInfo9.WarnType = "终端LCD或显示器故障";
                        warnInfo9.WarnTime = info.GpsInfo.Datetime;
                        warnInfo9.Longitude = info.GpsInfo.Longitude;
                        warnInfo9.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[9])
                    {
                        isNewStopInfo[9] = true;
                        hasAddedStopInfo[9] = false;
                        warnInfo9.Sequence = ++sequence;
                        warnInfo9.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo9.EndTime) - DateTime.Parse(warnInfo9.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo9.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo9.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo9);
                        warnInfo9 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[10])
                    {//TTS模块故障
                        isNewStopInfo[10] = false;
                        hasAddedStopInfo[10] = true;
                        warnInfo10.WarnType = "TTS模块故障";
                        warnInfo10.WarnTime = info.GpsInfo.Datetime;
                        warnInfo10.Longitude = info.GpsInfo.Longitude;
                        warnInfo10.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[10])
                    {
                        isNewStopInfo[10] = true;
                        hasAddedStopInfo[10] = false;
                        warnInfo10.Sequence = ++sequence;
                        warnInfo10.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo10.EndTime) - DateTime.Parse(warnInfo10.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo10.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo10.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo10);
                        warnInfo10 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[11])
                    {//摄像头故障
                        isNewStopInfo[11] = false;
                        hasAddedStopInfo[11] = true;
                        warnInfo11.WarnType = "摄像头故障";
                        warnInfo11.WarnTime = info.GpsInfo.Datetime;
                        warnInfo11.Longitude = info.GpsInfo.Longitude;
                        warnInfo11.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[11])
                    {
                        isNewStopInfo[11] = true;
                        hasAddedStopInfo[11] = false;
                        warnInfo11.Sequence = ++sequence;
                        warnInfo11.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo11.EndTime) - DateTime.Parse(warnInfo11.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo11.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo11.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo11);
                        warnInfo11 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[12])
                    {//当天累计驾驶超时
                        isNewStopInfo[12] = false;
                        hasAddedStopInfo[12] = true;
                        warnInfo12.WarnType = "当天累计驾驶超时";
                        warnInfo12.WarnTime = info.GpsInfo.Datetime;
                        warnInfo12.Longitude = info.GpsInfo.Longitude;
                        warnInfo12.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[12])
                    {
                        isNewStopInfo[12] = true;
                        hasAddedStopInfo[12] = false;
                        warnInfo12.Sequence = ++sequence;
                        warnInfo12.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo12.EndTime) - DateTime.Parse(warnInfo12.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo12.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo12.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo12);
                        warnInfo12 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[13])
                    {//超时停车
                        isNewStopInfo[13] = false;
                        hasAddedStopInfo[13] = true;
                        warnInfo13.WarnType = "超时停车";
                        warnInfo13.WarnTime = info.GpsInfo.Datetime;
                        warnInfo13.Longitude = info.GpsInfo.Longitude;
                        warnInfo13.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[13])
                    {
                        isNewStopInfo[13] = true;
                        hasAddedStopInfo[13] = false;
                        warnInfo13.Sequence = ++sequence;
                        warnInfo13.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo13.EndTime) - DateTime.Parse(warnInfo13.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo13.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo13.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo13);
                        warnInfo13 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[14])
                    {//进出区域
                        isNewStopInfo[14] = false;
                        hasAddedStopInfo[14] = true;
                        warnInfo14.WarnType = "进出区域";
                        warnInfo14.WarnTime = info.GpsInfo.Datetime;
                        warnInfo14.Longitude = info.GpsInfo.Longitude;
                        warnInfo14.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[14])
                    {
                        isNewStopInfo[14] = true;
                        hasAddedStopInfo[14] = false;
                        warnInfo14.Sequence = ++sequence;
                        warnInfo14.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo14.EndTime) - DateTime.Parse(warnInfo14.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo14.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo14.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo14);
                        warnInfo14 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[15])
                    {//进出路线
                        isNewStopInfo[15] = false;
                        hasAddedStopInfo[15] = true;
                        warnInfo15.WarnType = "进出路线";
                        warnInfo15.WarnTime = info.GpsInfo.Datetime;
                        warnInfo15.Longitude = info.GpsInfo.Longitude;
                        warnInfo15.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[15])
                    {
                        isNewStopInfo[15] = true;
                        hasAddedStopInfo[15] = false;
                        warnInfo15.Sequence = ++sequence;
                        warnInfo15.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo15.EndTime) - DateTime.Parse(warnInfo15.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo15.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo15.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo15);
                        warnInfo15 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[16])
                    {//路段行驶时间不足/过长
                        isNewStopInfo[16] = false;
                        hasAddedStopInfo[16] = true;
                        warnInfo16.WarnType = "路段行驶时间不足/过长";
                        warnInfo16.WarnTime = info.GpsInfo.Datetime;
                        warnInfo16.Longitude = info.GpsInfo.Longitude;
                        warnInfo16.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[16])
                    {
                        isNewStopInfo[16] = true;
                        hasAddedStopInfo[16] = false;
                        warnInfo16.Sequence = ++sequence;
                        warnInfo16.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo16.EndTime) - DateTime.Parse(warnInfo16.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo16.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo16.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo16);
                        warnInfo16 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[17])
                    {//路线偏离报警
                        isNewStopInfo[17] = false;
                        hasAddedStopInfo[17] = true;
                        warnInfo17.WarnType = "路线偏离报警";
                        warnInfo17.WarnTime = info.GpsInfo.Datetime;
                        warnInfo17.Longitude = info.GpsInfo.Longitude;
                        warnInfo17.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[17])
                    {
                        isNewStopInfo[17] = true;
                        hasAddedStopInfo[17] = false;
                        warnInfo17.Sequence = ++sequence;
                        warnInfo17.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo17.EndTime) - DateTime.Parse(warnInfo17.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo17.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo17.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo17);
                        warnInfo17 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[18])
                    {//车辆VSS故障
                        isNewStopInfo[18] = false;
                        hasAddedStopInfo[18] = true;
                        warnInfo18.WarnType = "车辆VSS故障";
                        warnInfo18.WarnTime = info.GpsInfo.Datetime;
                        warnInfo18.Longitude = info.GpsInfo.Longitude;
                        warnInfo18.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[18])
                    {
                        isNewStopInfo[18] = true;
                        hasAddedStopInfo[18] = false;
                        warnInfo18.Sequence = ++sequence;
                        warnInfo18.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo18.EndTime) - DateTime.Parse(warnInfo18.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo18.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo18.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo18);
                        warnInfo18 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[19])
                    {//车辆油量异常
                        isNewStopInfo[19] = false;
                        hasAddedStopInfo[19] = true;
                        warnInfo19.WarnType = "车辆油量异常";
                        warnInfo19.WarnTime = info.GpsInfo.Datetime;
                        warnInfo19.Longitude = info.GpsInfo.Longitude;
                        warnInfo19.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[19])
                    {
                        isNewStopInfo[19] = true;
                        hasAddedStopInfo[19] = false;
                        warnInfo19.Sequence = ++sequence;
                        warnInfo19.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo19.EndTime) - DateTime.Parse(warnInfo19.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo19.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo19.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo19);
                        warnInfo19 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[20])
                    {//车辆被盗
                        isNewStopInfo[20] = false;
                        hasAddedStopInfo[20] = true;
                        warnInfo20.WarnType = "车辆被盗";
                        warnInfo20.WarnTime = info.GpsInfo.Datetime;
                        warnInfo20.Longitude = info.GpsInfo.Longitude;
                        warnInfo20.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[20])
                    {
                        isNewStopInfo[20] = true;
                        hasAddedStopInfo[20] = false;
                        warnInfo20.Sequence = ++sequence;
                        warnInfo20.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo20.EndTime) - DateTime.Parse(warnInfo20.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo20.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo20.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo20);
                        warnInfo20 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[21])
                    {//车辆非法点火
                        isNewStopInfo[21] = false;
                        hasAddedStopInfo[21] = true;
                        warnInfo21.WarnType = "车辆非法点火";
                        warnInfo21.WarnTime = info.GpsInfo.Datetime;
                        warnInfo21.Longitude = info.GpsInfo.Longitude;
                        warnInfo21.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[21])
                    {
                        isNewStopInfo[21] = true;
                        hasAddedStopInfo[21] = false;
                        warnInfo21.Sequence = ++sequence;
                        warnInfo21.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo21.EndTime) - DateTime.Parse(warnInfo21.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo21.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo21.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo21);
                        warnInfo21 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[22])
                    {//车辆非法位移
                        isNewStopInfo[22] = false;
                        hasAddedStopInfo[22] = true;
                        warnInfo22.WarnType = "车辆非法位移";
                        warnInfo22.WarnTime = info.GpsInfo.Datetime;
                        warnInfo22.Longitude = info.GpsInfo.Longitude;
                        warnInfo22.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[22])
                    {
                        isNewStopInfo[22] = true;
                        hasAddedStopInfo[22] = false;
                        warnInfo22.Sequence = ++sequence;
                        warnInfo22.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo22.EndTime) - DateTime.Parse(warnInfo22.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo22.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo22.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo22);
                        warnInfo22 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[23])
                    {//终端主电源高压
                        isNewStopInfo[23] = false;
                        hasAddedStopInfo[23] = true;
                        warnInfo23.WarnType = "终端主电源高压";
                        warnInfo23.WarnTime = info.GpsInfo.Datetime;
                        warnInfo23.Longitude = info.GpsInfo.Longitude;
                        warnInfo23.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[23])
                    {
                        isNewStopInfo[23] = true;
                        hasAddedStopInfo[23] = false;
                        warnInfo23.Sequence = ++sequence;
                        warnInfo23.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo23.EndTime) - DateTime.Parse(warnInfo23.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo23.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo23.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo23);
                        warnInfo23 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[24])
                    {//道路运输证IC卡模块故障
                        isNewStopInfo[24] = false;
                        hasAddedStopInfo[24] = true;
                        warnInfo24.WarnType = "道路运输证IC卡模块故障";
                        warnInfo24.WarnTime = info.GpsInfo.Datetime;
                        warnInfo24.Longitude = info.GpsInfo.Longitude;
                        warnInfo24.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[24])
                    {
                        isNewStopInfo[24] = true;
                        hasAddedStopInfo[24] = false;
                        warnInfo24.Sequence = ++sequence;
                        warnInfo24.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo24.EndTime) - DateTime.Parse(warnInfo24.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo24.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo24.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo24);
                        warnInfo24 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[25])
                    {//超速预警
                        isNewStopInfo[25] = false;
                        hasAddedStopInfo[25] = true;
                        warnInfo25.WarnType = "超速预警";
                        warnInfo25.WarnTime = info.GpsInfo.Datetime;
                        warnInfo25.Longitude = info.GpsInfo.Longitude;
                        warnInfo25.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[25])
                    {
                        isNewStopInfo[25] = true;
                        hasAddedStopInfo[25] = false;
                        warnInfo25.Sequence = ++sequence;
                        warnInfo25.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo25.EndTime) - DateTime.Parse(warnInfo25.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo25.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo25.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo25);
                        warnInfo25 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[26])
                    {//疲劳驾驶预警
                        isNewStopInfo[26] = false;
                        hasAddedStopInfo[26] = true;
                        warnInfo26.WarnType = "疲劳驾驶预警";
                        warnInfo26.WarnTime = info.GpsInfo.Datetime;
                        warnInfo26.Longitude = info.GpsInfo.Longitude;
                        warnInfo26.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[26])
                    {
                        isNewStopInfo[26] = true;
                        hasAddedStopInfo[26] = false;
                        warnInfo26.Sequence = ++sequence;
                        warnInfo26.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo26.EndTime) - DateTime.Parse(warnInfo26.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo26.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo26.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo26);
                        warnInfo26 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[27])
                    {//碰撞预警
                        isNewStopInfo[27] = false;
                        hasAddedStopInfo[27] = true;
                        warnInfo27.WarnType = "碰撞预警";
                        warnInfo27.WarnTime = info.GpsInfo.Datetime;
                        warnInfo27.Longitude = info.GpsInfo.Longitude;
                        warnInfo27.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[27])
                    {
                        isNewStopInfo[27] = true;
                        hasAddedStopInfo[27] = false;
                        warnInfo27.Sequence = ++sequence;
                        warnInfo27.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo27.EndTime) - DateTime.Parse(warnInfo27.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo27.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo27.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo27);
                        warnInfo27 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[28])
                    {//侧翻预警
                        isNewStopInfo[28] = false;
                        hasAddedStopInfo[28] = true;
                        warnInfo28.WarnType = "侧翻预警";
                        warnInfo28.WarnTime = info.GpsInfo.Datetime;
                        warnInfo28.Longitude = info.GpsInfo.Longitude;
                        warnInfo28.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[28])
                    {
                        isNewStopInfo[28] = true;
                        hasAddedStopInfo[28] = false;
                        warnInfo28.Sequence = ++sequence;
                        warnInfo28.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo28.EndTime) - DateTime.Parse(warnInfo28.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo28.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo28.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo28);
                        warnInfo28 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[29])
                    {//非法开门报警
                        isNewStopInfo[29] = false;
                        hasAddedStopInfo[29] = true;
                        warnInfo29.WarnType = "非法开门报警";
                        warnInfo29.WarnTime = info.GpsInfo.Datetime;
                        warnInfo29.Longitude = info.GpsInfo.Longitude;
                        warnInfo29.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[29])
                    {
                        isNewStopInfo[29] = true;
                        hasAddedStopInfo[29] = false;
                        warnInfo29.Sequence = ++sequence;
                        warnInfo29.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo29.EndTime) - DateTime.Parse(warnInfo29.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo29.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo29.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo29);
                        warnInfo29 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[30])
                    {//视频丢失报警
                        isNewStopInfo[30] = false;
                        hasAddedStopInfo[30] = true;
                        warnInfo30.WarnType = "视频丢失报警";
                        warnInfo30.WarnTime = info.GpsInfo.Datetime;
                        warnInfo30.Longitude = info.GpsInfo.Longitude;
                        warnInfo30.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[30])
                    {
                        isNewStopInfo[30] = true;
                        hasAddedStopInfo[30] = false;
                        warnInfo30.Sequence = ++sequence;
                        warnInfo30.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo30.EndTime) - DateTime.Parse(warnInfo30.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo30.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo30.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo30);
                        warnInfo30 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[31])
                    {//视频遮挡报警
                        isNewStopInfo[31] = false;
                        hasAddedStopInfo[31] = true;
                        warnInfo31.WarnType = "视频遮挡报警";
                        warnInfo31.WarnTime = info.GpsInfo.Datetime;
                        warnInfo31.Longitude = info.GpsInfo.Longitude;
                        warnInfo31.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[31])
                    {
                        isNewStopInfo[31] = true;
                        hasAddedStopInfo[31] = false;
                        warnInfo31.Sequence = ++sequence;
                        warnInfo31.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo31.EndTime) - DateTime.Parse(warnInfo31.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo31.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo31.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo31);
                        warnInfo31 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[32])
                    {//劫警
                        isNewStopInfo[32] = false;
                        hasAddedStopInfo[32] = true;
                        warnInfo32.WarnType = "劫警";
                        warnInfo32.WarnTime = info.GpsInfo.Datetime;
                        warnInfo32.Longitude = info.GpsInfo.Longitude;
                        warnInfo32.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[32])
                    {
                        isNewStopInfo[32] = true;
                        hasAddedStopInfo[32] = false;
                        warnInfo32.Sequence = ++sequence;
                        warnInfo32.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo32.EndTime) - DateTime.Parse(warnInfo32.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo32.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo32.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo32);
                        warnInfo32 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[33])
                    {//非法时段行驶报警
                        isNewStopInfo[33] = false;
                        hasAddedStopInfo[33] = true;
                        warnInfo33.WarnType = "非法时段行驶报警";
                        warnInfo33.WarnTime = info.GpsInfo.Datetime;
                        warnInfo33.Longitude = info.GpsInfo.Longitude;
                        warnInfo33.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[33])
                    {
                        isNewStopInfo[33] = true;
                        hasAddedStopInfo[33] = false;
                        warnInfo33.Sequence = ++sequence;
                        warnInfo33.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo33.EndTime) - DateTime.Parse(warnInfo33.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo33.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo33.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo33);
                        warnInfo33 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[34])
                    {//停车休息时间不足报警
                        isNewStopInfo[34] = false;
                        hasAddedStopInfo[34] = true;
                        warnInfo34.WarnType = "停车休息时间不足报警";
                        warnInfo34.WarnTime = info.GpsInfo.Datetime;
                        warnInfo34.Longitude = info.GpsInfo.Longitude;
                        warnInfo34.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[34])
                    {
                        isNewStopInfo[34] = true;
                        hasAddedStopInfo[34] = false;
                        warnInfo34.Sequence = ++sequence;
                        warnInfo34.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo34.EndTime) - DateTime.Parse(warnInfo34.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo34.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo34.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo34);
                        warnInfo34 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[35])
                    {//越站报警
                        isNewStopInfo[35] = false;
                        hasAddedStopInfo[35] = true;
                        warnInfo35.WarnType = "越站报警";
                        warnInfo35.WarnTime = info.GpsInfo.Datetime;
                        warnInfo35.Longitude = info.GpsInfo.Longitude;
                        warnInfo35.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[35])
                    {
                        isNewStopInfo[35] = true;
                        hasAddedStopInfo[35] = false;
                        warnInfo35.Sequence = ++sequence;
                        warnInfo35.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo35.EndTime) - DateTime.Parse(warnInfo35.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo35.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo35.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo35);
                        warnInfo35 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[36])
                    {//设防
                        isNewStopInfo[36] = false;
                        hasAddedStopInfo[36] = true;
                        warnInfo36.WarnType = "设防";
                        warnInfo36.WarnTime = info.GpsInfo.Datetime;
                        warnInfo36.Longitude = info.GpsInfo.Longitude;
                        warnInfo36.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[36])
                    {
                        isNewStopInfo[36] = true;
                        hasAddedStopInfo[36] = false;
                        warnInfo36.Sequence = ++sequence;
                        warnInfo36.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo36.EndTime) - DateTime.Parse(warnInfo36.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo36.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo36.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo36);
                        warnInfo36 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[37])
                    {//剪线报警
                        isNewStopInfo[37] = false;
                        hasAddedStopInfo[37] = true;
                        warnInfo37.WarnType = "剪线报警";
                        warnInfo37.WarnTime = info.GpsInfo.Datetime;
                        warnInfo37.Longitude = info.GpsInfo.Longitude;
                        warnInfo37.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[37])
                    {
                        isNewStopInfo[37] = true;
                        hasAddedStopInfo[37] = false;
                        warnInfo37.Sequence = ++sequence;
                        warnInfo37.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo37.EndTime) - DateTime.Parse(warnInfo37.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo37.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo37.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo37);
                        warnInfo37 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[38])
                    {//电瓶电压低报警
                        isNewStopInfo[38] = false;
                        hasAddedStopInfo[38] = true;
                        warnInfo38.WarnType = "电瓶电压低报警";
                        warnInfo38.WarnTime = info.GpsInfo.Datetime;
                        warnInfo38.Longitude = info.GpsInfo.Longitude;
                        warnInfo38.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[38])
                    {
                        isNewStopInfo[38] = true;
                        hasAddedStopInfo[38] = false;
                        warnInfo38.Sequence = ++sequence;
                        warnInfo38.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo38.EndTime) - DateTime.Parse(warnInfo38.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo38.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo38.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo38);
                        warnInfo38 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[39])
                    {//密码错误报警
                        isNewStopInfo[39] = false;
                        hasAddedStopInfo[39] = true;
                        warnInfo39.WarnType = "密码错误报警";
                        warnInfo39.WarnTime = info.GpsInfo.Datetime;
                        warnInfo39.Longitude = info.GpsInfo.Longitude;
                        warnInfo39.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[39])
                    {
                        isNewStopInfo[39] = true;
                        hasAddedStopInfo[39] = false;
                        warnInfo39.Sequence = ++sequence;
                        warnInfo39.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo39.EndTime) - DateTime.Parse(warnInfo39.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo39.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo39.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo39);
                        warnInfo39 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[40])
                    {//禁行报警
                        isNewStopInfo[40] = false;
                        hasAddedStopInfo[40] = true;
                        warnInfo40.WarnType = "禁行报警";
                        warnInfo40.WarnTime = info.GpsInfo.Datetime;
                        warnInfo40.Longitude = info.GpsInfo.Longitude;
                        warnInfo40.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[40])
                    {
                        isNewStopInfo[40] = true;
                        hasAddedStopInfo[40] = false;
                        warnInfo40.Sequence = ++sequence;
                        warnInfo40.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo40.EndTime) - DateTime.Parse(warnInfo40.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo40.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo40.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo40);
                        warnInfo40 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[41])
                    {//非法停车报警
                        isNewStopInfo[41] = false;
                        hasAddedStopInfo[41] = true;
                        warnInfo41.WarnType = "非法停车报警";
                        warnInfo41.WarnTime = info.GpsInfo.Datetime;
                        warnInfo41.Longitude = info.GpsInfo.Longitude;
                        warnInfo41.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[41])
                    {
                        isNewStopInfo[41] = true;
                        hasAddedStopInfo[41] = false;
                        warnInfo41.Sequence = ++sequence;
                        warnInfo41.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo41.EndTime) - DateTime.Parse(warnInfo41.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo41.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo41.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo41);
                        warnInfo41 = new GPSWarnInfo();
                    }
                    if (info.GpsInfo.Tiredwarn == "报警" && isNewStopInfo[42])
                    {//SD卡异常
                        isNewStopInfo[42] = false;
                        hasAddedStopInfo[42] = true;
                        warnInfo42.WarnType = "SD卡异常";
                        warnInfo42.WarnTime = info.GpsInfo.Datetime;
                        warnInfo42.Longitude = info.GpsInfo.Longitude;
                        warnInfo42.Latitude = info.GpsInfo.Latitude;
                    }
                    else if (info.GpsInfo.Soswarn != "报警" && hasAddedStopInfo[42])
                    {
                        isNewStopInfo[42] = true;
                        hasAddedStopInfo[42] = false;
                        warnInfo42.Sequence = ++sequence;
                        warnInfo42.EndTime = tmp0.GpsInfo.Datetime;
                        TimeSpan ts = DateTime.Parse(warnInfo42.EndTime) - DateTime.Parse(warnInfo42.WarnTime);
                        if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                        {
                            warnInfo42.LastTime = "瞬时";
                        }
                        else
                        {
                            warnInfo42.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        }
                        tmpList.Add(warnInfo42);
                        warnInfo42 = new GPSWarnInfo();
                    }
                    tmp0 = info;//保存上一条数据
                }
                trackPlayVM.ListVehicleWarnInfo = tmpList;
            }
        }
        #endregion

        #region 获取超速数据
        private void GetOverSpeedDada()
        {
            if (trackPlayVM.ListVehicleInfo != null && trackPlayVM.ListVehicleInfo.Count != 0)
            {
                TimeSpan tstotal = new TimeSpan(0, 0, 0, 0, 0);
                List<GPSOverSpeedInfo> tmpList = new List<GPSOverSpeedInfo>();
                List<TrackBackGpsInfo> originInfo = new List<TrackBackGpsInfo>();
                int sequence = 0;
                GPSOverSpeedInfo overspeedInfo = new GPSOverSpeedInfo();
                bool isNewOverInfo = true;//是否是新来的停车信息
                bool hasAddedOverInfo = false;//是否已经添加了一条新的停车信息
                TrackBackGpsInfo tmpttb = new TrackBackGpsInfo();
                foreach (TrackBackGpsInfo info in trackPlayVM.ListVehicleInfo)
                {
                    if (info.GpsInfo.Overspeedwarn == null)
                    {
                        continue;
                    }
                    if (info.GpsInfo.Overspeedwarn.Equals("报警") && isNewOverInfo)
                    {
                        isNewOverInfo = false;
                        hasAddedOverInfo = true;
                        overspeedInfo.Sequence = (++sequence) + "";
                        overspeedInfo.StartTime = info.GpsInfo.Datetime;
                        overspeedInfo.Slng = info.GpsInfo.Longitude;
                        overspeedInfo.Slat = info.GpsInfo.Latitude;
                    }
                    else if (!(info.GpsInfo.Overspeedwarn.Equals("报警")) && hasAddedOverInfo)
                    {
                        isNewOverInfo = true;
                        hasAddedOverInfo = false;
                        if (tmpttb.GpsInfo.Datetime.Equals(overspeedInfo.StartTime))
                        {
                            overspeedInfo = new GPSOverSpeedInfo();
                            continue;
                        }
                        overspeedInfo.EndTime = tmpttb.GpsInfo.Datetime;
                        overspeedInfo.Elng = tmpttb.GpsInfo.Longitude;
                        overspeedInfo.Elat = tmpttb.GpsInfo.Latitude;
                        TimeSpan ts = DateTime.Parse(overspeedInfo.EndTime) - DateTime.Parse(overspeedInfo.StartTime);
                        tstotal += ts;
                        overspeedInfo.LastTime = ts.Hours + "时" + ts.Minutes + "分" + ts.Seconds + "秒";
                        tmpList.Add(overspeedInfo);
                        overspeedInfo = new GPSOverSpeedInfo();
                    }
                    tmpttb = info;
                }
                trackPlayVM.ListVehicleOverSpeedInfo = tmpList;
            }
            StaticTreeState.OverSpeedInfoLoad = LoadingState.LOADING;
        }
        #endregion

        #region 获取上下线明细
        public void InitOnlineInfo()
        {
            if (trackPlayVM.ListVehicleInfo != null && trackPlayVM.ListVehicleInfo.Count != 0)
            {
                GPSInfo lastone = new GPSInfo();
                lastone.OnlineStates = "离线";
                bool flag = false; //上线为真，下线为假
                foreach (TrackBackGpsInfo info in trackPlayVM.ListVehicleInfo)
                {
                    if (!(info.GpsInfo.OnlineStates.Equals(lastone.OnlineStates)))
                    {
                        GpsOnlineInfo go = new GpsOnlineInfo();
                        if (flag)
                        {
                            go.OfflineTime = info.GpsInfo.Datetime;
                            go.OfflineAddr = this.GetAddress(info);
                            go.OnlineTime = lastone.Datetime;
                            go.OnlineAddr = lastone.CurLocation;
                            lastone = info.GpsInfo;
                            flag = !flag;
                        }
                        else
                        {
                            go.OnlineTime = info.GpsInfo.Datetime;
                            go.OnlineAddr = this.GetAddress(info);
                            lastone.OnlineStates = info.GpsInfo.OnlineStates;
                            flag = !flag;
                        }
                        trackPlayVM.ListOnLineInfo.Add(go);
                    }
                }
            }
        }
        #endregion

        #region 统计数据
        public class RestTime
        {
            public RestTime()
            {
                this.hour = 0;
                this.minute = 0;
                this.second = 0;
            }
            public int hour;
            public int minute;
            public int second;
        }
        private void InitStatisticData()
        {
            if (trackPlayVM.ListVehicleInfo != null && trackPlayVM.ListVehicleInfo.Count != 0)
            {
                double maxSpeed = 0;//最高速度
                double sumSpeed = 0;//速度总和
                RestTime rt = new RestTime();//休息时间
                foreach (TrackBackGpsInfo info in trackPlayVM.ListVehicleInfo)
                {
                    double tmpSpeed = Convert.ToDouble(info.GpsInfo.Speed);
                    if (tmpSpeed > maxSpeed)
                    {
                        maxSpeed = tmpSpeed;
                    }
                    sumSpeed += tmpSpeed;
                }
                trackPlayVM.RecordCount = trackPlayVM.ListVehicleInfo.Count.ToString();
                trackPlayVM.MaxSpeed = maxSpeed.ToString() + "千米/小时";
                trackPlayVM.AverageSpeed = (sumSpeed / trackPlayVM.ListVehicleInfo.Count).ToString("0.00") + "千米/小时";
                trackPlayVM.DriveMileage = (Convert.ToDouble(trackPlayVM.ListVehicleInfo[trackPlayVM.ListVehicleInfo.Count - 1].GpsInfo.GPSMileage)
                    - Convert.ToDouble(trackPlayVM.ListVehicleInfo[0].GpsInfo.GPSMileage)).ToString("0.00") + "千米";
                trackPlayVM.DriveTime = this.GetDriveTime(rt, trackPlayVM.ListVehicleInfo[0].GpsInfo.Datetime,
                    trackPlayVM.ListVehicleInfo[trackPlayVM.ListVehicleInfo.Count - 1].GpsInfo.Datetime);
            }
        }
        /*计算休息时间*/
        private void AddRestTime(RestTime rt, string st)
        {
            int index = st.IndexOf("小时");
            if (index > 0)
            {
                rt.hour += int.Parse(st.Substring(0, index));
                st = st.Substring(index + 2);
            }
            index = st.IndexOf("分");
            if (index > 0)
            {
                rt.minute += int.Parse(st.Substring(0, index));
                if (rt.minute >= 60)
                {
                    rt.hour += 1;
                    rt.minute -= 60;
                }
                st = st.Substring(index + 1);
            }
            index = st.IndexOf("秒");
            if (index > 0)
            {
                rt.second += int.Parse(st.Substring(0, index));
                if (rt.second >= 60)
                {
                    rt.minute += 1;
                    rt.second -= 60;
                }
                st = st.Substring(index + 1);
            }
        }
        /*计算行驶时间*/
        private string GetDriveTime(RestTime restTime, string startTime, string endTime)
        {
            double restSecond = restTime.hour * 3600 + restTime.minute * 60 + restTime.second;
            DateTime td0 = new DateTime(1970, 1, 1);
            td0 = new DateTime(1970, 1, 1);
            TimeSpan tTs = DateTime.Parse(endTime) - DateTime.Parse(startTime);
            DateTime driveTime = td0.AddSeconds(tTs.TotalSeconds - restSecond);
            string ret = "";
            if (driveTime.Hour > 0)
            {
                ret += driveTime.Hour.ToString() + "小时";
            }
            if (driveTime.Minute > 0)
            {
                ret += driveTime.Minute.ToString() + "分";
            }
            if (driveTime.Second > 0)
            {
                ret += driveTime.Second.ToString() + "秒";
            }
            return ret;
        }
        /*构造休息时间显示字符串*/
        private string ConstructRestTime(RestTime rt)
        {
            string ret = "";
            if (rt.hour != 0)
            {
                ret += rt.hour.ToString() + "小时";
            }
            if (rt.minute != 0)
            {
                ret += rt.minute.ToString() + "分";
            }
            if (rt.second != 0)
            {
                ret += rt.second.ToString() + "秒";
            }
            return ret;
        }
        #endregion

        #region 根据经纬度获取地理位置信息
        public string GetAddress(TrackBackGpsInfo info)
        {
            string addr = info.CurrentLocation;
            GPSInfo tmpGpsInfo = info.GpsInfo;
            if (addr == "" || addr == null)
            {
                addr = this.ParseAddress(tmpGpsInfo.Longitude, tmpGpsInfo.Latitude);
                tmpGpsInfo.CurLocation = addr;
                info.CurrentLocation = addr;
            }
            return addr;
        }
        /*获取历史轨迹gps信息根据经纬度解析地址*/
        public string ParseAddress(string lng, string lat)
        {
            WebClient webClient = new WebClient();
            /*逆地址解析*/
            string addr = "";
            try
            {
                Uri endpoint = new Uri(VehicleConfig.GetInstance().URL_PARSEADDRESS + "&location=" + lat + "," + lng + "&output=xml");
                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string xmlStr = client.DownloadString(endpoint);
                XmlReader xReader = XmlReader.Create(new StringReader(xmlStr));
                while (xReader.Read())
                {
                    string elementName = xReader.Name;
                    if (elementName == "formatted_address")
                    {
                        xReader.Read();
                        addr = xReader.Value;
                        break;
                    }
                }
            }
            catch
            {
                return addr;
            }
            return addr;
        }
        #endregion
    }
}
