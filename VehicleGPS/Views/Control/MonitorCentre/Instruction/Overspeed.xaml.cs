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
using VehicleGPS.ViewModels.MonitorCentre.RealTimeMonitor;
using System.Net;
using VehicleGPS.Models.Login;
using VehicleGPS.Services;
using System.Data;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// Overspeed.xaml 的交互逻辑
    /// </summary>
    public partial class Overspeed : Window
    {
        private List<InstructionInfo> listVehicle = RealTimeTreeViewModel.GetInstance().GetSelectedVehicles();
        private static IList<InstructionDataGrid> instrcutionDataList = new List<InstructionDataGrid>();
        private string cmd_setparam_str = "", instruction_history_parameters = "", instruction_number_paramertsCount_parameters = "";
        private int parameterCount = 0;
        private string[] resultList = null;
        private WebClient client;
        private int currentIndex = -1;
        public Overspeed()
        {
            InitializeComponent();
            initInstructionDataGrid();

        }

        //初始化待发车辆信息
        private void initInstructionDataGrid()
        {
            if (instrcutionDataList.Count > 0)
            {
                instrcutionDataList.Clear();
            }
            InstructionDataGrid instructionData;
            for (int i = 0; i < listVehicle.Count; i++)
            {
                instructionData = null;
                instructionData = new InstructionDataGrid();
                instructionData.CurrentId = listVehicle[i].sequence;
                instructionData.VehicleId = listVehicle[i].name;
                instructionData.CustomerName = listVehicle[i].parent;
                instructionData.Sim = listVehicle[i].sim;
                instructionData.VehicleNumber = listVehicle[i].id;
                instructionData.States = "未发送";
                instrcutionDataList.Add(instructionData);
            }

            dg_InfoList.ItemsSource = null;
            dg_InfoList.ItemsSource = instrcutionDataList;
        }

        //发送指令
        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if (instrcutionDataList.Count <= 0)
            {
                MessageBox.Show("请选择指令发送对象（车辆）！");
                return;
            }

            //获取指令参数
            if (SpeedLimit.Text != null && ContinueTime.Text != null)
            {
                instruction_history_parameters = "";
                cmd_setparam_str += getTextBoxParameter(SpeedLimit, ref parameterCount, "0055", "d");
                cmd_setparam_str += getTextBoxParameter(ContinueTime, ref parameterCount, "0056", "d");
                instruction_history_parameters = SpeedLimit.Text + "," + ContinueTime.Text;
            }

            if (parameterCount > 0)
            {
                instruction_number_paramertsCount_parameters = "&comId=" + "8103" + "&reply=1&pn=" + parameterCount.ToString() + cmd_setparam_str + "&encode=0";
            }
            else
            {
                MessageBox.Show("指令参数不能为空！");
            }
            for (int i = 0; i < instrcutionDataList.Count; ++i)
            {
                instrcutionDataList[i].States = "待发送";
                instrcutionDataList[i].Result = "";
            }

            for (int i = 0; i < listVehicle.Count; i++)
            {
                string randomStr = null;
                Random urlRandom = new Random();
                randomStr = urlRandom.Next().ToString() + DateTime.Now.ToString() + urlRandom.Next().ToString();
                Uri endpoint = new Uri(VehicleConfig.GetInstance().CONCRETE_SEND_INSTRUCTION_WEB_URL + "?simId=" + listVehicle[i].sim + instruction_number_paramertsCount_parameters + "&stype=" + listVehicle[i].gpsType + "_" + listVehicle[i].gpsVersion + "&time=" + randomStr);
                client = null;
                client = new WebClient();

                //在异步资源下载操作完成时发生
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_GiveInstructionResultCompleted);
                client.DownloadStringAsync(endpoint);

                instrcutionDataList[i].States = "发送中";
            }

        }

        private void client_GiveInstructionResultCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                resultList = null;
                resultList = e.Result.Split(';');
                for (int i = 0; i < instrcutionDataList.Count; ++i)
                {
                    if (resultList[0].Equals(instrcutionDataList[i].Sim) && instrcutionDataList[i].States.Equals("发送中"))
                    {
                        instrcutionDataList[i].States = "完毕";
                        currentIndex++;
                        if ((e.Error == null) && (e.Result != null) && (!e.Result.Equals("")))
                        {
                            //MessageBox.Show("指令返回数据：" + e.Result.ToString());
                            if (i != -1)
                            {
                                //instrcutionDataList[currentIndex].Result = e.Result.ToString();
                                switch (resultList[1])
                                {
                                    case "0":
                                        instrcutionDataList[i].Result = "发送成功";
                                        //string sql = "INSERT INTO InstructionSendHistory(SIM,VehicleId,CustomerID,Instruction,InsDetails,Finsertdate,ZT,VehicleNum,Sender) VALUES ('"+
                                        //    instrcutionDataList[i].Sim + "','" + instrcutionDataList[i].VehicleId +"','"+instrcutionDataList[i].CustomerName +"','超速设置','"+ instruction_history_parameters + "','" +
                                        //    DateTime.Now + "',1,'" + instrcutionDataList[i].VehicleNumber + "','" + StaticLoginInfo.GetInstance().UserName+ "')";
                                        //string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                                        //DataTable dt = JsonHelper.JsonToDataTable(jsonStr);
                                        //dt[0][0] == "error";
                                        break;
                                    case "1":
                                        instrcutionDataList[i].Result = "发送失败";
                                        break;
                                    case "2":
                                        instrcutionDataList[i].Result = "消息有误";
                                        break;
                                    case "3":
                                        instrcutionDataList[i].Result = "暂不支持";
                                        break;
                                    case "4":
                                        instrcutionDataList[i].Result = "车辆不在线";
                                         string sql = "INSERT INTO InstructionSendHistory(SIM,VehicleId,CustomerID,Instruction,InsDetails,Finsertdate,ZT,VehicleNum,Sender) VALUES ('"+
                                            instrcutionDataList[i].Sim + "','" + instrcutionDataList[i].VehicleId +"','"+instrcutionDataList[i].CustomerName +"','超速设置','"+ instruction_history_parameters + "','" +
                                            DateTime.Now + "',1,'" + instrcutionDataList[i].VehicleNumber + "','" + StaticLoginInfo.GetInstance().UserName+ "')";
                                        string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                                        DataTable dt = JsonHelper.JsonToDataTable(jsonStr);
                                        break;
                                    default:
                                        instrcutionDataList[i].Result = "错误";
                                        break;
                                }
                            }
                        }
                        else if (e.Error != null)
                        {
                            if (currentIndex != -1)
                            {
                                //instrcutionDataList[currentIndex].Result = e.Error.ToString();
                                instrcutionDataList[i].Result = "指令下发失败";
                            }
                            //MessageBox.Show("指令下发失败" + e.Error.ToString());
                        }
                        break;
                    }
                }
            }

            if (currentIndex == instrcutionDataList.Count - 1)
            {

            }
        }

        //添加TextBox控件里的参数
        private string getTextBoxParameter(TextBox textBox, ref int parameterCount, string parameterId, string parameterType)
        {
            if ((textBox.Text.Length > 0) && (isNumberCharge(textBox.Text)))
            {
                ++parameterCount;
                return "&pid" + parameterCount.ToString() + "=" + parameterId + "&pv" + parameterCount.ToString() + "=" + parameterType + textBox.Text;

            }
            return "";
        }

        //判断是否为数字
        private bool isNumberCharge(string str)
        {
            bool flag = true;
            for (int i = 0; i < str.Length; ++i)
            {
                if (!Char.IsNumber(str[i]))
                {
                    flag = false;
                    MessageBox.Show("请输入数字！");
                    break;
                }
            }
            return flag;
        }



    }
}
