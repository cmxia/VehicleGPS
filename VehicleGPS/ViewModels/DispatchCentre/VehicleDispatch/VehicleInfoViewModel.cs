using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Services.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    class VehicleInfoViewModel:NotificationObject
    {
        private VehicleDispatchViewModel ParentViewModel { get; set; }
        private VehicleDispatchDataOperate DataOperate {get;set;}
        public VehicleInfoViewModel(object parentVM)
        {
            this.DataOperate = new VehicleDispatchDataOperate();
            this.ParentViewModel = (VehicleDispatchViewModel)parentVM;
            this.SelectedItem = this.ParentViewModel.SelectedItem;

            this.InitReachVehicle(this.SelectedItem.TaskNumberInfo.EndID);
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
        /*所有到站的车辆信息*/
        private List<DispatchVehicleViewModel> listReachVehicle;
        public List<DispatchVehicleViewModel> ListReachVehicle
        {
            get { return listReachVehicle; }
            set
            {
                if (listReachVehicle != value)
                {
                    listReachVehicle = value;
                    this.RaisePropertyChanged("ListReachVehicle");
                }
            }
        }

        private void InitReachVehicle(string siteID)
        {
            this.ListReachVehicle = this.SelectedItem.ListInSiteVehicle;
        }
    }
}
