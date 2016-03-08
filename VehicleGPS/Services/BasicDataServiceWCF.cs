
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models.Login;
using System.Data;
using System.Threading;
using VehicleGPS.Models;
using VehicleGPS.DBWCFServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using VehicleGPS.Views.Control.MonitorCentre.Instruction;
namespace VehicleGPS.Services
{
    class BasicDataServiceWCF : IBasicDataService
    {
        public void GetVehicleRightThread()
        {
            Thread nThread = new Thread(new ThreadStart(this.GetVehicleRightInfo));
            nThread.Start();
        }
        /// <summary>
        /// 获取单位权限
        /// </summary>
        public void GetClientRightThread()
        {
            Thread nThread = new Thread(new ThreadStart(this.GetClientRightInfo));
            nThread.Start();
        }
        public void GetRegionRightThread()
        {
            Thread nThread = new Thread(new ThreadStart(this.GetRegionRightInfo));
            nThread.Start();
        }
        /// <summary>
        /// 获取车辆详细信息（此处不包括GPS信息）
        /// </summary>
        public void GetVehicleDetailThread()
        {
            Thread nThread = new Thread(new ThreadStart(this.GetVehicleDetailInfo));
            nThread.Start();
        }
        public void GetBasicTypeThread()
        {
            Thread nThread = new Thread(new ThreadStart(this.GetBasicTypeInfo));
            nThread.Start();
        }
        public void GetRightThread()
        {
            Thread nThread = new Thread(new ThreadStart(this.GetRightInfo));
            nThread.Start();
        }
        public void GetInstructionRightThread()
        {
            Thread nThread = new Thread(new ThreadStart(this.GetInstructionRightInfo));
            nThread.Start();
        }
        public void GetVehicleRightInfo()
        {
            if (Monitor.TryEnter(StaticTreeState.VehicleBasicMutex, 10000))
            {
                try
                {
                    List<CVBasicInfo> listTmp = new List<CVBasicInfo>();
                    StaticTreeState.VehicleBasicInfo = LoadingState.LOADING;//正在加载车辆关键信息
                    StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
                    string jsonStr = VehicleCommon.wcfDBHelper.BexecuteProc(loginInfo.UserName, "getTreeVehicleWithRight");
                    DataTable vehicleDT = new DataTable();
                    vehicleDT = JsonHelper.JsonToDataTable(jsonStr);
                    this.VehicleDataTableToBasicList(vehicleDT, listTmp);
                    StaticBasicInfo basicInfo = StaticBasicInfo.GetInstance();
                    basicInfo.ListVehicleBasicInfo.Clear();//清空原有数据
                    basicInfo.ListVehicleBasicInfo = listTmp;//获取新数据
                    StaticTreeState.VehicleBasicInfo = LoadingState.LOADCOMPLETE;//用户关键信息加载完成
                }
                catch (Exception e)
                {
                    StaticTreeState.VehicleBasicInfo = LoadingState.LOADDINGFAIL;//用户关键信息加载失败
                }
                finally
                {
                    Monitor.Exit(StaticTreeState.VehicleBasicMutex);
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 获取单位权限
        /// </summary>
        public void GetClientRightInfo()
        {
            if (Monitor.TryEnter(StaticTreeState.ClientBasicMutex, 10000))
            {
                try
                {
                    List<CVBasicInfo> listTmp = new List<CVBasicInfo>();
                    StaticTreeState.ClientBasicInfo = LoadingState.LOADING;//正在加载用户关键信息
                    StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
                    string sql = "select NodeId as ID,NodeName as Name,ParentNodeId as ParentID,VehicleType as TypeID,SIM from RightsDetails a left join InfoVehicle b on a.NodeId=b.Vehiclenum where UserId='" + loginInfo.UserName + "'";
                    string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(loginInfo.UserName, sql);
                    string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                    listTmp = (List<CVBasicInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<CVBasicInfo>));
                    this.UnitVehicleToBasicList(jsonStr, listTmp);
                    StaticBasicInfo basicInfo = StaticBasicInfo.GetInstance();
                    basicInfo.ListClientBasicInfo.Clear();//清空原有数据
                    basicInfo.ListVehicleBasicInfo.Clear();

                    basicInfo.ListVehicleOfClientBaseInfo.Clear();
                    foreach (CVBasicInfo cbi in listTmp)
                    {
                        if (cbi.ID.Contains("VEHI"))
                        {
                            string[] str2 = cbi.Name.Split(new char[] { ',' });
                            cbi.Name = str2[0];
                            cbi.InnerID = str2[1];
                            basicInfo.ListVehicleBasicInfo.Add(cbi);
                            if (!basicInfo.ListVehicleOfClientBaseInfo.Contains(cbi.ParentID))
                                basicInfo.ListVehicleOfClientBaseInfo.Add(cbi.ParentID);

                        }
                        else
                        {
                            if (cbi.ParentID == null)
                                cbi.ParentID = "admin";
                            cbi.TypeID = "Unit";
                            basicInfo.ListClientBasicInfo.Add(cbi);
                        }
                    }
                    string parentid = null;
                    int i = 0;
                    foreach (CVBasicInfo client in basicInfo.ListClientBasicInfo)
                    {
                        //寻找父节点
                        if (i++ == 0)
                        {
                            parentid = client.ParentID;
                        }
                        else
                        {
                            if (string.Equals(parentid, client.ID))
                            {
                                parentid = client.ParentID;
                            }
                        }
                    }
                    foreach (CVBasicInfo client in basicInfo.ListClientBasicInfo)
                    {
                        if (string.Equals(client.ParentID, parentid))
                        {
                            client.ParentID = "admin";
                        }
                    }
                    StaticTreeState.ClientBasicInfo = LoadingState.LOADCOMPLETE;//用户关键信息加载完成
                    StaticTreeState.VehicleBasicInfo = LoadingState.LOADCOMPLETE;//用户关键信息加载完成
                }
                catch (Exception e)
                {
                    StaticTreeState.ClientBasicInfo = LoadingState.LOADDINGFAIL;//用户关键信息加载失败
                }
                finally
                {
                    Monitor.Exit(StaticTreeState.ClientBasicMutex);
                }
            }
            else
            {
                return;
            }
        }
        public void GetRegionRightInfo()
        {
            if (Monitor.TryEnter(StaticTreeState.RegionBasicMutex, 10000))
            {
                try
                {
                    List<CRegionInfo> listTmp = new List<CRegionInfo>();
                    StaticTreeState.RegionBasicInfo = LoadingState.LOADING;//正在加载用户关键信息
                    StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
                    string sql = "select IR.regId zoneId,IR.regName zoneName,IR.regLongitude Long," +
                        "IR.regLatitude lat,IR.regRadius radio,IR.unitId customerid," +
                        "IU.UNITNAME customername from InfoRegion IR,InfoUnit IU where IR.unitId=IU.UNITID and IR.unitId in (select NodeId from RightsDetails where UserId='" + loginInfo.UserName + "')";
                    string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(loginInfo.UserName, sql);
                    string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                    listTmp = (List<CRegionInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<CRegionInfo>));
                    StaticRegionInfo basicInfo = StaticRegionInfo.GetInstance();
                    basicInfo.ListRegionBasicInfo.Clear();//清空原有数据
                    basicInfo.ListRegionBasicInfo = listTmp;//获取新数据
                    StaticTreeState.RegionBasicInfo = LoadingState.LOADCOMPLETE;//用户关键信息加载完成
                }
                catch (Exception e)
                {
                    StaticTreeState.RegionBasicInfo = LoadingState.LOADDINGFAIL;//用户关键信息加载失败
                }
                finally
                {
                    Monitor.Exit(StaticTreeState.RegionBasicMutex);
                }
            }
            else
            {
                return;
            }
        }
        /*判断站点是否是脏数据，是否有父节点，若找不到父节点，则是脏数据*/
        private bool HasParent(CVBasicInfo info, List<CVBasicInfo> listInfo)
        {
            if (info.ParentID.Trim() == "admin")
            {
                return true;
            }
            foreach (CVBasicInfo tmp in listInfo)
            {
                if (info.ParentID.Trim() == tmp.ID.Trim())
                {
                    if (tmp.ParentID.Trim() == "admin")
                    {
                        return true;
                    }
                    else
                    {
                        return HasParent(tmp, listInfo);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取车辆详细信息（此处不包括GPS信息）
        /// </summary>
        public void GetVehicleDetailInfo()
        {
            if (Monitor.TryEnter(StaticTreeState.VeicleAllBasicMutex, 10000))
            {
                try
                {
                    List<CVDetailInfo> listTmp = new List<CVDetailInfo>();
                    StaticTreeState.VehicleAllBasicInfo = LoadingState.LOADING;
                    StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
                    string jsonStr = VehicleCommon.wcfDBHelper.BexecuteProc(loginInfo.UserName, "getVehiclesDetailsWithRight");
                    if (jsonStr == "error")
                    {
                        StaticTreeState.VehicleAllBasicInfo = LoadingState.LOADDINGFAIL;
                    }
                    else
                    {
                        string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                        listTmp = (List<CVDetailInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<CVDetailInfo>));
                        //DataTable vehicleDT = new DataTable();
                        //vehicleDT = JsonHelper.JsonToDataTable(jsonStr);
                        ////////14-6-6 没有做处理 this.DoVehicleInfo(listTmp);

                        StaticDetailInfo detailInfo = StaticDetailInfo.GetInstance();
                        //gps信息赋值
                        foreach (CVDetailInfo newInfo in listTmp)
                        {
                            foreach (CVDetailInfo oldInfo in detailInfo.ListVehicleDetailInfo)
                            {
                                if (newInfo.VehicleNum.Equals(oldInfo.VehicleNum))
                                {
                                    newInfo.VehicleGPSInfo = oldInfo.VehicleGPSInfo;
                                    break;
                                }
                            }
                        }
                        detailInfo.ListVehicleDetailInfo.Clear();//清空原有数据
                        detailInfo.ListVehicleDetailInfo = listTmp;//获取新数据
                        StaticTreeState.VehicleAllBasicInfo = LoadingState.LOADCOMPLETE;//车辆全部基础部信息加载完成
                    }
                }
                catch (Exception e)
                {
                    StaticTreeState.VehicleAllBasicInfo = LoadingState.LOADDINGFAIL;//用户关键信息加载失败
                }
                finally
                {
                    Monitor.Exit(StaticTreeState.VeicleAllBasicMutex);
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// 获取基本类型信息
        /// </summary>
        public void GetBasicTypeInfo()
        {
            if (Monitor.TryEnter(StaticTreeState.BasicTypeMutex, 10000))
            {
                try
                {
                    List<BasicTypeInfo> listTmp = new List<BasicTypeInfo>();
                    StaticTreeState.BasicTypeInfo = LoadingState.LOADING;
                    StaticLoginInfo loginInfo = StaticLoginInfo.GetInstance();
                    string jsonStr = VehicleCommon.wcfDBHelper.BexecuteProc(loginInfo.UserName, "getBasictypes");
                    DataTable basictypeDT = new DataTable();
                    basictypeDT = JsonHelper.JsonToDataTable(jsonStr);
                    this.BasicTypeDataTableToBasicList(basictypeDT, listTmp);
                    StaticBasicType basicInfo = StaticBasicType.GetInstance();
                    basicInfo.ListBasicTypeInfo.Clear();//清空原有数据
                    basicInfo.ListBasicTypeInfo = listTmp;//获取新数据
                    StaticTreeState.BasicTypeInfo = LoadingState.LOADCOMPLETE;//基本类型信息加载完成
                }
                catch (Exception e)
                {
                    StaticTreeState.BasicTypeInfo = LoadingState.LOADDINGFAIL;//基本类型信息加载失败
                }
                finally
                {
                    Monitor.Exit(StaticTreeState.BasicTypeMutex);
                }
            }
            else
            {
                return;
            }
        }

        #region 获取权限
        /// <summary>
        /// 获取权限（注释掉了）
        /// </summary>
        public void GetRightInfo()
        {
            try
            {
                string sql = "select * from MenuRights where parentmenuid is null and UserId='" + StaticLoginInfo.GetInstance().UserName + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (string.Compare(jsonStr, "error") != 0)
                {
                    DataTable dt = new DataTable();
                    dt = JsonHelper.JsonToDataTable(jsonStr);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        StaticTreeState.RigthInfo = LoadingState.LOADING;//正在加载权限信息
                        VehicleCommon.InitRightSet();//初始化权限集合
                        foreach (DataRow row in dt.Rows)
                        {
                            string menuname = row["MenuName"].ToString() == "" ? null : row["MenuName"].ToString();
                            if (menuname == null)
                            {
                                continue;
                            }
                            switch (menuname)
                            {
                                case "监控中心":
                                    VehicleCommon.MenuSet.Add(VehicleCommon.InitRightInfo("监控中心", "qx00002"));
                                    break;
                                case "调度中心":
                                    VehicleCommon.MenuSet.Add(VehicleCommon.InitRightInfo("调度中心", "qx00002"));
                                    break;
                                case "报表中心":
                                    VehicleCommon.MenuSet.Add(VehicleCommon.InitRightInfo("报表中心", "qx00002"));
                                    break;
                                default:
                                    break;
                            }
                        }
                        StaticRight.GetInstance().ListMenuRight = VehicleCommon.MenuSet;
                        StaticTreeState.RigthInfo = LoadingState.LOADCOMPLETE;//用户权限信息加载完成
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                StaticTreeState.RigthInfo = LoadingState.LOADDINGFAIL;//用户关键信息加载失败
            }
        }

        ///<summary>
        ///获取指令权限
        ///</summary>
        public void GetInstructionRightInfo()
        {
            try
            {
                string sql = "select * from MenuRights where UserId='" + StaticLoginInfo.GetInstance().UserName + "' and RightType='instruction'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (string.Compare(jsonStr, "error") != 0)
                {
                    DataTable dt = new DataTable();
                    dt = JsonHelper.JsonToDataTable(jsonStr);
                    if (dt != null)
                    {
                        this.InsRightDataTableToList(dt);
                    }
                    StaticTreeState.InstructionInfo = LoadingState.LOADCOMPLETE;//指令信息加载完成
                }
            }
            catch (System.Exception ex)
            {
                StaticTreeState.InstructionInfo = LoadingState.LOADDINGFAIL;//指令信息加载失败
            }
        }
        #endregion

        /*将DataTable转为List*/
        private void VehicleDataTableToBasicList(DataTable vehicleDT, List<CVBasicInfo> listBasicInfo)
        {
            for (int i = 0; i < vehicleDT.Rows.Count; i++)
            {
                CVBasicInfo basicInfo = new CVBasicInfo();
                basicInfo.ID = vehicleDT.Rows[i]["Vehiclenum"].ToString();
                basicInfo.InnerID = vehicleDT.Rows[i]["FInnerId"].ToString();
                basicInfo.Name = vehicleDT.Rows[i]["VehicleId"].ToString();
                basicInfo.ParentID = vehicleDT.Rows[i]["CustomerID"].ToString();
                basicInfo.SIM = vehicleDT.Rows[i]["SIM"].ToString();
                basicInfo.TypeID = vehicleDT.Rows[i]["VehicleType"].ToString();
                basicInfo.OnlineCount = 0;
                basicInfo.Count = 0;
                listBasicInfo.Add(basicInfo);
            }
        }
        /// <summary>
        /// 将单位信息的DataTable转为List
        /// </summary>
        /// <param name="clientDT"></param>
        /// <param name="listBasicInfo"></param>
        private void ClientDataTableToBasicList(DataTable clientDT, List<CVBasicInfo> listBasicInfo)
        {
            for (int i = 0; i < clientDT.Rows.Count; i++)
            {
                //CVBasicInfo basicInfo = new CVBasicInfo();
                //basicInfo.ID = clientDT.Rows[i]["USERID"].ToString();
                //basicInfo.InnerID = "0";
                //basicInfo.Name = clientDT.Rows[i]["EUSERNAME"].ToString();
                //basicInfo.ParentID = clientDT.Rows[i]["FartherID"].ToString();
                //basicInfo.TypeID = clientDT.Rows[i]["TypeID"].ToString();
                //basicInfo.SIM = "0";
                //basicInfo.OnlineCount = 0;
                //basicInfo.Count = 0;
                //listBasicInfo.Add(basicInfo);
                ///////userid,nodeid,nodename,parentnodeid
                CVBasicInfo basicInfo = new CVBasicInfo();
                basicInfo.ID = clientDT.Rows[i]["nodeid"].ToString();
                basicInfo.InnerID = "0";
                basicInfo.Name = clientDT.Rows[i]["nodename"].ToString();
                basicInfo.ParentID = clientDT.Rows[i]["parentnodeid"].ToString();
                basicInfo.TypeID = "0";
                basicInfo.SIM = "0";
                basicInfo.OnlineCount = 0;
                basicInfo.Count = 0;
                listBasicInfo.Add(basicInfo);
            }
        }
        /// <summary>
        /// 将单位信息的DataTable转为List
        /// </summary>
        /// <param name="clientDT"></param>
        /// <param name="listBasicInfo"></param>
        private void UnitVehicleToBasicList(string clientJson, List<CVBasicInfo> listBasicInfo)
        {
            if (clientJson == "error")
                return;
            /// Newtonsoft.Json.Linq.JObject ja = (JArray)JsonConvert.DeserializeObject(clientJson);
            //for (int i = 0; i < clientDT.Rows.Count; i++)
            //{

            //    CVBasicInfo basicInfo = new CVBasicInfo();
            //    basicInfo.ID = clientDT.Rows[i]["nodeid"].ToString();
            //    basicInfo.InnerID = "0";
            //    basicInfo.Name = clientDT.Rows[i]["nodename"].ToString();
            //    basicInfo.ParentID = clientDT.Rows[i]["parentnodeid"].ToString();
            //    basicInfo.TypeID = "0";
            //    basicInfo.SIM = "0";
            //    basicInfo.OnlineCount = 0;
            //    basicInfo.Count = 0;
            //    listBasicInfo.Add(basicInfo);
            //}
        }
        /*处理车辆详细数据*/
        private void DoVehicleInfo(List<CVDetailInfo> lisDetailInfo)
        {
            for (int i = 0; i < lisDetailInfo.Count; i++)
            {
                //车辆当前状态
                string vstate = lisDetailInfo[i].VehiclecurState;
                string cstate;
                if (vstate == "0")
                {
                    cstate = VehicleCommon.VStateNormal;
                }
                else if (vstate == "1")
                {
                    cstate = VehicleCommon.VStateTemp;
                }
                else if (vstate == "2")
                {
                    cstate = VehicleCommon.VStateMaintain;
                }
                else
                {
                    cstate = "";
                }
                lisDetailInfo[i].VehiclecurState = cstate;
                ////////////////////所属单位(数据库中查询)
                //////////////////if (Monitor.TryEnter(StaticTreeState.ClientBasicMutex, 1000))
                //////////////////{
                //////////////////    if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE)
                //////////////////    {
                //////////////////        foreach (CVBasicInfo cbi in StaticBasicInfo.GetInstance().ListClientBasicInfo)
                //////////////////        {
                //////////////////            //if (cbi.ID == lisDetailInfo[i].CustomerID)
                //////////////////            //{
                //////////////////            //    lisDetailInfo[i].CustomerName = cbi.Name;
                //////////////////            //    break;
                //////////////////            //}
                //////////////////            /// 14-6-6 修改字段
                //////////////////            if (cbi.ID == lisDetailInfo[i].ParentUnitId)
                //////////////////            {
                //////////////////                lisDetailInfo[i].ParentUnitName = cbi.Name;
                //////////////////                break;
                //////////////////            }
                //////////////////        }
                //////////////////    }
                //////////////////    Monitor.Exit(StaticTreeState.ClientBasicMutex);
                //////////////////}
                //车类型
                int loadTimesCounter = 0;
                int loadTimes = 5;
                while (StaticTreeState.BasicTypeInfo != LoadingState.LOADCOMPLETE)
                {
                    Thread.Sleep(200);
                    if (++loadTimesCounter == loadTimes)
                    {
                        break;
                    }
                }
                if (StaticTreeState.BasicTypeInfo == LoadingState.LOADCOMPLETE)
                {
                    foreach (BasicTypeInfo bti in StaticBasicType.GetInstance().ListBasicTypeInfo)
                    {
                        if (bti.TypeID == lisDetailInfo[i].VehicleType)
                        {
                            lisDetailInfo[i].VehicleTypeName = bti.TypeName;
                            break;
                        }
                    }
                }
                //任务状况
                vstate = lisDetailInfo[i].VehicleState;

                if (vstate == "0")
                {
                    cstate = VehicleCommon.TaskNo;
                }
                else if (vstate == "1")
                {
                    cstate = VehicleCommon.TaskIng;
                }
                else if (vstate == "2")
                {
                    cstate = VehicleCommon.TaskMaintain;
                }
                else if (vstate == "3")
                {
                    cstate = VehicleCommon.TaskOff;
                }
                else
                {
                    cstate = VehicleCommon.TaskNo;
                }
                lisDetailInfo[i].VehicleState = cstate;
            }
        }
        private void BasicTypeDataTableToBasicList(DataTable basicTypeDT, List<BasicTypeInfo> listBasicInfo)
        {
            for (int i = 0; i < basicTypeDT.Rows.Count; i++)
            {
                BasicTypeInfo basicInfo = new BasicTypeInfo();
                basicInfo.GroupName = basicTypeDT.Rows[i]["TYPEIDNAME"].ToString();
                basicInfo.TypeID = basicTypeDT.Rows[i]["TYPEID"].ToString();
                basicInfo.TypeName = basicTypeDT.Rows[i]["TYPENAME"].ToString();
                listBasicInfo.Add(basicInfo);
            }
        }
        private void RightDataTableToList(DataTable rightDT)
        {
            StaticRight rightInstance = StaticRight.GetInstance();
            for (int i = 0; i < rightDT.Rows.Count; i++)
            {
                string id = rightDT.Rows[i]["fautocounter"].ToString() == "null" ? "" : rightDT.Rows[i]["fautocounter"].ToString();
                string typeid = rightDT.Rows[i]["righttype"].ToString() == "null" ? "" : rightDT.Rows[i]["righttype"].ToString();
                string name = rightDT.Rows[i]["chnname"].ToString() == "null" ? "" : rightDT.Rows[i]["chnname"].ToString();
                string parentid = rightDT.Rows[i]["pid"].ToString() == "null" ? "" : rightDT.Rows[i]["pid"].ToString();
                if (typeid == VehicleCommon.MenuTypeID)
                {
                    foreach (RightInfo info in VehicleCommon.MenuSet)
                    {//菜单权限
                        if (info.Name == name)
                        {
                            info.ID = id;
                            info.ParentID = parentid;
                            rightInstance.ListMenuRight.Add(info);
                            break;
                        }
                    }
                }
                if (typeid == VehicleCommon.ReportTypeID)
                {
                    bool bFind = false;
                    foreach (RightInfo info in VehicleCommon.MenuSet)
                    {//信息报表菜单属于报表权限类
                        if (info.Name == name)
                        {
                            info.ID = id;
                            info.ParentID = parentid;
                            rightInstance.ListMenuRight.Add(info);
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind)
                    {
                        continue;
                    }
                    foreach (RightInfo info in VehicleCommon.CommonSet)
                    {//常用报表权限
                        if (info.Name == name)
                        {
                            info.ID = id;
                            info.ParentID = parentid;
                            rightInstance.ListCommonRight.Add(info);
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind)
                    {
                        continue;
                    }
                    foreach (RightInfo info in VehicleCommon.MileageSet)
                    {//里程分析权限
                        if (info.Name == name)
                        {
                            info.ID = id;
                            info.ParentID = parentid;
                            rightInstance.ListMileageRight.Add(info);
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind)
                    {
                        continue;
                    }
                    foreach (RightInfo info in VehicleCommon.OilSet)
                    {//油耗分析权限
                        if (info.Name == name)
                        {
                            info.ID = id;
                            info.ParentID = parentid;
                            rightInstance.ListOilRight.Add(info);
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind)
                    {
                        continue;
                    }
                    foreach (RightInfo info in VehicleCommon.RunSet)
                    {//运行分析权限
                        if (info.Name == name)
                        {
                            info.ID = id;
                            info.ParentID = parentid;
                            rightInstance.ListRunRight.Add(info);
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind)
                    {
                        continue;
                    }
                    foreach (RightInfo info in VehicleCommon.AlarmSet)
                    {//告警分析权限
                        if (info.Name == name)
                        {
                            info.ID = id;
                            info.ParentID = parentid;
                            rightInstance.ListAlarmRight.Add(info);
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind)
                    {
                        continue;
                    }
                    foreach (RightInfo info in VehicleCommon.RecordSet)
                    {//行车记录权限
                        if (info.Name == name)
                        {
                            info.ID = id;
                            info.ParentID = parentid;
                            rightInstance.ListRecordRight.Add(info);
                            bFind = true;
                            break;
                        }
                    }
                }
            }
        }

        private void InsRightDataTableToList(DataTable dt)
        {
            InstructionRight rightInstance = InstructionRight.GetInstance();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string id = dt.Rows[i]["MenuId"].ToString() == "null" ? "" : dt.Rows[i]["MenuId"].ToString();
                string typeid = dt.Rows[i]["RightType"].ToString() == "null" ? "" : dt.Rows[i]["RightType"].ToString();
                string name = dt.Rows[i]["MenuName"].ToString() == "null" ? "" : dt.Rows[i]["MenuName"].ToString();
                string parentid = dt.Rows[i]["ParentMenuId"].ToString() == "null" ? "" : dt.Rows[i]["ParentMenuId"].ToString();
                RightInfo item = new RightInfo();
                item.ID = id;
                item.TypeID = typeid;
                item.Name = name;
                item.ParentID = parentid;
                rightInstance.ListInstructionRight.Add(item);
            }
        }
        /*执行sql语句返回DataTable*/
        public DataTable ExecuteSql(string sql)
        {
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                return null;
            }
            return JsonHelper.JsonToDataTable(jsonStr);
        }

        /*插入调度信息*/
        public bool InsertDispatchInfo(string xmlParam)
        {
            string jsonStr = VehicleCommon.wcfDBHelper.BInsertTransDetailProc(StaticLoginInfo.GetInstance().UserName, "InsertTransDetail", xmlParam);
            if (jsonStr == "error")
            {
                return false;
            }
            return true;
        }
    }
}

