using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using System.Windows.Threading;
using System.Windows;
using VehicleGPS.Views;
using VehicleGPS.Views.Warn;
using System.Threading;
using System.IO;
using System.Xml;
using VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch;
using System.Windows.Documents;

namespace VehicleGPS.ViewModels.Login
{
    class LoginDataLoadViewModel : NotificationObject
    {
        private Window win;
        public LoginDataLoadViewModel(Window win)
        {
            this.win = win;
            this.DataLoadTip = "数据正在加载...";
            //this.ClientLogin();
            this.LoadRight();
            /////this.LoadBasicTypeInfo();
            this.LoadClientBasicInfo();//加载单位数据
            // this.LoadVehicleBasicInfo();
            this.LoadVehicleDetailInfo();//加载车辆数据
            this.LoadRegionInfo();//加载区域信息
            this.LoadGpsInfo();//加载车辆的GPS状态
            this.LoadWarnSetting();//加载报警设置信息
            this.LoadInstructionRight();//加载指令权限信息
            this.InitDispatcherTimer();
        }
        //加载报警设置信息 
        void LoadWarnSetting()
        {
            this.DataLoadTip = "正在加载报警类型...";
            if (StaticTreeState.WarnSettinInfo != LoadingState.LOADCOMPLETE)
            {
                Thread thread = new Thread(LoadWarnSettingInfo);
                thread.Start();
            }
        }
        void LoadWarnSettingInfo()
        {
            VehicleConfig.GetInstance();
            string path = VehicleConfig.GetInstance().warnSettingPath;
            if (!File.Exists(path))
            {
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            List<AlarmSettingInfo> warnsetinfolist = new List<AlarmSettingInfo>();
            XmlNode xn = xmlDoc.SelectSingleNode("Setting");
            // 得到根节点的所有子节点
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode node in xnl)
            {
                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)node;
                if (xe.GetAttribute("userName").ToString() == "Default")
                {
                    //得到默认设置的所有子节点
                    XmlNodeList xnlDefault = xe.ChildNodes;
                    foreach (XmlNode nodeDefualt in xnlDefault)
                    {
                        if (nodeDefualt.Name == "voicePath")
                        {
                            VehicleConfig.GetInstance().warnSoundPath = nodeDefualt.InnerText;
                            continue;
                        }
                        AlarmSettingInfo info = new AlarmSettingInfo();
                        info.WarnName = nodeDefualt.Name;
                        info.WarnID = nodeDefualt.Attributes["id"].Value;
                        info.IsOpen = (nodeDefualt.InnerText == "0" ? false : true);
                        warnsetinfolist.Add(info);
                    }
                    break;
                }
            }
            /*报警过滤字符串*/
            List<AlarmSettingInfo> tmp = new List<AlarmSettingInfo>();
            foreach (AlarmSettingInfo info in warnsetinfolist)
            {
                if (info.IsOpen == true)
                {
                    tmp.Add(info);
                }
            }
            StaticWarnInfo.warnsetlist = new List<AlarmSettingInfo>();
            StaticWarnInfo.warnsetlist = tmp;
            StaticTreeState.WarnSettinInfo = LoadingState.LOADCOMPLETE;
        }

