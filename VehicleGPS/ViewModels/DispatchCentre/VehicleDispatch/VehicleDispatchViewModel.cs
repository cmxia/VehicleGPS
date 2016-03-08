using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using VehicleGPS.Models.Login;
using System.Data;
using VehicleGPS.Services;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class VehicleDispatchViewModel : NotificationObject
    {
        private VehicleDispatchDataOperate DataOperate;
        private static VehicleDispatchViewModel instance;
        public static VehicleDispatchViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new VehicleDispatchViewModel();
            }
            return instance;
        }
        private VehicleDispatchViewModel()
        {
            /*初始化分页命令*/
            this.ComeFirstCommand = new DelegateCommand(new Action(this.ComeFirstCommandExecute));
            this.ComeLastCommand = new DelegateCommand(new Action(this.ComeLastCommandExecute));
            this.ComeNextCommand = new DelegateCommand(new Action(this.ComeNextCommandExecute));
            this.ComePrevCommand = new DelegateCommand(new Action(this.ComePrevCommandExecute));
            /*查询命令*/
            this.QueryCommand = new DelegateCommand(new Action(this.QueryCommandExecute));
            /*站点选择改变*/
            this.StationChangedCommand = new DelegateCommand(new Action(this.StationChangeCommandExecute));

            this.DataOperate = new VehicleDispatchDataOperate();

            /*初始化定时器*/
            //this.dispatcherTimer = new DispatcherTimer();
            //this.dispatcherTimer.Interval = TimeSpan.FromSeconds(VehicleConfig.GetInstance().VEHICLEDISPATCHTIMEINTERVAL);
            //this.dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            //this.dispatcherTimer.Start();

            this.timer = new System.Timers.Timer();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(dispatcherTimer_Tick);
            timer.Interval = VehicleConfig.GetInstance().VEHICLEDISPATCHTIMEINTERVAL;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();
            

            /*读取站点信息*/
            this.InitStationList();

            this.QueryCommandExecute();

        }

        /*当前选择的Item*/
        /// <summary>
        /// 当前选择的调度信息
        /// </summary>
        private VehicleDispatchItemViewModel selectedItem;
        public VehicleDispatchItemViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    this.RaisePropertyChanged("SelectedItem");
                }
            }
        }

        public void RefreshOneDispatchInfo(string type, string id)
        {
            string sql = "select regId,regName,regLongitude,regLatitude,regRadius from InfoRegion where regId='" + id + "";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                return;
            }
            DataTable dt = JsonHelper.JsonToDataTable(jsonStr);
            if (dt == null || dt.Rows.Count <= 0)
            {
                return;
            }
            DataRow row = dt.Rows[0];
            if (string.Equals(type, "REG"))
            {
                foreach (VehicleDispatchItemViewModel item in this.ListDispatchInfo)
                {
                    if (item.TaskNumberInfo.RegionID.Equals(id))
                    {
                        item.TaskNumberInfo.StartPoint = row["regName"].ToString() == "" ? "" : row["regName"].ToString();
                        item.TaskNumberInfo.RegionName = row["regName"].ToString() == "" ? "" : row["regName"].ToString();
                        item.TaskNumberInfo.StartLat = row["regLatitude"].ToString() == "" ? "" : row["regLatitude"].ToString();
                        item.TaskNumberInfo.StartLng = row["regLongitude"].ToString() == "" ? "" : row["regLongitude"].ToString();
                        item.TaskNumberInfo.StartRadius = row["regRadius"].ToString() == "" ? "" : row["regRadius"].ToString();
                    }
                }
            }
            else if (string.Equals(type, "SITE"))
            {
                foreach (VehicleDispatchItemViewModel item in this.ListDispatchInfo)
                {
                    if (item.TaskNumberInfo.EndID.Equals(id))
                    {
                        item.TaskNumberInfo.EndPoint = row["regName"].ToString() == "" ? "" : row["regName"].ToString();
                        item.TaskNumberInfo.EndLat = row["regLatitude"].ToString() == "" ? "" : row["regLatitude"].ToString();
                        item.TaskNumberInfo.EndLng = row["regLongitude"].ToString() == "" ? "" : row["regLongitude"].ToString();
                        item.TaskNumberInfo.EndRadius = row["regRadius"].ToString() == "" ? "" : row["regRadius"].ToString();
                    }
                }
            }
        }

        /*任务单详细数据*/
        /// <summary>
        /// 任务单详细数据列表
        /// </summary>
        private List<VehicleDispatchItemViewModel> listDispatchInfo;
        public List<VehicleDispatchItemViewModel> ListDispatchInfo
        {
            get { return listDispatchInfo; }
            set
            {
                if (listDispatchInfo != value)
                {
                    listDispatchInfo = value;
                    StaticTreeState.DispatchCenterPage = LoadingState.LOADCOMPLETE;
                    this.RaisePropertyChanged("ListDispatchInfo");
                    InitPage();
                }
            }
        }
        /*当前页*/
        /// <summary>
        /// 当前页调度信息
        /// </summary>
        private List<VehicleDispatchItemViewModel> listDispatchInfoCurrentPage;
        public List<VehicleDispatchItemViewModel> ListDispatchInfoCurrentPage
        {
            get { return listDispatchInfoCurrentPage; }
            set
            {
                if (listDispatchInfoCurrentPage != value)
                {
                    listDispatchInfoCurrentPage = value;
                    this.RaisePropertyChanged("ListDispatchInfoCurrentPage");
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += (o, ea) =>
                    {//放在后台线程执行，睡眠1.5秒，等待界面渲染完毕才能获取调度Grid的宽度
                        //否则计算偏移量时，界面还未渲染完毕，Grid宽度将为0
                        Thread.Sleep(1500);
                        this.InitDispatchInfo();//初始化调度信息
                    };
                    worker.RunWorkerAsync();
                }
            }
        }
        /*所有站点*/
        /// <summary>
        /// 选择站点
        /// </summary>
        private int selectedStationIndex;
        public int SelectedStationIndex
        {
            get { return selectedStationIndex; }
            set
            {
                if (selectedStationIndex != value)
                {
                    selectedStationIndex = value;
                    this.RaisePropertyChanged("SelectedStationIndex");
                    this.InitTaskList();
                }
            }
        }
        /// <summary>
        /// 站点信息列表
        /// </summary>
        private List<CVBasicInfo> listStation;
        public List<CVBasicInfo> ListStation
        {
            get { return listStation; }
            set
            {
                if (listStation != value)
                {
                    listStation = value;
                    this.RaisePropertyChanged("ListStation");
                    this.QueryEnable = true;
                    this.InitTaskList();
                }
            }
        }
        /*站点对应的任务单号*/
        /// <summary>
        /// 站点对应的任务单号
        /// </summary>
        private int selectedTaskIndex;
        public int SelectedTaskIndex
        {
            get { return selectedTaskIndex; }
            set
            {
                if (selectedTaskIndex != value)
                {
                    selectedTaskIndex = value;
                    this.RaisePropertyChanged("SelectedTaskIndex");
                }
            }
        }
        /// <summary>
        /// 任务单列表
        /// </summary>
        private List<string> listTaskNumber;
        public List<string> ListTaskNumber
        {
            get { return listTaskNumber; }
            set
            {
                if (listTaskNumber != value)
                {
                    listTaskNumber = value;
                    this.RaisePropertyChanged("ListTaskNumber");
                }
            }
        }
        /*查询按钮状态*/
        /*当前选择的Item*/
        /// <summary>
        /// 查询按钮状态(能否使用)
        /// </summary>
        private bool queryEnable = false;
        public bool QueryEnable
        {
            get { return queryEnable; }
            set
            {
                if (queryEnable != value)
                {
                    queryEnable = value;
                    this.RaisePropertyChanged("QueryEnable");
                }
            }
        }

        /*读取站点信息*/
        /// <summary>
        /// 读取站点信息
        /// </summary>
        private void InitStationList()
        {
            this.DataOperate.InitStationList(this);
        }

        /*忙等待*/
        /// <summary>
        /// 忙等待
        /// </summary>
        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    this.RaisePropertyChanged("IsBusy");
                }
            }
        }
        /*特定条件查询*/
        /// <summary>
        /// 特定条件查询(查询全部)
        /// </summary>
        private void QueryAllTask()
        {
            this.ListDispatchInfo = this.DataOperate.GetTaskDetailInfo("all", "全部");
        }
        public DelegateCommand QueryCommand { get; set; }
        public void QueryCommandExecute()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                this.InitTaskDetailList();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.IsBusy = false;
                this.QueryEnable = true;
            };
            this.QueryEnable = false;
            this.IsBusy = true;
            worker.RunWorkerAsync();
        }
        /*站点选择改变*/
        public DelegateCommand StationChangedCommand { get; set; }
        private void StationChangeCommandExecute()
        {
            QueryCommandExecute();
        }

        /*根据选择站点，初始化*/
        /// <summary>
        /// 根据选择站点，初始化
        /// </summary>
        private void InitTaskList()
        {
            if (this.listStation != null)
            {
                this.SelectedTaskIndex = 0;
                if (this.selectedStationIndex < 0)
                {
                    this.SelectedStationIndex = 0;
                }
                this.ListTaskNumber = this.DataOperate.GetTaskNumberList(this.listStation[this.selectedStationIndex].ID);
            }
        }
        /*根据选择站点和任务单号获取详细任务单数据*/
        /// <summary>
        /// 根据选择站点和任务单号获取详细任务单数据
        /// </summary>
        private void InitTaskDetailList()
        {
            string stationID = this.listStation[this.selectedStationIndex].ID;
            string taskID = this.listTaskNumber[this.selectedTaskIndex].ToString();
            StaticTreeState.DispatchCenter = LoadingState.LOADCOMPLETE;
            StaticTreeState.DispatchCenterPage = LoadingState.LOADING;
            this.ListDispatchInfo = this.DataOperate.GetTaskDetailInfo(stationID, taskID);
        }

        #region 定时获取当前页的调度车辆信息
        private DispatcherTimer dispatcherTimer;

        private System.Timers.Timer timer;
        /// <summary>
        /// 定时获取当前页的调度车辆信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.InitDispatchInfo();
        }
        #endregion

        #region 获取当前页任务单的调度信息
        /// <summary>
        /// 获取当前页任务单的调度信息
        /// </summary>
        public void InitDispatchInfo()
        {
            int loop_count = 0;
            while (true && loop_count++ < 15)
            {
                if (StaticTreeState.DispatchCenterPage == LoadingState.LOADCOMPLETE)
                {
                    StaticTreeState.DispatchCenterPage = LoadingState.LOADING;
                    if (this.listDispatchInfoCurrentPage != null && this.listDispatchInfoCurrentPage.Count != 0)
                    {
                        foreach (VehicleDispatchItemViewModel item in this.listDispatchInfoCurrentPage)
                        {
                            this.DataOperate.InitDispatchInfo(item);
                        }
                    }
                    StaticTreeState.DispatchCenterPage = LoadingState.LOADCOMPLETE;
                    break;
                }
                Thread.Sleep(50);
            }
        }
        #endregion
        #region 分页
        private int PageSize { get; set; }//分页大小
        private void InitPage()
        {
            this.PageSize = 5;
            this.TotalCount = this.listDispatchInfo.Count;
            this.TotalPage = (this.TotalCount / this.PageSize) * this.PageSize < totalCount ? this.TotalCount / this.PageSize + 1 : this.TotalCount / this.PageSize;
            this.CurrentPage = 0;
            this.CurrentStart = 0;
            this.CurrentEnd = 0;
            if (this.listDispatchInfoCurrentPage != null)
            {
                this.listDispatchInfoCurrentPage.Clear();
            }
            this.ListDispatchInfoCurrentPage = null;
            if (this.TotalCount != 0)
            {
                this.ComeFirstCommandExecute();
            }
        }
        public DelegateCommand ComeFirstCommand { get; set; }//首页
        public DelegateCommand ComePrevCommand { get; set; }//前一页
        public DelegateCommand ComeNextCommand { get; set; }//下一页
        public DelegateCommand ComeLastCommand { get; set; }//末页

        /*前一页按钮是否可用*/
        private bool prevEnable = true;
        public bool PrevEnable
        {
            get { return prevEnable; }
            set
            {
                if (prevEnable != value)
                {
                    prevEnable = value;
                    this.RaisePropertyChanged("PrevEnable");
                }
            }
        }
        /*首页按钮是否可用*/
        private bool firstEnable = true;
        public bool FirstEnable
        {
            get { return firstEnable; }
            set
            {
                if (firstEnable != value)
                {
                    firstEnable = value;
                    this.RaisePropertyChanged("FirstEnable");
                }
            }
        }
        /*尾页按钮是否可用*/
        private bool lastEnable = true;
        public bool LastEnable
        {
            get { return lastEnable; }
            set
            {
                if (lastEnable != value)
                {
                    lastEnable = value;
                    this.RaisePropertyChanged("LastEnable");
                }
            }
        }
        /*下一页按钮是否可用*/
        private bool nextEnable = true;
        public bool NextEnable
        {
            get { return nextEnable; }
            set
            {
                if (nextEnable != value)
                {
                    nextEnable = value;
                    this.RaisePropertyChanged("NextEnable");
                }
            }
        }
        private void SetPrevNextEnable()
        {
            if (this.CurrentPage == 1)
            {
                this.PrevEnable = false;
                this.FirstEnable = false;
            }
            else
            {
                this.PrevEnable = true;
                this.FirstEnable = true;
            }
            if (this.CurrentPage == this.TotalPage)
            {
                this.NextEnable = false;
                this.LastEnable = false;
            }
            else
            {
                this.NextEnable = true;
                this.LastEnable = true;
            }
        }

        public void ComeFirstCommandExecute()
        {
            while (true)
            {
                if (StaticTreeState.DispatchCenterPage == LoadingState.LOADCOMPLETE)
                {
                    break;
                }
            }
            StaticTreeState.DispatchCenterPage = LoadingState.LOADING;
            if (this.TotalCount == 0)
            {
                return;
            }
            this.CurrentPage = 1;
            this.CurrentStart = 1;
            this.CurrentEnd = this.PageSize > this.TotalCount ? this.TotalCount : this.PageSize;
            List<VehicleDispatchItemViewModel> pageTmp = new List<VehicleDispatchItemViewModel>();
            int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
            for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
            {
                pageTmp.Add(this.listDispatchInfo[i]);
            }
            if (this.listDispatchInfoCurrentPage != null)
            {
                this.listDispatchInfoCurrentPage.Clear();
            }
            this.ListDispatchInfoCurrentPage = pageTmp;

            SetPrevNextEnable();
            StaticTreeState.DispatchCenterPage = LoadingState.LOADCOMPLETE;
        }
        public void ComePrevCommandExecute()
        {
            while (true)
            {
                if (StaticTreeState.DispatchCenterPage == LoadingState.LOADCOMPLETE)
                {
                    break;
                }
            }
            StaticTreeState.DispatchCenterPage = LoadingState.LOADING;
            if (this.TotalCount == 0)
            {
                return;
            }
            if (this.CurrentPage > 1)
            {
                this.CurrentPage--;
                this.CurrentEnd = this.CurrentStart - 1;
                this.CurrentStart = (this.CurrentPage - 1) * this.PageSize + 1;
                List<VehicleDispatchItemViewModel> pageTmp = new List<VehicleDispatchItemViewModel>();
                int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
                for (int i = this.CurrentStart - 1; i < this.CurrentEnd; i++)
                {
                    pageTmp.Add(this.listDispatchInfo[i]);
                }
                if (this.listDispatchInfoCurrentPage != null)
                {
                    this.listDispatchInfoCurrentPage.Clear();
                }
                this.ListDispatchInfoCurrentPage = pageTmp;
            }
            SetPrevNextEnable();
            StaticTreeState.DispatchCenterPage = LoadingState.LOADCOMPLETE;
        }
        public void ComeNextCommandExecute()
        {
            while (true)
            {
                if (StaticTreeState.DispatchCenterPage == LoadingState.LOADCOMPLETE)
                {
                    break;
                }
            }
            StaticTreeState.DispatchCenterPage = LoadingState.LOADING;
            if (this.TotalCount == 0)
            {
                return;
            }
            if (this.CurrentPage < this.TotalPage)
            {
                this.CurrentStart = this.CurrentPage * this.PageSize + 1;
                this.CurrentEnd = (this.CurrentStart + this.PageSize - 1) > this.TotalCount ? this.TotalCount : (this.CurrentStart + this.PageSize - 1);
                this.CurrentPage++;
                List<VehicleDispatchItemViewModel> pageTmp = new List<VehicleDispatchItemViewModel>();
                int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
                for (int i = CurrentStart - 1; i < this.CurrentEnd; i++)
                {
                    pageTmp.Add(this.listDispatchInfo[i]);
                }
                if (this.listDispatchInfoCurrentPage != null)
                {
                    this.listDispatchInfoCurrentPage.Clear();
                }
                this.ListDispatchInfoCurrentPage = pageTmp;
            }
            SetPrevNextEnable();
            StaticTreeState.DispatchCenterPage = LoadingState.LOADCOMPLETE;
        }
        public void ComeLastCommandExecute()
        {
            while (true)
            {
                if (StaticTreeState.DispatchCenterPage == LoadingState.LOADCOMPLETE)
                {
                    break;
                }
            }
            StaticTreeState.DispatchCenterPage = LoadingState.LOADING;
            if (this.TotalCount == 0)
            {
                return;
            }
            this.CurrentPage = this.TotalPage;
            this.CurrentStart = this.TotalPage > 1 ? (this.CurrentPage - 1) * this.PageSize : 1;
            this.CurrentEnd = this.TotalCount;
            List<VehicleDispatchItemViewModel> pageTmp = new List<VehicleDispatchItemViewModel>();
            int sequence = (this.CurrentPage - 1) * this.PageSize + 1;//序号
            for (int i = CurrentStart; i < this.CurrentEnd; i++)
            {
                pageTmp.Add(this.listDispatchInfo[i]);
            }
            if (this.listDispatchInfoCurrentPage != null)
            {
                this.listDispatchInfoCurrentPage.Clear();
            }
            this.ListDispatchInfoCurrentPage = pageTmp;

            SetPrevNextEnable();
            StaticTreeState.DispatchCenterPage = LoadingState.LOADCOMPLETE;
        }
        /*当前页*/
        private int currentPage;
        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    this.RaisePropertyChanged("CurrentPage");
                }
            }
        }
        /*总共页数*/
        private int totalPage;
        public int TotalPage
        {
            get { return totalPage; }
            set
            {
                if (totalPage != value)
                {
                    totalPage = value;
                    this.RaisePropertyChanged("TotalPage");
                }
            }
        }
        /*当前页记录的开始条数*/
        private int currentStart;
        public int CurrentStart
        {
            get { return currentStart; }
            set
            {
                if (currentStart != value)
                {
                    currentStart = value;
                    this.RaisePropertyChanged("CurrentStart");
                }
            }
        }
        /*当前页记录的最后条数*/
        private int currentEnd;
        public int CurrentEnd
        {
            get { return currentEnd; }
            set
            {
                if (currentEnd != value)
                {
                    currentEnd = value;
                    this.RaisePropertyChanged("CurrentEnd");
                }
            }
        }
        /*总条数*/
        private int totalCount;
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                if (totalCount != value)
                {
                    totalCount = value;
                    this.RaisePropertyChanged("TotalCount");
                }
            }
        }
        #endregion
    }
}
