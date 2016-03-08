using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using VehicleGPS.Models.DispatchCentre.SiteManage;
using VehicleGPS.Services.DispatchCentre.SiteManage;
using VehicleGPS.Views.Control.DispatchCentre;
using VehicleGPS.Views.Control.DispatchCentre.SiteManage;
using VehicleGPS.Services;
using System.Data;


namespace VehicleGPS.ViewModels.DispatchCentre.SiteManage
{
    class SiteManageMainViewModel : NotificationObject
    {
        public CVBasicInfo SelectedStation { get; set; }//选择的站点

        /// <summary>
        /// site类别
        /// </summary>
        private SiteType siteType { get; set; }//操作类型（工地、区域）
        private SiteManageMain parentWin = null;
        #region 构造函数
        public SiteManageMainViewModel(CVBasicInfo selectedStation, WebBrowser webMap, SiteType type, Window siteManageWin)
        {
            parentWin = (SiteManageMain)siteManageWin;
            this.SelectedStation = selectedStation;
            this.SelectedItemChangedCommand = new DelegateCommand<object>(new Action<object>(this.SelectedItemChangedCommandExecute));
            this.DelSiteCommand = new DelegateCommand(new Action(this.DelSiteCommandExecute));
            this.ModSiteCommand = new DelegateCommand(new Action(this.ModSiteCommandExecute));
            this.CancelCommand = new DelegateCommand(new Action(this.CancelCommandExecute));
            this.RemarkCommand = new DelegateCommand(new Action(this.RemarkCommandExecute));
            this.ModifyCommand = new DelegateCommand(new Action(this.ModifyCommandExecute));
            this.GetGeoCommand = new DelegateCommand(new Action(this.GetGeoCommandExecute));
            this.siteType = type;
            this.GetSiteInfo();
            this.WebMap = webMap;
            this.siteMap = new SiteMapService(webMap);
            InitRegionInfo();
            Init();
        }
        /// <summary>
        /// 初始化页面的comboBox
        /// </summary>
        private void InitRegionInfo()
        {
            //初始化区域类型列表
            List<RegionType> regtplist = new List<RegionType>();
            RegionType rt = new RegionType();
            rt.regionid = "cq";
            rt.regionname = "出发地";
            regtplist.Add(rt);
            RegionType rt2 = new RegionType();
            rt2.regionid = "gd";
            rt2.regionname = "目的地";
            regtplist.Add(rt2);
            this.RegionTypeList = new List<RegionType>();
            this.RegionTypeList = regtplist;

            this.DepartmentList = new List<Department>();
            List<Department> departlist = new List<Department>();
            // 初始化所属单位
            string sql = "select UNITID,UNITNAME from InfoUnit where unitId in (select NodeId unitId from RightsDetails where UserId='" + StaticLoginInfo.GetInstance().UserName + "')";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
            {
                MessageBox.Show("所属单位初始化失败！请重试！", "提示", MessageBoxButton.OK);
                return;
            }
            string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
            departlist = (List<Department>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<Department>));
            this.DepartmentList = departlist;
        }

        private void Init()
        {
            this.SiteNameList = this.DepartmentList;
            for (int i = 0; i < SiteNameList.Count; i++)
            {
                if (string.Compare(SelectedStation.Name, SiteNameList[i].UNITNAME) == 0)
                {
                    this.SiteNameSelectedIndex = i;
                    break;
                }
            }
            if (siteType == SiteType.Building)
            {
                this.title = "工地管理";
                this.TypeName = "工地名称：";
                this.Tip = "工地";
            }
            else
            {
                this.title = "区域管理";
                this.TypeName = "区域名称：";
                this.Tip = "区域";
            }
        }
        #endregion

