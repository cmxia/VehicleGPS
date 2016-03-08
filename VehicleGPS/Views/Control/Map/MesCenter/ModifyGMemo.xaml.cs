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
using VehicleGPS.Models;
using VehicleGPS.Models.Login;

namespace VehicleGPS.Views.Control.MessCenter
{
    /// <summary>
    /// ModifyGMemo.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyGMemo : Window
    {
        private string mygnum;
        public ModifyGMemo(string myGnum, string myGname)
        {
            InitializeComponent();
            this.G_Name.Text = myGname;
            this.mygnum = myGnum;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sqlstr = @"update MsgGroup set groupname='" + this.Gmemo.Text.ToString() + "' where Groupnum='" + mygnum + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                ChildAlert cl = new ChildAlert("修改讨论组名称成功!");
                cl.Show();
                this.Close();
            }
            catch
            {
 
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
