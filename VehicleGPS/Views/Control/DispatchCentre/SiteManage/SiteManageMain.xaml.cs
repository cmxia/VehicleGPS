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
using VehicleGPS.ViewModels.DispatchCentre.SiteManage;
using VehicleGPS.Models;
using VehicleGPS.Models.DispatchCentre.SiteManage;
using System.Windows.Controls.Primitives;
using VehicleGPS.Views.Control.DispatchCentre;


namespace VehicleGPS.Views.Control.DispatchCentre.SiteManage
{
    /// <summary>
    /// SiteManageMain.xaml 的交互逻辑
    /// 工地或区域显示界面
    /// </summary>
    public partial class SiteManageMain : Window
    {
        private SiteType siteType { get; set; }
        public SiteManageMain()
        {
            InitializeComponent();
        }
        public SiteManageMain(CVBasicInfo selectedStation, SiteType siteType)
        {
            InitializeComponent();
            this.siteType = siteType;
            this.DataContext = new SiteManageMainViewModel(selectedStation, this.webMap, siteType, this);
        }

        /*增加*/
        private void Add_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SiteAddMod addWin = new SiteAddMod(this.DataContext, OperateType.ADD, this.siteType);
            addWin.Owner = this;
            addWin.ShowDialog();
        }
        /*修改*/
        private void Mod_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DispatchSiteInfo selectedRegion = ((SiteManageMainViewModel)this.DataContext).SelectedSite;
            if (selectedRegion == null)
            {
                if (this.siteType == SiteType.Building)
                {
                    MessageBox.Show("请选择工地", "提示", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("请选择区域", "提示", MessageBoxButton.OK);
                }
                return;
            }
            //SiteAddMod modWin = new SiteAddMod(this.DataContext, OperateType.MOD,this.siteType);
            //modWin.Owner = this;
            //modWin.ShowDialog();
        }
    }
}
