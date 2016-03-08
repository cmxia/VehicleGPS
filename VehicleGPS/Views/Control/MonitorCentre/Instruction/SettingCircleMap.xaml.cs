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
using System.Windows.Navigation;
using VehicleGPS.Views.Control.MonitorCentre.Instruction;
using VehicleGPS.ViewModels.MonitorCentre.Instruction;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// SettingCircleMap.xaml 的交互逻辑
    /// </summary>
    public partial class SettingCircleMap : Window
    {
       private ImageButton selectedBtn = null;//当前选中的ImageButton
        private List<ImageButton> list_VehicleMenu = null;//用户拥有权限的菜单功能
        private int showConfirmBtn = 0;//是否显示“确定/取消”按钮
        private static SettingCircleMap instance = null;
        public SettingCircleMap()
        {
            InitializeComponent();
            InitVehicleMenu();
            InitDefaultMenu();
            
            //this.DataContext = InstructionViewModel.GetInstance(this.webMap);
        }

        

        public static SettingCircleMap GetInstance()
        {
            if (instance == null)
            {
                instance = new SettingCircleMap();
            }
            return instance;
        }
        /*默认显示的菜单*/
        private void InitDefaultMenu()
        {
            //UserControl uc;
            //uc = RealTimeMonitor.RealTimeMonitor.GetInstance();//默认显示实时监控
            
            //if (sp_Content.Children!=null)
            //{
            //    sp_Content.Children.Clear();
            //}
            //sp_Content.Children.Add(uc);
        }
        /*初始化用户菜单功能*/
        private void InitVehicleMenu()
        {
            if (list_VehicleMenu == null)
            {
                list_VehicleMenu = new List<ImageButton>();
            }
            else
            {
                list_VehicleMenu.Clear();
            }
            int colCount = 0;
            GetVehicleMenu();
            if (Grid_VehicleMenu.Children != null)
            {
                Grid_VehicleMenu.Children.Clear();
            }
            foreach (ImageButton imgBtn in list_VehicleMenu)
            {
                ColumnDefinition cd;
                SeperateBorder border;

                /*添加ImageButton*/
                cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                Grid_VehicleMenu.ColumnDefinitions.Add(cd);
                Grid_VehicleMenu.Children.Add(imgBtn);
                imgBtn.SetValue(Grid.ColumnProperty, colCount);
                colCount++;
                if (colCount / 2 + 1 != list_VehicleMenu.Count)
                {/*添加分隔符*/
                    border = new SeperateBorder();
                    cd = new ColumnDefinition();
                    cd.Width = GridLength.Auto;
                    Grid_VehicleMenu.ColumnDefinitions.Add(cd);
                    Grid_VehicleMenu.Children.Add(border);
                    border.SetValue(Grid.ColumnProperty, colCount);
                    colCount++;
                }
            }
        }

        /*获取用户菜单功能*/
        private void GetVehicleMenu()
        {
            ImageButton imgBtn;

            imgBtn = GetMenuImageButton("imgBtn_AddRegion", "设置圆形区域");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            if (showConfirmBtn > 0)
            {
                AddConfirmCancelButton();
            }
        }

        /*获取菜单不同背景颜色ImageButton*/
        private ImageButton GetMenuImageButtonRight(string xname, string text)
        {
            ImageButton imgBtn = new ImageButton();
            imgBtn.Name = xname;
            imgBtn.Margin = new Thickness(3, 0, 3, 0);
            imgBtn.VerticalAlignment = VerticalAlignment.Center;
            imgBtn.HorizontalAlignment = HorizontalAlignment.Center;
            imgBtn.Background = new SolidColorBrush(Colors.Cyan);

            imgBtn.Text = text;
            imgBtn.TextFontColor = new SolidColorBrush(Colors.Black);
            imgBtn.TextMargin = new Thickness(3);

            imgBtn.MouseOverBorderBackground = new SolidColorBrush(Color.FromRgb(255, 243, 206));
            imgBtn.MouseOverBorderCorner = new CornerRadius(3);

            return imgBtn;
        }
        private void AddConfirmCancelButton()
        {
            ImageButton imgBtn;

            imgBtn = GetMenuImageButtonRight("imgBtn_AddRegionConfirm", "确定");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButtonRight("imgBtn_AddRegionCancel", "取消");
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

            return imgBtn;
        }

        private void Menu_Clicked(object sender, MouseEventArgs e)
        {
            //SetImgBtnState(sender);//修改于2013-12-11
            Window win = null;
            switch (((ImageButton)sender).Name)
            {
                
                case "imgBtn_AddRegion":
                    showConfirmBtn = 1;
                    //InstructionViewModel.GetInstance().MapService.AddCilckListener();
                    InitVehicleMenu();
                    break;
                case "imgBtn_AddRegionConfirm":
                    //判断是否选择了站点，且只能选择站点
                    if (!IsStationSelected())
                    {
                        MessageBox.Show("请选择站点", "提示", MessageBoxButton.OK);
                    }
                    else
                    {
                        //string regionInfo = InstructionViewModel.GetInstance().MapService.GetCircleInfo();
                        //if (string.IsNullOrEmpty(regionInfo))
                        //{
                        //    MessageBox.Show("请先在地图上标注区域", "提示", MessageBoxButton.OK);
                        //}
                        //else
                        //{
                        //    win = new SettingCircle();
                        //    win.Owner = Window.GetWindow(this);
                        //    win.ShowDialog();
                        //    InstructionViewModel.GetInstance().MapService.RemoveCircleByAdd();
                        //    showConfirmBtn = 0;
                        //    InitVehicleMenu();
                        //    InitDefaultMenu();
                        //}
                    }
                    break;
                case "imgBtn_AddRegionCancel":
                    //这里要清除标区
                    //InstructionViewModel.GetInstance().MapService.RemoveAllMarkers();
                    //InstructionViewModel.GetInstance().MapService.RemoveClickListener();
                    showConfirmBtn = 0;
                    InitVehicleMenu();
                    break;
                default:
                    break;
            }
        }

        private bool IsStationSelected()
        {
            return true;
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

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void vechicleTree_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void Context_Info_Click(object sender, RoutedEventArgs e)
        {

        }

        private void context_CheckBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
