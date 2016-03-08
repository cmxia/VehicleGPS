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
using System.Data;
using VehicleGPS.Services;

namespace VehicleGPS.Views.Control.MessCenter
{
    /// <summary>
    /// ModifyMemo.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyMemo : Window
    {
        public ModifyMemo(string FId)
        {
            InitializeComponent();
            this.F_Id.Text = FId;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sqlstr = "select * from MsgShip where ((Endid ='" + this.F_Id.Text.Trim() + "' and Getid ='" + StaticLoginInfo.GetInstance().UserName + "') or( Getid ='" + this.F_Id.Text.Trim() + "' and Endid ='" + StaticLoginInfo.GetInstance().UserName + "')) and Isenable = 1 and zt =1";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
                if (retDt == null)
                {
                    ChildAlert cd = new ChildAlert("好友备注修改失败！！");
                    cd.Show();
                }
                for (int i = 0; i < retDt.Rows.Count; i++)
                {
                    if (retDt.Rows[i]["Endid"].ToString() == StaticLoginInfo.GetInstance().UserName)
                    {
                        string sqlstr2 = "update MsgShip set GetNick='" + this.Fmemo.Text.Trim() + "' where Endid ='" + retDt.Rows[i]["Endid"].ToString() + "' and Getid ='" + this.F_Id.Text.Trim() + "' and Isenable = 1 and zt =1";
                        string jsonStr2 = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr2);
                    }
                    else
                    {
                        string sqlstr2 = "update MsgShip set EndNick='" + this.Fmemo.Text.Trim() + "' where Getid ='" + retDt.Rows[i]["Getid"].ToString() + "' and Endid ='" + this.F_Id.Text.Trim() + "' and Isenable = 1 and zt =1";
                        string jsonStr2 = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr2);
                    }
                }
                ChildAlert cal = new ChildAlert("好友备注修改成功！！");
                cal.Show();
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
