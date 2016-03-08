using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using System.Timers;
using VehicleGPS.zmq;
using System.Windows;
using VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor;

namespace VehicleGPS.ViewModels.Warn
{
    /// <summary>
    /// author:夏创铭
    /// 报警浮窗控制器
    /// </summary>
    class WarnInfoViewModel : NotificationObject
    {
        StaticWarnInfo warninstance = null;
        public WarnInfoViewModel()
        {
            this.AlermResetCommand = new DelegateCommand<Window>(new Action<Window>(AlermResteCommandExecute));
            this.RefreshCommand = new DelegateCommand(new Action(RefreshCommandExecute));
            this.DoubleClickCommand = new DelegateCommand(new Action(DoubleClickCommandExecute));
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(RefreshAlermInfo);
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();
            warninstance = StaticWarnInfo.GetInstance();
            while (true)
            {
                if (!RealTimeViewModel.GetInstance().IsBusy)
                {
                    this.AlerInfoListShow = RealTimeViewModel.GetInstance().AlermInfoList;
                    break;
                }
            }
        }
        //刷新滚动条
        private int i = 0;
        public static List<WarnInfo> warnlist { get; set; }
        private void RefreshAlermInfo(object sender, EventArgs e)
        {
            if (StaticTreeState.WarnInfo == LoadingState.LOADCOMPLETE && StaticTreeState.WarnSettinInfo == LoadingState.LOADCOMPLETE)
            {
                warnlist = warninstance.WarnInfoList;
                if (warnlist != null && warnlist.Count > 0)
                {
                    if (i > warnlist.Count - 1)
                    {
                        i--;
                    }
                    this.PromptInfo = "[" + warnlist[i].Time.Substring(11, 8) + "]目标：" + warnlist[i].VehicleNum + "," + warnlist[i].AlarmContent;
                    i = i >= ((warnlist.Count) - 1) ? 0 : i + 1;
                }
                else
                {
                    this.PromptInfo = "";
                }
            }
        }

