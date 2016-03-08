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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VehicleGPS.ViewModels.DispatchCentre.TaskManage;
using System.Windows.Controls.Primitives;
using VehicleGPS.Models.DispatchCentre.TaskManage;
using VehicleGPS.Models;

namespace VehicleGPS.Views.Control.DispatchCentre.TaskManage
{
    /// <summary>
    /// TaskManage.xaml 的交互逻辑
    /// </summary>
    public partial class TaskManage : Window
    {
        private List<ImageButton> list_OperateMenu = null;//用户拥有权限的功能
        public TaskManage(CVBasicInfo selectedStation)
        {
            InitializeComponent();
            this.DataContext = new TaskManageViewModel(selectedStation);
        }

        /*增加*/
        private void Add_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TaskAddModDel addWin = new TaskAddModDel(this.DataContext, OperateType.ADD);
            addWin.Owner = this;
            addWin.ShowDialog();
        }
        /*修改*/
        private void Mod_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DispatchTaskInfo selectedRegion = ((TaskManageViewModel)this.DataContext).SelectedTask;
            if (selectedRegion == null)
            {
                MessageBox.Show("请选择任务单", "提示", MessageBoxButton.OK);
                return;
            }
            TaskAddModDel modWin = new TaskAddModDel(this.DataContext, OperateType.MOD);
            modWin.Owner = this;
            modWin.ShowDialog();
        }

        /*双击事件*/
        private void taskInfoList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridRow) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            if (dep is DataGridRow)
            {
                this.Mod_PreviewMouseLeftButtonDown(null, null);
            }
        }

        private void Add_Loaded(object sender, RoutedEventArgs e)
        {
           //TaskAddModDel addWin = new TaskAddModDel(this.DataContext, OperateType.ADD);
            //addWin.Owner = this;
          //  addWin.ShowDialog();
        }

      
    }
}

