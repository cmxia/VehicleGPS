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
using Newtonsoft.Json.Linq;
using VehicleGPS.Models.Login;
using VehicleGPS.Models.MonitorCentre;
using Newtonsoft.Json;
using Microsoft.Practices.Prism.ViewModel;
using System.Data;
using VehicleGPS.Services;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// PhoneSet.xaml 的交互逻辑
    /// </summary>
    public partial class PhoneSet : Window
    {
        List<itemInfo> itemList = new List<itemInfo>();
        List<editinfo> editList = new List<editinfo>();
        private int phoneNum = 0;
        private string contextflag = "2";
        private string phoneflag = "1";
        public bool isDisplay = true;
        public PhoneSet()
        {
            if (VBaseInfo.GetInstance().GPSType_id != "2")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            InitEditList();
            this.dg_EditList.ItemsSource = editList;
            this.DataContext = VBaseInfo.GetInstance();
            States.Text = "未发送";
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            switch (contextflag)
            {
                case "0":
                    sendInfo();
            	    break;
                case "1":
                case "2":
                    itemInfo item = new itemInfo();
                    item.sign = phoneflag;
                    item.phoneNum = tb_length.Text;
                    item.name = tb_content.Text;
                    itemList.Add(item);
                    if (phoneNum == 0)
                    {
                        sendInfo();
                    }
                    else
                    {
                        for (int i = 1; i < phoneNum + 1; i++)
                        {
                            string phoneflagAdd = "1";
                            itemInfo additem = new itemInfo();
                            StackPanel sp = paraList.FindName("newSp" + phoneNum.ToString()) as StackPanel;
                            ComboBox key = sp.FindName("tb_key" + phoneNum.ToString()) as ComboBox;
                            if (key.SelectedItem.ToString() == "呼入")
                            {
                                phoneflagAdd = "1";
                            }
                            else if (key.SelectedItem.ToString() == "呼出")
                            {
                                phoneflagAdd = "2";
                            }
                            else if (key.SelectedItem.ToString() == "呼入/呼出")
                            {
                                phoneflagAdd = "3";
                            }
                            TextBox length = sp.FindName("tb_length" + phoneNum.ToString()) as TextBox;
                            TextBox content = sp.FindName("tb_content" + phoneNum.ToString()) as TextBox;
                            additem.sign = phoneflagAdd;
                            additem.phoneNum = length.Text;
                            additem.name = content.Text;
                            itemList.Add(additem);
                        }
                        sendInfo();
                    }
                    break;  
                case "3":
                    sendInfo();
                    break;
            
            }

        }

        private void sendInfo()
        {
            Result.Text=Texttest(SIM.Text);
            if (Result.Text == "指令已发出，正在处理！")
            {
                States.Text = "已发送";
                switch (contextflag)
                {
                    case "0":
                        string sql = "delete PhonebookInfo where terminal='" + VBaseInfo.GetInstance().GPSType + "' and sim='"+VBaseInfo.GetInstance().SIM+"'";
                        VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                        break;
                    case "1":
                        string sql_update = "delete PhonebookInfo where terminal='" + VBaseInfo.GetInstance().GPSType + "' and sim='" + VBaseInfo.GetInstance().SIM + "'";
                        VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_update);
                        foreach (itemInfo item in itemList)
                        {
                            string sql_insert = "INSERT INTO PhonebookInfo(sign,phonenum,name,sim,terminal,inserttime,username) VALUES ('" +
                            item.sign + "','" + item.phoneNum + "','" + item.name + "','" + VBaseInfo.GetInstance().SIM + "','" + VBaseInfo.GetInstance().GPSType + "','" + DateTime.Now.ToString() + "','" + StaticLoginInfo.GetInstance().UserName + "')";
                            string jsonstr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_insert);
                        }
                        break;
                    case "2":
                        foreach (itemInfo item in itemList)
                        {
                            string sql_insert = "INSERT INTO PhonebookInfo(sign,phonenum,name,sim,terminal,inserttime,username) VALUES ('" +
                            item.sign + "','" + item.phoneNum + "','" + item.name + "','" + VBaseInfo.GetInstance().SIM + "','" + VBaseInfo.GetInstance().GPSType + "','" + DateTime.Now.ToString() + "','" + StaticLoginInfo.GetInstance().UserName + "')";
                            string jsonstr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_insert);
                        }
                        break;
                    case "3":
                        foreach (itemInfo item in itemList)
                        {
                            string sql_edit = "update PhonebookInfo set phonenum='" + item.phoneNum + "', sign='" + item.sign + "' where name='" + item.name + "' and sim='" + VBaseInfo.GetInstance().SIM + "'";
                            string jsonstr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_edit);
                        }
                        break;
                }
                Socket.ExcuteSql("设置电话本", StaticLoginInfo.GetInstance().UserName, JsonConvert.SerializeObject(itemList), Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_PHONEBOOKCMD_TYPE";
                cmd.cmdContent = "设置电话本" + ":" + itemList;
                cmd.cmdSim = SIM.Text.ToString();
                cmd.cmdTime = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                cmd.SendStatus = Result.Text.ToString();
                cmd.VehicleNum = VehicleId.Text.ToString();
                while (true)
                {
                    if (StaticTreeState.CmdStatus == LoadingState.LOADCOMPLETE)
                    {
                        StaticTreeState.CmdStatus = LoadingState.LOADING;
                        StaticMessageInfo.GetInstance().CmdList.Add(cmd);
                        StaticTreeState.CmdStatus = LoadingState.LOADCOMPLETE;
                        break;
                    }
                }
            }
        }

        private string Texttest(string sim)
        {
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "PHONEBOOKCMD_TYPE ");
            jo.Add("simId", sim);
            jo.Add("type", contextflag);
            switch (contextflag)
            {
                case "1":
                case "2":
                    jo.Add("contactList", JsonConvert.SerializeObject(itemList));
                    break;
                case "3":
                    foreach(editinfo edititem in editList)
                    {
                        itemInfo item = new itemInfo();
                        item.name = edititem.name;
                        item.sign = (edititem.EditSelectedIndex + 1).ToString();
                        item.phoneNum = edititem.phoneNum;
                        itemList.Add(item);
                    }
                    jo.Add("contactList", JsonConvert.SerializeObject(itemList));
                    break;
            }
            jo.Add("cmdid", SIM.Text + "_PHONEBOOKCMD_TYPE");
            rebool = Socket.zmqInstructionsPack(sim, jo);
            return rebool;
        }

        private void AddPara_Click(object sender, RoutedEventArgs e)
        {
            phoneNum++;
            StackPanel sp = new StackPanel();
            //sp.Name = "sp" + phoneNum.ToString();
            sp.Width = 470;
            sp.Orientation = Orientation.Horizontal;
            sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            sp.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tbl = new TextBlock();
            tbl.Text = "标志";
            tbl.Margin = new Thickness(1, 5, 0, 5);
            tbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbl.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            ComboBox cbx = new ComboBox();
            cbx.Width = 80;
            cbx.Items.Add("呼入");
            cbx.Items.Add("呼出");
            cbx.Items.Add("呼入/呼出");
            cbx.SelectedIndex = 0;
            cbx.Margin = new Thickness(5, 5, 5, 5);
            cbx.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            cbx.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tb2 = new TextBlock();
            tb2.Text = "电话号码";
            tb2.Margin = new Thickness(5, 5, 0, 5);
            tb2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tb2.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tbx2 = new TextBox();
            //tbx2.Name = "tb_length" + phoneNum;
            tbx2.Width = 100;
            tbx2.Margin = new Thickness(0, 5, 0, 5);
            tbx2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbx2.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tb3 = new TextBlock();
            tb3.Text = "联系人";
            tb3.Margin = new Thickness(5, 5, 0, 5);
            tb3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tb3.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tbx3 = new TextBox();
            //tbx3.Name = "tb_content" + phoneNum;
            tbx3.Width = 100;
            tbx3.Margin = new Thickness(0, 5, 0, 5);
            tbx3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbx3.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            sp.Children.Add(tbl);
            sp.Children.Add(cbx);

            sp.Children.Add(tb2);
            sp.Children.Add(tbx2);

            sp.Children.Add(tb3);
            sp.Children.Add(tbx3);


            paraList.Children.Add(sp);
            paraList.RegisterName("newSp" + phoneNum.ToString(), sp);
            sp.RegisterName("tb_key" + phoneNum.ToString(), cbx);
            sp.RegisterName("tb_length" + phoneNum.ToString(), tbx2);
            sp.RegisterName("tb_content" + phoneNum.ToString(), tbx3);
        }

        private void DeletePara_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp = paraList.FindName("newSp" + phoneNum.ToString()) as StackPanel;
            //TextBox key = sp.FindName("tb_key" + phoneNum.ToString()) as TextBox;
            //TextBox length = sp.FindName("tb_length" + phoneNum.ToString()) as TextBox;
            //TextBox content = sp.FindName("tb_content" + phoneNum.ToString()) as TextBox;
            if (phoneNum == 0)
            {
                MessageBox.Show("至少设置一个联系人！");
                return;
            }
            if (sp != null)
            {
                sp.UnregisterName("tb_key" + phoneNum.ToString());
                sp.UnregisterName("tb_length" + phoneNum.ToString());
                sp.UnregisterName("tb_content" + phoneNum.ToString());
                paraList.Children.Remove(sp);
                paraList.UnregisterName("newSp" + phoneNum.ToString());

                phoneNum--;
            }
        }


        //事件设置类型
        private void Context_Info_Click(object sender, RoutedEventArgs e)
        {
            if (RB_Update.IsChecked == true)
            {
                contextflag = "1";
                paraList.Visibility = Visibility.Visible;
                SP_Edit.Visibility = Visibility.Collapsed;
            }
            else if (RB_Add.IsChecked == true)
            {
                contextflag = "2";
                paraList.Visibility = Visibility.Visible;
                SP_Edit.Visibility = Visibility.Collapsed;
            }
            else if (RB_Edit.IsChecked == true)
            {
                contextflag = "3";
                paraList.Visibility = Visibility.Collapsed;
                SP_Edit.Visibility = Visibility.Visible;
            }
            else if (RB_Delete.IsChecked == true)
            {
                contextflag = "0";
                paraList.Visibility = Visibility.Collapsed;
                SP_Edit.Visibility = Visibility.Collapsed;
            }
        }

        //下拉列表绑定事件
        private void cbChange_Click(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedItem.ToString()=="呼入")
            {
                phoneflag = "1";
            }
            else if (comboBox.SelectedItem.ToString() == "呼出")
            {
                phoneflag = "2";
            }
            else if (comboBox.SelectedItem.ToString() == "呼入/呼出")
            {
                phoneflag = "3";
            }
        }

        private class itemInfo
        {
            public string sign { get; set; }
            public string phoneNum { get; set; }
            public string name { get; set; }
        }

        private class editinfo : NotificationObject
        {
            private List<string> signlist = new List<string>() { "呼入", "呼出","呼入/呼出" };
            public List<string> SignList
            {
                get { return signlist; }
                set
                {
                    signlist = value;
                    this.RaisePropertyChanged("SignList");
                }
            }
            private int editselectedindex;

            public int EditSelectedIndex
            {
                get { return editselectedindex; }
                set
                {
                    editselectedindex = value;
                    this.RaisePropertyChanged("EditSelectedIndex");
                }
            }

            private string _name { get; set; }
            public string name
            {
                get { return _name; }
                set
                {
                    this._name = value;
                    this.RaisePropertyChanged("name");
                }
            }

            private string _phonenum { get; set; }
            public string phoneNum
            {
                get { return _phonenum; }
                set
                {
                    this._phonenum = value;
                    this.RaisePropertyChanged("phoneNum");
                }
            }
        }

        //初始化修改列表
        void InitEditList()
        {
            string sql = "select sign,phonenum,name from PhonebookInfo where terminal='" + VBaseInfo.GetInstance().GPSType + "' and sim='"
                + VBaseInfo.GetInstance().SIM + "'";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                int num = 0;
                if (dt != null)
                {
                    num = dt.Rows.Count;
                }
                for (int i = 0; i < num; i++)
                {
                    editinfo item = new editinfo();
                    item.name = dt.Rows[i]["name"].ToString();
                    item.phoneNum = dt.Rows[i]["phonenum"].ToString();
                    item.EditSelectedIndex = int.Parse(dt.Rows[i]["sign"].ToString())-1;
                    editList.Add(item);
                }
            }
        }

        
    }
}
