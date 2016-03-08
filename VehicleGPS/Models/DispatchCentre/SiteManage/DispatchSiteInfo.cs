using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models.DispatchCentre.SiteManage
{
    class DispatchSiteInfo : NotificationObject
    {
        public int Sequence { get; set; }//序号(数据库中无)
        private string _regId;//区域编号
        public string regId
        {
            get { return _regId; }
            set
            {
                if (_regId != value)
                {
                    this._regId = value;
                    this.RaisePropertyChanged("regId");
                }
            }
        }
        private string _regName;//区域名称
        public string regName
        {
            get { return _regName; }
            set
            {
                if (_regName != value)
                {
                    this._regName = value;
                    this.RaisePropertyChanged("regName");
                }
            }
        }
        public string _regType; //类别编号
        public string regType
        {
            get { return _regType; }
            set
            {
                if (_regType != value)
                {
                    this._regType = value;
                    this.RaisePropertyChanged("regType");
                }
            }
        }
        private string _regAddress; //所在城市名称
        public string regAddress
        {
            get { return _regAddress; }
            set
            {
                if (_regAddress != value)
                {
                    this._regAddress = value;
                    this.RaisePropertyChanged("regAddress");
                }
            }
        }
        private string _regLongitude;//圆心点经度
        public string regLongitude
        {
            get { return _regLongitude; }
            set
            {
                if (_regLongitude != value)
                {
                    this._regLongitude = value;
                    this.RaisePropertyChanged("regLongitude");
                }
            }
        }
        private string _regLatitude;//圆心点纬度
        public string regLatitude
        {
            get { return _regLatitude; }
            set
            {
                if (_regLatitude != value)
                {
                    this._regLatitude = value;
                    this.RaisePropertyChanged("regLatitude");
                }
            }
        }
        private string _RegRadius;//区域半径
        public string RegRadius
        {
            get { return _RegRadius; }
            set
            {
                if (_RegRadius != value)
                {
                    this._RegRadius = value;
                    this.RaisePropertyChanged("RegRadius");
                }
            }
        }
        private string _unitName;//单位(数据库中无)
        public string unitName
        {
            get { return _unitName; }
            set
            {
                if (_unitName != value)
                {
                    this._unitName = value;
                    this.RaisePropertyChanged("unitName");
                }
            }
        }
        private string _unitId;//单位编号
        public string unitId
        {
            get { return _unitId; }
            set
            {
                if (_unitId != value)
                {
                    this._unitId = value;
                    this.RaisePropertyChanged("unitId");
                }
            }
        }
        private string _contactName;//联系人
        public string contactName
        {
            get { return _contactName; }
            set
            {
                if (_contactName != value)
                {
                    this._contactName = value;
                    this.RaisePropertyChanged("contactName");
                }
            }
        }
        private string _contactPhone;//联系电话
        public string contactPhone
        {
            get { return _contactPhone; }
            set
            {
                if (_contactPhone != value)
                {
                    this._contactPhone = value;
                    this.RaisePropertyChanged("contactPhone");
                }
            }
        }

        
    }
}
