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
using VehicleGPS.ViewModels.MessCenter;
using System.Windows.Threading;
using VehicleGPS.Models;
using System.Data;
using VehicleGPS.Services;
using VehicleGPS.Models.Login;

namespace VehicleGPS.Views.Control.MessCenter
{
    /// <summary>
    /// MessagesCenter.xaml 的交互逻辑
    /// 消息中心主页面
    /// </summary>
    public partial class MessagesCenter : Window
    {
        private int mgcount = 0;//双击计数器
        private int mcount = 0;//双击计数器
        private int gcount = 0;//双击计数器
        private int count = 0;//双击计数器

        private string myFid;

        private string myGname;//讨论组名字
        private string myGnum;//讨论组编号
        private DataTable dt_Apply = new DataTable();//申请消息
        private DataTable dt_MsgDetails = new DataTable();//好友消息
        private DataTable dt_FDetails = new DataTable();//发过消息的好友
        private DataTable dt_GroupDetails = new DataTable();//群消息
        private DataTable dt_MyFriends = new DataTable();//好友
        private DataTable dt_MyNewGroup = new DataTable();//讨论组
        private DispatcherTimer timer1 = new DispatcherTimer();//定时获取好友申请消息 聊天消息 群消息
        private DispatcherTimer timer2 = new DispatcherTimer();//双击计时器
        private DispatcherTimer timer4 = new DispatcherTimer();//双击计时器
        private DispatcherTimer timer6 = new DispatcherTimer();//双击计时器
        private DispatcherTimer timer7 = new DispatcherTimer();//双击计时器
        private DispatcherTimer timer3 = new DispatcherTimer();//好友列表定时刷新计时器
        private DispatcherTimer timer5 = new DispatcherTimer();//讨论组列表定时刷新计时器

        public MessagesCenter(string msgCount)
        {
            InitializeComponent();
            this.AllList.SelectionChanged += new SelectionChangedEventHandler(AllList_SelectionChanged);
            if (msgCount == "")
            {
                this.AllList.SelectedIndex = 0;
                timer1_Tick(null, null);
                timer3_Tick(null, null);
                timer5_Tick(null, null);
            }
            else
            {
                this.AllList.SelectedIndex = 2;
                timer1_Tick(null, null);
                timer3_Tick(null, null);
                timer5_Tick(null, null);
            }
        }

        #region 页面切换
        void AllList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.addFriend.Visibility = Visibility.Collapsed;
            if (AllList.SelectedIndex == 2)
            {
                timer1.Interval = new TimeSpan(0, 0, 0, 5);
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Start();
            }
            else if (AllList.SelectedIndex == 1)
            {
                timer5.Interval = new TimeSpan(0, 0, 0, 5);
                timer5.Tick += new EventHandler(timer5_Tick);
                timer5.Start();
            }
            else
            {
                timer3.Interval = new TimeSpan(0, 0, 0, 5);
                timer3.Tick += new EventHandler(timer3_Tick);
                timer3.Start();
            }
        }
        #endregion

        #region 讨论组加载与事件
        // 获取讨论组数据定时器
        void timer5_Tick(object sender, EventArgs e)
        {
            dt_MyNewGroup = getMyNewGroup();
            loadMyNewGroup();
        }

