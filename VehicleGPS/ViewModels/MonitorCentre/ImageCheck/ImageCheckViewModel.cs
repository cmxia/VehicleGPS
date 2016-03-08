using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows;
using VehicleGPS.Services.MonitorCentre.ImageCheck;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls;
using VehicleGPS.Models.MonitorCentre;
using VehicleGPS.Models;
using System.Data;
using VehicleGPS.Models.Login;
using VehicleGPS.Services;
using System.ComponentModel;
using System.Threading;

namespace VehicleGPS.ViewModels.MonitorCentre.ImageCheck
{
    class ImageCheckViewModel : NotificationObject
    {

        public ImageCheckViewModel(WebBrowser webbrowser)
        {
            this.QueryCommand = new DelegateCommand(new Action(QueryCommandExecute));
            this.ComeFirstCommand = new DelegateCommand(new Action(ComeFirstCommandExecute));
            this.ComePrevCommand = new DelegateCommand(new Action(ComePrevCommandExecute));
            this.ComeNextCommand = new DelegateCommand(new Action(ComeNextCommandExecute));
            this.ComeLastCommand = new DelegateCommand(new Action(ComeLastCommandExecute));

            this.DoubleClickCommand = new DelegateCommand(new Action(DoubleClickCommandExe));

            this.WebMap = webbrowser;
            WebSite = new ImageCheckMapService(WebMap);
            this.FirstEnable = true;
            this.PrevEnable = true;
            this.InitUnit();
            this.InitVehicle();
        }

        private void InitUnit()
        {
            if (this.UnitList == null)
            {
                this.UnitList = new List<CVBasicInfo>();
            }
            this.UnitList.Clear();
            List<CVBasicInfo> tmp = new List<CVBasicInfo>();

            foreach (CVBasicInfo unit in StaticBasicInfo.GetInstance().ListClientBasicInfo)
            {
                tmp.Add(unit);
            }
            this.UnitList = tmp;
            if (this.UnitList.Count > 0)
            {
                this.UnitSelectedIndex = 0;
            }

        }

        private void InitVehicle()
        {

            if (this.UnitList == null || this.UnitList.Count == 0)
            {
                return;
            }
            //string sql="select distinct simId from "

            if (this.VehicleList == null)
            {
                this.VehicleList = new List<CVDetailInfo>();
            }
            List<CVDetailInfo> tmplist = new List<CVDetailInfo>();
            this.VehicleList.Clear();
            string unitId = this.UnitList[this.UnitSelectedIndex].ID;
            foreach (CVDetailInfo vehicle in StaticDetailInfo.GetInstance().ListVehicleDetailInfo)
            {
                if (vehicle.ParentUnitId.Equals(unitId))
                {
                    tmplist.Add(vehicle);
                }
            }
            this.VehicleList = tmplist;
            if (this.VehicleList.Count > 0)
            {
                this.VehicleSelectedIndex = 0;
            }

        }

        #region 百度地图
        public WebBrowser WebMap { get; set; }
        public ImageCheckMapService WebSite { get; set; }
        #endregion

        #region 分页
        public DelegateCommand ComeFirstCommand { get; set; }// 首页
        public DelegateCommand ComePrevCommand { get; set; }// 上一页
        public DelegateCommand ComeNextCommand { get; set; }// 下一页
        public DelegateCommand ComeLastCommand { get; set; }// 末页
        // 首页 执行函数
        public void ComeFirstCommandExecute()
        {
            this.TotalCount = 100;
            this.CurrentStart = 1;
            this.CurrentEnd = 10;
            this.TotalPage = 10;
            this.FirstEnable = false;
            this.NextEnable = true;
        }
        // 上一页 执行函数
        public void ComePrevCommandExecute()
        {
            this.NextEnable = true;
            this.LastEnable = true;
        }
        // 下一页 执行函数
        public void ComeNextCommandExecute()
        {
            this.NextEnable = true;
            this.LastEnable = true;
        }
        // 末页 执行函数
        public void ComeLastCommandExecute()
        {
            this.LastEnable = false;
            this.PrevEnable = true;
            this.FirstEnable = true;
        }
        private bool firstenable;//首页是否可操作

        public bool FirstEnable
        {
            get { return firstenable; }
            set
            {
                firstenable = value;
                this.RaisePropertyChanged("FirstEnable");
            }
        }
        private bool preEnable;//上一页是否可操作

        public bool PrevEnable
        {
            get { return preEnable; }
            set
            {
                preEnable = value;
                this.RaisePropertyChanged("PrevEnable");
            }
        }
        private bool nextenable;//下一页是否可操作

        public bool NextEnable
        {
            get { return nextenable; }
            set
            {
                nextenable = value;
                this.RaisePropertyChanged("NextEnable");
            }
        }
        private bool lastenable;//最后一页是否可操作

        public bool LastEnable
        {
            get { return lastenable; }
            set
            {
                lastenable = value;
                this.RaisePropertyChanged("LastEnable");
            }
        }

