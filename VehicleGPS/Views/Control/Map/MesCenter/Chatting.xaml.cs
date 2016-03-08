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
    /// Chatting.xaml 的交互逻辑
    /// </summary>
    public partial class Chatting : Window
    {
        private string Str;//接收人的ID
        private string CStr;//发送人的ID
        private string RecStr;

        public Chatting(string contact, string contactStr)
        {
            InitializeComponent();
            Str = contact.Substring(contact.IndexOf("(") + 1, contact.IndexOf(")") - contact.IndexOf("(") - 1).Trim();
            RecStr = contact.Trim();
            CStr = contactStr.Trim();
            InitializeComponent();
            ReceivePart();
            loadNewMessages();
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
                string sqlstr = @"insert into MsgDetails(Sendid,getid,IsRead,SendContent,inserttime,FMemo,ZT) 
                                values('" + StaticLoginInfo.GetInstance().UserName + "','" + Str + "',0,'" + sendMessage.Text.Trim() + "','"+dt+"',0,1)";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);

                ChatMes cm = new ChatMes();
                cm.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                cm.postId.Text = CStr;
                cm.postTime.Text = dt.ToString("yyyy-MM-dd HH:mm:ss");
                cm.OneMessage.Text = sendMessage.Text;
                sendMessage.Text = "";
                this.mesReceive.Children.Add(cm);
                //ChildAlert cal = new ChildAlert("发送消息成功!");
                //cal.Show();
            }
            catch 
            {
                ChildAlert cal = new ChildAlert("发送消息失败!");
                cal.Show();
            }
        }
        private void ReceivePart()
        {
            RcePart.Text = RecStr;
        }
        private void loadNewMessages()
        {
            string sqlstr = "select * from MsgDetails where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and Sendid='"+Str+"' and IsRead=0 and FMemo='0' and zt=1 order by inserttime ";
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
                cm.postId.Text = Str;
                cm.postTime.Text = retDt.Rows[i]["inserttime"].ToString();
                cm.OneMessage.Text = retDt.Rows[i]["SendContent"].ToString();
                sendMessage.Text = "";
                this.mesReceive.Children.Add(cm);
            }

            try
            {
                string up_sqlstr = "update MsgDetails set IsRead=1 where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and Sendid='" + Str + "' and IsRead=0 and FMemo='0' and zt=1";
                string up_jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, up_sqlstr);
            }
            catch
            {
 
            }
        }
    }
}
