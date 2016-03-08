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
using VehicleGPS.ViewModels.MonitorCentre.RegionSearch;

namespace VehicleGPS.Views.Control.MonitorCentre.RegionSearch
{
    /// <summary>
    /// RegionSearchView.xaml 的交互逻辑
    /// </summary>
    public partial class RegionSearchView : Window
    {
        public RegionSearchView()
        {
            InitializeComponent();
            InitImageButton();
            this.DataContext = new RegionSearchViewModel(this.Map);
        }

        private void InitImageButton()
        {
            Brush mouseOverBrush = new SolidColorBrush(Color.FromRgb(255, 243, 206));
            CornerRadius mouseOverCorner = new CornerRadius(3);
            imgBtn_Query.MouseOverBorderBackground = mouseOverBrush;
            imgBtn_Query.MouseOverBorderCorner = mouseOverCorner;
        }
    }
}
