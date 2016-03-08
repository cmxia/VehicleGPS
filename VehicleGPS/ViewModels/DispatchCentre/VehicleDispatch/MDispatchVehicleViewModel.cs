using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;

using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using System.Data;
using VehicleGPS.Services;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using System.Threading;
using System.ComponentModel;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class MDispatchVehicleViewModel : NotificationObject
    {
        public static MDispatchVehicleViewModel instance = null;
        private VehicleDispatchDataOperate DataOperate { get; set; }
        private VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch.DispatchVehicle parentWindow = null;
        public MDispatchVehicleViewModel(object window, object parentVM = null)
        {
            parentWindow = (VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch.DispatchVehicle)window;
            this.DataOperate = new VehicleDispatchDataOperate();
            this.SelectedItem = (VehicleDispatchItemViewModel)parentVM;
            this.driverAvaliable = DataOperate.GetDriverInfo(this.SelectedItem.TaskNumberInfo.FPlanId);
            this.towerlist = DataOperate.GetTowerInfo(this.SelectedItem.TaskNumberInfo.FPlanId);
            this.ListSelectedVehicle = new List<VehicleInfo>();
            this.ListAvailableVehicle = new List<VehicleInfo>();
            instance = null;
            instance = (MDispatchVehicleViewModel)this;
            this.ConfirmCommand = new DelegateCommand(new Action(this.ConfirmCommandExecute));
            this.GetVehicleInfo();
            this.InitHoliday();
        }

        private bool isbusy;

        public bool IsBusy
        {
            get { return isbusy; }
            set
            {
                isbusy = value;
                this.RaisePropertyChanged("IsBusy");
            }
        }


        public DelegateCommand ConfirmCommand { get; set; }
        List<VehicleInfo> failList = null;
        List<DispatchVehicleInfo> dispatchList = null;
        int succeedCount = 0;//判断是否有成功派出的车辆
        private void ExecuteConfirm()
        {

            this.failList = new List<VehicleInfo>();
            this.dispatchList = new List<DispatchVehicleInfo>();
            int loop_count = 0;
            while (true && loop_count++ < 15)
            {
                if (StaticTreeState.DispatchTreeLoad == LoadingState.LOADCOMPLETE)
                {
                    //锁住调度树形
                    StaticTreeState.DispatchTreeLoad = LoadingState.LOADING;
                    foreach (VehicleInfo vehicle in this.ListSelectedVehicle)
                    {
                        string taskDetailID = IDHelper.GetTaskDetailID();
                        string isloop = this.IsLoopDispatch ? "1" : "0";
                        if (SendInstruction(vehicle, taskDetailID, isloop))
                        {
                            succeedCount++;
                            double triptime = this.SelectedItem.HasDispatch + this.succeedCount;
                            string drivername = "", driverid = "", towername = "", towerid = "";
                            if (vehicle.DriverSelectedIndex > 0)
                            {
                                driverid = vehicle.DriverList[vehicle.DriverSelectedIndex].workID;
                                drivername = vehicle.DriverList[vehicle.DriverSelectedIndex].DriverName;
                            }
                            if (vehicle.TowerSelectedIndex > 0)
                            {
                                towername = vehicle.TowerList[vehicle.TowerSelectedIndex].towername;
                                towerid = vehicle.TowerList[vehicle.TowerSelectedIndex].towerid;
                            }
                            string sql = "insert into TranTaskListDetail"
                                + "(id,tranTaskListId,vehicleNum,transPerCap,insertTime,carStatus,workTimeType,assigncircle,triptime,driverId,driverName,towerId,towerName)"
                                + "values('" + taskDetailID
                                + "', '" + this.SelectedItem.TaskNumberInfo.FPlanId
                                + "','" + vehicle.vehicleNum
                                + "','" + vehicle.LoadCapacity
                                + "','" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                + "','1','" + this.ListHolidayType[SelectedHolidayIndex].DayName
                                + "','" + isloop
                                + "','" + triptime
                                + "','" + driverid + "','" + drivername + "','" + towerid + "','" + towername + "')";
                            string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                            //更新驾驶员数据库
                            sql = "update InfoStaff set IsFree='0' where StaffId='" + driverid + "'";
                            VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);

                            if (string.Compare(status, "error") != 0)
                            {//生成派车单数据
                                DispatchVehicleInfo dispatchInfo = new DispatchVehicleInfo();
                                dispatchInfo.FPlanId = SelectedItem.TaskNumberInfo.FPlanId;
                                dispatchInfo.ConcreteName = SelectedItem.TaskNumberInfo.ConcreteName;
                                dispatchInfo.UnitName = SelectedItem.TaskNumberInfo.UnitName;
                                dispatchInfo.StartRegion = SelectedItem.TaskNumberInfo.StartPoint;
                                dispatchInfo.EndRegion = SelectedItem.TaskNumberInfo.EndPoint;
                                dispatchInfo.Position = SelectedItem.TaskNumberInfo.Site;
                                dispatchInfo.TransCapPer = SelectedItem.TaskNumberInfo.TransedCap;
                                dispatchInfo.transDistance = SelectedItem.TaskNumberInfo.TransDistance;
                                dispatchInfo.VehicleID = vehicle.VehicleId;
                                dispatchInfo.DriverName = drivername;
                                dispatchInfo.TowerName = towername;
                                dispatchInfo.TripTime = triptime.ToString();
                                dispatchInfo.InsertTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                this.dispatchList.Add(dispatchInfo);
                            }
                            //更新内存数据
                            foreach (var item in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
                            {
                                if (item.VehicleId.Equals(vehicle.VehicleId))
                                {
                                    item.VehicleState = "1";
                                    break;
                                }
                            }
                            foreach (CVBasicInfo info in StaticBasicInfo.GetInstance().ListVehicleBasicInfo)
                            {
                                if (info.SIM.Equals(vehicle.SIM))
                                {
                                    info.TaskState = "进行任务中";
                                    break;
                                }
                            }
                            StaticTreeState.DispatchTreeLoad = LoadingState.LOADCOMPLETE;
                        }
                        else
                        {
                            failList.Add(vehicle);
                            StaticTreeState.DispatchTreeLoad = LoadingState.LOADCOMPLETE;
                        }
                        //SendInstruction(vehicle, taskDetailID, isloop);
                    }
                }
                break;
            }

            StaticTreeState.DispatchTreeRefresh = LoadingState.LOADING;
            VehicleDispatchViewModel.GetInstance().InitDispatchInfo();
            StaticTreeState.VehicleBasicInfo = LoadingState.LOADING;
            DispatchTreeViewModel.GetInstance().TreeOperate.RefreshTree();//更新树形
            while (true)
            {
                if (StaticTreeState.DispatchTreeRefresh == LoadingState.LOADCOMPLETE)
                {
                    Thread.Sleep(1000);
                    break;
                }
            }
        }
        private void ConfirmCommandExecute()
        {
            if (!CanSubmit())
            {
                return;
            }
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                this.ExecuteConfirm();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.IsBusy = false;
                if (succeedCount == 0)
                {
                    MessageBox.Show("派车失败，请重试！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (this.failList.Count > 0)
                {
                    string msg = "以下车辆未派成功：";
                    foreach (var item in this.failList)
                    {
                        msg += "\n" + item.VehicleId;
                    }
                    MessageBox.Show(msg, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("已成功派遣" + ListSelectedVehicle.Count + "辆车！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch.DispatchVehicleQuery win = new Views.Control.DispatchCentre.VehicleDispatch.DispatchVehicleQuery(this.dispatchList);
                win.Owner = parentWindow;
                win.ShowDialog();
                parentWindow.Close();
            };
            this.IsBusy = true;
            worker.RunWorkerAsync();
        }

        private bool SendInstruction(VehicleInfo vehicle, string detailID, string isLoop)
        {
            Dictionary<string, string> instruction = new Dictionary<string, string>();
            instruction.Add("cmd", "PAICHE_TYPE");
            instruction.Add("cmdid", vehicle.SIM + "_PAICHE_TYPE");
            instruction.Add("simId", vehicle.SIM);
            instruction.Add("taskListDetailId", detailID);
            instruction.Add("taskListId", SelectedItem.TaskNumberInfo.FPlanId);
            instruction.Add("assigncircle", isLoop);
            string insstring = JsonConvert.SerializeObject(instruction);
            return zmq.zmqPackHelper.zmqInstructionsPack(vehicle.SIM, insstring);
        }

        private bool CanSubmit()
        {
            if (!(this.ListSelectedVehicle.Count > 0))
            {
                MessageBox.Show("请选择车辆", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            int itemIndex = 1;
            for (int i = 0; i < this.ListSelectedVehicle.Count - 1; i++)
            {
                for (int j = i + 1; j < this.ListSelectedVehicle.Count; j++)
                {
                    VehicleInfo vi = new VehicleInfo();
                    vi = this.ListSelectedVehicle[i];
                    VehicleInfo vi2 = new VehicleInfo();
                    vi2 = ListSelectedVehicle[j];
                    if (vi.DriverSelectedIndex == vi2.DriverSelectedIndex)
                    {
                        if (vi.DriverSelectedIndex != 0)
                        {
                            MessageBox.Show("不能选择相同的驾驶员", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                }
            }
            double addcount = 0.0;
            foreach (var item in this.ListSelectedVehicle)
            {
                if (string.IsNullOrEmpty(item.LoadCapacity))
                {
                    MessageBox.Show("请填写第" + itemIndex + "辆车的运输方量");
                    return false;
                }
                double loadcapacity;
                if (!(double.TryParse(item.LoadCapacity, out loadcapacity)))
                {
                    MessageBox.Show("第" + itemIndex + "辆车的运输方量填写格式错误");
                    return false;
                }
                if (Convert.ToDouble(item.LoadCapacity) > Convert.ToDouble(item.LoadAmount))
                {
                    if (MessageBox.Show("第" + itemIndex + "辆车运输方量超过该车核定承载方量，是否仍然派车？", "承载方量确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return false;
                    }
                }
                addcount += loadcapacity;
                itemIndex++;
            }
            if (this.SelectedItem.TransedCap + addcount > Convert.ToDouble(this.SelectedItem.TaskNumberInfo.TransCap))
            {
                MessageBox.Show("新增方量大于剩余的任务方量！");
                if (MessageBox.Show("新增方量大于剩余的任务方量！是否仍然派车？", "承载方量确认", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
        }

        //循环派车
        private bool isloopdispatch;
        public bool IsLoopDispatch
        {
            get { return isloopdispatch; }
            set
            {
                isloopdispatch = value;
                this.RaisePropertyChanged("IsLoopDispatch");
            }
        }


        //工作时间
        private List<HolidayType> listHolidayType;
        public List<HolidayType> ListHolidayType
        {
            get { return listHolidayType; }
            set
            {
                listHolidayType = value;
                this.RaisePropertyChanged("ListHolidayType");
            }
        }

        private int listHolidayTypeSelectedIndex;
        public int SelectedHolidayIndex
        {
            get { return listHolidayTypeSelectedIndex; }
            set
            {
                listHolidayTypeSelectedIndex = value;
                this.RaisePropertyChanged("SelectedHolidayIndex");
            }
        }

        private void InitHoliday()
        {
            this.ListHolidayType = new List<HolidayType>();
            HolidayType ht = new HolidayType();
            ht.DayName = "正常";
            HolidayType ht1 = new HolidayType();
            ht1.DayName = "加班";
            HolidayType ht2 = new HolidayType();
            ht2.DayName = "周末";
            ListHolidayType.Add(ht);
            ListHolidayType.Add(ht1);
            ListHolidayType.Add(ht2);
        }

        private List<CVDetailInfo> listTest;
        public List<CVDetailInfo> ListTest
        {
            get { return listTest; }
            set
            {
                listTest = value;
                this.RaisePropertyChanged("ListTest");
            }
        }

        #region 更新操作
        //塔楼列表
        public List<TowerInfo> towerlist = new List<TowerInfo>();
        //可选驾驶员列表
        public List<DriverInfo> driverAvaliable = new List<DriverInfo>();
        //已选驾驶员列表
        public List<DriverInfo> driverlist = new List<DriverInfo>();

        public void RefreshSelectedVehicle()
        {
            if (this.ListSelectedVehicle.Count > 0)
            {
                ListSelectedVehicle.Clear();
            }
            List<VehicleInfo> tmp = new List<VehicleInfo>();
            foreach (VehicleInfo vehicle in this.ListAvailableVehicle)
            {
                if (vehicle.IsSelected)
                {
                    bool isAlready = false;//判断是否已经选了
                    foreach (VehicleInfo item in this.ListSelectedVehicle)
                    {
                        if (item.VehicleId == vehicle.VehicleId)
                        {
                            isAlready = true;
                            break;
                        }
                    }
                    if (isAlready)
                    {//已经选了
                        continue;
                    }
                    //没选就新增一个
                    vehicle.InitDriverList();
                    vehicle.EndRegName = this.SelectedItem.TaskNumberInfo.EndPoint;
                    vehicle.FPlanId = this.SelectedItem.TaskNumberInfo.FPlanId;
                    vehicle.startRegName = this.SelectedItem.TaskNumberInfo.StartPoint;
                    ListSelectedVehicle.Add(vehicle);
                }
                else
                {
                    foreach (VehicleInfo item in this.ListSelectedVehicle)
                    {
                        if (item.VehicleId == vehicle.VehicleId)
                        {
                            tmp.Add(item);
                            break;
                        }
                    }
                }
            }
            foreach (VehicleInfo item in tmp)
            {
                ListSelectedVehicle.Remove(item);
            }
            List<VehicleInfo> templist = new List<VehicleInfo>();
            foreach (VehicleInfo item in this.ListSelectedVehicle)
            {
                VehicleInfo vi = new VehicleInfo();
                vi = item;
                templist.Add(vi);
            }
            this.ListSelectedVehicle = null;
            this.ListSelectedVehicle = new List<VehicleInfo>();
            this.ListSelectedVehicle = templist;
        }
        #endregion
        /*可派车辆*/
        private List<VehicleInfo> listAvailableVehicle;
        public List<VehicleInfo> ListAvailableVehicle
        {
            get { return listAvailableVehicle; }
            set
            {
                listAvailableVehicle = value;
                this.RaisePropertyChanged("ListAvailableVehicle");
            }
        }

        /*已派车辆*/
        private List<VehicleInfo> listSelectedVehicle = new List<VehicleInfo>();
        public List<VehicleInfo> ListSelectedVehicle
        {
            get { return listSelectedVehicle; }
            set
            {
                if (listSelectedVehicle != value)
                {
                    listSelectedVehicle = value;
                    this.RaisePropertyChanged("ListSelectedVehicle");
                }

            }
        }

        /*当前选择的调度Item*/
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

        /*获取车辆信息*/
        public void GetVehicleInfo()
        {
            VehicleDispatchViewModel instance = VehicleDispatchViewModel.GetInstance();

            // StaticDetailInfo.GetInstance().ListVehicleDetailInfo = null;
            List<CVDetailInfo> vehiclelist = StaticDetailInfo.GetInstance().ListVehicleDetailInfo;
            List<VehicleInfo> listTest = new List<VehicleInfo>();
            string UnitId = null;
            if (instance.SelectedStationIndex == 0)
            {
                string sql = "select unitId from TranTaskList where taskListId='" + this.SelectedItem.TaskNumberInfo.FPlanId + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
                DataTable dt = JsonHelper.JsonToDataTable(jsonStr);
                if (dt != null)
                {
                    UnitId = dt.Rows[0]["unitId"].ToString();
                }

            }
            else
            {
                UnitId = instance.ListStation[instance.SelectedStationIndex].ID;
            }
            foreach (CVDetailInfo item in vehiclelist)
            {
                if (item.ParentUnitId == UnitId)
                {
                    int i = 0;

                }
                if (item.VehicleId.Contains("EJL686"))
                {
                    int iiiiiii = 0;
                }
                if (item.ParentUnitId == UnitId && (item.VehicleState.Equals("0") || item.VehicleState.Equals("空闲无任务") || item.VehicleState.Equals("3") || item.VehicleState.Equals("无任务离场")))
                {
                    VehicleInfo vi = new VehicleInfo();
                    vi.IsSelected = false;
                    vi.VehicleId = item.VehicleId;
                    vi.InnerId = item.FInnerId;
                    if (item.LoadAmount == null)
                    {
                        vi.LoadAmount = "0";
                    }
                    else
                    {
                        vi.LoadAmount = item.LoadAmount;
                    }
                    vi.vehicleNum = item.VehicleNum;
                    vi.SIM = item.SIM;
                    vi.Location = item.VehicleGPSInfo == null ? "" : item.VehicleGPSInfo.CurLocation;
                    listTest.Add(vi);
                }

            }
            this.ListAvailableVehicle = listTest;
        }

    }
    //工作日类型
    class HolidayType
    {
        public string DayName { get; set; }
    }
}
