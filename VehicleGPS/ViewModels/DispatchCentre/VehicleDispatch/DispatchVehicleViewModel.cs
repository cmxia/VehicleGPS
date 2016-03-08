using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using VehicleGPS.Models;
using System.Windows;
using Microsoft.Practices.Prism.Commands;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    public class DispatchVehicleViewModel : NotificationObject
    {
        public DispatchVehicleViewModel()
        {

        }
        public DispatchVehicleInfo DispatchInfo { get; set; }
        public CVDetailInfo DetailInfo { get; set; }
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
        private double marginDistance;//显示距离
        public double MarginDistance
        {
            get { return marginDistance; }
            set
            {
                if (marginDistance != value)
                {
                    marginDistance = value;
                }
            }
        }
        private double realDistance;//实际距离
        public double RealDistance
        {
            get { return realDistance; }
            set
            {
                if (realDistance != value)
                {
                    realDistance = value;
                    this.RaisePropertyChanged("RealDistance");
                }
            }
        }

        public double RegionDistance { get; set; }//距离区域的距离
        public double SiteDistance { get; set; }//距离工地的距离


        private string distanceType;//距离类型、离区域距离/离工地距离
        public string DistanceType
        {
            get { return distanceType; }
            set
            {
                if (distanceType != value)
                {
                    distanceType = value;
                    this.RaisePropertyChanged("DistanceType");
                }
            }
        }
    }
}
