using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VehicleGPS.Models;
using System.Windows;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch;
using System.Threading;
using System.Windows.Threading;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;

namespace VehicleGPS.Services.DispatchCentre.VehicleDispatch
{
    class VehicleDispatchDataOperate
    {
        private BasicDataServiceWCF WcfService { get; set; }
        private BusinessDataServiceWEB WebService { get; set; }
        public VehicleDispatchDataOperate()
        {
            this.WcfService = new BasicDataServiceWCF();
            this.WebService = new BusinessDataServiceWEB();
        }
        /*根据站点获取任务单号*/
        public List<string> GetTaskNumberList(string StationID)
        {
            List<string> listTask = new List<string>();
            listTask.Add("全部");
            if (StationID == "all")
            {
                return listTask;
            }
            string sql = "select FPlanId from PlanConcreteTran where RegionID='" + StationID + "' and zt=1";
            this.Task_DataTableToList(this.WcfService.ExecuteSql(sql), listTask);
            return listTask;
        }
        /*将任务单号DataTable转为List*/
        private void Task_DataTableToList(DataTable dt, List<string> listTask)
        {
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string task = dt.Rows[i]["FPlanId"].ToString();
                    listTask.Add(task);
                }
            }
        }
        /*初始化所有站点*/
        //public void InitStationListThread(VehicleDispatchViewModel dispatchVM)
        //{
        //    Thread nThread = new Thread(delegate() { this.InitStationList(dispatchVM); });
        //    nThread.Start();
        //}
        public void InitStationList(VehicleDispatchViewModel dispatchVM)
        {
            List<CVBasicInfo> tmpList = new List<CVBasicInfo>();
            CVBasicInfo all = new CVBasicInfo();
            all.ID = "all";
            all.Name = "全部";
            tmpList.Add(all);
            string sql = "select UNITID,UNITNAME from InfoUnit where unitId in (select NodeId unitId from RightsDetails where UserId='" + StaticLoginInfo.GetInstance().UserName + "')";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (!(string.Compare(jsonStr, "error") == 0))
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                foreach (DataRow dr in dt.Rows)
                {
                    CVBasicInfo cvb = new CVBasicInfo();
                    cvb.ID = dr["UNITID"].ToString();
                    cvb.Name = dr["UNITNAME"].ToString();
                    tmpList.Add(cvb);
                }
            }
            dispatchVM.ListStation = tmpList;
            dispatchVM.SelectedStationIndex = 0;
        }

        /*根据查找任务单号详细数据*/
        public List<VehicleDispatchItemViewModel> GetTaskDetailInfo(string stationID, string taskID)
        {
            List<VehicleDispatchItemViewModel> listTaskInfo = new List<VehicleDispatchItemViewModel>();
            string sql = @"select distinct *,t.concretNum concretNum,t.unitName UNITNAME,t.tripcount tripcount,r1.reglongitude startlng,r1.reglatitude startlat,r1.regradius startradius,"
                       + "r2.reglongitude endlng,r2.reglatitude endlat,r2.regradius endradius "
                       + " from TranTaskList t, inforegion r1, inforegion r2 where t.taskStatus='3' and r1.regid=startregid and r2.regid=endregid "
                       + " and t.unitId in (select NodeId unitId from RightsDetails where UserId='" + StaticLoginInfo.GetInstance().UserName + "')";
            if (stationID != "all")
            {
                sql += " and t.unitId='" + stationID + "'";
            }
            if (taskID != "全部")
            {
                sql += " and t.taskListId='" + taskID + "'";
            }
            this.TaskInfo_DataTableToList(this.WcfService.ExecuteSql(sql), listTaskInfo);
            return listTaskInfo;
        }
        public void TaskInfo_DataTableToList(DataTable dt, List<VehicleDispatchItemViewModel> list)
        {
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TaskInfo info = new TaskInfo();
                    info.ConcreteName = dt.Rows[i]["concretNum"] == null ? "" : dt.Rows[i]["concretNum"].ToString();
                    info.UnitName = dt.Rows[i]["UNITNAME"] == null ? "" : dt.Rows[i]["UNITNAME"].ToString();
                    info.tripcount = dt.Rows[i]["tripcount"] == null ? 0 : dt.Rows[i]["tripcount"].ToString() == "null" ? 0 : int.Parse(dt.Rows[i]["tripcount"].ToString());
                    info.FProjName = dt.Rows[i]["endRegName"].ToString();
                    info.FPlanId = dt.Rows[i]["taskListId"].ToString();
                    info.StartPoint = dt.Rows[i]["startRegName"].ToString();
                    info.EndPoint = dt.Rows[i]["endRegName"].ToString();
                    info.TransCap = dt.Rows[i]["transTotalCube"].ToString();
                    info.TransedCap = dt.Rows[i]["transedCube"].ToString() == "null" ? 0 + "" : dt.Rows[i]["transedCube"].ToString();
                    info.NoTransCap = (Convert.ToDouble(info.TransCap) - Convert.ToDouble(info.TransedCap)).ToString();
                    info.TransDistance = dt.Rows[i]["transDistance"].ToString();
                    info.CarsPlanStatus = dt.Rows[i]["taskStatus"].ToString();
                    info.Count = dt.Rows[i]["carTranCount"].ToString();
                    info.Site = dt.Rows[i]["site"].ToString();
                    info.StartTime = dt.Rows[i]["startTime"].ToString();
                    info.EndTime = dt.Rows[i]["endTime"].ToString();
                    info.FinsertTime = dt.Rows[i]["insertTime"].ToString();
                    info.StartID = dt.Rows[i]["startRegId"].ToString();
                    info.EndID = dt.Rows[i]["endRegId"].ToString();
                    info.RegionID = dt.Rows[i]["startRegId"].ToString() == "null" ? "" : dt.Rows[i]["startRegId"].ToString();
                    info.RegionName = dt.Rows[i]["startRegName"].ToString() == "null" ? "" : dt.Rows[i]["startRegName"].ToString();

                    info.StartLat = dt.Rows[i]["startlat"].ToString() == "null" ? "" : dt.Rows[i]["startlat"].ToString();
                    info.StartLng = dt.Rows[i]["startlng"].ToString() == "null" ? "" : dt.Rows[i]["startlng"].ToString();
                    info.StartRadius = dt.Rows[i]["startradius"].ToString() == "null" ? "" : dt.Rows[i]["startradius"].ToString();
                    info.EndLat = dt.Rows[i]["endlat"].ToString() == "null" ? "" : dt.Rows[i]["endlat"].ToString();
                    info.EndLng = dt.Rows[i]["endlng"].ToString() == "null" ? "" : dt.Rows[i]["endlng"].ToString();
                    info.EndRadius = dt.Rows[i]["endradius"].ToString() == "null" ? "" : dt.Rows[i]["endradius"].ToString();
                    info.RealTransDistance = VehicleCommon.GetDistance(Convert.ToDouble(info.StartLat), Convert.ToDouble(info.StartLng),
                            Convert.ToDouble(info.EndLat), Convert.ToDouble(info.EndLng)) - Convert.ToDouble(info.StartRadius) / 1000 - Convert.ToDouble(info.EndRadius) / 1000;
                    VehicleDispatchItemViewModel itemVM = new VehicleDispatchItemViewModel();
                    itemVM.TaskNumberInfo = info;
                    list.Add(itemVM);
                }
            }
        }
        /*获取运输物品*/
        public List<BasicTypeInfo> GetTransmitObject(string objectIDs)
        {
            List<BasicTypeInfo> listBasicInfo = new List<BasicTypeInfo>();
            string[] objects = objectIDs.Split(';');
            if (StaticTreeState.BasicTypeInfo != LoadingState.LOADCOMPLETE)
            {
                MessageBox.Show("混凝土基本类型未加载完成，请稍后或刷新树形目录", "数据加载", MessageBoxButton.OKCancel);
                return null;
            }

            foreach (string obj in objects)
            {
                foreach (BasicTypeInfo info in StaticBasicType.GetInstance().ListBasicTypeInfo)
                {
                    if (obj == info.TypeID)
                    {
                        listBasicInfo.Add(info);
                        break;
                    }
                }
            }
            BasicTypeInfo noSelected = new BasicTypeInfo();//不选择
            noSelected.TypeID = "noselected";
            noSelected.TypeName = "不选择";
            listBasicInfo.Insert(0, noSelected);
            return listBasicInfo;
        }
        /*获取驾驶员信息*/
        public List<DriverInfo> GetDriverInfo(string taskId)
        {
            List<DriverInfo> list = new List<DriverInfo>();
            if (taskId != "")
            {
                string sql = "select StaffId,StaffName,SexType,ParentUnitId "
                           + "from InfoStaff "
                           + "where StaffType='driverstaff' and IsFree='1' and ParentUnitId=(select unitId from TranTaskList where taskListId='" + taskId + "')";
                this.DriverInfo_DataTableToList(this.WcfService.ExecuteSql(sql), list);
            }
            DriverInfo noselected = new DriverInfo();
            noselected.workID = "不选择驾驶员";
            noselected.DriverName = "不选择驾驶员";
            list.Insert(0, noselected);
            return list;
        }
        public void DriverInfo_DataTableToList(DataTable dt, List<DriverInfo> list)
        {
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DriverInfo info = new DriverInfo();
                    info.sequence = (i + 1).ToString();
                    info.workID = dt.Rows[i]["StaffId"].ToString() == "null" ? "" : dt.Rows[i]["StaffId"].ToString();
                    info.DriverName = dt.Rows[i]["StaffName"].ToString() == "null" ? "" : dt.Rows[i]["StaffName"].ToString();
                    info.sex = dt.Rows[i]["SexType"].ToString() == "null" ? "" : dt.Rows[i]["SexType"].ToString();
                    info.zoneID = dt.Rows[i]["ParentUnitId"].ToString() == "null" ? "" : dt.Rows[i]["ParentUnitId"].ToString();

                    list.Add(info);
                }
            }
        }
        /*获取塔楼信息*/
        public List<TowerInfo> GetTowerInfo(string taskId)
        {
            List<TowerInfo> list = new List<TowerInfo>();
            if (taskId != "")
            {
                string sql = "select Towerid,TowerName,ParentUnitName,ParentUnitID "
                           + "from InfoTower "
                           + "where ParentUnitID=(select unitId from TranTaskList where taskListId='" + taskId + "')";
                this.TowerInfo_DataTableToList(this.WcfService.ExecuteSql(sql), list);
            }
            TowerInfo noSelected = new TowerInfo();//不选择
            noSelected.towerid = "noselected";
            noSelected.towername = "不选择";
            list.Insert(0, noSelected);
            return list;
        }
        private void TowerInfo_DataTableToList(DataTable dt, List<TowerInfo> list)
        {
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TowerInfo info = new TowerInfo();
                    info.towerid = dt.Rows[i]["Towerid"].ToString() == "null" ? "" : dt.Rows[i]["Towerid"].ToString();
                    info.towername = dt.Rows[i]["TowerName"].ToString() == "null" ? "" : dt.Rows[i]["TowerName"].ToString();
                    info.parentid = dt.Rows[i]["ParentUnitID"].ToString() == "null" ? "" : dt.Rows[i]["ParentUnitID"].ToString();

                    list.Add(info);
                }
            }
        }
        #region 获取调度的所有运输途中、返回途中以及在工地卸料的车辆信息
        public void InitDispatchInfo(VehicleDispatchItemViewModel taskItem)
        {
            string startID = taskItem.TaskNumberInfo.StartID;
            string endID = taskItem.TaskNumberInfo.EndID;
            string taskID = taskItem.TaskNumberInfo.FPlanId;
            List<DispatchVehicleInfo> list = new List<DispatchVehicleInfo>();
            string sql = "select tranTaskListId FPlanId,"
                       + "driverId,driverName,vehicleNum,"
                       + "worktimetype Holiday,"
                       + "transgoods ConcreteID,"
                       + "transpercap TransCapPer,"
                       + "unloadtype offtype,"
                       + "outStartRegTime LeaveStartPointTime,"
                       + "carStatus CarsStatus "
                       + "from TranTaskListDetail where trantasklistid = '" + taskID + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
            List<DispatchVehicleInfo> listTmp = (List<DispatchVehicleInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<DispatchVehicleInfo>));
            //if (string.Compare(jsonStr,"error")!=0)
            //{
            //    tmp = new List<DispatchVehicleInfo>();
            //    DataTable dt = new DataTable();
            //    dt = JsonHelper.JsonToDataTable(jsonStr);
            //    int i=1;
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        DispatchVehicleInfo dvi = new DispatchVehicleInfo();
            //        dvi.sequence = i.ToString();
            //        dvi.FPlanId = dr["tranTaskListId"].ToString();
            //        dvi.FPlanId = dr["vehicleNum"].ToString();
            //        dvi.FPlanId = dr["workTimeType"].ToString();
            //        dvi.FPlanId = dr["transGoods"].ToString();
            //        dvi.FPlanId = dr["transPerCap"].ToString();
            //        dvi.FPlanId = dr["unloadType"].ToString();
            //        dvi.FPlanId = dr["driverId"].ToString();
            //        dvi.FPlanId = dr["driverName"].ToString();
            //        dvi.FPlanId = dr["assignTime"].ToString();
            //        dvi.FPlanId = dr["inStartRegTime"].ToString();
            //        dvi.FPlanId = dr["outStartRegTime"].ToString();
            //        dvi.FPlanId = dr["inEndRegTime"].ToString();
            //        dvi.FPlanId = dr["outEndRegTime"].ToString();
            //        dvi.FPlanId = dr["startUnloadTime"].ToString();
            //        dvi.FPlanId = dr["endUnloadTime"].ToString();
            //        dvi.FPlanId = dr["insertTime"].ToString();
            //        dvi.FPlanId = dr["carStatus"].ToString();
            //    }
            //}

            this.DispatchVehicle_DataComplete(listTmp, taskItem);
        }
        private void DispatchVehicle_DataComplete(List<DispatchVehicleInfo> tmpList, VehicleDispatchItemViewModel taskItem)
        {
            if (tmpList != null && tmpList.Count != 0)
            {
                List<DispatchVehicleViewModel> listTaskIng = new List<DispatchVehicleViewModel>();
                List<DispatchVehicleViewModel> listInSite = new List<DispatchVehicleViewModel>();
                List<DispatchVehicleViewModel> listInRegion = new List<DispatchVehicleViewModel>();
                List<DispatchVehicleViewModel> listRetRegion = new List<DispatchVehicleViewModel>();
                double transedCap = 0;//交付的方量
                int hasDispatch = 0;//已发车次
                for (int i = 0; i < tmpList.Count; i++)
                {
                    if (tmpList[i].LeaveStartPointTime != "")
                    {
                        tmpList[i].LeaveStartPointTime = Convert.ToDateTime(tmpList[i].LeaveStartPointTime).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        tmpList[i].LeaveStartPointTime = "未出区域";
                    }
                    if (StaticTreeState.BasicTypeInfo == LoadingState.LOADCOMPLETE)
                    {
                        foreach (BasicTypeInfo basicInfo in StaticBasicType.GetInstance().ListBasicTypeInfo)
                        {
                            if (basicInfo.TypeID == tmpList[i].VehicleType)
                            {//车辆类别
                                tmpList[i].VehicleTypeName = basicInfo.TypeName;
                            }
                            if (basicInfo.TypeID == tmpList[i].ConcreteID)
                            {//混凝土标号
                                tmpList[i].ConcreteName = basicInfo.TypeName;
                            }
                            if (basicInfo.TypeID == tmpList[i].offtype)
                            {//卸料方式
                                tmpList[i].offTypeName = basicInfo.TypeName;
                            }
                        }
                    }
                    else
                    {//否则显示ID
                        tmpList[i].VehicleTypeName = tmpList[i].VehicleType;
                        tmpList[i].ConcreteName = tmpList[i].ConcreteID;
                    }
                    /*判断车辆任务状态*/
                    DispatchVehicleViewModel vm = new DispatchVehicleViewModel();
                    if (tmpList[i].CarsStatus == ((int)VehicleCommon.TaskDriveState.ReachUnload).ToString())
                    {//工地内卸料
                        tmpList[i].CarsStatus = VehicleCommon.TaskDriveReachUnload;
                        vm.DispatchInfo = tmpList[i];
                        listInSite.Add(vm);

                        tmpList[i].sequence = (listInSite.Count).ToString();
                        //增加交付方量
                        transedCap += Convert.ToDouble(tmpList[i].TransCapPer);
                    }
                    else if (tmpList[i].CarsStatus == ((int)VehicleCommon.TaskDriveState.ReturnIng).ToString())
                    {//返程途中
                        tmpList[i].CarsStatus = VehicleCommon.TaskDriveReturnIng;
                        vm.DispatchInfo = tmpList[i];
                        listTaskIng.Add(vm);

                        tmpList[i].sequence = (listTaskIng.Count).ToString();
                        //增加交付方量
                        transedCap += Convert.ToDouble(tmpList[i].TransCapPer);
                    }
                    else if (tmpList[i].CarsStatus == ((int)VehicleCommon.TaskDriveState.TransIng).ToString())
                    {//运输途中
                        tmpList[i].CarsStatus = VehicleCommon.TaskDriveTransIng;
                        vm.DispatchInfo = tmpList[i];
                        listTaskIng.Add(vm);

                        tmpList[i].sequence = (listTaskIng.Count).ToString();
                        //增加交付方量
                        transedCap += Convert.ToDouble(tmpList[i].TransCapPer);
                    }
                    else if (tmpList[i].CarsStatus == ((int)VehicleCommon.TaskDriveState.InStartPoint).ToString())
                    {//站点内未发车
                        tmpList[i].CarsStatus = VehicleCommon.TaskDriveInStartPoint;
                        vm.DispatchInfo = tmpList[i];
                        listInRegion.Add(vm);

                        tmpList[i].sequence = (listInRegion.Count).ToString();
                        //增加交付方量
                        transedCap += Convert.ToDouble(tmpList[i].TransCapPer);
                    }
                    else if (tmpList[i].CarsStatus == ((int)VehicleCommon.TaskDriveState.ReturnStartPoint).ToString())
                    {//已返回站内
                        tmpList[i].CarsStatus = VehicleCommon.TaskDriveReturnStartPoint;
                        vm.DispatchInfo = tmpList[i];
                        listRetRegion.Add(vm);
                        //增加交付方量
                        transedCap += Convert.ToDouble(tmpList[i].TransCapPer);
                    }
                    if (!(tmpList[i].CarsStatus.Equals("6")))
                    {
                        hasDispatch++;//已发车次
                    }
                    /*找到该车辆详细基础信息*/
                    if (StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE)
                    {
                        foreach (CVDetailInfo vdi in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                        {
                            if (vdi.VehicleNum == tmpList[i].VehicleNum)
                            {
                                try
                                {
                                    vm.DispatchInfo.VehicleID = vdi.VehicleId;
                                    vm.DetailInfo = vdi;
                                    break;
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }
                taskItem.ListInSiteVehicle = listInSite;
                taskItem.ListDispatchVehicle = listTaskIng;
                taskItem.ListInRegionVehicle = listInRegion;
                taskItem.ListRetRegionVehicle = listRetRegion;
                /*此刻已经交付的方量和进度以及已发车次*/
                taskItem.HasDispatch = hasDispatch;
                taskItem.TransedCap = transedCap;
                taskItem.TaskNumberInfo.TransedCap = transedCap.ToString("0.0");
                taskItem.TaskNumberInfo.NoTransCap = (Double.Parse(taskItem.TaskNumberInfo.TransCap) - transedCap).ToString("0.0");
                taskItem.TransedProgress = (transedCap / Convert.ToDouble(taskItem.TaskNumberInfo.TransCap) * 100).ToString("0.00");
                /*获取最新的gps信息*/
                this.WebService.GetLatestVehicleGPSInfoByVehicleID(taskItem);
            }
        }
        #endregion

        #region 插入调度数据到数据库
        public void InsertDispatchInfo(string xmlStr)
        {
            if (xmlStr != "")
            {
                bool ret = this.WcfService.InsertDispatchInfo(xmlStr);
                if (ret == true)
                {
                    MessageBox.Show("调度信息添加成功", "数据库操作", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("调度信息添加失败", "数据库操作", MessageBoxButton.OK);
                }
            }
        }
        #endregion

        #region 查询工地信息
        public List<SiteInfo> GetSiteInfo()
        {
            List<SiteInfo> listTmp = new List<SiteInfo>();
            StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
            string jsonStr = VehicleCommon.wcfDBHelper.BexecuteProc(loginInfo.UserName, "proc_GongDiRights");
            if (jsonStr == "error")
            {
                return null;
            }
            DataTable siteDT = new DataTable();
            siteDT = JsonHelper.JsonToDataTable(jsonStr);
            this.SiteTableToDetailList(siteDT, listTmp);
            return listTmp;
        }
        private void SiteTableToDetailList(DataTable dt, List<SiteInfo> list)
        {
            if (dt != null && dt.Rows.Count != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SiteInfo info = new SiteInfo();
                    info.ZoneID = dt.Rows[i]["ZoneID"].ToString() == "null" ? "" : dt.Rows[i]["ZoneID"].ToString();
                    info.ZoneName = dt.Rows[i]["ZoneName"].ToString() == "null" ? "" : dt.Rows[i]["ZoneName"].ToString();
                    info.Long = dt.Rows[i]["Long"].ToString() == "null" ? "" : dt.Rows[i]["Long"].ToString();
                    info.Lat = dt.Rows[i]["Lat"].ToString() == "null" ? "" : dt.Rows[i]["Lat"].ToString();
                    info.Radio = dt.Rows[i]["Radio"].ToString() == "null" ? "" : dt.Rows[i]["Radio"].ToString();
                    info.TransTime = dt.Rows[i]["TransTime"].ToString() == "null" ? "" : dt.Rows[i]["TransTime"].ToString();
                    info.OffTime = dt.Rows[i]["OffTime"].ToString() == "null" ? "" : dt.Rows[i]["OffTime"].ToString();
                    info.StartFdate = dt.Rows[i]["MyStartFdate"].ToString() == "null" ? "" : dt.Rows[i]["MyStartFdate"].ToString();
                    info.IsTransAlarm = dt.Rows[i]["IsTransAlarm"].ToString() == "null" ? "" : dt.Rows[i]["IsTransAlarm"].ToString();
                    info.IsOffAlarm = dt.Rows[i]["IsOffAlarm"].ToString() == "null" ? "" : dt.Rows[i]["IsOffAlarm"].ToString();
                    info.IsStartFAlarm = dt.Rows[i]["IsStartFAlarm"].ToString() == "null" ? "" : dt.Rows[i]["IsStartFAlarm"].ToString();
                    list.Add(info);
                }
            }
        }

        /*更新时间提醒*/
        public bool UpdateTimeRemindSetting(string TaskListId, string transTime, bool IsTransAlarm, string offTime, bool IsOffAlarm, string startFdate, bool IsStartFAlarm)
        {
            //string sql = "update inforegion set transtime=" + TransTime
            //                                + ",offtime=" + OffTime
            //                                + ",startfdate='" + StartFdate
            //                                + "',istransalarm=" + (IsTransAlarm == true ? 1 : 0)
            //                                + ",isoffalarm=" + (IsOffAlarm == true ? 1 : 0)
            //                                + ",isstartfalarm=" + (IsStartFAlarm == true ? 1 : 0)
            //                                + " where zt=1 and zoneid='" + taskListId + "'";
            if (IsTransAlarm)
            {
                string sql = "update TranTaskList set TransTime='" + transTime + "'where taskListId='" + TaskListId + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (jsonStr == "error")
                {
                    return false;
                }
            }
            if (IsOffAlarm)
            {
                string sql = "update TranTaskList set OffTime='" + offTime + "'where taskListId='" + TaskListId + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (jsonStr == "error")
                {
                    return false;
                }
            }
            if (IsStartFAlarm)
            {
                string sql = "update TranTaskList set StartFdate='" + startFdate + "'where taskListId='" + TaskListId + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (jsonStr == "error")
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