        private DispatcherTimer dispatcherTimer;
        private bool bRight;//是否已经提示过
        private bool bBasicTypeTip;//是否已经提示过
        private bool bClientBasicTip;//是否已经提示过
        private bool bVehicleBasicTip;//是否已经提示过
        private bool bVehicleDetailTip;//是否已经提示过
        private bool bRegionTip;//是否已经提示过
        private bool bGpsInfo;//是否已经提示过
        private void InitDispatcherTimer()
        {
            this.bRight = false;
            this.bBasicTypeTip = true;
            this.bClientBasicTip = false;
            this.bVehicleBasicTip = true;
            this.bVehicleDetailTip = false;
            this.bRegionTip = true;
            this.bGpsInfo = false;
            count = 0;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
        }
        private int count = 0;

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE && this.bRight == false)
            {
                this.DataLoadTip = "权限数据加载完成！";
                this.bRight = true;
            }
            if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE && this.bClientBasicTip == false)
            {
                this.DataLoadTip = "用户基础数据加载完成！";
                this.bClientBasicTip = true;
            }
            if (StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE && this.bVehicleDetailTip == false)
            {
                this.DataLoadTip = "车辆详细数据加载完成！";
                this.bVehicleDetailTip = true;
            }
            if (StaticTreeState.VehicleGPSInfo == LoadingState.NOLOADING)
            {
                
                if (count > 20)
                {
                    StaticTreeState.VehicleGPSInfo = LoadingState.LOADDINGFAIL;
                }

            }
            count++;
            if (StaticTreeState.VehicleGPSInfo == LoadingState.LOADCOMPLETE && this.bGpsInfo == false)
            {
                this.DataLoadTip = "最新GPS数据加载完成！";
                this.bGpsInfo = true;
            }
            if (StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE && this.bVehicleDetailTip == true
                && StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE && this.bVehicleDetailTip == true
                && StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE && this.bClientBasicTip == true
                && StaticTreeState.VehicleAllBasicInfo == LoadingState.LOADCOMPLETE && this.bVehicleDetailTip == true
                && (count > 20 || StaticTreeState.VehicleGPSInfo == LoadingState.LOADCOMPLETE && bGpsInfo == true)
                && StaticTreeState.RigthInfo == LoadingState.LOADCOMPLETE)
            {
                dispatcherTimer.Stop();
                this.DataLoadTip = "所有数据加载成功！";
                this.dispatcherTimer.Stop();
                MainView mainView = new MainView();
                this.win.Close();
                mainView.Show();
            }
        }

        private void ClientLogin()
        {
            StaticDetailInfo cvDetailInfo = StaticDetailInfo.GetInstance();
            cvDetailInfo.ClientLogin();
        }
        /*获取权限*/
        private void LoadRight()
        {
            this.DataLoadTip = "正在加载权限数据...";
            if (StaticTreeState.RigthInfo != LoadingState.LOADCOMPLETE)
            {
                StaticRight right = StaticRight.GetInstance();
                right.RefreshRightInfo();
            }
        }
        //获取指令权限
        private void LoadInstructionRight()
        {
            this.DataLoadTip = "正在加载指令权限...";
            if (StaticTreeState.InstructionInfo != LoadingState.LOADCOMPLETE)
            {
                InstructionRight insright = InstructionRight.GetInstance();
                insright.RefreshInstructionRightInfo();
            }
        }
        /*获取基本类型数据*/
        private void LoadBasicTypeInfo()
        {
            //this.AddTip("正在加载基本类型数据...");
            this.DataLoadTip = "正在加载基本类型数据...";
            if (StaticTreeState.BasicTypeInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicType basicType = StaticBasicType.GetInstance();
                basicType.RefreshBasicInfo();
            }
        }
        /*加载用户基础数据*/
        private void LoadClientBasicInfo()
        {
            //this.AddTip("正在加载用户基础数据...");
            this.DataLoadTip = "正在加载用户基础数据...";
            if (StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshClientBasicInfo();
            }
        }
        /*加载车辆基础数据*/
        private void LoadVehicleBasicInfo()
        {
            //this.AddTip("正在加载车辆基础数据...");
            this.DataLoadTip = "正在加载车辆基础数据...";
            if (StaticTreeState.VehicleBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshVehicleBasicInfo();
            }
        }
        /*加载车辆详细数据*/
        private void LoadVehicleDetailInfo()
        {
            //this.AddTip("正在加载车辆详细数据...");
            this.DataLoadTip = "正在加载车辆详细数据...";
            if (StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshVehicleDetailInfo();
            }
        }
        /*获取区域数据*/
        private void LoadRegionInfo()
        {
            //this.AddTip("正在加载基本类型数据...");
            this.DataLoadTip = "正在加载区域数据...";
            if (StaticTreeState.RegionBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticRegionInfo regionInfo = StaticRegionInfo.GetInstance();
                regionInfo.RefreshRegionInfo();
            }
        }

        private void LoadGpsInfo()
        {
            this.DataLoadTip = "正在加载最新GPS数据...";
            if (StaticTreeState.VehicleGPSInfo != LoadingState.LOADCOMPLETE)
            {
                StaticDetailInfo cvDetailInfo = StaticDetailInfo.GetInstance();
                cvDetailInfo.RefreshGPSInfo();
            }
        }
        private string dataLoadTip;
        public string DataLoadTip
        {
            get { return dataLoadTip; }
            set
            {
                if (dataLoadTip != value)
                {
                    dataLoadTip = value;
                    this.RaisePropertyChanged("DataLoadTip");
                }
            }
        }

    }
}
