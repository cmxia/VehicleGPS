using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using Microsoft.Practices.Prism.Commands;
using Newtonsoft.Json.Linq;
using VehicleGPS.Views.Control.MonitorCentre.Instruction;
using VehicleGPS.Models.Login;
using VehicleGPS.Models.MonitorCentre;
using System;
using System.Windows;

namespace VehicleGPS.ViewModels.MonitorCentre.Instruction
{
    class ShotNowViewModel : NotificationObject
    {
        private int saveSignFlag = 1;
        public ShotNowViewModel()
        {
            this.SendInstructionCommand = new DelegateCommand(this.SendInstructionCommandExecute);
            InitVehicleInfo();
        }

        public DelegateCommand SendInstructionCommand { get; set; }//发送指令
        private void SendInstructionCommandExecute()
        {
            try
            {
                if ((int.Parse(ChanelID) > 0 ) == false)
                {
                    MessageBox.Show("通道ID应为大于0的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("通道ID应为大于0的正整数！");
                return;
            }
            try
            {
                if ((int.Parse(ShotCommand) >= 0) == false)
                {
                    MessageBox.Show("拍摄命令应为大于等于0的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("拍摄命令应为大于等于0的正整数！");
                return;
            }
            try
            {
                if ((int.Parse(ShotTime) >= 0) == false)
                {
                    MessageBox.Show("拍照间隔/录像时间应为大于等于0的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("拍照间隔/录像时间应为大于等于0的正整数！");
                return;
            }
            try
            {
                if ((int.Parse(PictureQuality) >= 1 && int.Parse(PictureQuality) <= 10) == false)
                {
                    MessageBox.Show("图像/视频质量应为1到10的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("图像/视频质量应为1到10的正整数！");
                return;
            }
            try
            {
                if ((int.Parse(Brightness) >= 0 && int.Parse(Brightness) <= 255) == false)
                {
                    MessageBox.Show("亮度应为0到255的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("亮度应为0到255的正整数！");
                return;
            }
            try
            {
                if ((int.Parse(Contrast) >= 0 && int.Parse(Contrast) <= 127) == false)
                {
                    MessageBox.Show("对比度应为0到127的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("对比度应为0到127的正整数！");
                return;
            }
            try
            {
                if ((int.Parse(Saturation) >= 0 && int.Parse(Saturation) <= 127) == false)
                {
                    MessageBox.Show("饱和度应为0到127的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("饱和度应为0到127的正整数！");
                return;
            }
            try
            {
                if ((int.Parse(Chroma) >= 0 && int.Parse(Chroma) <= 255) == false)
                {
                    MessageBox.Show("色度应为0到255的正整数！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("色度应为0到255的正整数！");
                return;
            }
            Result=Text();
            if (Result == "指令已发出，正在处理！")
            {
                States = "已发送";
                Socket.ExcuteSql("摄像头立即拍摄命令", StaticLoginInfo.GetInstance().UserName, ChanelID, Result, VBaseInfo.GetInstance().SIM);
                CommandInfo cmd = new CommandInfo();
                cmd.cmdId = SIM + "_CAMERASHOOTCMD_TYPE";
                cmd.cmdContent = "摄像头立即拍摄命令" + ":" + ChanelID;
                cmd.SendStatus = Result.ToString();
                cmd.cmdSim = SIM.ToString();
                cmd.VehicleNum = VehicleId.ToString();
                cmd.cmdTime = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
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

        #region 绑定指令信息
        private string chanelID;

        public string ChanelID
        {
            get { return chanelID; }
            set
            {
                chanelID = value;
                this.RaisePropertyChanged("ChanelID");
            }
        }
        private string shotCommand;

        public string ShotCommand
        {
            get { return shotCommand; }
            set
            {
                shotCommand = value;
                this.RaisePropertyChanged("ShotCommand");
            }
        }
        private string shotTime;

        public string ShotTime
        {
            get { return shotTime; }
            set
            {
                shotTime = value;
                this.RaisePropertyChanged("ShotTime");
            }
        }
        private bool isKeep=true;

        public bool IsKeep
        {
            get { return isKeep; }
            set
            {
                isKeep = value;
                this.RaisePropertyChanged("IsKeep");
                if (isKeep==true)
                {
                    saveSignFlag = 1;
                }
            }
        }
        private bool isRealTimeUpload;

        public bool IsRealTimeUpload
        {
            get { return isRealTimeUpload; }
            set
            {
                isRealTimeUpload = value;
                this.RaisePropertyChanged("IsRealTimeUpload");
                if (isRealTimeUpload==true)
                {
                    saveSignFlag = 0;
                }
            }
        }
        private List<string> resolution = new List<string>() { "320*240", "640*480", "800*600", "1024*768", "176*144", "352*288", "704*288", "704*576" };
        public List<string> Resolution
        {
            get { return resolution; }
            set
            {
                resolution = value;
                this.RaisePropertyChanged("Resolution");
            }
        }
        private int resolutionSelectedIndex;

        public int ResolutionSelectedIndex
        {
            get { return resolutionSelectedIndex; }
            set
            {
                resolutionSelectedIndex = value;
                this.RaisePropertyChanged("ResolutionSelectedIndex");
            }
        }
        private string pictureQuality;

        public string PictureQuality
        {
            get { return pictureQuality; }
            set
            {
                pictureQuality = value;
                this.RaisePropertyChanged("PictureQuality");
            }
        }
        private string brightness;

        public string Brightness
        {
            get { return brightness; }
            set
            {
                brightness = value;
                this.RaisePropertyChanged("Brightness");
            }
        }
        private string contrast;

        public string Contrast
        {
            get { return contrast; }
            set
            {
                contrast = value;
                this.RaisePropertyChanged("Contrast");
            }
        }
        private string saturation;

        public string Saturation
        {
            get { return saturation; }
            set
            {
                saturation = value;
                this.RaisePropertyChanged("Saturation");
            }
        }
        private string chroma;

        public string Chroma
        {
            get { return chroma; }
            set
            {
                chroma = value;
                this.RaisePropertyChanged("Chroma");
            }
        }
        #endregion

        #region 绑定车辆信息

        private void InitVehicleInfo()
        {
            this.VehicleId = VBaseInfo.GetInstance().VehicleId;
            this.SIM = VBaseInfo.GetInstance().SIM;
            this.EUSERNAME = VBaseInfo.GetInstance().EUSERNAME;
            this.States = "未发送";
        }

        private string vehicleid;
        public string VehicleId
        {
            get { return vehicleid; }
            set
            {
                vehicleid = value;
                this.RaisePropertyChanged("VehicleId");
            }
        }

        private string sim;
        public string SIM
        {
            get { return sim; }
            set
            {
                sim = value;
                this.RaisePropertyChanged("SIM");
            }
        }

        private string eusername;
        public string EUSERNAME
        {
            get { return eusername; }
            set
            {
                eusername = value;
                this.RaisePropertyChanged("EUSERNAME");
            }
        }

        private string states;
        public string States
        {
            get { return states; }
            set
            {
                states = value;
                this.RaisePropertyChanged("States");
            }
        }

        private string result;
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                this.RaisePropertyChanged("Result");
            }
        }

        #endregion

        private string Text()
        {
            string rebool = "";
            JObject jo = new JObject();
            jo.Add("cmd", "CAMERASHOOTCMD_TYPE");
            jo.Add("simId", SIM);
            jo.Add("pipeId", ChanelID);
            jo.Add("cameracmd", ShotCommand);
            jo.Add("interval",ShotTime );
            jo.Add("saveSign", saveSignFlag);
            jo.Add("resolution",ResolutionSelectedIndex+1);
            jo.Add("quality",PictureQuality );
            jo.Add("brightness",Brightness );
            jo.Add("contrast",Contrast);
            jo.Add("saturation",Saturation);
            jo.Add("chromaticity",Chroma);
            jo.Add("cmdid", SIM + "_CAMERASHOOTCMD_TYPE");
            rebool = Socket.zmqInstructionsPack(SIM, jo);
            return rebool;
        }
    }
}
