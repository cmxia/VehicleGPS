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
using System.Data;
using VehicleGPS.Services;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;

namespace VehicleGPS.Views.Control.MessCenter
{
    /// <summary>
    /// GroupCreating.xaml 的交互逻辑
    /// </summary>
    public partial class GroupCreating : Window
    {
        private DataTable AllFriend;
        private string gnum;//新建的讨论组编号
        public GroupCreating(DataTable MyAllFriends)
        {
            InitializeComponent();
            this.AllFriend = MyAllFriends;
        }


        #region 设置讨论组名称模块

        private void GroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (this.GroupName.Text.Trim() == "" || this.GroupName.Text.Trim() == null)
                {
                    ChildAlert al = new ChildAlert("未输入讨论组名称！");
                    al.Show();
                    return;
                }
                try
                {
                    string result = VehicleCommon.wcfDBHelper.BInsertIntoMsgGroup(UniqueStrHelper.getStrSequence(), this.GroupName.Text.Trim(), StaticLoginInfo.GetInstance().UserName, "");
                    DataTable retDt = JsonHelper.JsonToDataTable(result);
                    if (retDt == null)
                    {
                        ChildAlert al = new ChildAlert("讨论组创建失败,请重试！");
                        al.Show();
                    }
                    this.alltab.SelectedIndex = 1;
                    this.gname.Text = this.GroupName.Text.Trim();
                    gnum = retDt.Rows[0]["Groupnum"].ToString();
                    All_Friends();
                }
                catch
                {
                }
            }
        }

       
        //下一步事件
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (this.GroupName.Text.Trim() == "" || this.GroupName.Text.Trim() == null)
            {
                ChildAlert al = new ChildAlert("未输入讨论组名称！");
                al.Show();
                return;
            }
            try
            {
                string result = VehicleCommon.wcfDBHelper.BInsertIntoMsgGroup(UniqueStrHelper.getStrSequence(), this.GroupName.Text.Trim(), StaticLoginInfo.GetInstance().UserName, "");
                DataTable retDt = JsonHelper.JsonToDataTable(result);
                if (retDt == null)
                {
                    ChildAlert al = new ChildAlert("讨论组创建失败,请重试！");
                    al.Show();
                }
                this.alltab.SelectedIndex = 1;
                this.gname.Text = this.GroupName.Text.Trim();
                gnum = retDt.Rows[0]["Groupnum"].ToString();
                All_Friends();
            }
            catch
            { 
            }
            
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion


        #region 添加讨论组成员模块

        //在左侧显示所有好友
        void All_Friends()
        {
            if (AllFriend == null)
            {
                ChildAlert al = new ChildAlert("您还没有好友,请添加好友后再试！");
                al.Show();
                this.Close();
                return;
            }
            for(int i=0;i<AllFriend.Rows.Count;i++)
            {
                CheckBox cb = new CheckBox();
                if (AllFriend.Rows[i]["Getid"].ToString() == StaticLoginInfo.GetInstance().UserName)
                {
                    cb.Tag = AllFriend.Rows[i]["Endid"].ToString();
                    cb.Height = 25;
                    cb.FontSize = 13;
                    cb.Content = AllFriend.Rows[i]["EndNick"].ToString() + "(" + AllFriend.Rows[i]["Endid"].ToString() + ")";
                    cb.Checked += new RoutedEventHandler(cb_Checked);
                    cb.Unchecked += new RoutedEventHandler(cb_Unchecked);
                    cb.IsChecked = true;
                }
                else
                {
                    cb.Tag = AllFriend.Rows[i]["Getid"].ToString();
                    cb.Height = 25;
                    cb.FontSize = 13;
                    cb.Content = AllFriend.Rows[i]["GetNick"].ToString() + "(" + AllFriend.Rows[i]["Getid"].ToString() + ")";
                    cb.Checked += new RoutedEventHandler(cb_Checked);
                    cb.Unchecked += new RoutedEventHandler(cb_Unchecked);
                    cb.IsChecked = true;
                }
                this.friendlist.Children.Add(cb);
            }

        }

        // 选中好友后在右侧显示
        void cb_Unchecked(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (var v in selectFri.Children)
            {
                if (((TextBlock)v).Text == ((CheckBox)sender).Tag.ToString())
                {
                    selectFri.Children.RemoveAt(i);
                    return;
                }
                i++;
            }
        }

        // 取消选中后从右侧删除
        void cb_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var v in selectFri.Children)
            {
                if (((TextBlock)v).Text == ((CheckBox)sender).Tag.ToString())
                {
                    return;
                }
            }
            TextBlock tb = new TextBlock();
            tb.Text = ((CheckBox)sender).Tag.ToString();
            this.selectFri.Children.Add(tb);
        }

        //全选事件
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in this.friendlist.Children)
            {
                CheckBox cb = (CheckBox)v;
                cb.IsChecked = true;
            }
        }

        //全不选
        private void nSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in this.friendlist.Children)
            {
                CheckBox cb = (CheckBox)v;
                cb.IsChecked = false;
            }
        }

        //反选
        private void SelObject_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in this.friendlist.Children)
            {
                CheckBox cb = (CheckBox)v;
                cb.IsChecked = !cb.IsChecked;
            }
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (gnum == "" || gnum == null)
            {
                ChildAlert al = new ChildAlert("未创建讨论组！");
                al.Show();
                return;
            }
            try
            {
                string sqlstr = @"insert into MsgGroupDetails(Groupnum,groupMem,inserttime,FMemo,ZT) 
                               values('" + gnum + "','" + StaticLoginInfo.GetInstance().UserName + "','" + DateTime.Now + "','',1)";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                foreach (var v in selectFri.Children)
                {
                    string sqlstr2 = @"insert into MsgGroupDetails(Groupnum,groupMem,inserttime,FMemo,ZT) 
                               values('" + gnum + "','" + ((TextBlock)v).Text.Trim() + "','" + DateTime.Now + "','',1)";
                    string jsonStr2 = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr2);
                }
                ChildAlert cl = new ChildAlert("讨论组创建成功!");
                cl.Show();
                this.Close();
            }
            catch
            { 

            }

        }
        #endregion
    }
}