        private int currentpage;//当前页 页码

        public int CurrentPage
        {
            get { return currentpage; }
            set
            {
                currentpage = value;
                this.RaisePropertyChanged("CurrentPage");
            }
        }
        private int totalpage;//总页数

        public int TotalPage
        {
            get { return totalpage; }
            set
            {
                totalpage = value;
                this.RaisePropertyChanged("TotalPage");
            }
        }
        private int currentstart;//当前页的开始条数索引

        public int CurrentStart
        {
            get { return currentstart; }
            set
            {
                currentstart = value;
                this.RaisePropertyChanged("CurrentStart");
            }
        }
        private int currentend;//当前页的最后一条的索引

        public int CurrentEnd
        {
            get { return currentend; }
            set
            {
                currentend = value;
                this.RaisePropertyChanged("CurrentEnd");
            }
        }
        private int totalcount;//总条数

        public int TotalCount
        {
            get { return totalcount; }
            set
            {
                totalcount = value;
                this.RaisePropertyChanged("TotalCount");
            }
        }

        #endregion

        #region 查询条件
        private bool isBusy = false;//忙等待
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    this.RaisePropertyChanged("IsBusy");
                }
            }
        }
        private DateTime begintime = DateTime.Now.AddDays(-7);  //开始时间
        public DateTime BeginTime
        {
            get { return begintime; }
            set
            {
                begintime = value;
                this.RaisePropertyChanged("BeginTime");
            }
        }

        private DateTime endtime = DateTime.Now;  //结束时间
        public DateTime EndTime
        {
            get { return endtime; }
            set
            {
                endtime = value;
                this.RaisePropertyChanged("EndTime");
            }
        }

        private bool byunitchecked;//按单位
        public bool ByUnitChecked
        {
            get { return byunitchecked; }
            set
            {
                byunitchecked = value;
                this.RaisePropertyChanged("ByUnitChecked");
            }
        }

        private bool byvehiclechecked;//按车辆
        public bool ByVehicleChecked
        {
            get { return byvehiclechecked; }
            set
            {
                byvehiclechecked = value;
                this.RaisePropertyChanged("ByVehicleChecked");
            }
        }

        private List<CVBasicInfo> unitlist;//单位列表
        public List<CVBasicInfo> UnitList
        {
            get { return unitlist; }
            set
            {
                unitlist = value;
                this.RaisePropertyChanged("UnitList");
            }
        }

        private int unitselectedindex = 0;
        public int UnitSelectedIndex
        {
            get { return unitselectedindex; }
            set
            {
                unitselectedindex = value;
                InitVehicle();
                this.RaisePropertyChanged("UnitSelectedIndex");
            }
        }

        private List<CVDetailInfo> vehiclelist;//车辆列表
        public List<CVDetailInfo> VehicleList
        {
            get { return vehiclelist; }
            set
            {
                vehiclelist = value;
                this.RaisePropertyChanged("VehicleList");
            }
        }

        private int vehicleselectedindex = 0;
        public int VehicleSelectedIndex
        {
            get { return vehicleselectedindex; }
            set
            {
                vehicleselectedindex = value;
                this.RaisePropertyChanged("VehicleSelectedIndex");
            }
        }

        #endregion

        #region 绑定数据

        //所有的图片信息列表
        private List<ImageInfo> imageinfolistall;

        public List<ImageInfo> ImageInfoListAll
        {
            get { return imageinfolistall; }
            set
            {
                imageinfolistall = value;
                Thread th = new Thread(ImageDeserialize);
                th.Start();
                this.RaisePropertyChanged("ImageInfoListAll");
            }
        }
        private void ImageDeserialize()
        {
            foreach (ImageInfo imageinfo in this.ImageInfoListAll)
            {
                imageinfo.imageUrl = ImageCheckOperate.SaveImage(ImageCheckOperate.StringToHex(imageinfo.imgData), IDHelper.GetImageName(imageinfo.SIM));
            }
        }
        private ImageInfo selectedimage;

        public ImageInfo SelectedImage
        {
            get { return selectedimage; }
            set
            {
                selectedimage = value;
                this.RaisePropertyChanged("SelectedImage");
            }
        }


        private List<ImageInfo> imageinfopage;//一页的图片信息
        public List<ImageInfo> ImageInfoPage
        {
            get { return imageinfopage; }
            set
            {
                imageinfopage = value;
                this.RaisePropertyChanged("ImageInfoPage");
            }
        }
        private int imageselectedindex;//图片的点击索引值
        public int ImageSelectedInde
        {
            get { return imageselectedindex; }
            set
            {
                imageselectedindex = value;
                this.RaisePropertyChanged("ImageSelectedInde");
            }
        }

        private Visibility mapvisible;

        public Visibility mapVisible
        {
            get { return mapvisible; }
            set
            {
                mapvisible = value;
                this.RaisePropertyChanged("mapVisible");
            }
        }


        //大图的路径
        private string imagedetailurl;

        public string ImageDetailUrl
        {
            get { return imagedetailurl; }
            set
            {
                imagedetailurl = value;
                this.RaisePropertyChanged("ImageDetailUrl");
            }
        }

        #endregion

        #region 绑定操作
        //双击
        public DelegateCommand DoubleClickCommand { get; set; }
        private void DoubleClickCommandExe()
        {
            try
            {
                this.ImageDetailUrl = this.SelectedImage.imageUrl;
                string addr = (new BusinessDataServiceWEB(1)).ParseOneAddress(this.SelectedImage.lng, this.SelectedImage.lat);
                this.WebSite.removeAllMacker();
                this.WebSite.ShowInMap(SelectedImage.lng, SelectedImage.lat, addr, selectedimage.VehicleId, selectedimage.SIM, selectedimage.recordtime);
            }
            catch (Exception)
            {
                throw;
            }
        }


        //查询命令
        public DelegateCommand QueryCommand { get; set; }
        private void GetResult()
        {
            if (IsQueryRight())
            {
                //初始化图片列表
                if (ImageInfoListAll == null)
                {
                    ImageInfoListAll = new List<ImageInfo>();
                }
                ImageInfoListAll.Clear();
                List<ImageInfo> tmpList = null;
                string simCondition = null;
                if (this.ByVehicleChecked)
                {
                    simCondition = "simId = '" + this.VehicleList[this.VehicleSelectedIndex].SIM + "' ";
                }
                else
                {
                    if (this.ByUnitChecked)
                    {
                        simCondition = "simId in (";
                        foreach (CVDetailInfo vehicle in this.VehicleList)
                        {
                            simCondition += "'" + vehicle.SIM + "',";
                        }
                        simCondition += "'12') ";
                    }
                }
                string sql = "select simId,longitude,latitude,data,Recordtime,Inserttime from GpsMedia where Recordtime > '"
                    + this.BeginTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and Recordtime < '" + this.EndTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and " + simCondition
                    + " order by simId,Recordtime desc";
                string jsonStr = VehicleCommon.wcfDBHelper.YExecuteReportSql(StaticLoginInfo.GetInstance().UserName, this.BeginTime, this.EndTime, sql, "0");
                DataTable dt = JsonHelper.JsonToDataTable(jsonStr);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ImageInfo imageinfo = null;
                    string recordtime = "";
                    int sequence = 1;
                    tmpList = new List<ImageInfo>();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (string.Equals(recordtime, row["Recordtime"].ToString()))
                        {
                            continue;
                        }
                        imageinfo = new ImageInfo();
                        if (this.ByVehicleChecked)
                        {
                            imageinfo.VehicleId = this.VehicleList[this.VehicleSelectedIndex].VehicleId;
                        }
                        else
                        {
                            foreach (CVDetailInfo item in this.VehicleList)
                            {
                                try
                                {
                                    if (string.Compare(item.SIM, row["simId"].ToString()) == 0)
                                    {
                                        imageinfo.VehicleId = item.VehicleId;
                                        break;
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            }
                        }
                        imageinfo.Sequence = sequence++;
                        imageinfo.inserttime = row["Inserttime"].ToString();
                        imageinfo.recordtime = row["Recordtime"].ToString();
                        imageinfo.SIM = row["simId"].ToString();
                        imageinfo.lat = row["latitude"].ToString();
                        imageinfo.lng = row["longitude"].ToString();
                        if (double.Parse(imageinfo.lng) < 0)
                        {
                            imageinfo.lng = (-1 * double.Parse(imageinfo.lng)).ToString();
                        }
                        if (double.Parse(imageinfo.lat) < 0)
                        {
                            imageinfo.lat = (-1 * double.Parse(imageinfo.lat)).ToString();
                        }
                        imageinfo.imgData = row["data"].ToString();
                        imageinfo.imageName = IDHelper.GetImageName(imageinfo.SIM);
                        tmpList.Add(imageinfo);
                        recordtime = imageinfo.recordtime;
                    }
                    this.ImageInfoListAll = tmpList;
                }
            }
        }
        public void QueryCommandExecute()
        {
            this.IsBusy = true;
            this.mapVisible = Visibility.Hidden;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                GetResult();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.IsBusy = false;
                this.mapVisible = Visibility.Visible;
            };
            worker.RunWorkerAsync();

        }
        private bool IsQueryRight()
        {
            if (this.BeginTime == null)
            {
                MessageBox.Show("请选择开始时间！");
                return false;
            }
            if (this.EndTime == null)
            {
                MessageBox.Show("请选择结束时间！");
                return false;
            }
            DateTime begintime = BeginTime;
            DateTime endtime = EndTime;
            if (DateTime.Compare(begintime, endtime) > 0)
            {
                MessageBox.Show("开始时间不能大于结束时间！");
                return false;
            }
            return true;
        }
        #endregion
    }
}
