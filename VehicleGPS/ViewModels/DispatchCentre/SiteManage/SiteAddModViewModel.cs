using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Services.DispatchCentre.SiteManage;
using VehicleGPS.Views.Control.DispatchCentre;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models.DispatchCentre.SiteManage;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows;
using VehicleGPS.Services;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;

namespace VehicleGPS.ViewModels.DispatchCentre.SiteManage
{
    class SiteAddModViewModel : NotificationObject
    {
        public SiteManageMainViewModel parentViewModel = null;//父ViewModel
        private OperateType OpeType;
        private Window window { get; set; }

        public SiteAddModViewModel(Object parentDataContext, OperateType ot,SiteType siteType,object window)
        {
            this.window =(Window) window;
            this.OpeType = ot;
            this.parentViewModel = (SiteManageMainViewModel)parentDataContext;
            this.ConfirmCommand = new DelegateCommand(new Action(ConfirmCommandExecute));
            if (siteType == SiteType.Building)
                this.typeID = "gd";//工地
            else
                this.typeID = "cq";//厂区
            /*对于修改和添加*/
            if (ot == OperateType.MOD)
            {
                this.InitData();
            }
            if (ot == OperateType.ADD)
            {

            }
            this.GetDepart();
        }

        #region 添加修改操作
        /*确定*/
        public DelegateCommand ConfirmCommand { get; set; }
        private void ConfirmCommandExecute()
        {
            if (!string.IsNullOrEmpty(this.contactPhone) && !TelHelper.isTelRight(this.contactPhone))
            {
                MessageBox.Show("电话号码不合法", "提示", MessageBoxButton.OK);
                return;
            }
            if (string.IsNullOrEmpty(this.regName))
            {
                MessageBox.Show("请填写"+parentViewModel.Tip+"名称", "提示", MessageBoxButton.OK);
                return;
            }
            if (this.OpeType == OperateType.MOD)
            {
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
        private void  CloseWindow()
        {
              this.window.Close();
        }
        /*添加站点*/
        private void AddRegion()
        {
            this.regId = IDHelper.GetRegionOrBuildingID(typeID);
            string sql = "insert into InfoRegion(unitId,regName,regId,regType,regAddress,regLongitude,regLatitude,RegRadius,insertTime,contactName,contactPhone)" +
                         "values('" + this.ListDepart[this.SelectedDepartIndex].ID +
                              
                              "','" + this.regName +
                              "','" + this.regId +
                               "','" + this.typeID + 
                             
                              "','" + this.regAddress +
                              "','" + this.regLongitude +
                              "','" + this.regLatitude +
                              "','" + this.RegRadius +
                              "','" + DateTime.Now.ToString() +
                              
                              "','" + this.contactName +
                              "','" + this.contactPhone +
                              "')";
            string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName,sql);
            if (string.Compare(status, "error") != 0)
            {
                MessageBox.Show("添加成功!", "提示", MessageBoxButton.OK);
                DispatchSiteInfo newInfo = new DispatchSiteInfo();
                newInfo.Sequence = this.parentViewModel.ListDispatchSiteInfo.Count + 1;
                newInfo.regId = this.regId;
                newInfo.regName = this.regName;
                newInfo.regAddress = this.regAddress;
                newInfo.regLongitude = this.regLongitude;
                newInfo.regLatitude = this.regLatitude;
                newInfo.RegRadius = this.RegRadius;
               
                newInfo.contactName = this.contactName;
                newInfo.contactPhone = this.contactPhone;
                newInfo.unitId = this.ListDepart[this.SelectedDepartIndex].ID;
                newInfo.unitName = this.ListDepart[this.SelectedDepartIndex].Name;

                if (parentViewModel.SelectedStation.ID.Equals(newInfo.unitId))
                {
                    this.parentViewModel.ListDispatchSiteInfo.Add(newInfo);
                    this.parentViewModel.LocalRefreshData();
                }
                else
                {
                    MessageBox.Show("新添加项在" + parentViewModel.SelectedStation.Name+"列表下！", "提示", MessageBoxButton.OK);
                }
                CloseWindow();
            }
            else
            {
                MessageBox.Show("添加失败!", "提示", MessageBoxButton.OK);
            }
        }
        /*修改站点*/
        private void ModRegion()
        {
            string sql = "update InfoRegion " +
                "set regName='" + this.regName +
                "',unitId='" + this.ListDepart[this.SelectedDepartIndex].ID +
               
                "',regAddress='" + this.regAddress +
                "',regLongitude='" + this.regLongitude +
                "',regLatitude='" + this.regLatitude +
                "',RegRadius='" + this.RegRadius +
                //"',insertTime='" + DateTime.Now.ToString() +

                "',contactName='" + this.ContactName +
                "',contactPhone='" + this.contactPhone +
                "' where regId='" + this.regId + "'";
            string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(status, "error") != 0)
            {
                MessageBox.Show("修改成功!", "提示", MessageBoxButton.OK);
                DispatchSiteInfo selectedRegion = this.parentViewModel.SelectedSite;
                string oldUnitId = selectedRegion.unitId;
                selectedRegion.regId= this.regId;
                selectedRegion.regName= this.regName;
                selectedRegion.regAddress = this.regAddress;
                selectedRegion.regLongitude = this.regLongitude;
                selectedRegion.regLatitude= this.regLatitude;
                selectedRegion.RegRadius= this.RegRadius;
               
                selectedRegion.contactName = this.contactName;
                selectedRegion.contactPhone = this.contactPhone;
                selectedRegion.unitId = this.ListDepart[this.SelectedDepartIndex].ID;
                selectedRegion.unitName = this.ListDepart[this.SelectedDepartIndex].Name;
                //this.parentViewModel.LocalRefreshData();
                if(oldUnitId.Equals(selectedRegion.unitId))
                    this.parentViewModel.LocalRemoveAndRefreshData(false);
                else
                    this.parentViewModel.LocalRemoveAndRefreshData(true);
                CloseWindow();
            }
            else
            {
                MessageBox.Show("修改失败!", "提示", MessageBoxButton.OK);
            }
        }
        #endregion

