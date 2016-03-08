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
using System.Data;
using VehicleGPS.Services;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// EventSet.xaml 的交互逻辑
    /// </summary>
    public partial class EventSet : Window
    {
        List<itemInfo> itemList = new List<itemInfo>();
        private List<deleteInfo> deleteList = new List<deleteInfo>();
        private List<deleteInfo> editList = new List<deleteInfo>();
        private int phoneNum = 0;
        //private int deleteNum = 0;
        private string contextflag = "2";
        public bool isDisplay = true;
        public EventSet()
        {
            if (VBaseInfo.GetInstance().GPSType_id != "2")
            {
                MessageBox.Show("该车辆终端设备不包含该指令！");
                isDisplay = false;
                return;
            }
            InitializeComponent();
            InitDelList();
            this.dg_DeleteList.ItemsSource = this.deleteList;
            this.dg_EditList.ItemsSource = this.editList;
            
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
                    item.id = tb_key.Text;
                    item.len = tb_length.Text;
                    item.data = tb_content.Text;
                    itemList.Add(item);
                    if (phoneNum==0)
                    {
                        sendInfo();
                    }
                    else
                    {
                        for (int i = 1; i < phoneNum+1; i++)
                        {
                            itemInfo additem = new itemInfo();
                            StackPanel sp = paraList.FindName("newSp" + phoneNum.ToString()) as StackPanel;
                            TextBox key = sp.FindName("tb_key" + phoneNum.ToString()) as TextBox;
                            TextBox length = sp.FindName("tb_length" + phoneNum.ToString()) as TextBox;
                            TextBox content = sp.FindName("tb_content" + phoneNum.ToString()) as TextBox;
                            additem.id = key.Text;
                            additem.len = length.Text;
                            additem.data = content.Text;
                            itemList.Add(additem);
                        }
                        sendInfo();
                    }
                    break;
                case "3":
                    foreach(deleteInfo editinfo in editList)
                    {
                        itemInfo iteminfo = new itemInfo();
                        iteminfo.id = editinfo.id;
                        iteminfo.len = editinfo.length;
                        iteminfo.data = editinfo.data;
                        itemList.Add(iteminfo);
                    }
                    sendInfo();
                    break;
                case "4":
                    foreach (deleteInfo deleteitem in deleteList)
                    {
                        if (deleteitem.IsChecked == true)
                        {
                            phoneNum++;
                            itemInfo iteminfo = new itemInfo();
                            iteminfo.id = deleteitem.id;
                            iteminfo.len = deleteitem.length;
                            //iteminfo.data = deleteitem.data;
                            itemList.Add(iteminfo);
                        }
                    }
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
                        string sql = "delete EventsettingInfo where terminal='" + VBaseInfo.GetInstance().GPSType  +"' and sim='" + VBaseInfo.GetInstance().SIM + "'";
                        VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                        break;
                    case "1":
                        string sql_update = "delete EventsettingInfo where terminal='" + VBaseInfo.GetInstance().GPSType + "' and sim='"+VBaseInfo.GetInstance().SIM+ "'";
                        VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_update);
                        foreach (itemInfo item in itemList)
                        {
                            string sql_insert = "INSERT INTO EventsettingInfo(eventId,length,data,sim,terminal,inserttime,username) VALUES ('" +
                            item.id + "','" + item.len + "','" + item.data + "','" + VBaseInfo.GetInstance().SIM + "','" + VBaseInfo.GetInstance().GPSType + "','" + DateTime.Now.ToString() + "','" + StaticLoginInfo.GetInstance().UserName  + "')";
                            VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_insert);
                        }
                        break;
                    case "2":
                        foreach (itemInfo item in itemList)
                        {
                            string sql_insert = "INSERT INTO EventsettingInfo(eventId,length,data,sim,terminal,inserttime,username) VALUES ('" +
                            item.id + "','" + item.len + "','" + item.data + "','" + VBaseInfo.GetInstance().SIM + "','" + VBaseInfo.GetInstance().GPSType + "','" + DateTime.Now.ToString() + "','" + StaticLoginInfo.GetInstance().UserName + "')";
                            string jsonstr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_insert);
                        }
                        break;
                    case "3":
                        foreach (itemInfo item in itemList)
                        {
                            string sql_edit = "update EventsettingInfo set length='" + item.len + "', data='" + item.data + "' where eventId='" + item.id + "' and sim='" + VBaseInfo.GetInstance().SIM + "'";
                            string jsonstr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_edit);
                        }
                        break;
                    case "4":
                        foreach (itemInfo item in itemList)
                        {
                            string sql_delete = "delete EventsettingInfo where eventId='" + int.Parse(item.id) + " and sim='" + VBaseInfo.GetInstance().SIM + "'";
                            string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql_delete);
                        }
                        break;
                }
                Socket.ExcuteSql("事件设置", StaticLoginInfo.GetInstance().UserName, JsonConvert.SerializeObject(itemList), Result.Text, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM.Text + "_EVENTSETCMD_TYPE";
                cmd.cmdContent = "事件设置" + ":" + itemList;
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
            jo.Add("cmd", "EVENTSETCMD_TYPE ");
            jo.Add("simId", sim);
            jo.Add("type", contextflag);
            switch(contextflag)
            {
                case "1":
                case "2":
                    jo.Add("num", phoneNum+1);
                    jo.Add("eventList", JsonConvert.SerializeObject(itemList));
                    break;
                case "3":
                    jo.Add("num", editList.Count);
                    jo.Add("eventList", JsonConvert.SerializeObject(itemList));
                    break;
                case "4":
                    jo.Add("num", phoneNum);
                    jo.Add("eventList", JsonConvert.SerializeObject(itemList));
                    break;
            }
            jo.Add("cmdid", SIM.Text + "_EVENTSETCMD_TYPE");
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
            tbl.Text = "事件ID";
            tbl.Margin = new Thickness(1,5,0,5);
            tbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbl.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBox tbx1 = new TextBox();
            //tbx1.Name = "tb_key" + phoneNum;
            tbx1.Width = 100;
            tbx1.Margin = new Thickness(0, 5, 0, 5);
            tbx1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            tbx1.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            TextBlock tb2 = new TextBlock();
            tb2.Text = "事件内容长度";
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
            tb3.Text = "事件内容";
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
            sp.Children.Add(tbx1);
            
            sp.Children.Add(tb2);
            sp.Children.Add(tbx2);
            
            sp.Children.Add(tb3);
            sp.Children.Add(tbx3);
            

            paraList.Children.Add(sp);
            paraList.RegisterName("newSp" + phoneNum.ToString(), sp);
            sp.RegisterName("tb_key" + phoneNum.ToString(), tbx1);
            sp.RegisterName("tb_length" + phoneNum.ToString(), tbx2);
            sp.RegisterName("tb_content" + phoneNum.ToString(), tbx3);
        }

        private void DeletePara_Click(object sender, RoutedEventArgs e)
        {
            StackPanel sp = paraList.FindName("newSp" + phoneNum.ToString()) as StackPanel;
            //TextBox key = sp.FindName("tb_key" + phoneNum.ToString()) as TextBox;
            //TextBox length = sp.FindName("tb_length" + phoneNum.ToString()) as TextBox;
            //TextBox content = sp.FindName("tb_content" + phoneNum.ToString()) as TextBox;
            if (phoneNum==0)
            {
                MessageBox.Show("至少设置一个事件！");
                return;
            }
            if (sp!=null)
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
                SP_Delete.Visibility = Visibility.Collapsed;
                SP_Edit.Visibility = Visibility.Collapsed;
            }
            else if (RB_DelAll.IsChecked ==true)
            {
                contextflag = "0";
                paraList.Visibility = Visibility.Collapsed;
                SP_Delete.Visibility = Visibility.Collapsed;
                SP_Edit.Visibility = Visibility.Collapsed;
            }
            else if (RB_Add.IsChecked == true)
            {
                contextflag = "2";
                paraList.Visibility = Visibility.Visible;
                SP_Delete.Visibility = Visibility.Collapsed;
                SP_Edit.Visibility = Visibility.Collapsed;
            }   
            else if (RB_Edit.IsChecked == true)
            {
                contextflag = "3";
                paraList.Visibility = Visibility.Collapsed;
                SP_Delete.Visibility = Visibility.Collapsed;
                SP_Edit.Visibility = Visibility.Visible;
            }
            else if (RB_Delete.IsChecked == true)
            {
                contextflag = "4";
                paraList.Visibility = Visibility.Collapsed;
                SP_Delete.Visibility = Visibility.Visible;
                SP_Edit.Visibility = Visibility.Collapsed;
            }
        }

        private class itemInfo
        {
            public string id { get; set; }
            public string len { get; set; }
            public string data { get; set; }
        }

        public class deleteInfo : NotificationObject
        {
            public string id { get; set; }

            private string _length { get; set; }
            public string length
            {
                get { return _length; }
                set
                {
                    this._length = value;
                    this.RaisePropertyChanged("length");
                }
            }

            private string _data { get; set; }
            public string data
            {
                get { return _data; }
                set
                {
                    this._data = value;
                    this.RaisePropertyChanged("data");
                }
            }
            private bool ischecked;
            public bool IsChecked
            {
                get { return ischecked; }
                set
                {
                    this.ischecked = value;
                    this.RaisePropertyChanged("IsChecked");
                }
            }
        }
        
  
        //初始化删除列表
        void InitDelList()
        {
            string sql = "select eventId,length,data from EventsettingInfo where terminal='"+VBaseInfo.GetInstance().GPSType+"' and sim='"
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
                    deleteInfo item = new deleteInfo();
                    item.id = dt.Rows[i]["eventId"].ToString();
                    item.length = dt.Rows[i]["length"].ToString();
                    item.data = dt.Rows[i]["data"].ToString();
                    deleteList.Add(item);
                    editList.Add(item);
                }
            }
        }
    }
}
