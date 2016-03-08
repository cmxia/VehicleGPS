using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class BackToStationViewModel:NotificationObject
    {
        private BackToStationDataOperate DataOperate { get; set; }
        public BackToStationViewModel()
        {
            this.ConfirmCommand = new DelegateCommand<Window>(new Action<Window>(this.ConfirmCommandExecute));
            this.CloseCommand = new DelegateCommand<Window>(new Action<Window>(this.CloseCommandExecute));
            this.DataOperate = new BackToStationDataOperate();
        }
        //public string sequence { get; set; }//序号
        //public string InnerId { get; set; }//内部编号
        //public string DriverId { get; set; }//驾驶员编号
        //public string DriverName { get; set; }//驾驶员姓名
        private string fPlanId; //任务单编号
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
        private string vehicleID; //车牌号
        public string VehicleID
        {
            get { return vehicleID; }
            set
            {
                if (vehicleID != value)
                {
                    vehicleID = value;
                    this.RaisePropertyChanged("VehicleID");
                }
            }
        }
        private string sim; //SIM
        public string SIM
        {
            get { return sim; }
            set
            {
                if (sim != value)
                {
                    sim = value;
                    this.RaisePropertyChanged("SIM");
                }
            }
        }
        private string vehicleTypeName;//车辆类型
        public string VehicleTypeName
        {
            get { return vehicleTypeName; }
            set
            {
                if (vehicleTypeName != value)
                {
                    vehicleTypeName = value;
                    this.RaisePropertyChanged("VehicleTypeName");
                }
            }
        }
        private string startPoint;//发车区域
        public string StartPoint
        {
            get { return startPoint; }
            set
            {
                if (startPoint != value)
                {
                    startPoint = value;
                    this.RaisePropertyChanged("StartPoint");
                }
            }
        }
        private string endPoint;//运往工地
        public string EndPoint
        {
            get { return endPoint; }
            set
            {
                if (endPoint != value)
                {
                    endPoint = value;
                    this.RaisePropertyChanged("EndPoint");
                }
            }
        }
        private string transCapPer;//运输方量
        public string TransCapPer
        {
            get { return transCapPer; }
            set
            {
                if (transCapPer != value)
                {
                    transCapPer = value;
                    this.RaisePropertyChanged("TransCapPer");
                }
            }
        }
        private string loadAmount;//核载方量
        public string LoadAmount
        {
            get { return loadAmount; }
            set
            {
                if (loadAmount != value)
                {
                    loadAmount = value;
                    this.RaisePropertyChanged("LoadAmount");
                }
            }
        }
        private string concreteName;//混凝土标号（包含水）
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
        private string carsStatus;//车辆出行状态
        public string CarsStatus
        {
            get { return carsStatus; }
            set
            {
                if (carsStatus != value)
                {
                    carsStatus = value;
                    this.RaisePropertyChanged("CarsStatus");
                }
            }
        }
        private string offTypeName;//卸料方式
        public string OffTypeName
        {
            get { return offTypeName; }
            set
            {
                if (offTypeName != value)
                {
                    offTypeName = value;
                    this.RaisePropertyChanged("OffTypeName");
                }
            }
        }
        private string driverId;//驾驶员编号
        public string DriverId
        {
            get { return driverId; }
            set
            {
                if (driverId != value)
                {
                    driverId = value;
                    this.RaisePropertyChanged("DriverId");
                }
            }
        }
        private string driverName;//驾驶员姓名
        public string DriverName
        {
            get { return driverName; }
            set
            {
                if (driverName != value)
                {
                    driverName = value;
                    this.RaisePropertyChanged("DriverName");
                }
            }
        }

        private bool isAdd;//是否增加方量
        public bool IsAdd
        {
            get { return isAdd; }
            set
            {
                if (isAdd != value)
                {
                    isAdd = value;
                    this.RaisePropertyChanged("IsAdd");
                }
            }
        }
        private string imageUrl;
        public string ImageUrl
        {
            get { return imageUrl; }
            set
            {
                if (imageUrl != value)
                {
                    imageUrl = value;
                    this.RaisePropertyChanged("ImageUrl");
                }
            }
        }

        private Visibility isVisibility = Visibility.Hidden;
        public Visibility IsVisibility
        {
            get { return isVisibility; }
            set
            {
                if (isVisibility != value)
                {
                    isVisibility = value;
                    this.RaisePropertyChanged("IsVisibility");
                }
            }
        }

        #region 关闭和确定
        public DelegateCommand<Window> ConfirmCommand { get; set; }
        public DelegateCommand<Window> CloseCommand { get; set; }
        private void ConfirmCommandExecute(Window win)
        {
            if (this.IsVisibility != Visibility.Visible)
            {
                MessageBox.Show("您还未选择强制回站车辆", "强制回站", MessageBoxButton.OK);
                return;
            }
            if (this.DataOperate.ConfirmBackToStation(this.driverId, this.fPlanId, this.vehicleID, this.isAdd))
            {
                MessageBox.Show("强制回车成功", "强制回车", MessageBoxButton.OK);
                win.Close();
            }
            else
            {
                MessageBox.Show("强制回车失败，请重试", "强制回车", MessageBoxButton.OK);
            }
        }
        private void CloseCommandExecute(Window win)
        {
            win.Close();
        }
        #endregion
    }
}