        /// <summary>
        /// 区域工地列表
        /// </summary>
        private List<DispatchSiteInfo> listDispatchSiteInfo = new List<DispatchSiteInfo>();
        /// <summary>
        /// 区域工地列表
        /// </summary>
        public List<DispatchSiteInfo> ListDispatchSiteInfo
        {
            get { return listDispatchSiteInfo; }
            set
            {
                listDispatchSiteInfo = value;
                this.RaisePropertyChanged("ListDispatchSiteInfo");
            }
        }
        /// <summary>
        /// 选中的区域或工地
        /// </summary>
        private DispatchSiteInfo selectedSite;//选择的区域
        /// <summary>
        /// 选择的区域或工地
        /// </summary>
        public DispatchSiteInfo SelectedSite
        {
            get { return selectedSite; }
            set
            {
                if (selectedSite != value)
                {
                    selectedSite = value;
                    if (selectedSite != null)
                    {
                        this.parentWin.regDetailInfo.IsEnabled = false;
                        InitBaiduMap();
                        InitData();
                        addRegion(selectedSite.regName, selectedSite.regLongitude, selectedSite.regLatitude, selectedSite.RegRadius);
                    }
                    else
                    {
                        InitBaiduMap();
                    }
                    this.RaisePropertyChanged("SelectedSite");
                }

            }
        }


        #region 绑定区分条件 public
        /// <summary>
        /// 选择的站点的名称
        /// </summary>
        public string siteName { get; set; }//选择的站点
        /// <summary>
        /// 操作的类别（工地名称：、区域名称：）
        /// </summary>
        public string TypeName { get; set; }//查询时提示名称
        /// <summary>
        /// 操作的类别（工地管理、区域管理）
        /// </summary>
        public string title { get; set; }//窗口的标题
        public string Tip { get; set; }//删除时的提示信息
        #endregion

        #region 查询条件
        /// <summary>
        /// 查询内容
        /// </summary>
        private string querytext;
        public string QueryText
        {
            get { return querytext; }
            set
            {
                querytext = value;
                this.RaisePropertyChanged("QueryText");
            }
        }
        #endregion

        #region 获取选择的工地或区域
        /// <summary>
        /// 获取选择的工地或区域 command命令
        /// </summary>
        public DelegateCommand<object> SelectedItemChangedCommand { get; set; }
        private void SelectedItemChangedCommandExecute(object selectedObject)
        {
            if (selectedObject != null)
            {
                this.SelectedSite = (DispatchSiteInfo)selectedObject;
            }
        }
        #endregion

