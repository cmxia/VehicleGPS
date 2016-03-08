using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Services.DispatchCentre.TaskManage;
using VehicleGPS.Views.Control.DispatchCentre;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models.DispatchCentre.TaskManage;
using System.Windows;
using VehicleGPS.Models;
using VehicleGPS.Views.Control.DispatchCentre.TaskManage;
using VehicleGPS.Services;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.ViewModels.DispatchCentre.TaskManage
{
    class TaskAddModDelViewModel : NotificationObject
    {
        public TaskManageViewModel parentViewModel = null;//父ViewModel
        private OperateType OpeType;
        private Window window { get; set; }
        public TaskAddModDelViewModel(Object parentDataContext, OperateType ot, object window)
        {
            this.window = (Window)window;
            this.OpeType = ot;
            this.parentViewModel = (TaskManageViewModel)parentDataContext;
            this.ConfirmCommand = new DelegateCommand(new Action(ConfirmCommandExecute));
            /*对于修改和添加*/
            if (ot == OperateType.MOD)
            {
                this.SetEnable(true);
                this.InitData();
            }
            if (ot == OperateType.ADD)
            {
                this.RegionName = this.parentViewModel.SelectedStation.Name;
                this.SetEnable(false);
            }
            this.GetGoods();
            this.GetSite();
            this.GetRegion();
        }

        #region 添加修改操作
        /*确定*/
        public DelegateCommand ConfirmCommand { get; set; }
        private void ConfirmCommandExecute()
        {
            if (this.ListRegion == null || this.ListRegion.Count == 0)
            {
                MessageBox.Show("没有区域信息", "提示", MessageBoxButton.OK);
                return;
            }
            if (this.ListSite == null || this.ListSite.Count == 0)
            {
                MessageBox.Show("没有工地信息", "提示", MessageBoxButton.OK);
                return;
            }
            //if (string.IsNullOrEmpty(this.ConcreteName))
            //{
            //    MessageBox.Show("请选择运输物品", "提示", MessageBoxButton.OK);
            //    return;
            //}
            //if (string.IsNullOrEmpty(this.FAgreementid))
            //{
            //    MessageBox.Show("请填写合同编号", "提示", MessageBoxButton.OK);
            //    return;
            //}
            //if (string.IsNullOrEmpty(this.FAgreementid))
            //{
            //    MessageBox.Show("请填写合同编号", "提示", MessageBoxButton.OK);
            //    return;
            //}
            //if (Convert.ToDateTime(this.StartTime) > Convert.ToDateTime(this.EndTime))
            //{
            //    MessageBox.Show("时间格式不正确", "提示", MessageBoxButton.OK);
            //    return;
            //}
            //if (string.IsNullOrEmpty(this.FAgreementid))
            //{
            //    MessageBox.Show("请填写合同编号", "提示", MessageBoxButton.OK);
            //    return;
            //}
            if (string.IsNullOrEmpty(this.startTime))
            {
                MessageBox.Show("请填写计划开始时间", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(this.EndTime))
            {
                MessageBox.Show("请填写计划结束时间", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(this.TransCap))
            {
                MessageBox.Show("请填写方量", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(this.TransDistance))
            {
                MessageBox.Show("请填写运输距离", "提示", MessageBoxButton.OK);
                return;
            }
            //if (string.IsNullOrEmpty(this.Site))
            //{
            //    MessageBox.Show("请填写施工部位", "提示", MessageBoxButton.OK);
            //    return;
            //}
            //if (string.IsNullOrEmpty(this.FAgreementid))
            //{
            //    MessageBox.Show("请填写合同编号", "提示", MessageBoxButton.OK);
            //    return;
            //}
            if (this.OpeType == OperateType.MOD)
            {
                // if (string.IsNullOrEmpty(this.Count))
                // {
                //    MessageBox.Show("请填写已发车次", "提示", MessageBoxButton.OK);
                //     return;
                //   }
                // if (string.IsNullOrEmpty(this.TransedCap))
                // {
                //     MessageBox.Show("请填写已运输方量", "提示", MessageBoxButton.OK);
                //     return;
                //   }
                this.ModRegion();
            }
            if (this.OpeType == OperateType.ADD)
            {
                this.AddRegion();
            }

        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void CloseWindow()
        {
            this.window.Close();
        }
        /*添加任务单*/
        private void AddRegion()
        {
            this.FPlanId = IDHelper.GetTaskID();
            // this.TransedCap = "0";//已运输放量
            this.taskStatus = "3";//任务状态
            //this.Count = "0";//已发车次
            string insertNameList = "taskListId,unitName,unitId,endRegName,endRegId,startRegName,startRegId,transTotalCube,site,transDistance,taskStatus,startTime,EndTime,viscosity,insertTime,firstOverTime,secondOverTime,concretNum,tripcount,transedCube";
            string insertValueList = "'" + this.FPlanId + "'," +
                //"'" + this.FAgreementid + "'," +
                // "'" + this.ConcreteId + "'," +
                //  "'" + this.ListGoods[this.SelectedGoodsIndex].Name + "'," +
                "'" + this.parentViewModel.SelectedStation.Name + "'," +
                "'" + this.parentViewModel.SelectedStation.ID + "'," +
                "'" + this.ListSite[this.SelectedSiteIndex].Name + "'," +
                "'" + this.ListSite[this.SelectedSiteIndex].ID + "'," +
                "'" + this.ListRegion[this.SelectedRegionIndex].Name + "'," +
                "'" + this.ListRegion[this.SelectedRegionIndex].ID + "'," +
                "'" + this.TransCap + "'," +
                // "'" + this.TransedCap + "'," +
                "'" + this.Site + "'," +
                "'" + this.TransDistance + "'," +
                //"'" + this.CarsPlanStatus + "'," +
                //  "'" + this.Count + "'," +
                "'" + this.taskStatus + "'," +
                "'" + this.startTime + "'," +
                //"'" + this.StartTime + "'," +
              "'" + this.EndTime + "'," +
              "'" + this.Viscosity + "'," +
              "'" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," +
              "'" + this.FirstOverTime + "'," +
              "'" + this.SecondOverTime + "'," +
              "'" + ConcreteName + "','0','0'";
            // "'"+this.TaskStatus+"'";
            // "'" + this.Site + "'," +
            //"'" + this.FMemo + "','1'";
            string sql = "insert into TranTaskList(" + insertNameList + ") values(" + insertValueList + ")";
            string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(status, "error") != 0)
            {
                MessageBox.Show("添加成功!", "提示", MessageBoxButton.OK);
                Dictionary<string, string> instruction = new Dictionary<string, string>();
                instruction.Add("cmd", "DISPATCH_TYPE");
                instruction.Add("cmdid", "123_DISPATCH_TYPE");
                instruction.Add("ID", FPlanId);
                instruction.Add("type", "TASK");
                instruction.Add("OPERATETYPE", "1");
                string insstring = JsonConvert.SerializeObject(instruction);
                zmq.zmqPackHelper.zmqInstructionsPack("123", insstring);
                //DispatchTaskInfo newInfo = new DispatchTaskInfo();
                //newInfo.Sequence = this.parentViewModel.ListDispatchTaskInfo.Count + 1;
                //newInfo.TaskListId = this.FPlanId;
                //// newInfo.FAgreementid = this.FAgreementid;
                //newInfo.TransGoods = this.ListGoods[this.SelectedGoodsIndex].Name;
                //newInfo.UnitName = this.parentViewModel.SelectedStation.Name;
                //newInfo.UnitId = this.parentViewModel.SelectedStation.ID;
                //newInfo.EndRegName = this.ListSite[this.SelectedSiteIndex].Name;
                //newInfo.EndRegId = this.ListSite[this.SelectedSiteIndex].ID;
                //newInfo.StartRegName = this.ListRegion[this.SelectedRegionIndex].Name;
                //newInfo.StartRegId = this.ListRegion[this.SelectedRegionIndex].ID;
                //newInfo.TransTotalCube = this.TransCap;
                //// newInfo.TransedCube = this.TransedCap;
                //newInfo.site = this.Site;
                //newInfo.TransDistance = this.TransDistance;
                //newInfo.taskStatus = VehicleCommon.GetTaskState(this.TaskStatus);
                ////  newInfo.CarTranCount = this.Count;
                //newInfo.startTime = this.StartTime;
                //newInfo.EndTime = this.EndTime;
                //newInfo.viscosity = this.Viscosity;
                //newInfo.concretNum = this.ConcreteName;
                //newInfo.InsertTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //newInfo.firstOverTime = this.FirstOverTime;
                //newInfo.secondOverTime = this.SecondOverTime;
                ////newInfo.FMemo = this.FMemo;
                //this.parentViewModel.ListDispatchTaskInfo.Add(newInfo);
                //this.parentViewModel.LocalRefreshData();
                CloseWindow();
                VehicleDispatchViewModel.GetInstance().QueryCommandExecute();
            }
            else
            {
                MessageBox.Show("添加失败!", "提示", MessageBoxButton.OK);
                //this.taskStatus = VehicleCommon.GetTaskState(this.TaskStatus);
            }
        }
        /*修改站点*/
        private void ModRegion()
        {
            string sql = "update TranTaskList " +
                "set " +
                //"FAgreementid='" + this.FAgreementid + "'," +
                //  "transGoods='" + this.ListGoods[this.SelectedGoodsIndex].Name +"'," +
                "unitName='" + this.parentViewModel.SelectedStation.Name + "'," +
                "unitId='" + this.parentViewModel.SelectedStation.ID + "'," +
                "endRegName='" + this.ListSite[this.SelectedSiteIndex].Name + "'," +
                "endRegId='" + this.ListSite[this.SelectedSiteIndex].ID + "'," +
                "startRegName='" + this.ListRegion[this.SelectedRegionIndex].Name + "'," +
                "startRegId='" + this.ListRegion[this.SelectedRegionIndex].ID + "'," +
                "transTotalCube='" + this.TransCap + "'," +
                // "TransedCube='" + this.TransedCap + "'," +
               "site='" + this.site + "'," +
                "TransDistance='" + this.TransDistance + "'," +
               "carTranCount='" + this.Count + "'," +
                "startTime='" + this.StartTime + "'," +
               "EndTime='" + this.EndTime + "'," +
               "viscosity='" + this.viscosity + "'," +
               "firstOverTime='" + this.FirstOverTime + "'," +
               "concretNum='" + this.ConcreteName + "'," +
               "secondOverTime='" + this.SecondOverTime + "'" +
                //"FMemo='" + this.FMemo + "'" +
                " where taskListId='" + this.FPlanId + "'";
            string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(status, "error") != 0)
            {
                MessageBox.Show("修改成功!", "提示", MessageBoxButton.OK);
                Dictionary<string, string> instruction = new Dictionary<string, string>();
                instruction.Add("cmd", "DISPATCH_TYPE");
                instruction.Add("cmdid", "123_DISPATCH_TYPE");
                instruction.Add("ID", FPlanId);
                instruction.Add("type", "TASK");
                instruction.Add("OPERATETYPE", "2");
                string insstring = JsonConvert.SerializeObject(instruction);
                zmq.zmqPackHelper.zmqInstructionsPack("123", insstring);
                //DispatchTaskInfo newInfo = new DispatchTaskInfo();
                //DispatchTaskInfo selectedRegion = this.parentViewModel.SelectedTask;

                //// selectedRegion.FAgreementid = this.FAgreementid;
                ////selectedRegion.TransGoods = this.ListGoods[this.SelectedGoodsIndex].Name;
                //selectedRegion.UnitName = this.parentViewModel.SelectedStation.Name;
                //selectedRegion.UnitId = this.parentViewModel.SelectedStation.ID;
                //selectedRegion.EndRegName = this.ListSite[this.SelectedSiteIndex].Name;
                //selectedRegion.EndRegId = this.ListSite[this.SelectedSiteIndex].ID;
                //selectedRegion.StartRegName = this.ListRegion[this.SelectedRegionIndex].Name;
                //selectedRegion.StartRegId = this.ListRegion[this.SelectedRegionIndex].ID;
                //selectedRegion.TransTotalCube = this.TransCap;
                //selectedRegion.site = this.Site;
                ////selectedRegion.TransedCube = this.TransedCap;
                //selectedRegion.TransDistance = this.TransDistance;
                ////  selectedRegion.taskStatus = this.TaskStatus;
                //// selectedRegion.CarTranCount = this.Count;
                //selectedRegion.concretNum = this.ConcreteName;
                //selectedRegion.startTime = this.StartTime;
                //selectedRegion.EndTime = this.EndTime;
                //selectedRegion.viscosity = this.Viscosity;
                //selectedRegion.firstOverTime = this.FirstOverTime;
                //selectedRegion.secondOverTime = this.SecondOverTime;
                ////selectedRegion.FMemo = this.FMemo;
                //this.parentViewModel.LocalRefreshData();
                CloseWindow();
                VehicleDispatchViewModel.GetInstance().QueryCommandExecute();
            }
            else
            {
                MessageBox.Show("修改失败!", "提示", MessageBoxButton.OK);
            }
        }
        #endregion

        #region 是否可编辑
        private void SetEnable(bool b)
        {
            this.SiteEnable = !b;
            this.RegionEnable = !b;
            this.CountEnable = b;
            this.TransedEnable = b;
        }
        private bool siteEnable;
        public bool SiteEnable
        {
            get { return siteEnable; }
            set
            {
                if (siteEnable != value)
                {
                    siteEnable = value;
                    this.RaisePropertyChanged("SiteEnable");
                }
            }
        }
        private bool regionEnable;
        public bool RegionEnable
        {
            get { return regionEnable; }
            set
            {
                if (regionEnable != value)
                {
                    regionEnable = value;
                    this.RaisePropertyChanged("RegionEnable");
                }
            }
        }
        private bool countEnable;
        public bool CountEnable
        {
            get { return countEnable; }
            set
            {
                if (countEnable != value)
                {
                    countEnable = value;
                    this.RaisePropertyChanged("CountEnable");
                }
            }
        }
        private bool transedEnable;
        public bool TransedEnable
        {
            get { return transedEnable; }
            set
            {
                if (transedEnable != value)
                {
                    transedEnable = value;
                    this.RaisePropertyChanged("TransedEnable");
                }
            }
        }
        #endregion

        #region 数据源
        //选择的工地索引
        private int selectedSiteIndex = 0;
        public int SelectedSiteIndex
        {
            get { return selectedSiteIndex; }
            set
            {
                if (selectedSiteIndex != value)
                {
                    selectedSiteIndex = value;
                    this.RaisePropertyChanged("SelectedSiteIndex");
                }
            }
        }

        //所有工地
        private List<CVBasicInfo> listSite = new List<CVBasicInfo>();
        public List<CVBasicInfo> ListSite
        {
            get { return listSite; }
            set
            {
                if (listSite != value)
                {
                    listSite = value;
                    this.RaisePropertyChanged("ListSite");
                }
            }
        }
        //获取所有工地
        /// <summary>
        /// 所在单位下的获取所有工地
        /// </summary>
        private void GetSite()
        {

            string sql = "select regId as ID,regName as Name from InfoRegion where regType='gd'  and unitId='" + this.parentViewModel.SelectedStation.ID + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
            List<CVBasicInfo> tmp = (List<CVBasicInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<CVBasicInfo>));
            this.ListSite = tmp;
            /*获取所在单位*/
            if (this.OpeType == OperateType.MOD)
            {
                for (int i = 0; i < tmp.Count; i++)
                {
                    if (tmp[i].ID == this.parentViewModel.SelectedTask.EndRegId)
                    {
                        this.SelectedSiteIndex = i;
                        break;
                    }
                }
            }
            if (this.OpeType == OperateType.ADD)
            {
                this.SelectedSiteIndex = 0;
            }
        }

        //选择的区域索引
        private int selectedRegionIndex = 0;
        public int SelectedRegionIndex
        {
            get { return selectedRegionIndex; }
            set
            {
                if (selectedRegionIndex != value)
                {
                    selectedRegionIndex = value;
                    this.RaisePropertyChanged("SelectedRegionIndex");
                }
            }
        }
        //所有区域
        private List<CVBasicInfo> listRegion = new List<CVBasicInfo>();
        public List<CVBasicInfo> ListRegion
        {
            get { return listRegion; }
            set
            {
                if (listRegion != value)
                {
                    listRegion = value;
                    this.RaisePropertyChanged("ListRegion");
                }
            }
        }
        //获取所有区域
        private void GetRegion()
        {
            string sql = "select regId as ID,regName as Name from inforegion where regType='cq' and unitId='" + this.parentViewModel.SelectedStation.ID + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
            List<CVBasicInfo> tmp = (List<CVBasicInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<CVBasicInfo>));
            this.ListRegion = tmp;
            /*获取所在单位*/
            if (this.OpeType == OperateType.MOD)
            {
                for (int i = 0; i < tmp.Count; i++)
                {
                    if (tmp[i].ID == this.parentViewModel.SelectedTask.StartRegId)
                    {
                        this.SelectedRegionIndex = i;
                        break;
                    }
                }
            }
            if (this.OpeType == OperateType.ADD)
            {
                this.SelectedRegionIndex = 0;
            }
        }

        //选择的运输物品索引
        private int selectedGoodsIndex = 0;
        public int SelectedGoodsIndex
        {
            get { return selectedGoodsIndex; }
            set
            {
                if (selectedGoodsIndex != value)
                {
                    selectedGoodsIndex = value;
                    this.RaisePropertyChanged("SelectedGoodsIndex");
                }
            }
        }
        //所有区域
        private List<CVBasicInfo> listGoods = new List<CVBasicInfo>();
        public List<CVBasicInfo> ListGoods
        {
            get { return listGoods; }
            set
            {
                if (listGoods != value)
                {
                    listGoods = value;
                    this.RaisePropertyChanged("ListGoods");
                }
            }
        }
        //获取所有区域
        private void GetGoods()
        {
            //string sql = "select regId as ID,regName as Name from inforegion where regType='cq' and unitId='" + this.parentViewModel.SelectedStation.ID + "'";
            //string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            //string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
            //List<CVBasicInfo> tmp = (List<CVBasicInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<CVBasicInfo>));
            CVBasicInfo goods = new CVBasicInfo();

            this.ListGoods.Add(new CVBasicInfo() { ID = "goods_1", Name = "水" });
            this.ListGoods.Add(new CVBasicInfo() { ID = "goods_2", Name = "混凝土" });
            /*获取所在单位*/
            if (this.OpeType == OperateType.MOD)
            {
                for (int i = 0; i < ListGoods.Count; i++)
                {
                    if (ListGoods[i].Name == this.parentViewModel.SelectedTask.TransGoods)
                    {
                        this.SelectedGoodsIndex = i;
                        break;
                    }
                }
            }
            if (this.OpeType == OperateType.ADD)
            {
                this.SelectedGoodsIndex = 0;
            }
        }

        /*任务单号*/
        private string fPlanId;
        public string FPlanId
        {
            get { return fPlanId; }
            set
            {
                if (fPlanId != value)
                {
                    fPlanId = value;
                    this.RaisePropertyChanged("FPlanId");
                }
            }
        }
        ///*混凝土标号*/
        //private string concreteId;
        //public string ConcreteId
        //{
        //    get { return concreteId; }
        //    set
        //    {
        //        if (concreteId != value)
        //        {
        //            concreteId = value;
        //            this.RaisePropertyChanged("ConcreteId");
        //        }
        //    }
        //}
        /*运输总方量*/
        private string transCap;
        public string TransCap
        {
            get { return transCap; }
            set
            {
                if (transCap != value)
                {
                    transCap = value;
                    this.RaisePropertyChanged("TransCap");
                }
            }
        }
        /*运输距离*/
        private string transDistance;
        public string TransDistance
        {
            get { return transDistance; }
            set
            {
                if (transDistance != value)
                {
                    transDistance = value;
                    this.RaisePropertyChanged("TransDistance");
                }
            }
        }
        /*已发车次统计*/
        private string count = "0";
        public string Count
        {
            get { return count; }
            set
            {
                if (count != value)
                {
                    count = value;
                    this.RaisePropertyChanged("Count");
                }
            }
        }
        ///*施工部位*/
        private string site;
        public string Site
        {
            get { return site; }
            set
            {
                if (site != value)
                {
                    site = value;
                    this.RaisePropertyChanged("Site");
                }
            }
        }
        /*开始时间*/
        private string startTime;
        public string StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime != value)
                {
                    startTime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                    this.RaisePropertyChanged("StartTime");
                }
            }
        }
        ///*结束时间*/
        private string endTime;
        public string EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime != value)
                {
                    endTime = Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
                    this.RaisePropertyChanged("EndTime");
                }
            }
        }
        /*车辆执行任务状态*/
        private string carsPlanStatus = "等待执行";
        public string CarsPlanStatus
        {
            get { return carsPlanStatus; }
            set
            {
                if (carsPlanStatus != value)
                {
                    carsPlanStatus = value;
                    this.RaisePropertyChanged("CarsPlanStatus");
                }
            }
        }
        ///*合同编号*/
        //private string fAgreementid;
        //public string FAgreementid
        //{
        //    get { return fAgreementid; }
        //    set
        //    {
        //        if (fAgreementid != value)
        //        {
        //            fAgreementid = value;
        //            this.RaisePropertyChanged("FAgreementid");
        //        }
        //    }
        //}
        /*已运输方量*/
        private string transedCap = "0";
        public string TransedCap
        {
            get { return transedCap; }
            set
            {
                if (transedCap != value)
                {
                    transedCap = value;
                    this.RaisePropertyChanged("TransedCap");
                }
            }
        }
        /*标号名称*/
        private string concreteName;
        public string ConcreteName
        {
            get { return concreteName; }
            set
            {
                if (concreteName != value)
                {
                    concreteName = value;
                    this.RaisePropertyChanged("ConcreteName");
                }
            }
        }
        /*站点名称*/
        private string regionName;
        public string RegionName
        {
            get { return regionName; }
            set
            {
                if (regionName != value)
                {
                    regionName = value;
                    this.RaisePropertyChanged("RegionName");
                }
            }
        }
        /*塌落度*/
        private string viscosity;
        public string Viscosity
        {
            get { return viscosity; }
            set
            {
                if (viscosity != value)
                {
                    viscosity = value;
                    this.RaisePropertyChanged("Viscosity");
                }
            }
        }
        /*任务单完成情况*/
        private string taskStatus;
        public string TaskStatus
        {
            get { return taskStatus; }
            set
            {
                if (taskStatus != value)
                {
                    taskStatus = value;
                    this.RaisePropertyChanged("TaskStatus");
                }
            }
        }
        /*首次提醒时间*/
        private string firstOverTime;
        public string FirstOverTime
        {
            get { return firstOverTime; }
            set
            {
                if (firstOverTime != value)
                {
                    firstOverTime = value;
                    this.RaisePropertyChanged("FirstOverTime");
                }
            }
        }
        /*再次提醒时间*/
        private string secondOverTime;
        public string SecondOverTime
        {
            get { return secondOverTime; }
            set
            {
                if (secondOverTime != value)
                {
                    secondOverTime = value;
                    this.RaisePropertyChanged("SecondOverTime");
                }
            }
        }


        ///*备注*/
        //private string fMemo;
        //public string FMemo
        //{
        //    get { return fMemo; }
        //    set
        //    {
        //        if (fMemo != value)
        //        {
        //            fMemo = value;
        //            this.RaisePropertyChanged("FMemo");
        //        }
        //    }
        //}
        /*初始化数据*/
        private void InitData()
        {
            DispatchTaskInfo selectedInfo = this.parentViewModel.SelectedTask;
            this.FPlanId = selectedInfo.TaskListId;
            this.RegionName = selectedInfo.UnitName;
            this.ConcreteName = selectedInfo.concretNum;
            //this.FAgreementid = selectedInfo.FAgreementid;
            this.StartTime = selectedInfo.InsertTime;
            //this.FMemo = selectedInfo.FMemo;
            this.EndTime = selectedInfo.EndTime;
            this.TransCap = selectedInfo.TransTotalCube;
            this.Count = selectedInfo.CarTranCount;
            this.TransDistance = selectedInfo.TransDistance;
            this.TransedCap = selectedInfo.TransedCube;
            this.CarsPlanStatus = selectedInfo.TaskStatus;
            this.Viscosity = selectedInfo.viscosity;
            this.TaskStatus = selectedInfo.taskStatus;
            this.Site = selectedInfo.site;
            this.FirstOverTime = selectedInfo.firstOverTime;
            this.SecondOverTime = selectedInfo.secondOverTime;
        }
        #endregion
    }
}
