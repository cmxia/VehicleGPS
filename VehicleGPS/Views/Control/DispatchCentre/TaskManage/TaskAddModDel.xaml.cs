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
using VehicleGPS.ViewModels.DispatchCentre.TaskManage;

namespace VehicleGPS.Views.Control.DispatchCentre.TaskManage
{
    /// <summary>
    /// TaskAddMod.xaml 的交互逻辑
    /// </summary>
    public partial class TaskAddModDel : Window
    {
        public TaskAddModDel(Object dataContext, OperateType m_ot)
        {
            InitializeComponent();
            /*根据不同的操作类型初始化窗体*/
            if (m_ot == OperateType.ADD)
            {
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/add.png", UriKind.Absolute));
                this.Title = "添加任务单";
            }
            if (m_ot == OperateType.MOD)
            {
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/update.png", UriKind.Absolute));
                this.Title = "修改任务单";
            }
            this.DataContext = new TaskAddModDelViewModel(dataContext, m_ot,this);
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /*选择混泥土标号*/
        private void img_Concrete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ConcreteNumSelection concreteWin = new ConcreteNumSelection(this.DataContext);
            concreteWin.Owner = this;
            concreteWin.ShowDialog();
        }      
    }
}