        #region  获取工地gd 或区域cq的数据数据
        /// <summary>
        /// 获取工地gd 或区域 cqxq00006的数据数据列表
        /// </summary>
        private void GetSiteInfo()
        {
            if (this.ListDispatchSiteInfo != null && ListDispatchSiteInfo.Count != 0)
            {
                ListDispatchSiteInfo.Clear();
            }
            string sql = string.Empty;
            if (siteType == SiteType.Building)
            {
                sql = "select * from InfoRegion where regType='gd' and unitId='" + this.SelectedStation.ID + "'";
            }
            else
            {
                sql = "select * from InfoRegion where (regType='cq' or regType='gd') and unitId='" + this.SelectedStation.ID + "'";
            }
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr == "error")
                return;
            string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
            List<DispatchSiteInfo> tmp = (List<DispatchSiteInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<DispatchSiteInfo>));
            for (int i = 0; i < tmp.Count; i++)
            {
                tmp[i].Sequence = i + 1;
                tmp[i].unitName = this.SelectedStation.Name;
            }
            this.ListDispatchSiteInfo = tmp;
        }
        #endregion

        #region  本地刷新数据
        /// <summary>
        /// 本地刷新数据
        /// </summary>
        public void LocalRefreshData()
        {
            List<DispatchSiteInfo> tmp = this.ListDispatchSiteInfo;
            this.ListDispatchSiteInfo = null;
            this.ListDispatchSiteInfo = tmp;
            /////////////this.selectedSite = null;//2014-6-26
            //this.RaisePropertyChanged("ListDispatchSiteInfo");
        }
        public void LocalRemoveAndRefreshData(bool isRemove)
        {
            if (isRemove)
            {
                this.listDispatchSiteInfo.Remove(this.SelectedSite);
                this.selectedSite = null;
            }
            List<DispatchSiteInfo> tmp = this.ListDispatchSiteInfo;
            this.ListDispatchSiteInfo = null;
            this.ListDispatchSiteInfo = tmp;

        }
        #endregion

        #region 绑定字段
        //左侧站点选择
        private List<Department> sitenamelist;

        public List<Department> SiteNameList
        {
            get { return sitenamelist; }
            set
            {
                sitenamelist = value;
                this.RaisePropertyChanged("SiteNameList");
            }
        }
        private int sitenameselectedindex;

        public int SiteNameSelectedIndex
        {
            get { return sitenameselectedindex; }
            set
            {
                sitenameselectedindex = value;
                CVBasicInfo cvb = new CVBasicInfo();
                cvb.Name = SiteNameList[SiteNameSelectedIndex].UNITNAME;
                cvb.ID = SiteNameList[SiteNameSelectedIndex].UNITID;
                this.SelectedStation = cvb;
                GetSiteInfo();
                this.SelectedSite = null;
                this.RaisePropertyChanged("SiteNameSelectedIndex");
            }
        }


        /// <summary>
        /// 所属单位列表
        /// </summary>
        private List<Department> departmentlist;

        public List<Department> DepartmentList
        {
            get { return departmentlist; }
            set
            {
                departmentlist = value;
                this.RaisePropertyChanged("DepartmentList");
            }
        }

        private int departselectedindex;

        public int DepartSelecteIndex
        {
            get { return departselectedindex; }
            set
            {
                departselectedindex = value;
                this.RaisePropertyChanged("DepartSelecteIndex");
            }
        }


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
        private string typeID { get; set; }
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
            for (int i = 0; i < this.ListDepart.Count; i++)
            {
                if (this.ListDepart[i].ID == this.SelectedSite.unitId)
                {
                    this.SelectedDepartIndex = i;
                    break;
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
        /*区域类型*/
        private List<RegionType> regiontypelist;
        public List<RegionType> RegionTypeList
        {
            get { return regiontypelist; }
            set
            {
                regiontypelist = value;
                this.RaisePropertyChanged("RegionTypeList");
            }
        }
        private int regiontypeselectedindex;
        public int RegTypeSelectedIndex
        {
            get { return regiontypeselectedindex; }
            set
            {
                regiontypeselectedindex = value;
                this.RaisePropertyChanged("RegTypeSelectedIndex");
            }
        }

        /*初始化数据*/
        /// <summary>
        /// 修改时 初始化化数据
        /// </summary>
        private void InitData()
        {
            DispatchSiteInfo selectedInfo = this.SelectedSite;
            this.regId = selectedInfo.regId;
            this.regName = selectedInfo.regName;
            for (int i = 0; i < this.DepartmentList.Count; i++)
            {
                if (string.Compare(DepartmentList[i].UNITID, SelectedSite.unitId) == 0)
                {
                    this.DepartSelecteIndex = i;
                    break;
                }
            }
            if (string.Compare(SelectedSite.regType, "cq") == 0)
            {
                this.RegTypeSelectedIndex = 0;
            }
            else
            {
                this.RegTypeSelectedIndex = 1;
            }
            this.ContactName = selectedInfo.contactName;
            this.contactPhone = selectedInfo.contactPhone;

            this.RegRadius = selectedInfo.RegRadius;
            this.regLongitude = selectedInfo.regLongitude;
            this.regLatitude = selectedInfo.regLatitude;
            this.regAddress = selectedInfo.regAddress;
        }
        #endregion

        #region 按钮事件绑定

        #region 取消重新标定区域
        public DelegateCommand CancelCommand { get; set; }
        public void CancelCommandExecute()
        {
            InitBaiduMap();
            InitData();
            this.siteMap.RemoveAllMarkers();
            addRegion(selectedSite.regName, selectedSite.regLongitude, selectedSite.regLatitude, selectedSite.RegRadius);
            parentWin.regDetailInfo.IsEnabled = false;
        }
        #endregion

        #region 重新标定区域
        public DelegateCommand RemarkCommand { get; set; }
        public void RemarkCommandExecute()
        {
            parentWin.getgeobtn.IsEnabled = true;
            parentWin.modconfirmbtn.IsEnabled = false;
            siteMap.RemoveAllMarkers();
            siteMap.addClickListener();
        }
        #endregion

        #region 获取重新标定的区域信息
        public DelegateCommand GetGeoCommand { get; set; }
        public void GetGeoCommandExecute()
        {
            parentWin.modconfirmbtn.IsEnabled = true;
            string siteinfostr = siteMap.GetRegionGeoInfo();
            string[] regionInfoMat = siteinfostr.Split(';');
            this.regLatitude = regionInfoMat[1];
            this.regLongitude = regionInfoMat[0];
            this.regAddress = (new BusinessDataServiceWEB()).ParseOneAddress(regionInfoMat[0], regionInfoMat[1]);
            this.RegRadius = regionInfoMat[2];
        }
        #endregion

        #region 修改工地或区域
        public DelegateCommand ModSiteCommand { get; set; }
        public void ModSiteCommandExecute()
        {
            parentWin.regDetailInfo.IsEnabled = true;
        }
        #endregion

        #region 修改工地或区域确认
        public DelegateCommand ModifyCommand { get; set; }
        public void ModifyCommandExecute()
        {
            if (VerifyRegionInfo())
            {
                string sql = "update InfoRegion " +
                    "set regName='" + this.regName +
                    "',unitId='" + this.DepartmentList[DepartSelecteIndex].UNITID +

                    "',regAddress='" + this.regAddress +
                    "',regLongitude='" + this.regLongitude +
                    "',regLatitude='" + this.regLatitude +
                    "',RegRadius='" + this.RegRadius +
                    //"',insertTime='" + DateTime.Now.ToString() +
                    "',regType='" + this.RegionTypeList[RegTypeSelectedIndex].regionid +
                    "',contactName='" + this.ContactName +
                    "',contactPhone='" + this.contactPhone +
                    "' where regId='" + this.regId + "'";
                string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (string.Compare(status, "error") != 0)
                {
                    MessageBox.Show("修改成功!", "提示", MessageBoxButton.OK);
                    //更新车辆任务状态
                    VehicleStateUpdateServices.UpdateVehicleState(this.DepartmentList[DepartSelecteIndex].UNITID, Convert.ToDouble(this.RegRadius), Convert.ToDouble(this.regLongitude), Convert.ToDouble(this.regLongitude), 2);
                    DispatchTreeViewModel.GetInstance().TreeOperate.RefreshTree();//更新树形
                    //发指令
                    string tmp = this.RegionTypeList[RegTypeSelectedIndex].regionid;
                    string insType = tmp.Equals("gd") ? "SITE" : "REG";
                    Dictionary<string, string> instruction = new Dictionary<string, string>();
                    instruction.Add("cmd", "DISPATCH_TYPE");
                    instruction.Add("cmdid", "123_DISPATCH_TYPE");
                    instruction.Add("ID", regId);
                    instruction.Add("type", insType);
                    instruction.Add("OPERATETYPE", "2");
                    string insstring = JsonConvert.SerializeObject(instruction);
                    zmq.zmqPackHelper.zmqInstructionsPack("123", insstring);
                    GetSiteInfo();
                    LocalRefreshData();
                }
                else
                {
                    MessageBox.Show("修改失败!", "提示", MessageBoxButton.OK);
                }
            }
        }
        public bool VerifyRegionInfo()
        {
            if (string.IsNullOrEmpty(regName.Trim()))
            {
                MessageBox.Show("区域名称不能为空!", "提示", MessageBoxButton.OK);
                return false;
            }

            string sql = "select distinct unitId from TranTaskList where taskStatus='3' and (startRegId='" + this.SelectedSite.regId + "' or endRegId='" + this.SelectedSite.regId + "')";
            string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(status, "error") != 0)
            {
                DataTable dt = JsonHelper.JsonToDataTable(status);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string unitid = row["unitId"].ToString();
                        if (unitid.Equals(this.SelectedSite.unitId))
                        {
                            if (!this.SelectedSite.unitId.Equals(this.DepartmentList[this.DepartSelecteIndex].UNITID))
                            {
                                MessageBox.Show("此区域正在使用中，不能修改所属单位！", "提示", MessageBoxButton.OK);
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        #endregion

        #region 删除工地或区域
        /// <summary>
        /// 删除工地或区域
        /// </summary>
        public DelegateCommand DelSiteCommand { get; set; }
        public void DelSiteCommandExecute()
        {
            if (this.SelectedSite == null)
            {
                MessageBox.Show("请选择" + this.Tip, "提示", MessageBoxButton.OK);
                return;
            }
            if (!CanDeleteRegion(this.SelectedSite.regId))
            {
                MessageBox.Show("该区域下还有任务没有执行完，不能删除");
                return;
            }
            if (MessageBox.Show("确实要删除该" + this.Tip + "的信息吗？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string sql = "delete InfoRegion " +
                             "where regId='" + this.SelectedSite.regId + "'";
                string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (string.Compare(status, "error") != 0)
                {
                    MessageBox.Show("删除成功!", "提示", MessageBoxButton.OK);
                    this.listDispatchSiteInfo.Remove(this.SelectedSite);
                    /*重新编号*/
                    for (int i = 0; i < this.listDispatchSiteInfo.Count; i++)
                    {
                        this.listDispatchSiteInfo[i].Sequence = i + 1;
                    }
                    this.LocalRefreshData();

                    //更新车辆任务状态
                    VehicleStateUpdateServices.UpdateVehicleState(this.DepartmentList[DepartSelecteIndex].UNITID, Convert.ToDouble(this.RegRadius), Convert.ToDouble(this.regLongitude), Convert.ToDouble(this.regLongitude), 1);
                    DispatchTreeViewModel.GetInstance().TreeOperate.RefreshTree();//更新树形
                    string tmp = this.SelectedSite.regId;
                    string insType = tmp.Contains("GD") ? "SITE" : "REG";
                    Dictionary<string, string> instruction = new Dictionary<string, string>();
                    instruction.Add("cmd", "DISPATCH_TYPE");
                    instruction.Add("cmdid", "123_DISPATCH_TYPE");
                    instruction.Add("ID", this.DepartmentList[DepartSelecteIndex].UNITID);
                    instruction.Add("type", insType);
                    instruction.Add("OPERATETYPE", "3");
                    string insstring = JsonConvert.SerializeObject(instruction);
                    zmq.zmqPackHelper.zmqInstructionsPack("123", insstring);
                }
                else
                {
                    MessageBox.Show("删除失败!", "提示", MessageBoxButton.OK);
                }
            }
        }
        private bool CanDeleteRegion(string RegionId)
        {
            string sql = "select id from TranTaskList where taskStatus='3' and (startRegId='" + RegionId + "' or endRegId='" + RegionId + "')";
            string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(status, "error") != 0)
            {
                DataTable dt = JsonHelper.JsonToDataTable(status);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #endregion

        #region 百度地图
        public WebBrowser WebMap { get; set; }
        public SiteMapService siteMap { get; set; }
        private void InitBaiduMap()
        {
            siteMap.InitSiteMap();
        }
        private void removeRegion()
        {
            this.WebMap.InvokeScript("removeCircle");
        }
        private void addRegion(string name, string lng, string lat, string radius)
        {
            siteMap.addRegionByOne(name, lng, lat, radius);
        }
        #endregion
    }
    /// <summary>
    /// 区域类型
    /// </summary>
    class RegionType
    {
        public string regionid { get; set; }
        public string regionname { get; set; }
    }
    /// <summary>
    /// 所属单位
    /// </summary>
    class Department
    {
        public string UNITID { get; set; }
        public string UNITNAME { get; set; }
    }
}

