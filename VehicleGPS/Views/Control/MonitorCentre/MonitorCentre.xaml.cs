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
using VehicleGPS.Views.Warn;
using VehicleGPS.Views.Control.MonitorCentre.Instruction;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using VehicleGPS.Views.Control.MonitorCentre.AbnoermalStatistic;
using VehicleGPS.Views.Control.MonitorCentre.RealTimeMonitor;
using VehicleGPS.Models;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;
using VehicleGPS.Views.Control.MonitorCentre.RegionSearch;
using VehicleGPS.Services;

namespace VehicleGPS.Views.Control.MonitorCentre
{
    /// <summary>
    /// MonitorCentre.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorCentre : UserControl
    {
        public static CheckBox cluster_CB = null;
        private ImageButton selectedBtn = null;//当前选中的ImageButton
        private List<ImageButton> list_VehicleMenu = null;//用户拥有权限的菜单功能
        private int showConfirmBtn = 0;//是否显示“确定/取消”按钮
        private static MonitorCentre instance = null;
        private MonitorCentre()
        {
            InitializeComponent();
            InitVehicleMenu();
            InitDefaultMenu();
        }
        public static MonitorCentre GetInstance()
        {
            if (instance == null)
            {
                instance = new MonitorCentre();
            }
            return instance;
        }
        /*默认显示的菜单*/
        private void InitDefaultMenu()
        {
            UserControl uc;
            uc = RealTimeMonitor.RealTimeMonitor.GetInstance();//默认显示实时监控

            if (sp_Content.Children != null)
            {
                sp_Content.Children.Clear();
            }
            sp_Content.Children.Add(uc);
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
            ColumnDefinition cdnew;
            SeperateBorder bordernew;

            /*聚类按钮*/
            cdnew = new ColumnDefinition();
            cdnew.Width = GridLength.Auto;
            Grid_VehicleMenu.ColumnDefinitions.Add(cdnew);

            CheckBox cb = new CheckBox();
            cb.Content = "聚类";
            cb.VerticalAlignment = VerticalAlignment.Center;
            cb.Margin = new Thickness(3, 0, 3, 0);
            cb.IsChecked = false;
            cb.Name = "cluster_cb";
            cb.Click += new RoutedEventHandler(clusterVehicle);
            cluster_CB = cb;
            Grid_VehicleMenu.Children.Add(cb);
            colCount++;
            bordernew = new SeperateBorder();
            cdnew = new ColumnDefinition();
            cdnew.Width = GridLength.Auto;
            Grid_VehicleMenu.ColumnDefinitions.Add(cdnew);
            Grid_VehicleMenu.Children.Add(bordernew);
            bordernew.SetValue(Grid.ColumnProperty, colCount);
            colCount++;
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
                if (colCount / 2 != list_VehicleMenu.Count)
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

        private void clusterVehicle(object sender, EventArgs e)
        {
            cluster_CB.IsEnabled = false;
            RealTimeTree.treeStatic.IsEnabled = false;
            CheckBox cb = (CheckBox)sender;
            if (StaticTreeState.ClusterReady == LoadingState.LOADCOMPLETE)
            {
                if (cb.IsChecked == true)
                {
                    StaticRealTimeInfo.GetInstance().iscluster = true;
                    RealTimeViewModel.GetInstance().InitBaiduMap();
                }
                else
                {
                    StaticRealTimeInfo.GetInstance().iscluster = false;
                    RealTimeViewModel.GetInstance().InitBaiduMap();
                }
            }
            else
            {
                if (cb.IsChecked == true)
                {
                    cb.IsChecked = false;
                }
                else
                {
                    cb.IsChecked = true;
                }
            }
        }

        /*获取用户菜单功能*/
        private void GetVehicleMenu()
        {
            ImageButton imgBtn;
            //imgBtn = GetMenuImageButton("imgBtn_RealMonitor", "实时监控");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            //list_VehicleMenu.Add(imgBtn);

            //imgBtn = GetMenuImageButton("imgBtn_Track", "车辆跟踪");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            //list_VehicleMenu.Add(imgBtn);

            //imgBtn = GetMenuImageButton("imgBtn_Playback", "轨迹回放");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            //list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_RefreshMap", "刷新");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            //imgBtn = GetMenuImageButton("imgBtn_VehicleConfig", "车辆显示设置");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            //list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_ImageCheck", "图片查看");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            //imgBtn = GetMenuImageButton("imgBtn_Remove", "报警解除");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            //list_VehicleMenu.Add(imgBtn);

            //imgBtn = GetMenuImageButton("imgBtn_Send", "信息发送");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            //list_VehicleMenu.Add(imgBtn);

            //imgBtn = GetMenuImageButton("imgBtn_Overspeed", "超速设置");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            //list_VehicleMenu.Add(imgBtn);
            //新增
            imgBtn = GetMenuImageButton("imgBtn_Abnormalstatistics", "异常统计");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            //新增
            imgBtn = GetMenuImageButton("imgBtn_DrawingMap", "绘图测量工具");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);
            //新增
            imgBtn = GetMenuImageButton("imgBtn_DrawingMapClear", "清除测量绘图");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_SetVehicle", "实时状态显示");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_SetWarn", "报警配置");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_SetPwd", "报警解除密码配置");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_Search", "位置检索");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_RegionSearchVehicle", "区域查车");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Menu_Clicked), true);
            list_VehicleMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_AddRegion", "区域标注");
            if (showConfirmBtn > 0)
            {
                imgBtn.IsEnabled = false;
            }
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
                /*修改于2013-12-11*/
                //case "imgBtn_RealMonitor":
                //    uc = new RealTimeMonitor.RealTimeMonitor();
                //    break;

