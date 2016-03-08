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
using VehicleGPS.ViewModels.MonitorCentre.VehicleTrack;

namespace VehicleGPS.Views.Control.MonitorCentre.VehicleTrack
{
    /// <summary>
    /// VehicleTrack.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleTrack : Window
    {
        public VehicleTrack()
        {
            InitializeComponent();
            InitImageButton();
            this.DataContext = new VehicleTrackViewModel(this.webMap);
        }
        public VehicleTrack(string vehicleNum)
        {
            InitializeComponent();
            InitImageButton();
            this.DataContext = new VehicleTrackViewModel(this.webMap,vehicleNum);
        }
        private void InitImageButton()
        {
            Brush mouseOverBrush = new SolidColorBrush(Color.FromRgb(255, 243, 206));
            CornerRadius mouseOverCorner = new CornerRadius(3);
            imgBtn_Select.MouseOverBorderBackground = mouseOverBrush;
            imgBtn_Start.MouseOverBorderBackground = mouseOverBrush;
            imgBtn_Stop.MouseOverBorderBackground = mouseOverBrush;

            imgBtn_Select.MouseOverBorderCorner = mouseOverCorner;
            imgBtn_Start.MouseOverBorderCorner = mouseOverCorner;
            imgBtn_Stop.MouseOverBorderCorner = mouseOverCorner;

            imgBtn_Select.AddHandler(ImageButton.MouseLeftButtonUpEvent, new MouseButtonEventHandler(SelectVehicle_MouseLeftButtonUp), true);
        }

        /*打开选择车辆树*/
        private void SelectVehicle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VehicleTrackTree vehicleTreeWin = new VehicleTrackTree(this.DataContext);
            vehicleTreeWin.Owner = this;
            vehicleTreeWin.Height = this.Height - 245;
            vehicleTreeWin.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            vehicleTreeWin.Left = this.Left;
            vehicleTreeWin.Top = this.Top + 75;
            vehicleTreeWin.Show();
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
