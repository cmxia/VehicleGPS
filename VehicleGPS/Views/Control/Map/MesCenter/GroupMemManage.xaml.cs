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
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using VehicleGPS.Services;

namespace VehicleGPS.Views.Control.MessCenter
{
    /// <summary>
    /// GroupMemManage.xaml 的交互逻辑
    /// </summary>
    public partial class GroupMemManage : Window
    {
        private string myGroupNum;
        private DataTable dt_myAllFriends = new DataTable();//用于存储我所有的好友
        private DataTable dt_myAddedFriends = new DataTable();//用于存储已在讨论组中的好友
        private DataTable dt_DeleteFriends = new DataTable();

        public GroupMemManage(string gnum)
        {
            this.myGroupNum = gnum;
            InitializeComponent();

            this.MemberManage.SelectionChanged += new SelectionChangedEventHandler(MemberManage_SelectionChanged);
            this.MemberManage.SelectedIndex = 0;

            dt_myAllFriends = GetAllMyFriends();
            dt_myAddedFriends = GetAddedFriends();

            LoadMyFriends();
        }

        void MemberManage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MemberManage.SelectedIndex == 0)
            {
                dt_myAllFriends = GetAllMyFriends();
                LoadMyFriends();
            }
            else
            {
                dt_DeleteFriends = GetAddedFriends(); ;
                DeleteAddedFriends();
            }
        }

        private DataTable GetAllMyFriends()
        {
            string sqlstr = @" select * from MsgShip where (Getid='" + StaticLoginInfo.GetInstance().UserName + "' or Endid='" + StaticLoginInfo.GetInstance().UserName + "') and Isenable=1 and zt=1 ";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt;        
        }

        private DataTable GetAddedFriends()
        {
            string sqlstr = @" select * from MsgGroupDetails where(Groupnum = '" + myGroupNum + "' and  Groupnum !='" + StaticLoginInfo.GetInstance().UserName + "' and ZT = 1) ";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt; 
        }


        void LoadMyFriends()
        {
            this.friendlist.Children.Clear();

            for (int i = 0; i < dt_myAllFriends.Rows.Count; i++)
            {
                CheckBox cb = new CheckBox();
                if (dt_myAllFriends.Rows[i]["Endid"].ToString() == StaticLoginInfo.GetInstance().UserName)
                {
                    int sign = 0;
                    for (int j = 0; j < dt_myAddedFriends.Rows.Count; j++)
                    {
                        if (dt_myAddedFriends.Rows[j]["groupMem"].ToString() == dt_myAllFriends.Rows[i]["Getid"].ToString())
                        {
                            cb.Tag = dt_myAllFriends.Rows[i]["Getid"].ToString();
                            cb.Height = 25;
                            cb.FontSize = 13;
                            cb.Content = dt_myAllFriends.Rows[i]["Getid"].ToString();
                            cb.IsEnabled = false;
                            sign = 1;
                        }
                    }
                    if (sign == 0)
                    {
                        cb.Tag = dt_myAllFriends.Rows[i]["Getid"].ToString();
                        cb.Height = 25;
                        cb.FontSize = 13;
                        cb.Content = dt_myAllFriends.Rows[i]["Getid"].ToString();
                        cb.IsEnabled = true;

                        cb.Checked += new RoutedEventHandler(cb_Checked);
                        cb.Unchecked += new RoutedEventHandler(cb_Unchecked);
                    }

                }
                else
                {
                    int sign = 0;
                    for (int j = 0; j < dt_myAddedFriends.Rows.Count; j++)
                    {
                        if (dt_myAddedFriends.Rows[j]["groupMem"].ToString() == dt_myAllFriends.Rows[i]["Endid"].ToString())
                        {
                            cb.Tag = dt_myAllFriends.Rows[i]["Endid"].ToString();
                            cb.Height = 25;
                            cb.FontSize = 13;
                            cb.Content = dt_myAllFriends.Rows[i]["Endid"].ToString();
                            cb.IsEnabled = false;
                            sign = 1;
                        }                  
                    }
                    if (sign == 0)
                    {
                        cb.Tag = dt_myAllFriends.Rows[i]["Endid"].ToString();
                        cb.Height = 25;
                        cb.FontSize = 13;
                        cb.Content = dt_myAllFriends.Rows[i]["Endid"].ToString();
                        cb.IsEnabled = true;

                         cb.Checked += new RoutedEventHandler(cb_Checked);
                        cb.Unchecked += new RoutedEventHandler(cb_Unchecked);                   
                    }
                }
                this.friendlist.Children.Add(cb);            
            }        
        }

        void DeleteAddedFriends()
        {
            this.GroupMems.Children.Clear();

            for (int i = 0; i < dt_DeleteFriends.Rows.Count; i++)
            {
                CheckBox cb = new CheckBox();
                cb.Tag = dt_DeleteFriends.Rows[i]["groupMem"].ToString();
                cb.Margin = new Thickness(30, 4, 0, 0);
                cb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                cb.FontSize = 14;
                cb.Height = 25;
                cb.Content = dt_DeleteFriends.Rows[i]["groupMem"].ToString();

                this.GroupMems.Children.Add(cb);
                if (cb.Tag.ToString() == StaticLoginInfo.GetInstance().UserName)
                {
                    cb.IsEnabled = false;
                }
            
            }
        }


        //未选中从右侧删除
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

        //选中添加到右侧
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

        //全选
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in this.friendlist.Children)
            {
                CheckBox cb = (CheckBox)v;
                if (cb.IsEnabled == true)
                {
                    cb.IsChecked = true;
                }
            }

        }

        //全不选
        private void nSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in this.friendlist.Children)
            {
                CheckBox cb = (CheckBox)v;
                if (cb.IsEnabled == true)
                {
                    cb.IsChecked = false;
                }
            }
        }

        //反选
        private void SelObject_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in this.friendlist.Children)
            {
                CheckBox cb = (CheckBox)v;
                if (cb.IsEnabled == true)
                {
                    cb.IsChecked = !cb.IsChecked;
                }
            }
        }


        #region 添加新成员模块
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in selectFri.Children)
            {
                string sqlstr = "insert into MsgGroupDetails(Groupnum,groupMem,inserttime,ZT) values('" + myGroupNum + "','" + ((TextBlock)v).Text.Trim() + "','"+ DateTime.Now+"','1')";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            }

            ChildAlert cd = new ChildAlert("添加成员成功！！");
            cd.Show();
            this.Close();
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 删除讨论组成员模块

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dt_DeleteFriends.Rows.Count; i++)
            {
                foreach (CheckBox cb in this.GroupMems.Children)
                {
                    if (dt_DeleteFriends.Rows[i]["groupMem"].ToString() == cb.Tag.ToString() && cb.IsChecked == true)
                    {
                        string sqlstr = "update MsgGroupDetails set zt=0 where groupMem='" + cb.Tag.ToString() + "' and Groupnum='"+myGroupNum+"'";
                        string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                    }
               }            
            }

            ChildAlert cd = new ChildAlert("删除成员成功！！");
            cd.Show();
            this.Close();
        }


        private void NCommit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
