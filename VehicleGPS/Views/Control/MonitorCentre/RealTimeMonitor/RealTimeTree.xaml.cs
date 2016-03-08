//RealTimeTree
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
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;

namespace VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor
{
    /// <summary>
    /// RealTimeTree.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeTree : UserControl
    {
        public static TreeView treeStatic = null;
        public RealTimeTree()
        {
            InitializeComponent();
            InitButton();
            this.DataContext = RealTimeTreeViewModel.GetInstance();
            treeStatic = this.tv_Vehicle;
        }

        /*设置“设置按钮”“刷新按钮”样式*/
        private void InitButton()
        {
            SetButton(imgBtn_Setting);
            SetButton(imgBtn_Refresh);
        }
        private void SetButton(ImageButton imgBtn)
        {
            imgBtn.Margin = new Thickness(2, 0, 2, 0);
            imgBtn.VerticalAlignment = VerticalAlignment.Center;
            imgBtn.HorizontalAlignment = HorizontalAlignment.Center;

            imgBtn.TextFontColor = new SolidColorBrush(Colors.Black);
            imgBtn.TextMargin = new Thickness(3);

            imgBtn.MouseOverBorderBackground = new SolidColorBrush(Color.FromRgb(255, 243, 206));
            imgBtn.MouseOverBorderCorner = new CornerRadius(3);

            if (imgBtn.Name == "imgBtn_Setting")
            {
                imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(opereate_Clicked), true);
            }
        }

        private void opereate_Clicked(object sender, RoutedEventArgs e)
        {
            switch (((ImageButton)sender).Name)
            {
                case "imgBtn_Setting":
                    Window win = new MonitorTreeSetting(this.DataContext);
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                default:
                    break;
            }
        }

        private void tv_Vehicle_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FrameworkElement item = sender as FrameworkElement;
            item.BringIntoView();
        }
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                treeViewItem.IsSelected = true;
                e.Handled = true;
            }
           
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
          if(  source.GetType()==typeof(T))
                return null;
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }
    }
}
