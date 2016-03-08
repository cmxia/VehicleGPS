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
using ZeroMQ;
using Newtonsoft.Json.Linq;
using VehicleGPS.Models.Login;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    /// <summary>
    /// ServerSite.xaml 的交互逻辑
    /// </summary>
    public partial class ServerSite : Window
    {
        private string termType;//终端类型
        private string[] ServerType;//服务器类型
        public bool isDisplay { get; set; }
        public ServerSite(string e)
        {
            InitializeComponent();
            ServerType = e.Split(',');
            this.DataContext = VBaseInfo.GetInstance();
            States.Text = "未发送";
            switch (VBaseInfo.GetInstance().GPSType)
            {
                case "华强协议":
                    termType = "1";
                    break;
                case "部标协议":
                    termType = "2";
                    break;
                case "Queclink协议":
                    termType = "3";
                    break;
                case "康凯斯协议":
                    termType = "4";
                    break;
                case "世通协议":
                    termType = "5";
                    break;
                case "交大神州协议":
                    termType = "6";
                    break;
                default:
                    termType = "0";
                    break;
            }
            if ("备份服务器"==ServerType[0])
            {
                Title = "备份服务器设置";
                if ("APN"==ServerType[1])
                {
                    lb_ServerSite.Text = "备份服务器APN";
                }
                else if ("用户名"==ServerType[1])
                {
                    lb_ServerSite.Text = "备份服务器无线通信拨号用户名";
                }
                else if ("密码"==ServerType[1])
                {
                    lb_ServerSite.Text="备份服务器无线通信拨号密码";
                }
                else if ("IP"==ServerType[1])
                {
                    lb_ServerSite.Text = "备份服务器地址，IP或域名";
                }
            }
            else if ("主服务器"==ServerType[0])
            {
                Title = "主服务器设置";
                if ("APN"==ServerType[1])
                {
                    lb_ServerSite.Text = "主服务器APN";
                }
                else if ("用户名"==ServerType[1])
                {
                    lb_ServerSite.Text = "主服务器无线通信拨号用户名";
                }
                else if ("密码"==ServerType[1])
                {
                    lb_ServerSite.Text="主服务器无线通信拨号密码";
                }
                else if ("IP"==ServerType[1])
                {
                    lb_ServerSite.Text="主服务器地址，IP或域名";
                }
            }
        }

        private void SendIntruction_Click(object sender, RoutedEventArgs e)
        {
            if (ServerType[1]=="APN")
            {
                if ((tb_ServerSite.Text.Length > 0))
                {
                    if ("主服务器" == ServerType[0])
                    {
                        if (Socket.Texttest(SIM.Text, termType, tb_ServerSite.Text, "16"))
                        {
                            States.Text = "已发送";
                            Result.Text = "发送成功";
                            Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                        else
                        {
                            States.Text = "未发送";
                            Result.Text = "发送失败";
                            //Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                    }
                    else
                    {
                        if (Socket.Texttest(SIM.Text, termType, tb_ServerSite.Text, "20"))
                        {
                            States.Text = "已发送";
                            Result.Text = "发送成功";
                            Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                        else
                        {
                            States.Text = "未发送";
                            Result.Text = "发送失败";
                            //Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("服务器APN不能为空！");
                }
            }
            else if (ServerType[1]=="用户名")
            {
                if ((tb_ServerSite.Text.Length > 0))
                {
                    if ("主服务器" == ServerType[0])
                    {
                        if (Socket.Texttest(SIM.Text, termType, tb_ServerSite.Text, "17"))
                        {
                            States.Text = "已发送";
                            Result.Text = "发送成功";
                            Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                        else
                        {
                            States.Text = "未发送";
                            Result.Text = "发送失败";
                            //Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                    }
                    else
                    {
                        if (Socket.Texttest(SIM.Text, termType, tb_ServerSite.Text, "21"))
                        {
                            States.Text = "已发送";
                            Result.Text = "发送成功";
                            Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                        else
                        {
                            States.Text = "未发送";
                            Result.Text = "发送失败";
                            //Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("无线通信拨号用户名不能为空！");
                }
            }
            else if (ServerType[1] == "密码")
            {
                if ((tb_ServerSite.Text.Length > 0))
                {
                    if ("主服务器" == ServerType[0])
                    {
                        if (Socket.Texttest(SIM.Text, termType, tb_ServerSite.Text, "18"))
                        {
                            States.Text = "已发送";
                            Result.Text = "发送成功";
                            Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                        else
                        {
                            States.Text = "未发送";
                            Result.Text = "发送失败";
                            //Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                    }
                    else
                    {
                        if (Socket.Texttest(SIM.Text, termType, tb_ServerSite.Text, "22"))
                        {
                            States.Text = "已发送";
                            Result.Text = "发送成功";
                            Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                        else
                        {
                            States.Text = "未发送";
                            Result.Text = "发送失败";
                            //Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("无线通信拨号密码不能为空！");
                }
            }
            else if (ServerType[1] == "IP")
            {
                if ((tb_ServerSite.Text.Length > 0))
                {
                    if ("主服务器" == ServerType[0])
                    {
                        if (Socket.Texttest(SIM.Text, termType, tb_ServerSite.Text, "19"))
                        {
                            States.Text = "已发送";
                            Result.Text = "发送成功";
                            Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                        else
                        {
                            States.Text = "未发送";
                            Result.Text = "发送失败";
                            //Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                    }
                    else
                    {
                        if (Socket.Texttest(SIM.Text, termType, tb_ServerSite.Text, "23"))
                        {
                            States.Text = "已发送";
                            Result.Text = "发送成功";
                            Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                        else
                        {
                            States.Text = "未发送";
                            Result.Text = "发送失败";
                            //Socket.ExcuteSql(lb_ServerSite.Text, StaticLoginInfo.GetInstance().UserName, tb_ServerSite.Text, Result.Text, VBaseInfo.GetInstance().SIM);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("地址IP或域名不能为空！");
                }
            }
        }
    }
}
