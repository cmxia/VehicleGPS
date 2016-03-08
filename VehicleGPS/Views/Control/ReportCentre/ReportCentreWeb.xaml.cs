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

namespace VehicleGPS.Views.Control.ReportCentre
{
    /// <summary>
    /// ReportCentreWeb.xaml 的交互逻辑
    /// </summary>
    public partial class ReportCentreWeb : UserControl
    {
        public ReportCentreWeb()
        {
            InitializeComponent();
            string appPath = Environment.CurrentDirectory ;
            string reportPath = appPath + "/Map/ReportCenter.htm";
            Uri uri = new Uri(reportPath, UriKind.RelativeOrAbsolute);
            this.report_web.Navigate(uri);
        }
    }
}