        // 定时获取讨论组数据
        private DataTable getMyNewGroup()
        {
            string sqlstr = @"select * from vehicle.dbo.MsgGroup where Groupnum in (select Groupnum from vehicle.dbo.MsgGroupDetails 
	                        where groupMem = '" + StaticLoginInfo.GetInstance().UserName + @"' and ZT = 1)
	                        and ZT = 1";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt;
        }

        // 加载讨论组
        private void loadMyNewGroup()
        {
            TreeViewItem tvGroup = new TreeViewItem();
            tvGroup.Header = "讨论组";
            tvGroup.IsExpanded = true;
            if (dt_MyNewGroup == null)
            {
                return;
            }
            for (int i = 0; i < dt_MyNewGroup.Rows.Count; i++)
            {
                TreeViewItem tvG = new TreeViewItem();
                tvG.Header = dt_MyNewGroup.Rows[i]["groupname"].ToString();
                tvG.Tag = "{" + dt_MyNewGroup.Rows[i]["Groupnum"].ToString() + "}" + "(" + dt_MyNewGroup.Rows[i]["groupfounder"].ToString() + ")";
                tvGroup.Items.Add(tvG);
                tvG.MouseLeftButtonUp += new MouseButtonEventHandler(tvG_MouseLeftButtonUp);
                tvG.MouseRightButtonUp += new MouseButtonEventHandler(tvG_MouseRightButtonUp);
            }
            this.GroupList.Items.Clear();
            this.GroupList.Items.Add(tvGroup);
        }

        // 讨论组右键菜单
        void tvG_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem TV = (TreeViewItem)sender;
            string founder = TV.Tag.ToString();
            string Founder = founder.Substring(founder.IndexOf("(") + 1, founder.IndexOf(")") - founder.IndexOf("(") - 1);
            string Gnum = founder.Substring(founder.IndexOf("{") + 1, founder.IndexOf("}") - founder.IndexOf("{") - 1);

            if (Founder == StaticLoginInfo.GetInstance().UserName)
            {
                ContextMenu contextMenu = new ContextMenu();
                contextMenu.Background = new SolidColorBrush(Colors.White);

                MenuItem item1 = new MenuItem();
                //Image im = new Image();
                //im.Source = (new ImageSourceConverter()).ConvertFromString("../../Images/delete.png") as ImageSource;
                //item1.Icon = im;
                item1.Header = "删除讨论组";
                item1.Tag = Gnum;
                item1.FontSize = 13;
                item1.Margin = new Thickness(3, 0, 0, 0);
                item1.Width = 180;
                item1.Click += new RoutedEventHandler(ItemDele_Click);
                contextMenu.Items.Add(item1);

                MenuItem item2 = new MenuItem();
                //Image imt = new Image();
                //imt.Source = (new ImageSourceConverter()).ConvertFromString("../../Images/update.png") as ImageSource;
                //item2.Icon = imt;
                item2.Header = "修改讨论组名称";
                item2.Tag = Gnum;
                item2.FontSize = 13;
                item2.Margin = new Thickness(3, 0, 0, 0);
                item2.Width = 180;
                myGname = TV.Header.ToString();
                myGnum = Gnum;
                item2.Click += new RoutedEventHandler(ItemUpdate_Click);

                contextMenu.Items.Add(item2);

                MenuItem item3 = new MenuItem();
                item3.Header = "讨论组成员管理";
                item3.Tag = Gnum;
                item3.FontSize = 13;
                item3.Margin = new Thickness(3, 0, 0, 0);
                item3.Width = 180;
                item3.Click += new RoutedEventHandler(ItemManage_Click);
                contextMenu.Items.Add(item3);

                ContextMenuService.SetContextMenu(TV, contextMenu);
            }
            else
            {
                return;
            }
        }

        //讨论组成员管理
        void ItemManage_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            GroupMemManage gmm = new GroupMemManage(mi.Tag.ToString());
            gmm.Show();
        }

        //删除讨论组
        void ItemDele_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            string gnum = mi.Tag.ToString();

            try
            {
                string sqlstr = "update MsgGroup set zt=0 where Groupnum ='" + myGnum + "'";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                ChildAlert cd = new ChildAlert("删除讨论组成功！！");
                cd.Show();
            }
            catch
            {
                ChildAlert cd = new ChildAlert("删除讨论组失败！！");
                cd.Show();
            }

        }

        //更改讨论组名称
        void ItemUpdate_Click(object sender, RoutedEventArgs e)
        {
            ModifyGMemo mgm = new ModifyGMemo(myGnum, myGname);
            mgm.Show();
        }

        // 讨论组双击事件
        void tvG_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvG = (TreeViewItem)sender;
            string G_num = tvG.Tag.ToString();
            string g_num = G_num.Substring(G_num.IndexOf("{") + 1, G_num.IndexOf("}") - G_num.IndexOf("{") - 1);
            string g_name = tvG.Header.ToString();

            gcount++;
            timer6.Interval = new TimeSpan(0, 0, 0, 0, 800);
            timer6.Tick += new EventHandler(timer6_Tick);
            timer6.Start();
            if (gcount % 2 == 0)
            {
                GroupChatting gc = new GroupChatting(g_num, g_name);
                gc.Show();
            }
        }

        void timer6_Tick(object sender, EventArgs e)
        {
            gcount = 0;
        }

        #endregion

        #region 好友加载与事件
        // 定时获取好友数据
        void timer3_Tick(object sender, EventArgs e)
        {
            dt_MyFriends = getMyNewFriends();
            MyFriends();
        }

        // 查询好友
        private DataTable getMyNewFriends()
        {
            string sqlstr = "select * from MsgShip where (Getid='" + StaticLoginInfo.GetInstance().UserName + "' or Endid='" + StaticLoginInfo.GetInstance().UserName + "') and Isenable=1 and zt=1 ";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt;
        }

        // 加载好友
        void MyFriends()
        {
            TreeViewItem tvFirst = new TreeViewItem();
            tvFirst.Header = "我的好友";
            tvFirst.IsExpanded = true;
            if (dt_MyFriends == null)
            {
                this.FriendList.Items.Clear();
                this.FriendList.Items.Add(tvFirst);
                return;
            }
            for (int i = 0; i < dt_MyFriends.Rows.Count; i++)
            {
                TreeViewItem tv = new TreeViewItem();
                if (dt_MyFriends.Rows[i]["Getid"].ToString() == StaticLoginInfo.GetInstance().UserName)
                {
                    if (dt_MyFriends.Rows[i]["EndNick"].ToString() == "null")
                    {
                        tv.Header = "(" + dt_MyFriends.Rows[i]["Endid"].ToString() + ")";
                        tv.Tag = dt_MyFriends.Rows[i]["Endid"].ToString();
                    }
                    else
                    {
                        tv.Header = dt_MyFriends.Rows[i]["EndNick"].ToString() + "(" + dt_MyFriends.Rows[i]["Endid"].ToString() + ")";
                        tv.Tag = dt_MyFriends.Rows[i]["Endid"].ToString();
                    }
                }
                else
                {
                    if (dt_MyFriends.Rows[i]["GetNick"].ToString() == "null")
                    {
                        tv.Header = "(" + dt_MyFriends.Rows[i]["Getid"].ToString() + ")";
                        tv.Tag = dt_MyFriends.Rows[i]["Getid"].ToString();
                    }
                    else
                    {
                        tv.Header = dt_MyFriends.Rows[i]["GetNick"].ToString() + "(" + dt_MyFriends.Rows[i]["Getid"].ToString() + ")";
                        tv.Tag = dt_MyFriends.Rows[i]["Getid"].ToString();
                    }
                }

                tv.MouseLeftButtonUp += new MouseButtonEventHandler(tv_MouseLeftButtonUp);
                tv.MouseRightButtonUp += new MouseButtonEventHandler(tv_MouseRightButtonUp);
                tvFirst.Items.Add(tv);
            }
            this.FriendList.Items.Clear();
            this.FriendList.Items.Add(tvFirst);
        }

        // 好友右键菜单
        void tv_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem Tr = (TreeViewItem)sender;

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Background = new SolidColorBrush(Colors.White);
            MenuItem item1 = new MenuItem();

            //Image im = new Image();
            //im.Source = (new ImageSourceConverter()).ConvertFromString("../../Images/delete.png") as ImageSource;
            //item1.Icon = im;

            item1.Header = "删除";
            item1.FontSize = 13;
            item1.Margin = new Thickness(3, 0, 0, 0);
            item1.Width = 160;
            item1.Tag = Tr.Tag;
            item1.Click += new RoutedEventHandler(item1_Click);
            contextMenu.Items.Add(item1);

            MenuItem item2 = new MenuItem();

            //Image imT = new Image();
            //imT.Source = (new ImageSourceConverter()).ConvertFromString("../../Images/update.png") as ImageSource;
            //item2.Icon = imT;

            item2.Header = "修改备注姓名";
            item2.FontSize = 13;
            item2.Margin = new Thickness(3, 0, 0, 0);
            item2.Width = 170;
            item2.Tag = Tr.Tag;
            item2.Click += new RoutedEventHandler(item2_Click);
            contextMenu.Items.Add(item2);
            ContextMenuService.SetContextMenu(Tr, contextMenu);

        }

        // 好友菜单删除事件
        void item1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            myFid = "";
            myFid = item.Tag.ToString();

            try
            {
                string sqlstr = "update MsgShip set zt=0 where ((Endid ='" + myFid + "' and Getid ='" + StaticLoginInfo.GetInstance().UserName + "') or( Getid ='" + myFid + "' and Endid ='" + StaticLoginInfo.GetInstance().UserName + "')) and Isenable = 1 and zt =1";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                ChildAlert cd = new ChildAlert("删除好友成功！！");
                cd.Show();
            }
            catch
            {
                ChildAlert cd = new ChildAlert("删除好友失败！！");
                cd.Show();
            }

        }

        // 好友菜单修改备注事件
        void item2_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            ModifyMemo mm = new ModifyMemo(item.Tag.ToString());
            mm.Show();
        }

        // 好友聊天
        void tv_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tr = (TreeViewItem)sender;
            string Contact = tr.Header.ToString();
            string ContactStr = StaticLoginInfo.GetInstance().UserName;

            count++;
            timer4.Interval = new TimeSpan(0, 0, 0, 0, 800);
            timer4.Tick += new EventHandler(timer4_Tick);
            timer4.Start();
            if (count % 2 == 0)
            {
                Chatting ch = new Chatting(Contact, ContactStr);
                ch.Show();
            }
        }

        //双击计数事件
        void timer4_Tick(object sender, EventArgs e)
        {
            count = 0;
        }

        #endregion

        #region 消息处理

        // 加载好友申请和消息
        void timer1_Tick(object sender, EventArgs e)
        {
            dt_Apply = getNewApply();
            dt_MsgDetails = getNewMsgDetails();
            dt_GroupDetails = getNewGroupDetails();
            dt_FDetails = getNewFDetails();
            this.myMes.Children.Clear();
            MyAPPMessage();
            GetChatMsg();
            LoadMyGMes();
        }

        #region 群消息加载与处理

        // 获取群消息
        private DataTable getNewGroupDetails()
        {
            string sqlstr = "select * from MsgDetails where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and IsRead=0 and FMemo!='0' and zt=1 ";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt;
        }

        #region
        //// 获取有新消息的群
        //private DataTable getNewGroup()
        //{
        //    string sqlstr = "select distinct FMemo from MsgDetails where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and IsRead=0 and FMemo!='0' and zt=1 ";
        //    string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
        //    DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
        //    return retDt;
        //}
        #endregion

        // 加载群消息
        private void LoadMyGMes()
        {
            DataTable dt = getMyNewGroup();
            if (dt == null)
            {
                return;
            }
            if (dt_GroupDetails == null)
            {
                return;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int count = 0;
                for (int j = 0; j < dt_GroupDetails.Rows.Count; j++)
                {
                    if (dt_GroupDetails.Rows[j]["FMemo"].ToString() == dt.Rows[i]["Groupnum"].ToString())
                    {
                        count++;
                    }
                }
                if (count > 0)
                {
                    SLChatMsg slcm = new SLChatMsg(count, "讨论组：" + dt.Rows[i]["groupname"].ToString());
                    slcm.Tag = dt.Rows[i]["Groupnum"].ToString();
                    slcm.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    slcm.Margin = new Thickness(2, 0, 1, 0);
                    slcm.MouseLeftButtonDown += new MouseButtonEventHandler(slcm_MouseLeftButtonDown);

                    this.myMes.Children.Add(slcm);
                }
            }


        }

        // 群消息双击事件
        void slcm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SLChatMsg sl = (SLChatMsg)sender;
            mgcount++;
            timer7.Interval = new TimeSpan(0, 0, 0, 0, 800);
            timer7.Tick += new EventHandler(timer7_Tick);
            timer7.Start();
            if (mgcount % 2 == 0)
            {
                string c_name = sl.cname.Substring(sl.cname.IndexOf(':') + 1, (sl.cname.Length - sl.cname.IndexOf(':') - 1));
                GroupChatting gc = new GroupChatting(sl.Tag.ToString(), sl.cname);
                gc.Show();
                this.myMes.Children.Remove(sl);
            }
        }

        // 双击计时器
        void timer7_Tick(object sender, EventArgs e)
        {
            mgcount = 0;
        }

        #endregion

        #region 好友消息

        // 获取好友消息
        private DataTable getNewMsgDetails()
        {
            string sqlstr = "select * from MsgDetails where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and IsRead=0 and FMemo='0' and zt=1 ";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt;
        }

        // 获取所有发送消息的好友
        private DataTable getNewFDetails()
        {
            string sqlstr = "select distinct Sendid from MsgDetails where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and IsRead=0 and zt=1 ";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt;
        }

        // 显示好友消息
        void GetChatMsg()
        {
            if (dt_FDetails == null)
            {
                return;
            }
            if (dt_MsgDetails == null)
            {
                return;
            }
            for (int i = 0; i < dt_FDetails.Rows.Count; i++)
            {
                int Count = 0;
                for (int j = 0; j < dt_MsgDetails.Rows.Count; j++)
                {
                    if (dt_FDetails.Rows[i][0].ToString() == dt_MsgDetails.Rows[j]["Sendid"].ToString())
                    {
                        Count++;
                    }

                }

                SLChatMsg sl = new SLChatMsg(Count, "(" + dt_FDetails.Rows[i][0].ToString() + ")");
                sl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                sl.Margin = new Thickness(2, 0, 1, 0);
                sl.MouseLeftButtonDown += new MouseButtonEventHandler(sl_MouseLeftButtonDown);

                this.myMes.Children.Add(sl);
            }

        }

        // 显示聊天框
        void sl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SLChatMsg sl = (SLChatMsg)sender;
            mcount++;
            timer2.Interval = new TimeSpan(0, 0, 0, 0, 800);
            timer2.Tick += new EventHandler(timers_Tick);
            timer2.Start();
            if (mcount % 2 == 0)
            {
                Chatting ch = new Chatting(sl.cname, StaticLoginInfo.GetInstance().UserName);
                ch.Show();
                this.myMes.Children.Remove(sl);
            }
        }

        // 双击计时事件
        void timers_Tick(object sender, EventArgs e)
        {
            mcount = 0;
        }


        #endregion

        #region 申请消息
        // 获取申请消息
        private DataTable getNewApply()
        {
            string sqlstr = "select * from MsgShip where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and Isenable=0 and zt=1 ";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt;
        }

        //申请消息显示
        void MyAPPMessage()
        {
            if (dt_Apply == null)
            {
                return;
            }
            for (int i = 0; i < dt_Apply.Rows.Count; i++)
            {
                StackPanel s = new StackPanel();
                s.Orientation = Orientation.Horizontal;
                s.Margin = new Thickness(0, 0, 0, 3);

                TextBlock tl = new TextBlock();
                tl.Text = "好友申请来自";
                tl.FontSize = 13;
                tl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tl.Padding = new Thickness(0, 5, 0, 0);
                tl.Margin = new Thickness(2, 0, 1, 0);

                TextBlock tll = new TextBlock();
                tll.Text = dt_Apply.Rows[0]["Endid"].ToString();
                tll.FontSize = 13;
                tll.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                tll.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tll.Margin = new Thickness(1, 5, 5, 0);

                Button bt = new Button();
                bt.Content = "同 意";
                bt.Height = 23;
                bt.Width = 50;
                bt.Background = new SolidColorBrush(Color.FromArgb(255, 153, 187, 232));
                bt.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                bt.Margin = new Thickness(16, 2, 0, 0);
                bt.Click += new RoutedEventHandler(Agree_Click);
                bt.Tag = dt_Apply.Rows[0]["Endid"].ToString();

                Button btt = new Button();
                btt.Content = "拒 绝";
                btt.Height = 23;
                btt.Width = 50;
                btt.Background = new SolidColorBrush(Color.FromArgb(255, 153, 187, 232));
                bt.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                btt.SetValue(MarginProperty, new Thickness(12, 2, 0, 0));
                btt.Click += new RoutedEventHandler(DisAgree_Click);
                btt.Tag = dt_Apply.Rows[0]["Endid"].ToString();

                s.Children.Add(tl);
                s.Children.Add(tll);
                s.Children.Add(bt);
                s.Children.Add(btt);

                this.myMes.Children.Add(s);
            }
        }

        //同意
        private void Agree_Click(object sender, RoutedEventArgs e)
        {
            Button bu = (Button)sender;
            string _id = bu.Tag.ToString();
            try
            {
                string sqlstr = "update MsgShip set Isenable=1 where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and Endid='" + _id + "' and Isenable=0 and zt=1 ";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                ChildAlert cal = new ChildAlert("好友添加成功!");
                cal.Show();
            }
            catch
            {
                ChildAlert cal = new ChildAlert("好友添加失败!");
                cal.Show();
            }
        }

        //拒绝
        private void DisAgree_Click(object sender, RoutedEventArgs e)
        {
            Button but = (Button)sender;
            string _id = but.Tag.ToString();
            try
            {
                string sqlstr = "update MsgShip set zt=0 where Getid='" + StaticLoginInfo.GetInstance().UserName + "' and Endid='" + _id + "' and Isenable=0 and zt=1 ";
                string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
                ChildAlert cal = new ChildAlert("好友拒绝成功!");
                cal.Show();
            }
            catch
            {
                ChildAlert cal = new ChildAlert("好友拒绝失败!");
                cal.Show();
            }
        }

        #endregion

        #endregion

        #region 添加好友

        //好友申请(添加)
        private void addFriend_Click(object sender, MouseButtonEventArgs e)
        {
            if (this.addFriend.Visibility == Visibility.Visible)
            {
                this.addFriend.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.addFriend.Visibility = Visibility.Visible;
            }
        }

        //好友申请(确定)
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = getUserId(this.FriendID.Text.Trim());
            if (dt == null)
            {
                ChildAlert cal = new ChildAlert("该好友不存在!");
                cal.Show();
            }
            else
            {
                try
                {
                    addNewFriends(this.FriendID.Text.Trim(), this.NickName.Text.Trim());
                    ChildAlert cal = new ChildAlert("好友申请发送成功!");
                    cal.Show();
                }
                catch
                {
                    ChildAlert cal = new ChildAlert("操作失败!");
                    cal.Show();
                }
            }
        }

        //查询好友是否存在
        private DataTable getUserId(string Fid)
        {
            string sqlstr = "select UserId from LoginUsers where zt=1 and UserId='" + Fid + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
            DataTable retDt = JsonHelper.JsonToDataTable(jsonStr);
            return retDt;
        }

        // 插入好友申请
        public void addNewFriends(string Fid, string Fnick)
        {
            string sqlstr = @"insert into MsgShip(Endid,Getid,GetNick,Isenable,inserttime,zt) 
                            values('" + StaticLoginInfo.GetInstance().UserName + "','" + Fid + "','" + Fnick + "',0,'" + DateTime.Now + "',1)";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sqlstr);
        }

        //好友申请(取消)
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.addFriend.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region 创建讨论组

        private void OnCreate(object sender, MouseButtonEventArgs e)
        {
            this.addFriend.Visibility = Visibility.Collapsed;
            GroupCreating gc = new GroupCreating(dt_MyFriends);
            gc.Show();
        }
        #endregion
    }
}
