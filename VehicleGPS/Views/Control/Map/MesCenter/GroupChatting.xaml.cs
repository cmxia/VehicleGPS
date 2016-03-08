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
using System.Data;
using VehicleGPS.Services;
using VehicleGPS.Models.Login;

namespace VehicleGPS.Views.Control.MessCenter
{
    /// <summary>
    /// GroupChatting.xaml 的交互逻辑
    /// </summary>
    public partial class GroupChatting : Window
    {
        private string GStr;//讨论组内部编号
        private string GName;//讨论组名称
        private DataTable mGroup = new DataTable();//讨论组成员
        public GroupChatting(string g_num, string g_name)
        {
            GStr = g_num;
            GName = g_name;
            InitializeComponent();
            this.RcePart.Text = "讨论组：" + g_name;
            loadMessage();
            loadGroupDetails();
            showGroupDetails();
        }

        private void loadGroupDetails()
        {
            string sqlstr = "select * from MsgGroupDetails where Groupnum='" + GStr + "' and ZT=1";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            mGroup = retDt;
        }

        private void showGroupDetails()
        {
            TreeViewItem tviF = new TreeViewItem();
            tviF.Tag = GStr;
            tviF.Header = GName;
            tviF.IsExpanded = true;
            for (int i = 0; i < mGroup.Rows.Count;i++ )
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.Header = mGroup.Rows[i]["groupMem"].ToString();
                tviF.Items.Add(tvi);
            }
            this.gdetails.Items.Add(tviF);
        }

        private void loadMessage()
        {
            string sqlstr = "select * from MsgDetails where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and IsRead=0 and FMemo='" + GStr + "' and zt=1 order by inserttime ";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            if (retDt == null)
            {
                return;
            }
            for (int i = 0; i < retDt.Rows.Count; i++)
            {
                ChatMes cm = new ChatMes();
                cm.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                cm.postId.Text = retDt.Rows[i]["Sendid"].ToString();
                cm.postTime.Text = retDt.Rows[i]["inserttime"].ToString();
                cm.OneMessage.Text = retDt.Rows[i]["SendContent"].ToString();
                sendMessage.Text = "";
                this.mesReceive.Children.Add(cm);
            }

            try
            {
                string up_sqlstr = "update MsgDetails set IsRead=1 where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and IsRead=0 and FMemo='" + GStr + "' and zt=1";
                string up_jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, up_sqlstr);
            }
            catch
            {

            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            if (sendMessage.Text.Trim() == null || sendMessage.Text.Trim() == "")
            {
                ChildAlert al = new ChildAlert("发送消息不能为空！");
                al.Show();
                return;
            }
            try
            {
                DateTime dt = DateTime.Now;
                for (int i = 0; i < mGroup.Rows.Count; i++)
                {
                    if (mGroup.Rows[i]["groupMem"].ToString() == StaticLoginInfo.GetInstance().UserName)
                    {
                        continue;
                    }
                    string sqlstr = @"insert into MsgDetails(Sendid,getid,IsRead,SendContent,inserttime,FMemo,ZT) 
                                values('" + StaticLoginInfo.GetInstance().UserName + "','" + mGroup.Rows[i]["groupMem"].ToString() + "',0,'" + sendMessage.Text.Trim() + "','" + dt + "','" + GStr + "',1)";
                    string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                }
                ChatMes cm = new ChatMes();
                cm.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                cm.postId.Text = StaticLoginInfo.GetInstance().UserName;
                cm.postTime.Text = dt.ToString("yyyy-MM-dd HH:mm:ss");
                cm.OneMessage.Text = sendMessage.Text;
                sendMessage.Text = "";
                this.mesReceive.Children.Add(cm);
            }
            catch
            {
                ChildAlert cal = new ChildAlert("发送消息失败!");
                cal.Show();
            }
        }
    }
}
