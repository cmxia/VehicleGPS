//RealTimeMonitor.xaml.cs
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
using VehicleGPS.Models;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;

namespace VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor
{
    /// <summary>
    /// RealTimeMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeMonitor : UserControl
    {
        private ImageButton selectedBtn = null;//当前选中的ImageButton(2)
        private List<ImageButton> list_VehicleMenu = null;//用户拥有权限的菜单功能(2)

        private static RealTimeMonitor instance = null;
        private RealTimeMonitor()
        {
            InitializeComponent();
            //InitVehicleMenu();
            this.DataContext = RealTimeViewModel.GetInstance(this.webMap);
            ///推送 6-11
            StaticTreeState.RealTimeViewContruct = true;
        }
        public static RealTimeMonitor GetInstance()
        {
            if (instance == null)
            {
                instance = new RealTimeMonitor();
            }
            return instance;
        }
        /*初始化用户菜单功能*/
        private void InitVehicleMenu()
        {
            list_VehicleMenu = new List<ImageButton>();
            int colCount = 0;
            GetVehicleMenu();

            foreach (ImageButton imgBtn in list_VehicleMenu)
            {
                ColumnDefinition cd;
                SeperateBorder border;

                /*添加ImageButton*/
                cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                //Grid_VehicleMenu.ColumnDefinitions.Add(cd);
                //Grid_VehicleMenu.Children.Add(imgBtn);
                imgBtn.SetValue(Grid.ColumnProperty, colCount);
                colCount++;
                if (colCount / 2 + 1 != list_VehicleMenu.Count)//最后不需要分隔符
                {/*添加分隔符*/
                    border = new SeperateBorder();
                    cd = new ColumnDefinition();
                    cd.Width = GridLength.Auto;
                    //Grid_VehicleMenu.ColumnDefinitions.Add(cd);
                    //Grid_VehicleMenu.Children.Add(border);
                    border.SetValue(Grid.ColumnProperty, colCount);
                    colCount++;
                }
            }
        }

        /*获取用户菜单功能*/
        private void GetVehicleMenu()
        {
            ImageButton imgBtn;
            imgBtn = GetMenuImageButton("imgBtn_SetVehicle", "实时状态显示");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);
            imgBtn = GetMenuImageButton("imgBtn_SetWarn", "报警配置");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);
            imgBtn = GetMenuImageButton("imgBtn_SetPwd", "报警解除密码配置");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);
        }
        /*获取菜单ImageButton*/
        private ImageButton GetMenuImageButton(string xname, string text)
        {
            ImageButton imgBtn = new ImageButton();
            imgBtn.Name = xname;
            imgBtn.Margin = new Thickness(3, 0, 3, 0);
            imgBtn.VerticalAlignment = VerticalAlignment.Center;
            imgBtn.HorizontalAlignment = HorizontalAlignment.Center;

            imgBtn.Text = text;
            imgBtn.TextFontColor = new SolidColorBrush(Colors.Black);
            imgBtn.TextMargin = new Thickness(3);

            imgBtn.MouseOverBorderBackground = new SolidColorBrush(Color.FromRgb(255, 243, 206));
            imgBtn.MouseOverBorderCorner = new CornerRadius(3);

            imgBtn.ToolTip = "点击设置实时状态显示字段";

            return imgBtn;
        }

        private void Menu_Clicked(object sender, MouseEventArgs e)
        {
            //SetImgBtnState(sender);
            Window win = null;
               
            switch (((ImageButton)sender).Name)
            {
                case "imgBtn_SetWarn"://报警配置
                    win = new AlarmSetting();
                    break;
                case "imgBtn_SetVehicle"://实时状态显示
                    win = new VehicleInfoShowSetting();
                    break;
                case "imgBtn_SetPwd":
                    win = new SetPwdView();
                    break;
                default:
                    break;
            }
            if (win != null)
            {
                win.Owner = Window.GetWindow(this);
                win.ShowDialog();
            }
        }
        /*设置菜单选中状态*/
        private void SetImgBtnState(object sender)
        {
            if (selectedBtn != null)
            {
                selectedBtn.NormalBorderBackground = new SolidColorBrush(Colors.Transparent);//取消前一次菜单的选中状态
            }
            selectedBtn = (ImageButton)sender; ;
            selectedBtn.NormalBorderBackground = new SolidColorBrush(Color.FromRgb(255, 243, 206));//设置当前菜单为选中状态
            selectedBtn.NormalBorderCorner = new CornerRadius(3);
        }

        private void Grid_VehicleInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Grid_VehicleInfo.SelectedItem != null)
            {
                Grid_VehicleInfo.ScrollIntoView(Grid_VehicleInfo.SelectedItem);
            }
        }
    }
}
