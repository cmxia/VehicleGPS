using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using System.Threading;
using VehicleGPS.ViewModels.MonitorCentre.ImageCheck;
using System.Windows;
using System.IO;
using System.Drawing;

namespace VehicleGPS.Services.MonitorCentre.ImageCheck
{
    class ImageCheckOperate
    {
        /// <summary>
        /// 将十六进制数据字符串转换成图片字节数组
        /// </summary>
        /// <param name="s">图片数据字符串</param>
        /// <returns></returns>
        public static byte[] StringToHex(string s)
        {
            s = s.Replace(" ", "");
            if ((s.Length % 2) != 0)
            {
                s += "";
            }
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename">图片名称 </param>
        /// <returns>图片路径</returns>
        public static string SaveImage(byte[] data, string filename)
        {

            System.Drawing.Image image = ReturnPhoto(data);
            if (image == null)
            {
                return null;
            }
            if (!Directory.Exists(VehicleConfig.GetInstance().cachePath))
            {
                Directory.CreateDirectory(VehicleConfig.GetInstance().cachePath);
            }
            string filePath = VehicleConfig.GetInstance().cachePath + "/" + filename + ".png";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            image.Save(filePath);
            return filePath;
        }


        private byte[] GetPictureData(string imagepath)
        {
            /**/
            ////根据图片文件的路径使用文件流打开，并保存为byte[] 
            FileStream fs = new FileStream(imagepath, FileMode.Open);//可以是其他重载方法 
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            fs.Close();
            return byData;
        }

        private static System.Drawing.Image ReturnPhoto(byte[] streamByte)
        {
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream(streamByte);
                System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                return img;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ReadTree()
        {
            RefreshTree();
        }
        private void RefreshTree()
        {
            if (StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE
                   || StaticTreeState.VehicleAllBasicInfo != LoadingState.LOADCOMPLETE)
            {
                StaticBasicInfo cvBasicInfo = StaticBasicInfo.GetInstance();
                cvBasicInfo.RefreshBasicInfo();
            }
            /*启动线程初始化树形*/
            Thread initTreeThread = new Thread(new ThreadStart(this.InitClientVehicleTree));
            initTreeThread.Start();
        }

        private void InitClientVehicleTree()
        {
            /*创建根节点*/
            ImageCheckTreeNodeViewModel rootnode = new ImageCheckTreeNodeViewModel();
            rootnode.NodeInfo = new CVBasicInfo();
            rootnode.NodeInfo.ID = "admin";
            rootnode.ListChildNodes = new List<ImageCheckTreeNodeViewModel>();

            /*创建用户或车辆节点*/
            List<ImageCheckTreeNodeViewModel> clientNodeList = new List<ImageCheckTreeNodeViewModel>();
            List<ImageCheckTreeNodeViewModel> vehicleNodeList = new List<ImageCheckTreeNodeViewModel>();

            /*从静态数据中获取车辆或用户信息*/
            StaticBasicInfo basicInfo = StaticBasicInfo.GetInstance();


            bool loadSucess = false;
            int loadTimesCounter = 1;
            int loadTiems = 15;
            while (Monitor.TryEnter(StaticTreeState.ClientBasicMutex, 10000))
            {//等待10秒
                if (StaticTreeState.ClientBasicInfo != LoadingState.LOADCOMPLETE)
                {//加载用户失败
                    Monitor.Exit(StaticTreeState.ClientBasicMutex);
                    Thread.Sleep(200);
                    if (loadTimesCounter++ == loadTiems)//尝试15次
                    {
                        break;
                    }
                    continue;
                }
                if (StaticTreeState.ClientBasicInfo == LoadingState.LOADCOMPLETE)
                {
                    /*获取用户节点*/
                    foreach (CVBasicInfo cbi in basicInfo.ListClientBasicInfo)
                    {
                        ImageCheckTreeNodeViewModel clientNode = new ImageCheckTreeNodeViewModel();
                        InitClientNode(clientNode, cbi);
                        clientNodeList.Add(clientNode);
                    }
                    int rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
                    while (UnUseCount(clientNodeList) != 0)
                    {
                        foreach (ImageCheckTreeNodeViewModel vtnv in clientNodeList)
                        {
                            if (vtnv.isUsed == false)
                            {
                                if (AddClientTree(rootnode, vtnv))
                                {
                                    vtnv.isUsed = true;
                                }
                            }
                        }
                        if (++rollTimes == 5)//最多轮询五次
                        {
                            break;
                        }
                    }
                    loadSucess = true;
                }
                Monitor.Exit(StaticTreeState.ClientBasicMutex);
                break;
            }
            if (loadSucess == false)
            {//等待10秒超时或者加载用户信息失败
                StaticTreeState.ClientBasicInfo = LoadingState.LOADDINGFAIL;
                ImageCheckTreeViewModel.GetInstance().LoadTreeFail();
                return;
            }

            ImageCheckTreeViewModel.GetInstance().RootNode = rootnode;
            /*****************车辆信息**************/
            int _rollTimes = 0;//轮询次数，防止脏数据导致陷入死循环
            foreach (CVBasicInfo vbi in basicInfo.ListVehicleBasicInfo)
            {
                ImageCheckTreeNodeViewModel vehicleNode = new ImageCheckTreeNodeViewModel();
                InitVehicleNode(vehicleNode, vbi);
                vehicleNodeList.Add(vehicleNode);
            }
            while (UnUseCount(vehicleNodeList) != 0)
            {
                foreach (ImageCheckTreeNodeViewModel vtnv in vehicleNodeList)
                {
                    if (vtnv.isUsed == false)
                    {
                        if (AddVehicleTree(rootnode, vtnv))
                        {
                            vtnv.isUsed = true;
                        }
                    }
                }
                if (++_rollTimes == 1)//最多轮询1次
                {
                    break;
                }
            }
            //如果没找到父节点，全部放到根节点下一节点（我的商砼）目录下
            if (UnUseCount(vehicleNodeList) != 0)
            {
                foreach (ImageCheckTreeNodeViewModel vtnv in vehicleNodeList)
                {
                    if (vtnv.isUsed == false)
                    {
                        vtnv.ParentNode = rootnode.ListChildNodes[0];//将其父节点设为（我的商砼）
                        vtnv.nodeInfo.ParentID = rootnode.ListChildNodes[0].nodeInfo.ID;
                        if (AddVehicleTree(rootnode.ListChildNodes[0], vtnv))
                        {
                            vtnv.isUsed = true;
                        }
                    }
                }
            }
            ImageCheckTreeViewModel.GetInstance().RootNode = rootnode;
            ImageCheckTreeViewModel.GetInstance().InitAutoComplete();/*初始化AutoComplete查询数据集*/
            rootnode.ListChildNodes[0].isExpand = true;
        }

        /*添加车辆节点*/
        private bool AddVehicleTree(ImageCheckTreeNodeViewModel node, ImageCheckTreeNodeViewModel t_node)
        {
            if (node.NodeInfo.ID == t_node.NodeInfo.ParentID)
            {
                if (node.ListChildNodes == null)
                {
                    node.ListChildNodes = new List<ImageCheckTreeNodeViewModel>();
                }
                t_node.ParentNode = node;
                node.ListChildNodes.Add(t_node);
                return true;
            }
            else
            {
                if (node.ListChildNodes != null)
                {
                    for (int i = 0; i < node.ListChildNodes.Count; i++)
                    {
                        ImageCheckTreeNodeViewModel vtnv = node.ListChildNodes[i];
                        if (AddVehicleTree(vtnv, t_node))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /*初始化车辆节点*/
        private void InitVehicleNode(ImageCheckTreeNodeViewModel vehicleNode, CVBasicInfo vbi)
        {
            vehicleNode.nodeInfo = vbi;
            vehicleNode.isSelected = false;
            vehicleNode.isUsed = false;
            vehicleNode.ParentNode = null;
            vehicleNode.ListChildNodes = null;
            vehicleNode.onlineNumberVisible = Visibility.Collapsed;
            vehicleNode.innerIDVisible = Visibility.Visible;
            vehicleNode.nameVisible = Visibility.Visible;
            vehicleNode.imageUrl = "pack://application:,,,/Images/Car/vehicle_unknow_64.png";
            vehicleNode.ImageTip = "未知";
        }
        private bool AddClientTree(ImageCheckTreeNodeViewModel node, ImageCheckTreeNodeViewModel t_node)
        {
            if (node.NodeInfo.ID == t_node.NodeInfo.ParentID)
            {
                if (node.ListChildNodes == null)
                {
                    node.ListChildNodes = new List<ImageCheckTreeNodeViewModel>();
                }
                t_node.ParentNode = node;
                node.ListChildNodes.Add(t_node);
                return true;
            }
            else
            {
                if (node.ListChildNodes != null)
                {
                    for (int i = 0; i < node.ListChildNodes.Count; i++)
                    {
                        ImageCheckTreeNodeViewModel vtnv = node.ListChildNodes[i];
                        if (AddClientTree(vtnv, t_node))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /*初始化单位、车队、经营商、站点节点*/
        private void InitClientNode(ImageCheckTreeNodeViewModel clientNode, CVBasicInfo cbi)
        {
            clientNode.nodeInfo = cbi;
            clientNode.isSelected = false;
            clientNode.isUsed = false;
            clientNode.ParentNode = null;
            clientNode.ListChildNodes = null;
            clientNode.onlineNumberVisible = Visibility.Visible;
            clientNode.innerIDVisible = Visibility.Collapsed;
            clientNode.nameVisible = Visibility.Visible;
        }
        #region 获取未使用的节点
        /*获取未使用的节点*/
        private int UnUseCount(List<ImageCheckTreeNodeViewModel> nodeList)
        {
            int count = 0;
            foreach (ImageCheckTreeNodeViewModel vtnv in nodeList)
            {
                if (vtnv.isUsed == false)
                {
                    count++;
                }
            }
            return count;
        }
        #endregion
    }
}
