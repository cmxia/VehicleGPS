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

namespace VehicleGPS.Views.Control.DispatchCentre.SiteManage
{
    /// <summary>
    /// SiteAddMod.xaml 的交互逻辑
    /// </summary>
    public partial class SiteAddMod : Window
    {
        public SiteAddMod(Object dataContext, OperateType m_ot,SiteType siteType)
        {
            InitializeComponent();
            
            /*根据不同的操作类型初始化窗体*/
            if (m_ot == OperateType.ADD)
            {
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/add.png", UriKind.Absolute));
                if (siteType == SiteType.Building)
                {
                    this.Title = "添加工地";
                }
                else
                {
                    this.Title = "添加区域";
                }
            }
            if (m_ot == OperateType.MOD)
            {
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/update.png", UriKind.Absolute));
                if (siteType == SiteType.Building)
                {
                    this.Title = "修改工地";
                }
                else
                {
                    this.Title = "修改区域";
                }
            }
           // this.DataContext = new SiteAddModDelViewModel(dataContext, m_ot);
            this.DataContext = new SiteAddModViewModel(dataContext, m_ot,siteType,this);
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_SetPoint_Click(object sender, RoutedEventArgs e)
        {
            SiteMapOperation mapWin = new SiteMapOperation(this.DataContext);
            mapWin.Owner = this;
            mapWin.ShowDialog();
        }
    }
}