        #region 数据源
        //选择的单位索引
        /// <summary>
        /// 选中单位索引
        /// </summary>
        private int selectedDepartIndex = 0;
        public int SelectedDepartIndex
        {
            get { return selectedDepartIndex; }
            set
            {
                if (selectedDepartIndex != value)
                {
                    selectedDepartIndex = value;
                    this.RaisePropertyChanged("SelectedDepartIndex");
                }
            }
        }
        /// <summary>
        /// 类型号 xq00001工地 xq00006区域
        /// </summary>
        private string typeID{get;set;}
        //所有单位
        /// <summary>
        /// 所有单位列表
        /// </summary>
        private List<CVBasicInfo> listDepart = new List<CVBasicInfo>();
        public List<CVBasicInfo> ListDepart
        {
            get { return listDepart; }
            set
            {
                if (listDepart != value)
                {
                    listDepart = value;
                    this.RaisePropertyChanged("ListDepart");
                }
            }
        }
        //获取所有单位
        /// <summary>
        /// 获取所有单位
        /// </summary>
        private void GetDepart()
        {
            List<CVBasicInfo> tmp = new List<CVBasicInfo>();
            if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo baseInfo = StaticBasicInfo.GetInstance();
                foreach (CVBasicInfo info in baseInfo.ListClientBasicInfo)
                {

                    if (baseInfo.ListVehicleOfClientBaseInfo.Contains(info.ID))//选择含有车辆的单位
                    {
                        tmp.Add(info);
                    }
                } 
            }
            this.ListDepart = tmp;
            /*获取所在单位*/
            if (this.OpeType == OperateType.MOD)
            {
                for (int i = 0; i < this.ListDepart.Count; i++)
                {
                    if (this.ListDepart[i].ID == this.parentViewModel.SelectedSite.unitId)
                    {
                        this.SelectedDepartIndex = i;
                        break;
                    }
                }
            }
            if (this.OpeType == OperateType.ADD)
            {
                for (int i = 0; i < this.ListDepart.Count; i++)
                {
                    if (this.ListDepart[i].ID == this.parentViewModel.SelectedStation.ID)
                    {
                        this.SelectedDepartIndex = i;
                        break;
                    }
                }
            }
        }
        /*区域编号*/
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
        /*区域名称*/
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
        /*联系人*/
        private string contactName;
        public string ContactName
        {
            get { return contactName; }
            set
            {
                if (contactName != value)
                {
                    contactName = value;
                    this.RaisePropertyChanged("ContactName");
                }
            }
        }
        /*联系电话*/
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
        /*区域半径*/
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
        /*圆心点经度*/
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
        /*圆心点纬度*/
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
        /*所在城市名称*/
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
        
        /*初始化数据*/
        /// <summary>
        /// 修改时 初始化化数据
        /// </summary>
        private void InitData()
        {
            DispatchSiteInfo selectedInfo = this.parentViewModel.SelectedSite;
            this.regId = selectedInfo.regId;
            this.regName = selectedInfo.regName;
            this.contactName = selectedInfo.contactName;
            this.contactPhone = selectedInfo.contactPhone;
          
            this.RegRadius = selectedInfo.RegRadius;
            this.regLongitude = selectedInfo.regLongitude;
            this.regLatitude = selectedInfo.regLatitude;
            this.regAddress = selectedInfo.regAddress;
        }
        #endregion
    }
}
