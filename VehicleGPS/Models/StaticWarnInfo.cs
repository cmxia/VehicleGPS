using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using VehicleGPS.ViewModels.Warn;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.Models
{
    class StaticWarnInfo : NotificationObject
    {
        public static bool isInitialed = false;//warningInfo 窗口是否已经初始化
        private static StaticWarnInfo instance;
        private StaticWarnInfo()
        {
            WarnInfoList = new List<WarnInfo>();
            MessageList = new List<WarnInfo>();
        }
        public static StaticWarnInfo GetInstance()
        {
            if (instance == null)
            {
                instance = new StaticWarnInfo();
            }
            return instance;
        }

        public static List<AlarmSettingInfo> warnsetlist { get; set; }

        public bool isbusy = false;
        //报警信息列表
        private List<WarnInfo> warninfolist = null;

        public List<WarnInfo> WarnInfoList
        {
            get { return warninfolist; }
            set
            {
                isbusy = true;
                warninfolist = value;
                isbusy = false;
                if (isInitialed)
                {
                    VehicleGPS.Views.Warn.WarnInfo warnWin = VehicleGPS.Views.Warn.WarnInfo.GetInstance();
                    if (warnWin != null)
                    {

                        if (warninfolist.Count > 0)
                        {
                            warnWin.Dispatcher.BeginInvoke((Action)delegate()
                            {
                                if (!VehicleGPS.Views.Warn.WarnInfo.GetInstance().IsWinShow())
                                {
                                    VehicleGPS.Views.Warn.WarnInfo.GetInstance().ShowWin();
                                }
                            });
                        }
                        else
                        {
                            warnWin.Dispatcher.BeginInvoke((Action)delegate()
                            {
                                if (VehicleGPS.Views.Warn.WarnInfo.GetInstance().IsWinShow())
                                {
                                    VehicleGPS.Views.Warn.WarnInfo.GetInstance().HideWin();
                                }
                            });
                        }
                    }
                }
                this.RaisePropertyChanged("WarnInfoList");
            }
        }


        //根据报警配置里的报警显示项更新报警信息列表
        public void RefreshWarnInfo()
        {
            List<WarnInfo> tmp = new List<WarnInfo>();
            foreach (WarnInfo warn in this.WarnInfoList)
            {
                foreach (AlarmSettingInfo alarmset in warnsetlist)
                {
                    if (warn.AlarmType == alarmset.WarnID)
                    {
                        tmp.Add(warn);
                        break;
                    }
                }
            }
            this.WarnInfoList = tmp;
        }

        //消息列表
        private List<WarnInfo> messagelist;

        public List<WarnInfo> MessageList
        {
            get { return messagelist; }
            set
            {
                messagelist = value;
                this.RaisePropertyChanged("MessageList");
                if (messagelist.Count > 0)
                {
                    MessageInfoViewModel.GetInstance().MessageList = value;
                }
            }
        }

        public static string[] getWarnType()
        {
            return warnType;
        }

        public static bool IsWarnOrMessage(string warntype)
        {

            if (int.Parse(warntype) > 43)
            {
                return false;
            }
            return true;
        }
        //报警类型
        private static string[] warnType = { "紧急报警(0)", "超速报警(1)", "疲劳驾驶(2)", "危险预警(3)", "GNSS模块发生故障(4)",
                                          "GNSS天线未接或被剪断(5)","GNSS天线短路(6)","终端主电源欠压(7)","终端主电源掉电(8)","终端LCD或显示器故障(9)",
                                          "TTS模块故障(10)","摄像头故障(11)","当天累计驾驶超时(12)","超时停车(13)","进出区域(14)",
                                          "进出路线(15)","路段行驶时间不足/过长(16)","路线偏离报警(17)","车辆VSS故障(18)","车辆油量异常(19)",     
                                          "车辆被盗(20)","车辆非法点火(21)","车辆非法位移(22)","终端主电源高压(23)","道路运输证IC卡模块故障(24)",
                                          "超速预警(25)","疲劳驾驶预警(26)","碰撞预警(27)","侧翻预警(28)","非法开门报警(29)",
                                          "视频丢失报警(30)","视频遮挡报警(31)","劫警(32)","非法时段行驶报警(33)","停车休息时间不足报警(34)",
                                          "越站报警(35)","设防(36)","剪线报警(37)","电瓶电压低报警(38)","密码错误报警(39)",
                                          "禁行报警(40)","非法停车报警(41)","SD卡异常(42)","出区域报警(43)","调度车辆出起始区域(44)",
                                          "调度车辆进结束区域(45)","调度车辆出结束区域(46)","调度车辆进起始区域(47)","调度车辆开始卸料(48)",
                                          "调度车辆结束卸料(49)","任务开盘时间到了(50)","运输时间超时(51)","卸料时间超时(52)","无任务离场[出区域](53)",
                                          "无任务离场[回区域](54)","油量上升异常报警(55)","油量下降异常报警(56)"};
    }
}