                case "imgBtn_VehicleConfig"://车辆显示设置
                    win = new VehicleInfoConfig();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                case "imgBtn_RefreshMap"://刷新地图
                    RealTimeViewModel.GetInstance().RefreshRealtimeMap();
                    break;
                case "imgBtn_RegionSearchVehicle"://区域查车
                    win = new RegionSearchView();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                case "imgBtn_SetWarn"://报警配置
                    win = new AlarmSetting();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                case "imgBtn_SetVehicle"://实时状态显示
                    win = new VehicleInfoShowSetting();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                case "imgBtn_SetPwd"://报警解除密码设置
                    win = new SetPwdView();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                case "imgBtn_Search"://位置检索
                    win = new KeyWordSearchView();
                    //win.Owner = Window.GetWindow(this);
                    win.Show();
                    break;
                case "imgBtn_ImageCheck":
                    win = new ImageCheck.ImageCheck();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;

                case "imgBtn_Track":
                    win = new VehicleTrack.VehicleTrack();
                    win.Owner = Window.GetWindow(this);
                    win.Show();
                    break;
                case "imgBtn_Playback":
                    win = new TrackPlayBack.TrackPlayBack();
                    win.Owner = Window.GetWindow(this);
                    win.Show();
                    break;
                case "imgBtn_Send":
                    if (RealTimeTreeViewModel.GetInstance().GetSelectedVehicles().Count == 0)
                    {
                        MessageBox.Show("请选择发送指令的对象（车辆）", "提示", MessageBoxButton.OK);
                        return;
                    }
                    win = new SendMessage();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                case "imgBtn_Overspeed":
                    if (RealTimeTreeViewModel.GetInstance().GetSelectedVehicles().Count == 0)
                    {
                        MessageBox.Show("请选择发送指令的对象（车辆）", "提示", MessageBoxButton.OK);
                        return;
                    }
                    win = new Overspeed();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                case "imgBtn_Abnormalstatistics":
                    win = new AbnormalStaticticMonitor();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                    break;
                case "imgBtn_DrawingMap":

                    RealTimeViewModel.GetInstance().MapService.OpenDrawingManager();
                    break;
                case "imgBtn_DrawingMapClear":

                    RealTimeViewModel.GetInstance().MapService.CloseDrawingManager();
                    break;
                case "imgBtn_AddRegion":
                    showConfirmBtn = 1;
                    RealTimeViewModel.GetInstance().MapService.AddCilckListener();
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
                        string regionInfo = RealTimeViewModel.GetInstance().MapService.GetCircleInfo();
                        if (string.IsNullOrEmpty(regionInfo))
                        {
                            MessageBox.Show("请先在地图上标注区域", "提示", MessageBoxButton.OK);
                        }
                        else
                        {
                            win = new AddRegion();
                            win.Owner = Window.GetWindow(this);
                            win.ShowDialog();
                            RealTimeViewModel.GetInstance().MapService.RemoveCircleByAdd();
                            showConfirmBtn = 0;
                            InitVehicleMenu();
                            InitDefaultMenu();
                        }
                    }
                    break;
                case "imgBtn_AddRegionCancel":
                    //这里要清除标区
                    RealTimeViewModel.GetInstance().MapService.RemoveCircleByAdd();
                    RealTimeViewModel.GetInstance().MapService.RemoveClickListener();
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
    }
}
