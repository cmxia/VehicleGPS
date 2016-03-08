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

namespace VehicleGPS.Views.Control.ReportCentre
{
    /// <summary>
    /// ReportTreeSetting.xaml 的交互逻辑
    /// </summary>
    public partial class ReportTreeSetting : Window
    {
        public ReportTreeSetting(object parentDataContext)
        {
            InitializeComponent();
            this.DataContext = parentDataContext;
        }

        private void btn_Confirm_Click(object sender, RoutedEventArgs e)
        {
            BindingExpression beInnerID = cb_InnerID.GetBindingExpression(CheckBox.IsCheckedProperty);
            BindingExpression beName = cb_Name.GetBindingExpression(CheckBox.IsCheckedProperty);
            beInnerID.UpdateSource();
            beName.UpdateSource();
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
