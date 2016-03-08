using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models.Login;
using VehicleGPS.Models;
using Newtonsoft.Json;
using VehicleGPS.Models.DispatchCentre.TaskManage;
using System.Data;
using VehicleGPS.Services;

namespace VehicleGPS.ViewModels.DispatchCentre.TaskManage
{
    class TaskStatisticsViewModel : NotificationObject
    {
        public TaskStatisticsViewModel()
        {
            this.DoubleClickCommand = new DelegateCommand(new Action(this.DoubleClickCommandExecute));
            this.QueryCommand = new DelegateCommand(new Action(this.QueryCommandExecute));
            this.ExportDetailCommand = new DelegateCommand(new Action(this.ExportDetailCommandExecute));
            this.ExportStatisticsCommand = new DelegateCommand(new Action(this.ExportStatisticsCommandExecute));

            InitQueryCondition();
            InitQueryInfo();
        }

        List<StatisticsTaskInfo> OriginList = new List<StatisticsTaskInfo>();

        #region DataGrid数据源
        //选中的数据
        private StatisticsTaskInfo selectedtask;
        public StatisticsTaskInfo SelectedTask
        {
            get { return selectedtask; }
            set
            {
                selectedtask = value;
                this.RaisePropertyChanged("SelectedTask");
            }
        }
        //统计数据
        private List<StatisticsTaskInfo> listtaskinfototal;
        public List<StatisticsTaskInfo> ListTaskInfoTotal
        {
            get { return listtaskinfototal; }
            set
            {
                listtaskinfototal = value;
                this.RaisePropertyChanged("ListTaskInfoTotal");
            }
        }
        //详细数据
        private List<StatisticsTaskInfo> listtaskinfodetail;
        public List<StatisticsTaskInfo> ListTaskInfoDetail
        {
            get { return listtaskinfodetail; }
            set
            {
                listtaskinfodetail = value;
                this.RaisePropertyChanged("ListTaskInfoDetail");
            }
        }

        #endregion

        #region 初始化界面数据
        private void InitQueryCondition()
        {
            /*初始化单位列表*/
            string sql = @"select distinct NodeId UnitId,NodeName UnitName from RightsDetails where UserId='" + StaticLoginInfo.GetInstance().UserName + "' and (NodeId like 'UNIT%' or NodeId='root')";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                MessageBox.Show("所属单位初始化失败！请重试！", "提示", MessageBoxButton.OK);
            }
            else
            {
                string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                UnitListQuery = (List<Unit>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<Unit>));
            }
            Unit unit = new Unit();
            unit.UnitId = "";
            unit.UnitName = "全部";
            UnitListQuery.Insert(0, unit);