        #region 单例
        private static WarnInfoViewModel instance = null;
        public static WarnInfoViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new WarnInfoViewModel();
            }
            return instance;
        }
        #endregion

        #region 绑定数据源

        private WarnInfo selectedItem;

        public WarnInfo SelectedAlermItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                this.RaisePropertyChanged("SelectedAlermItem");
            }
        }

        //滚动显示信息
        private string promptinfo;

        public string PromptInfo
        {
            get { return promptinfo; }
            set
            {
                promptinfo = value;
                this.RaisePropertyChanged("PromptInfo");
            }
        }
        private List<WarnInfo> alerminfolistshow;

        public List<WarnInfo> AlerInfoListShow
        {
            get { return alerminfolistshow; }
            set
            {
                alerminfolistshow = value;
                this.RaisePropertyChanged("AlerInfoListShow");
            }
        }
        private bool allchecked;

        public bool AllChecked
        {
            get { return allchecked; }
            set
            {
                allchecked = value;
                foreach (WarnInfo warn in this.AlerInfoListShow)
                {
                    warn.IsSelected = true;
                }
                this.RaisePropertyChanged("AlerInfoListShow");
                this.RaisePropertyChanged("AllChecked");
            }
        }
        private bool allnotchecked;

        public bool AllNotChecked
        {
            get { return allnotchecked; }
            set
            {
                allnotchecked = value;
                foreach (WarnInfo warn in this.AlerInfoListShow)
                {
                    warn.IsSelected = false;
                }
                this.RaisePropertyChanged("AlerInfoListShow");
                this.RaisePropertyChanged("AllNotChecked");
            }
        }
        private bool inversechecked;

        public bool InverseChecked
        {
            get { return inversechecked; }
            set
            {
                inversechecked = value;
                foreach (WarnInfo warn in this.AlerInfoListShow)
                {
                    if (warn.IsSelected)
                    {
                        warn.IsSelected = false;
                    }
                    else
                    {
                        warn.IsSelected = true;
                    }
                }
                this.RaisePropertyChanged("AlerInfoListShow");
                this.RaisePropertyChanged("InverseChecked");
            }
        }
        #endregion

        #region 绑定操作
        //报警解除
        public DelegateCommand<Window> AlermResetCommand { get; set; }
        private void AlermResteCommandExecute(Window parentWin)
        {
            if (IsAnyOneSelected())
            {
                MessageBox.Show("请选择至少一条报警数据");
                return;
            }
            if (!WarnRelievePwd.isRight)
            {
                WarnRelievePwd win = new WarnRelievePwd();
                win.Owner = Window.GetWindow(parentWin);
                win.ShowDialog();
            }

            if (!WarnRelievePwd.isRight)
            {
                return;
            }
            List<WarnInfo> faillist = new List<WarnInfo>();
            List<WarnInfo> tmplist = new List<WarnInfo>();
            foreach (WarnInfo warn in this.AlerInfoListShow)
            {
                if (warn.IsSelected == true)
                {
                    if (zmqPackHelper.zmqInstructionsPack(warn.SimId, getCMD(warn)))
                    {
                        tmplist.Add(warn);
                    }
                    else
                    {
                        faillist.Add(warn);
                    }
                }
            }
            StaticTreeState.WarnInfo = LoadingState.LOADING;
            foreach (WarnInfo item in tmplist)
            {
                foreach (WarnInfo warn in this.AlerInfoListShow)
                {
                    if (warn.SimId == item.SimId && warn.AlarmType == item.AlarmType)
                    {
                        this.AlerInfoListShow.Remove(warn);
                        break;
                    }
                }

            }
            warninstance.WarnInfoList = this.AlerInfoListShow;
            if (faillist.Count > 0)
            {
                string failstring = null;
                foreach (WarnInfo item in faillist)
                {
                    failstring += "\n" + item.AlarmContent;
                }
                MessageBox.Show("以下报警未成功解除：" + failstring);
            }
            else
            {
                MessageBox.Show("解除成功！");
            }
            StaticTreeState.WarnInfo = LoadingState.LOADCOMPLETE;
        }
        private bool IsAnyOneSelected()
        {
            bool isanyoneselected = true;
            foreach (WarnInfo warn in this.AlerInfoListShow)
            {
                if (warn.IsSelected)
                {
                    isanyoneselected = false;
                }
            }
            return isanyoneselected;
        }
        /// <summary>
        /// 生成指令字符串
        /// </summary>
        /// <param name="warn"></param>
        /// <returns></returns>
        private string getCMD(WarnInfo warn)
        {
            string cmd = "{\"gType\" : \" gpsCommand\",\"gpsBasic\" : {" +
                "\"cmd\":\"QUERENBAOJING_TYPE\"," +
                "\"simId\":\"" + warn.SimId + "\"," +
                "\"warntype\":\"" + warn.AlarmType + "\"," +
                "\"dealtype\":\"1\"," +
                "\"cmdId\":\"" + warn.cmdId + "\"," +
                "\"serialNum\":\"" + warn.serialNum + "\"," +
                "\"warn\":\"" + getWarnToBinary(warn.AlarmType) + "\"}}";
            return cmd;
        }
        /// <summary>
        /// 根据报警类型生成报警反馈数据
        /// </summary>
        /// <param name="warntype"></param>
        /// <returns></returns>
        private int getWarnToBinary(string warntype)
        {
            switch (warntype)
            {
                case "0":
                    return 1;
                case "3":
                    return 8;
                case "14":
                    return (int)Math.Pow(2, 20);
                case "15":
                    return (int)Math.Pow(2, 21);
                case "16":
                    return (int)Math.Pow(2, 22);
                case "21":
                    return (int)Math.Pow(2, 27);
                case "22":
                    return (int)Math.Pow(2, 28);
                default:
                    return 0;
            }
        }
        //刷新
        public DelegateCommand RefreshCommand { get; set; }
        public void RefreshCommandExecute()
        {
            while (true)
            {
                if (StaticTreeState.WarnInfo == LoadingState.LOADCOMPLETE)
                {
                    StaticTreeState.WarnInfo = LoadingState.LOADING;
                    this.RaisePropertyChanged("AlerInfoListShow");
                    List<WarnInfo> tmp = new List<WarnInfo>();
                    foreach (WarnInfo warn in StaticWarnInfo.GetInstance().WarnInfoList)
                    {
                        tmp.Add(warn);
                    }
                    this.AlerInfoListShow = tmp;
                    StaticTreeState.WarnInfo = LoadingState.LOADCOMPLETE;
                    break;
                }
            }
        }

        public DelegateCommand DoubleClickCommand { get; set; }
        private void DoubleClickCommandExecute()
        {
            try
            {
                RealTimeTreeViewModel.GetInstance().FocusAndShowInMap(this.SelectedAlermItem.SimId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
