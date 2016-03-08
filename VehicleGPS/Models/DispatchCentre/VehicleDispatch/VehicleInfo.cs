using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.Models.DispatchCentre.VehicleDispatch
{
    public class VehicleInfo : NotificationObject
    {
        MDispatchVehicleViewModel Instance = MDispatchVehicleViewModel.instance;
        public GPSInfo GpsInfo { get; set; }
        public string VehicleId { get; set; }//车牌号
        public string InnerId { get; set; }//内部编号
        public string LoadAmount { get; set; }//核定承载方量
        public string startRegName { get; set; }//厂区
        public string ParentUnitId { get; set; }//所属单位id
        public string InstallDate { get; set; }//进厂时间
        public string Location { get; set; }//当前位置
        public string TowerName { get; set; }//塔楼
        public string EndRegName { get; set; }//工地
        public string FPlanId { get; set; }//任务单号
        public string vehicleNum { get; set; }//车辆编号
        public string SIM { get; set; }//SIM卡号
        public string id { get; set; }//运输单id
        public string IsCircle { get; set; }//是否循环派车
        /*承载容量*/
        private string loadCapacity;
        public string LoadCapacity
        {
            get { return loadCapacity; }
            set
            {
                if (loadCapacity != value)
                {
                    loadCapacity = value;
                    this.RaisePropertyChanged("LoadCapacity");
                }
            }
        }
        /*车辆是否被选中*/
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                MDispatchVehicleViewModel.instance.RefreshSelectedVehicle();
                this.RaisePropertyChanged("IsSelected");
            }
        }
        //塔楼列表
        private List<TowerInfo> towerlist;
        public List<TowerInfo> TowerList
        {
            get { return towerlist; }
            set
            {
                towerlist = value;
                this.RaisePropertyChanged("TowerList");
            }
        }
        private int towerselectedindex;
        public int TowerSelectedIndex
        {
            get { return towerselectedindex; }
            set
            {
                towerselectedindex = value;
                this.RaisePropertyChanged("TowerSelectedIndex");
            }
        }

        public VehicleInfo()
        {
            this.TowerList = Instance.towerlist;
        }

        //驾驶员列表
        private List<DriverInfo> driverlist;
        public List<DriverInfo> DriverList
        {
            get { return driverlist; }
            set
            {
                driverlist = value;
                this.RaisePropertyChanged("DriverList");
            }
        }
        private DriverInfo driver = new DriverInfo();
        private int driverselectedIndex;
        public int DriverSelectedIndex
        {
            get { return driverselectedIndex; }
            set
            {
                //if (driverselectedIndex != 0)
                //{
                //    DriverInfo tmp = null;
                //    foreach (var item in Instance.driverlist)
                //    {
                //        if (item.workID == this.DriverList[driverselectedIndex].workID)
                //        {
                //            tmp = new DriverInfo();
                //            tmp = item;
                //        }
                //    }
                //    if (tmp != null)
                //    {
                //        Instance.driverlist.Remove(tmp);
                //    }
                //}
                driverselectedIndex = value;

                //if (driverselectedIndex != 0)
                //{
                //    Instance.driverlist.Add(this.driverlist[driverselectedIndex]);
                //}

                //this.copy(this.driver, this.driverlist[DriverSelectedIndex]);
                //Instance.RefreshSelectedVehicle();
                this.RaisePropertyChanged("DriverSelectedIndex");
            }
        }
        /// <summary>
        /// 将driver2复制给driver1
        /// </summary>
        /// <param name="driver1"></param>
        /// <param name="driver2"></param>
        public void copy(DriverInfo driver1,DriverInfo driver2) {
            driver1.DriverName = driver2.DriverName;
            driver1.sequence = driver2.sequence;
            driver1.sex = driver2.sex;
            driver1.workID = driver2.workID;
            driver1.workNum = driver2.workNum;
            driver1.zoneID = driver2.zoneID;
        }
        public void InitDriverList()
        {
            //清空驾驶员列表
            if (this.DriverList == null)
            {
                this.DriverList = new List<DriverInfo>();
            }
            this.DriverList.Clear();

            int i = 0;
            int confirmIndex = 0;
            foreach (DriverInfo item in Instance.driverAvaliable)
            {
                DriverInfo tmp = null;
                if (Instance.driverlist.Count > 0)
                {
                    foreach (DriverInfo driver in Instance.driverlist)
                    {
                        if (item.workID == driver.workID && driver.workID != this.driver.workID)
                        {
                            i++;
                            tmp = driver;
                            break;
                        }
                        else if (item.workID == this.driver.workID)
                        {
                            confirmIndex = i;
                        }
                    }
                }
                if (tmp == null)
                {
                    DriverInfo temp=new DriverInfo();
                    this.copy(temp, item);
                    this.DriverList.Add(temp);
                }
            }
        }
    }
}
