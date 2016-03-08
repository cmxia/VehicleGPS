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
using VehicleGPS.Views.Control.ReportCentre.Reports.Common;
using VehicleGPS.Models.Login;
using Newtonsoft.Json;
using System.ComponentModel;

namespace VehicleGPS.Views.Control.ReportCentre.Reports
{
    /// <summary>
    /// 指令下发历史 夏创铭
    /// HistoryOfIssuedInstructions.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryOfIssuedInstructions : Window
    {
        private List<InstructionHistory> transList = new List<InstructionHistory>();
        private List<InstructionHistory> currentTransList = new List<InstructionHistory>();
        private List<CVBasicInfo> selectedVehicleList;
        private DateTime startTime;
        private DateTime endTime;
        private int pageSize = 20;

        public HistoryOfIssuedInstructions(List<CVBasicInfo> selectedList, DateTime sTime, DateTime eTime)
        {
            InitializeComponent();

            this.selectedVehicleList = selectedList;
            this.startTime = sTime;
            this.endTime = eTime;

            BackgroundWorker worker = new BackgroundWorker();
            //worker.DoWork += (o, ea) =>
            //{
            //    this.InitData();
            //};
            //worker.RunWorkerCompleted += (o, ea) =>
            //{
            //    this.Indicator.IsBusy = false;
            //};
            //this.Indicator.IsBusy = true;
            //worker.RunWorkerAsync();
            this.InitData();
        }
        private void InitData()
        {
            string sql = "select * from instructionsendhistory where zt=1 and convert(varchar(100),Finsertdate,120)>'"
                + this.startTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and convert(varchar(100),Finsertdate,120)<'"
                + this.endTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and (";
            string sqlVehicleNum = "";
            for (int i = 0; i < this.selectedVehicleList.Count; i++)
            {
                if (i == 0)
                {
                    sqlVehicleNum = "vehiclenum='" + this.selectedVehicleList[i].ID + "'";
                }
                else
                {
                    sqlVehicleNum += " or vehiclenum='" + this.selectedVehicleList[i].ID + "'";
                }
            }
            sql += sqlVehicleNum + ")";
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (jsonStr.Equals("error"))
            {
                MessageBox.Show("查询数据失败，请重新尝试", "提示", MessageBoxButton.OK);
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    this.Close();
                });
                return;
            }
            string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
            List<InstructionHistory> tmp = (List<InstructionHistory>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<InstructionHistory>));
            this.InitInstructionDetail(tmp);
        }
        /*初始化指令详细设置*/
        private void InitInstructionDetail(List<InstructionHistory> tmp)
        {
            int count = 0;
            foreach (InstructionHistory info in tmp)
            {
                //if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE)
                //{
                //    foreach (CVBasicInfo basicInfo in StaticBasicInfo.GetInstance().ListClientBasicInfo)
                //    {
                //        if (info.CustomerID == basicInfo.ID)
                //        {
                //            info.CustomerName = basicInfo.Name;
                //            break;
                //        }
                //    }
                //}
                //else
                //{//基础数据加载没完成，用id替代
                //    info.CustomerName = info.CustomerID;
                //}
                info.Finsertdate = Convert.ToDateTime(info.Finsertdate).ToString("yyyy/MM/dd HH:mm:dd");
                string[] serverStr = info.InsDetails.Split(new Char[] { ',' });
                info.InsDetailsStr = "";
                info.Sequence = ++count;
                switch (info.Instruction)
                {
                    case "服务器设置":
                        //  instruction_history_parameters += host_server_entrance.Text + "," + host_server_username.Text + "," + host_server_password.Text + "," + host_server_address.Text
                        // + "," + standby_server_entrance.Text + "," + standby_server_passport.Text + "," + IP_address.Text + "," + standby_server_username.Text + "," + TCP_port.Text + "," + UDP_port.Text;
                        info.InsDetailsStr = "主服务器APN，无线通信拨号访问点" + serverStr[0] + ",主服务器无线通信拨号用户名" + serverStr[1] + ",主服务器无线通信拨号密码" + serverStr[2] +
                            ",备份服务器APN，无线通信拨号访问点" + serverStr[4] + ",备份服务器无线通信拨号密码" + serverStr[5] + ",备份服务器地址,IP或域名" + serverStr[6] + ",主服务器地址,IP或域名" + serverStr[3] + ",备份服务器无线通信拨号用户名" + serverStr[7]
                            + ",服务器TCP端口" + serverStr[8] + ",服务器UDP端口" + serverStr[9];
                        break;
                    case "电话指令":
                        //       instruction_history_parameters += Monitoring_platform_number.Text + "," + Reset_phone_number.Text + "," + Restore_factory_settings_phone_number.Text + ","
                        //+ Monitoring_platform_SMS_phone_number.Text + "," + receiving_terminal_SMS_text_alarm_number.Text + "," + Terminal_telephone_answering_strategy.Text + "," +
                        //time_last.Text + "," + month_time_last.Text + "," + Phone_number.Text + "," + Supervision_platform_privilege_SMS_number.Text;
                        //string[] serverStr = ins.InsDetails.Split(new Char[] { ',' });
                        info.InsDetailsStr = "监控平台电话号码" + serverStr[0] + ",复位电话号码" + serverStr[1] + ",恢复出厂设置电话号码" + serverStr[2] + ",监控平台SMS电话号码" + serverStr[3] +
                            ",接收终端SMS文本报警号码" + serverStr[4] + ",终端电话接听策略" + serverStr[5] + ",每次最长通话时间" + serverStr[6] + "s,当月最长通话时间" + serverStr[7] + "s,监听电话号码" + serverStr[8]
                            + ",监管平台特权短信号码" + serverStr[9];
                        break;
                    case "位置上报设置":
                        if (serverStr[0].Equals("0"))
                        {
                            info.InsDetailsStr += "根据ACC状态,";
                        }
                        else
                        {
                            info.InsDetailsStr += "根据登录和ACC状态,";
                        }
                        if (serverStr[1].Equals("3"))
                        {
                            info.InsDetailsStr += "定时定距" + serverStr[2] + "s，" +
                        serverStr[3] + "m，缺省" + serverStr[4] + "s，" + serverStr[5] + "m，紧急报警时" + serverStr[6] + "s，" + serverStr[7] + "m," + "拐点补传角度" + serverStr[8];
                        }
                        else if (serverStr[1].Equals("2"))
                        {
                            //instruction_history_parameters += "定距汇报" + drivers_are_not_logged_reporting_distance_interval.Text + "m，缺省" + the_default_distance_interval_reporting.Text + "m，" + "紧急报警时" + the_default_distance_interval_reporting_onem.Text + "m，";
                            info.InsDetailsStr += "定距汇报" + serverStr[2] + "m，缺省" + serverStr[3] + "m，紧急报警时" + serverStr[4] + "m," + "拐点补传角度" + serverStr[5];
                        }
                        else if (serverStr[1].Equals("1"))
                        {
                            info.InsDetailsStr += "定时汇报" + serverStr[2] + "s，缺省" + serverStr[3] + "s，紧急报警时" + serverStr[4] + "s," + "拐点补传角度" + serverStr[5];
                        }
                        break;
                    case "休眠设置":
                        info.InsDetailsStr = "休眠时汇报时间间隔" + serverStr[0] + "s，" + "距离间隔" + serverStr[1] + "m";
                        break;
                    case "短信指令":
                        info.InsDetailsStr = "SMS消息应答超时时间" + serverStr[0] + "s，" + "SMS消息重传次数" + serverStr[1];
                        break;
                    case "车辆信息":
                        info.InsDetailsStr = "车辆里程表读数" + serverStr[0] + "," + serverStr[1] + "," + serverStr[2] + ",";
                        if (serverStr.Length == 5)
                        {
                            info.InsDetailsStr += serverStr[3] + "," + serverStr[4];
                        }
                        else
                        {
                            info.InsDetailsStr += serverStr[3];
                        }
                        break;
                    case "参数设置":
                        if (serverStr[4].Equals("j")) //警报
                        {
                            info.InsDetailsStr = "紧急报警时间间隔" + serverStr[0] + "s，距离间隔" + serverStr[1];
                            if (serverStr[2].Equals("1"))
                            {
                                info.InsDetailsStr += ",报警屏蔽字";
                            }
                            if (serverStr[3].Equals("1"))
                            {
                                info.InsDetailsStr += ",报警拍摄存储标志";
                            }
                        }
                        else
                        {
                            info.InsDetailsStr = "心跳发送间隔" + serverStr[0] + "s,TCP消息应答超时时间" + serverStr[1] + "s,UDP消息应答超时时间" + serverStr[2]
                               + "s,TCP重传次数" + serverStr[3] + ",UDP重传次数" + serverStr[4];
                        }
                        break;
                    case "开关设置":
                        if (serverStr[2].Equals("j"))
                        {
                            if (serverStr[0].Equals("0"))
                            {
                                info.InsDetailsStr = "报警拍摄关,";
                            }
                            else
                            {
                                info.InsDetailsStr = "报警拍摄开,";
                            }
                            if (serverStr[1].Equals("0"))
                            {
                                info.InsDetailsStr += "报警发送文本SMS关";
                            }
                            else
                            {
                                info.InsDetailsStr += "报警发送文本SMS开";
                            }
                        }
                        break;
                    case "超速设置":
                        info.InsDetailsStr = "限速值" + serverStr[0] + "s，持续时间" + serverStr[1] + "s";
                        break;
                    case "终端管理":
                        info.InsDetailsStr = "版本号" + serverStr[0] + ",";
                        if (serverStr[1].Equals("0"))
                        {
                            info.InsDetailsStr += "普通防爆,";
                        }
                        else if (serverStr[1].Equals("1"))
                        {
                            info.InsDetailsStr += "ACC防爆,";
                        }
                        else if (serverStr[1].Equals("2"))
                        {
                            info.InsDetailsStr += "区域防爆,";
                        }
                        else
                        {
                            info.InsDetailsStr += "ACC+区域防爆,";
                        }

                        if (serverStr[2].Equals("0"))
                        {
                            info.InsDetailsStr += "普通,";
                        }
                        else if (serverStr[1].Equals("1"))
                        {
                            info.InsDetailsStr += "can,";
                        }
                        else
                        {
                            info.InsDetailsStr += "485,";
                        }

                        if (serverStr[3].Equals("0"))
                        {
                            info.InsDetailsStr += "不启用转鼓检测,";
                        }
                        else
                        {
                            info.InsDetailsStr += "启用转鼓检测,";
                        }

                        if (serverStr[4].Equals("0"))
                        {
                            info.InsDetailsStr += "ACC是与位置刷新不关联,";
                        }
                        else
                        {
                            info.InsDetailsStr += "ACC是与位置刷新关联,";
                        }

                        if (serverStr[5].Equals("0"))
                        {
                            info.InsDetailsStr += "与电子速度不关联,";
                        }
                        else
                        {
                            info.InsDetailsStr += "与电子速度关联,";
                        }

                        break;
                    case "文本信息下发":
                        if (serverStr[0].Equals("1"))
                        {
                            info.InsDetailsStr += "紧急,";
                        }

                        if (serverStr[1].Equals("1"))
                        {
                            info.InsDetailsStr += "显示器显示,";
                        }
                        if (serverStr[2].Equals("1"))
                        {
                            info.InsDetailsStr += "TTS播读";
                        }
                        if (serverStr[3].Equals("1"))
                        {
                            info.InsDetailsStr += "广告屏显示，";
                        }
                        if (serverStr[4].Equals("0"))
                        {
                            info.InsDetailsStr += "中心导航，";
                        }
                        else
                        {
                            info.InsDetailsStr += "CAN故障码，";
                        }
                        info.InsDetailsStr += ",下发内容:" + serverStr[5];
                        break;
                    case "远程锁车":
                        if (serverStr[0].Equals("0"))
                        {
                            info.InsDetailsStr = "远程解锁";
                        }
                        else
                        {
                            info.InsDetailsStr = "远程锁车";
                        }
                        break;
                    case "点名":
                        info.InsDetailsStr = serverStr[0] + "," + serverStr[1];
                        break;
                }
            }
            this.transList = tmp;
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
                this.PageIndexChanging(0, null);
                Pager pager = new Pager(this.transList.Count, this.pageSize);
                pager.PageIndexChanging += new PagerIndexChangingEvent(this.PageIndexChanging);
                this.pagerContainer.Children.Clear();
                this.pagerContainer.Children.Add(pager);
            });
        }
        private void PageIndexChanging(int pageIndex, EventArgs e)
        {
            int startIndex, endIndex;
            startIndex = this.pageSize * pageIndex;
            if (startIndex + this.pageSize > this.transList.Count)
            {
                endIndex = this.transList.Count - 1;
            }
            else
            {
                endIndex = startIndex + this.pageSize - 1;
            }
            this.currentTransList.Clear();
            for (; startIndex <= endIndex; startIndex++)
            {
                this.currentTransList.Add(this.transList[startIndex]);
            }
            this.dg_TransList.ItemsSource = null;
            this.dg_TransList.ItemsSource = this.currentTransList;
        }
        private void export_static_Click(object sender, RoutedEventArgs e)
        {

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

