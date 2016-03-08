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
using VehicleGPS.Views.Control;
using VehicleGPS.ViewModels.DispatchCentre;
using VehicleGPS.Models;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;
using System.Threading;
using VehicleGPS.Views.Control.DispatchCentre.TaskManage;

namespace VehicleGPS.Views.Control.DispatchCentre
{
    /// <summary>
    /// DispatchCentre.xaml 的交互逻辑
    /// </summary>
    public partial class DispatchCentre : UserControl
    {
        private ImageButton selectedBtn = null;//当前选中的ImageButton
        private List<ImageButton> list_DispatchMenu = null;//用户拥有权限的菜单功能
        private static DispatchCentre instance;
        private DispatchCentre()
        {
            InitializeComponent();
            InitDispatchMenu();
            InitDispatchWin();
        }

        public static DispatchCentre GetInstance()
        {
            if (instance == null)
            {
                instance = new DispatchCentre();
            }
            return instance;
        }
        /*初始化用户菜单功能*/
        private void InitDispatchMenu()
        {
            list_DispatchMenu = new List<ImageButton>();
            int colCount = 0;
            GetDispatchMenu();

            foreach (ImageButton imgBtn in list_DispatchMenu)
            {
                ColumnDefinition cd;
                SeperateBorder border;

                /*添加ImageButton*/
                cd = new ColumnDefinition();
                cd.Width = GridLength.Auto;
                Grid_DispatchMenu.ColumnDefinitions.Add(cd);
                Grid_DispatchMenu.Children.Add(imgBtn);
                imgBtn.SetValue(Grid.ColumnProperty, colCount);
                colCount++;
                if (colCount / 2 + 1 != list_DispatchMenu.Count)
                {/*添加分隔符*/
                    border = new SeperateBorder();
                    cd = new ColumnDefinition();
                    cd.Width = GridLength.Auto;
                    Grid_DispatchMenu.ColumnDefinitions.Add(cd);
                    Grid_DispatchMenu.Children.Add(border);
                    border.SetValue(Grid.ColumnProperty, colCount);
                    colCount++;
                }
            }
        }

        /*获取用户菜单功能*/
        private void GetDispatchMenu()
        {
            ImageButton imgBtn;
            //imgBtn = GetMenuImageButton("imgBtn_VehicleDispatch", "车辆调度");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(DispatchMenu_MouseLeftButtonUp), true);
            //list_DispatchMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_DispatchRegion", "区域管理");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(DispatchMenu_MouseLeftButtonUp), true);
            list_DispatchMenu.Add(imgBtn);

            //imgBtn = GetMenuImageButton("imgBtn_SiteManage", "工地管理");
            //imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(DispatchMenu_MouseLeftButtonUp), true);
            //list_DispatchMenu.Add(imgBtn);

            imgBtn = GetMenuImageButton("imgBtn_TaskManage", "任务单管理");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(DispatchMenu_MouseLeftButtonUp), true);
            list_DispatchMenu.Add(imgBtn);
            imgBtn = GetMenuImageButton("imgBtn_TaskStatistic", "任务统计");
            imgBtn.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(DispatchMenu_MouseLeftButtonUp), true);
            list_DispatchMenu.Add(imgBtn);
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
        private void InitDispatchWin()
        {
            UserControl uc = null;
            uc = new VehicleDispatch.VehicleDispatch();
            this.DispatchContentGrid.Children.Add(uc);
        }
        private void DispatchMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string menuName = ((ImageButton)sender).Name.ToString();
            CVBasicInfo selectedNode = DispatchTreeViewModel.GetInstance().SelectedNode;//获取树型数据
            switch (menuName)
            {
                //case "imgBtn_VehicleDispatch":
                //    uc = new VehicleDispatch.VehicleDispatch();
                //    break;
                case "imgBtn_DispatchRegion"://区域管理
                    if (selectedNode == null || string.Compare(selectedNode.TypeID, "Unit") != 0)
                    {
                        //默认选择第一个单位
                        selectedNode = DispatchTreeViewModel.GetInstance().RootNode.ListChildNodes[0].ListChildNodes[0].NodeInfo;                       
                    }
                    SiteManage.SiteManageMain regionMWin = new SiteManage.SiteManageMain(selectedNode, SiteType.Region);
                    regionMWin.Owner = Window.GetWindow(this);
                    regionMWin.ShowDialog();
                    //DispatchRegionManage.DispatchRegionManage regionWin = new DispatchRegionManage.DispatchRegionManage(selectedNode);
                    //regionWin.Owner = Window.GetWindow(this);
                    //regionWin.ShowDialog();
                    break;
                case "imgBtn_SiteManage"://工地管理
                    if (selectedNode == null || string.Compare(selectedNode.TypeID, "Unit") != 0)
                    {
                        //默认选择第一个单位
                        selectedNode = DispatchTreeViewModel.GetInstance().RootNode.ListChildNodes[0].ListChildNodes[0].NodeInfo;   
                    }
                    SiteManage.SiteManageMain buildingMWin = new SiteManage.SiteManageMain(selectedNode, SiteType.Building);
                    buildingMWin.Owner = Window.GetWindow(this);
                    buildingMWin.ShowDialog();
                    break;
                case "imgBtn_TaskManage"://任务单管理
                    if (selectedNode == null || string.Compare(selectedNode.TypeID, "Unit") != 0)
                    {
                        //默认选择第一个单位
                        selectedNode = DispatchTreeViewModel.GetInstance().RootNode.ListChildNodes[0].ListChildNodes[0].NodeInfo;  
                    }
                    TaskManage.TaskManage taskWin = new TaskManage.TaskManage(selectedNode);
                    taskWin.Owner = Window.GetWindow(this);
                    taskWin.ShowDialog();
                    break;
                case "imgBtn_TaskStatistic"://任务统计
                    //if (selectedNode == null || string.Compare(selectedNode.TypeID, "Unit") != 0)
                    //{
                    //    //默认选择第一个单位
                    //    selectedNode = DispatchTreeViewModel.GetInstance().RootNode.ListChildNodes[0].ListChildNodes[0].NodeInfo;
                    //}
                    TaskStatistics win = new TaskStatistics();
                    win.Owner = Window.GetWindow(this);
                    win.ShowDialog();
                   
                    break;
                default:
                    break;
            }
            //if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE)
            //{
            //    CVBasicInfo basicInfo = DispatchTreeViewModel.GetInstance().SelectedNode;//获取树型数据
            //    if (basicInfo != null)
            //    {
            //        if (string.Compare(basicInfo.TypeID, "xq00002") == 0)
            //        {
            //            if (win != null)
            //            {
            //                SetImgBtnState(sender);
            //                win.Owner = Window.GetWindow(this);
            //                win.ShowDialog();
            //                //this.DispatchContentGrid.Children.Add(win);
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("只能选择站点");
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("请先选择站点");
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("正在加载站点数据，请稍后", "加载数据", MessageBoxButton.OKCancel);
            //}
        }
    }
}
