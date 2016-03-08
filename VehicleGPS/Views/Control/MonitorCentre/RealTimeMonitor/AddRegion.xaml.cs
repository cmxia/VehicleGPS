using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using VehicleGPS.Services;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using System.Data;

namespace VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor
{
    /// <summary>
    /// AddRegion.xaml 的交互逻辑
    /// </summary>
    public partial class AddRegion : Window
    {
        private List<DepartmentInfo> departmentList = null;
        public AddRegion()
        {
            InitializeComponent();
            InitRegionGeoInfo();
            InitRegionBaseInfo();
        }
        string lng = null;
        string lat = null;

        /// <summary>
        /// 初始化区域地理信息
        /// </summary>
        private void InitRegionGeoInfo()
        {
            string regionInfo = RealTimeViewModel.GetInstance().MapService.GetCircleInfo();
            string[] regionInfoMat = regionInfo.Split(';');
            this.regLat.Text = regionInfoMat[1];
            this.regLng.Text = regionInfoMat[0];
            this.lat = regionInfoMat[1];
            this.lng = regionInfoMat[0];
            this.regAddr.Text = (new BusinessDataServiceWEB()).ParseOneAddress(regionInfoMat[0], regionInfoMat[1]);
            this.RegRadius.Text = regionInfoMat[2];
        }
        /// <summary>
        /// 初始化区域的基本信息
        /// </summary>
        private void InitRegionBaseInfo()
        {
            // 初始化区域类型
            List<string> RegionTypeList = new List<string>();
            RegionTypeList.Add("出发地");
            RegionTypeList.Add("目的地");
            regType.ItemsSource = RegionTypeList;
            regType.SelectedIndex = 0;
            DepartmentInfo department = null;
            departmentList = new List<DepartmentInfo>();
            foreach (CVBasicInfo unit in StaticBasicInfo.GetInstance().ListClientBasicInfo)
            {
                department = new DepartmentInfo();
                department.UNITID = unit.ID;
                department.UNITNAME = unit.Name;
                departmentList.Add(department);
            }
            listDepart.ItemsSource = departmentList;
            listDepart.SelectedIndex = 0;
        }
        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(regName.Text))
            {
                MessageBox.Show("区域名称为空！", "提示", MessageBoxButton.OK);
                return;
            }
            if (!string.IsNullOrEmpty(this.contactPhone.Text.ToString().Trim()))
            {

                bool istellright = TelHelper.isTelRight(this.contactPhone.Text.ToString().Trim());
                if (!istellright)
                {
                    MessageBox.Show("电话号码不合法！", "提示", MessageBoxButton.OK);
                    return;
                }
            }
            string sql = null;
            string status = null;
            if (regType.SelectedIndex == 0)
            {
                string unitId = departmentList[listDepart.SelectedIndex].UNITID;
                sql = "select * from InfoRegion where unitId = '" + unitId + "' and regType='cq'";
                status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (string.Compare(status, "error") != 0)
                {
                    DataTable dt = new DataTable();
                    dt = JsonHelper.JsonToDataTable(status);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        MessageBox.Show("单位" + departmentList[listDepart.SelectedIndex].UNITNAME + "下已经有了区域，不能重复添加。您可以在调度中心修改该单位下的区域信息");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("校验区域合法性失败！请重试");
                    return;
                }
            }
            string regionType = null;//区域类型
            string regId = null;//区域ID
            string insType = null;
            if (this.regType.SelectedIndex == 0)
            {
                regionType = "cq";
                insType = "REG";
                regId = IDHelper.GetRegionOrBuildingID("cq");
            }
            else
            {
                regionType = "gd";
                insType = "SITE";
                regId = IDHelper.GetRegionOrBuildingID("gd");
            }
            sql = "insert into InfoRegion(unitId,regName,regId,regType,regAddress,regLongitude,regLatitude,RegRadius,insertTime,contactName,contactPhone)" +
                         "values('" + departmentList[listDepart.SelectedIndex].UNITID +
                              "','" + this.regName.Text.ToString() +
                              "','" + regId +
                               "','" + regionType +
                              "','" + this.regAddr.Text.ToString() +
                              "','" + this.regLng.Text.ToString() +
                              "','" + this.regLat.Text.ToString() +
                              "','" + this.RegRadius.Text.ToString() +
                              "','" + DateTime.Now.ToString() +
                              "','" + this.ContactName.Text.ToString().Trim() +
                              "','" + this.contactPhone.Text.ToString().Trim() +
                              "')";
            status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(status, "error") != 0)
            {
                MessageBox.Show("添加成功!", "提示", MessageBoxButton.OK);
                //更新车辆任务状态
                VehicleStateUpdateServices.UpdateVehicleState(departmentList[listDepart.SelectedIndex].UNITID, Convert.ToDouble(this.RegRadius.Text.ToString()), Convert.ToDouble(this.lng), Convert.ToDouble(this.lat), 2);
                Dictionary<string, string> instruction = new Dictionary<string, string>();
                instruction.Add("cmd", "DISPATCH_TYPE");
                instruction.Add("cmdid", "123_DISPATCH_TYPE");
                instruction.Add("ID", regId);
                instruction.Add("type", insType);
                instruction.Add("OPERATETYPE", "1");
                string insstring = JsonConvert.SerializeObject(instruction);
                zmq.zmqPackHelper.zmqInstructionsPack("123", insstring);
                RealTimeViewModel.GetInstance().MapService.addRegionByOne(regName.Text.ToString().Trim(),
                    regLng.Text.ToString(), regLat.Text.ToString(), RegRadius.Text.ToString(), "#f00");
            }
            else
            {
                MessageBox.Show("添加失败!", "提示", MessageBoxButton.OK);
            }
            this.Close();
        }
    }
    public class DepartmentInfo
    {
        public string UNITID { get; set; }
        public string UNITNAME { get; set; }
    }
}
