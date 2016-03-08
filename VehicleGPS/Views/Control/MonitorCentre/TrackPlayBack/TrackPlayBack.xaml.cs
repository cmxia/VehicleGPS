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
using VehicleGPS.ViewModels.MonitorCentre.TrackPlayBack;
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using VehicleGPS.Models;

namespace VehicleGPS.Views.Control.MonitorCentre.TrackPlayBack
{
    /// <summary>
    /// TrackPlayBack.xaml 的交互逻辑
    /// </summary>
    public partial class TrackPlayBack : Window
    {
        public TrackPlayBack()
        {
            InitializeComponent();
            InitImageButton();
            InitDateTimePicker();
            this.DataContext = new TrackPlayViewModel(this.webMap);
        }
        public TrackPlayBack(CVBasicInfo basic, DateTime starttime, DateTime endtime)
        {
            InitializeComponent();
            InitImageButton();
            InitDateTimePicker();
            this.DataContext = new TrackPlayViewModel(this.webMap, basic, starttime, endtime);
        }
        private void InitDateTimePicker()
        {

        }
        private void InitImageButton()
        {
            Brush mouseOverBrush = new SolidColorBrush(Color.FromRgb(255, 243, 206));
            CornerRadius mouseOverCorner = new CornerRadius(3);
            imgBtn_Query.MouseOverBorderBackground = mouseOverBrush;
            imgBtn_StartPlay.MouseOverBorderBackground = mouseOverBrush;
            imgBtn_PausePlay.MouseOverBorderBackground = mouseOverBrush;
            imgBtn_StopPlay.MouseOverBorderBackground = mouseOverBrush;
            imgBtn_MinusSpeed.MouseOverBorderBackground = mouseOverBrush;
            imgBtn_PlusSpeed.MouseOverBorderBackground = mouseOverBrush;
            //imgBtn_Select.MouseOverBorderBackground = mouseOverBrush;

            imgBtn_Query.MouseOverBorderCorner = mouseOverCorner;
            imgBtn_StartPlay.MouseOverBorderCorner = mouseOverCorner;
            imgBtn_PausePlay.MouseOverBorderCorner = mouseOverCorner;
            imgBtn_StopPlay.MouseOverBorderCorner = mouseOverCorner;
            imgBtn_MinusSpeed.MouseOverBorderCorner = mouseOverCorner;
            imgBtn_PlusSpeed.MouseOverBorderCorner = mouseOverCorner;
            //imgBtn_Select.MouseOverBorderCorner = mouseOverCorner;

            //imgBtn_Select.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(SelectVehicle_MouseLeftButtonUp), true);
        }
        /*打开选择车辆树*/
        private void SelectVehicle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TrackPlayTree vehicleTreeWin = new TrackPlayTree(this.DataContext);
            vehicleTreeWin.Owner = this;
            vehicleTreeWin.Height = this.Height - 245;
            vehicleTreeWin.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            vehicleTreeWin.Left = this.Left;
            vehicleTreeWin.Top = this.Top + 75;
            vehicleTreeWin.ShowDialog();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Collapsed;
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