            sql = @"select distinct ir.regId SiteId,ir.regName SiteName,ir.unitId UnitId from RightsDetails rd,InfoRegion ir where rd.UserId='" + StaticLoginInfo.GetInstance().UserName
                + "' and (rd.NodeId like 'UNIT%' or rd.NodeId='root') and rd.NodeId=ir.unitId and ir.regType='gd'";
            jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                MessageBox.Show("站点列表初始化失败！请重试！", "提示", MessageBoxButton.OK);
            }
            else
            {
                string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                if (sitelist==null)
                {
                    sitelist = new List<Site>();
                }
                sitelist.Clear();
                sitelist = (List<Site>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<Site>));
            }

            this.UnitQuerySelected = 0;
        }
        private void InitSiteInfoList()
        {
            if (this.SiteQueryList == null)
            {
                this.SiteQueryList = new List<Site>();
            }
            this.SiteQueryList.Clear();
            List<Site> tmplist = new List<Site>();
            Site site = null;
            if (this.UnitQuerySelected == 0)
            {
                this.SiteQueryList = this.sitelist;
            }
            else
            {
                string unitid = this.UnitListQuery[this.UnitQuerySelected].UnitId;
                foreach (Site siteinfo in this.sitelist)
                {
                    if (siteinfo.UnitId.Equals(unitid))
                    {
                        site = new Site();
                        site.SiteId = siteinfo.SiteId;
                        site.SiteName = siteinfo.SiteName;
                        tmplist.Add(site);
                    }
                }
            }
            site = new Site();
            site.SiteId = "";
            site.SiteName = "全部";
            tmplist.Insert(0, site);
            this.SiteQueryList = tmplist;
            this.SiteQuerySelected = 0;
        }
        private void InitQueryInfo()
        {
            ListTaskInfoTotal = null;
            ListTaskInfoDetail = null;
            string querycontent = @"select taskListId taskId,unitId,
                    unitName UnitName,startRegName StartRegion,endRegName EndRegion,
                    site Position,transTotalCube PlanCount,transedCube CompleteCount,tripcount CompleteNum from TranTaskList where ";
            string querycondition = "taskListId is not null ";
            if (this.SiteQuerySelected > 0)
            {//工地条件
                querycondition += "and endRegId='" + this.SiteQueryList[this.SiteQuerySelected].SiteId + "' ";
            }
            if (this.UnitQuerySelected > 0)
            {//单位条件
                querycondition += "and unitId='" + this.UnitListQuery[this.UnitQuerySelected].UnitId + "' ";
            }
            else
            {
                querycondition += "and unitId in (select NodeId unitId from RightsDetails where UserId='" + StaticLoginInfo.GetInstance().UserName + "') ";
            }
            if (!string.IsNullOrEmpty(this.TaskNumQuery))
            {//任务单号模糊查询
                querycondition += "and taskListId like '%" + this.TaskNumQuery.Trim() + "%' ";
            }
            if (this.StartTimeQuery != null)
            {
                querycondition += "and insertTime > '" + this.StartTimeQuery.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            }
            if (this.EndTimeQuery != null)
            {
                querycondition += "and insertTime < '" + this.EndTimeQuery.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
            }
            string sql = querycontent + querycondition + "order by unitId";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                MessageBox.Show("查询失败！请重试！", "提示", MessageBoxButton.OK);
            }
            else
            {
                string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                OriginList = (List<StatisticsTaskInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<StatisticsTaskInfo>));
            }
            if (this.OriginList==null||this.OriginList.Count==0)
            {
                MessageBox.Show("该时间段没有任务，请更换时间段后再查询");
                return;
            }
            GetCountAndNum();
            GetTaskTotalInfo();
        }

        private void GetCountAndNum()
        {
            foreach (StatisticsTaskInfo taskInfo in this.OriginList)
            {
                string sql = "select transPerCap from TranTaskListDetail where (carStatus='4' or carStatus='5') and tranTaskListId='" + taskInfo.taskId + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (jsonStr.Equals("error"))
                {
                    MessageBox.Show("计算任务单数据失败！请重试");
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt = JsonHelper.JsonToDataTable(jsonStr);
                    if (dt == null)
                    {
                        continue;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        taskInfo.CompleteNum = dt.Rows.Count;
                        taskInfo.CompleteCount = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            taskInfo.CompleteCount += row["transPerCap"] == null ? 0 : Convert.ToDouble(row["transPerCap"].ToString());
                        }
                    }
                    else
                    {
                        taskInfo.CompleteCount = 0;
                        taskInfo.CompleteNum = 0;
                    }
                }
            }
        }

        private void GetTaskTotalInfo()
        {
            List<StatisticsTaskInfo> listTmp = new List<StatisticsTaskInfo>();
            string unitid = "";
            StatisticsTaskInfo taskAdd = null;
            int i = 0;
            foreach (StatisticsTaskInfo taskInfo in this.OriginList)
            {
                if (i == 0)
                {//第一条数据
                    taskAdd = new StatisticsTaskInfo();
                    taskAdd.UnitName = taskInfo.UnitName;
                    taskAdd.CompleteNum += taskInfo.CompleteNum;
                    taskAdd.CompleteCount += taskInfo.CompleteCount;
                    taskAdd.unitId = taskInfo.unitId;
                    unitid = taskInfo.unitId;
                    i++;
                }
                else
                {

                    if (string.Compare(taskInfo.unitId, unitid) == 0)
                    {
                        taskAdd.CompleteNum += taskInfo.CompleteNum;
                        taskAdd.CompleteCount += taskInfo.CompleteCount;
                    }
                    else
                    {
                        foreach (Unit unit in this.UnitListQuery)
                        {
                            if (string.Compare(unit.UnitId, taskInfo.unitId) == 0)
                            {
                                listTmp.Add(taskAdd);
                                taskAdd = new StatisticsTaskInfo();
                                taskAdd.UnitName = taskInfo.UnitName;
                                taskAdd.CompleteNum += taskInfo.CompleteNum;
                                taskAdd.CompleteCount += taskInfo.CompleteCount;
                                taskAdd.unitId = taskInfo.unitId;
                                unitid = taskInfo.unitId;
                                break;
                            }
                        }
                    }
                }
            }
            if (taskAdd != null)
            {
                listTmp.Add(taskAdd);
            }
            this.ListTaskInfoTotal = null;
            this.ListTaskInfoTotal = listTmp;
            if (listTmp.Count > 0)
            {
                this.SelectedTask = listTmp[0];
                this.DoubleClickCommandExecute();
            }
            else
            {
                this.SelectedTask = null;
            }
        }
        #endregion

        #region 绑定操作
        //双击
        public DelegateCommand DoubleClickCommand { get; set; }
        private void DoubleClickCommandExecute()
        {
            this.ListTaskInfoDetail = null;
            if (this.SelectedTask == null)
            {
                return;
            }
            List<StatisticsTaskInfo> listTmp = new List<StatisticsTaskInfo>();
            foreach (StatisticsTaskInfo task in this.OriginList)
            {
                if (task.unitId == this.SelectedTask.unitId)
                {
                    listTmp.Add(task);
                }
            }
            this.ListTaskInfoDetail = listTmp;
        }
        //查询
        public DelegateCommand QueryCommand { get; set; }
        private void QueryCommandExecute()
        {
            InitQueryInfo();
        }
        //导出统计数据
        public DelegateCommand ExportStatisticsCommand { get; set; }
        private void ExportStatisticsCommandExecute()
        {
            if (this.ListTaskInfoTotal == null || this.ListTaskInfoTotal.Count == 0)
            {
                MessageBox.Show("没有数据需要导出！");
                return;
            }
            Microsoft.Win32.SaveFileDialog savedl = new Microsoft.Win32.SaveFileDialog();
            savedl.Filter = "excel2007|*.xlsx|所有文件|*.*";//文件扩展名
            savedl.ShowDialog();
            string path = savedl.FileName;
            if (path == "")
            {
                return;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("UnitName", typeof(string));
            dt.Columns.Add("CompleteCount", typeof(string));
            dt.Columns.Add("CompleteNum", typeof(string));
            DataRow row = dt.NewRow();
            row["UnitName"] = "单位名称";
            row["CompleteCount"] = "完成方量";
            row["CompleteNum"] = "完成趟次";
            dt.Rows.Add(row);
            foreach (StatisticsTaskInfo info in this.ListTaskInfoTotal)
            {
                row = dt.NewRow();
                row["UnitName"] = info.UnitName;
                row["CompleteCount"] = info.CompleteCount.ToString();
                row["CompleteNum"] = info.CompleteNum.ToString();
                dt.Rows.Add(row);
            }
            ExcelHelper.ExportExcel(dt, path);
        }
        //导出详细数据
        public DelegateCommand ExportDetailCommand { get; set; }
        private void ExportDetailCommandExecute()
        {
            if (this.ListTaskInfoDetail == null || this.ListTaskInfoDetail.Count == 0)
            {
                MessageBox.Show("没有数据需要导出！");
                return;
            }
            Microsoft.Win32.SaveFileDialog savedl = new Microsoft.Win32.SaveFileDialog();
            savedl.Filter = "excel2007|*.xlsx|所有文件|*.*";//文件扩展名
            savedl.ShowDialog();
            string path = savedl.FileName;
            if (path == "")
            {
                return;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("taskId", typeof(string));
            dt.Columns.Add("UnitName", typeof(string));
            dt.Columns.Add("StartRegion", typeof(string));
            dt.Columns.Add("EndRegion", typeof(string));
            dt.Columns.Add("Position", typeof(string));
            dt.Columns.Add("PlanCount", typeof(string));
            dt.Columns.Add("CompleteCount", typeof(string));
            dt.Columns.Add("CompleteNum", typeof(string));
            DataRow row = dt.NewRow();
            row["UnitName"] = "单位名称";
            row["CompleteCount"] = "完成方量";
            row["CompleteNum"] = "完成趟次";
            row["taskId"] = "任务单号";
            row["StartRegion"] = "出车区域";
            row["EndRegion"] = "工地名称";
            row["Position"] = "施工部位";
            row["PlanCount"] = "计划方量";
            dt.Rows.Add(row);
            foreach (StatisticsTaskInfo info in this.ListTaskInfoDetail)
            {
                row = dt.NewRow();
                row["UnitName"] = info.UnitName;
                row["CompleteCount"] = info.CompleteCount.ToString();
                row["CompleteNum"] = info.CompleteNum.ToString();
                row["taskId"] = info.taskId;
                row["StartRegion"] = info.StartRegion;
                row["EndRegion"] = info.EndRegion;
                row["Position"] = info.Position;
                row["PlanCount"] = info.PlanCount;
                dt.Rows.Add(row);
            }
            ExcelHelper.ExportExcel(dt, path);
        }

        #endregion

        #region 绑定查询字段

        public List<Site> sitelist { get; set; }//用于存储所有的站点

        //查询选项选择
        //单位名称
        private List<Unit> unitlistquery;
        public List<Unit> UnitListQuery
        {
            get { return unitlistquery; }
            set
            {
                unitlistquery = value;
                this.RaisePropertyChanged("UnitListQuery");
            }
        }
        private int unitqueryselected;
        public int UnitQuerySelected
        {
            get { return unitqueryselected; }
            set
            {
                unitqueryselected = value;
                InitQueryInfo();
                InitSiteInfoList();
                this.RaisePropertyChanged("UnitQuerySelected");
            }
        }
        //工地名称
        private List<Site> sitequerylist;
        public List<Site> SiteQueryList
        {
            get { return sitequerylist; }
            set
            {
                sitequerylist = value;
                this.RaisePropertyChanged("SiteQueryList");
            }
        }
        private int sitequeryselected;
        public int SiteQuerySelected
        {
            get { return sitequeryselected; }
            set
            {
                sitequeryselected = value;
                InitQueryInfo();
                this.RaisePropertyChanged("SiteQuerySelected");
            }
        }
        //任务单号
        private string tasknumquery;
        public string TaskNumQuery
        {
            get { return tasknumquery; }
            set
            {
                tasknumquery = value;
                InitQueryInfo();
                this.RaisePropertyChanged("TaskNumQuery");
            }
        }
        //起止时间
        private DateTime starttiemquery = DateTime.Now.AddDays(-10);
        public DateTime StartTimeQuery
        {
            get { return starttiemquery; }
            set
            {
                starttiemquery = value;
                //InitQueryInfo();
                this.RaisePropertyChanged("StartTimeQuery");
            }
        }
        private DateTime endtimequery = DateTime.Now;
        public DateTime EndTimeQuery
        {
            get { return endtimequery; }
            set
            {
                endtimequery = value;
                this.RaisePropertyChanged("EndTimeQuery");
            }
        }
        #endregion
    }
    //单位
    class Unit
    {
        public string UnitId { get; set; }
        public string UnitName { get; set; }
    }
    //工地 
    class Site
    {
        public string UnitId { get; set; }
        public string SiteId { get; set; }
        public string SiteName { get; set; }
    }
}
