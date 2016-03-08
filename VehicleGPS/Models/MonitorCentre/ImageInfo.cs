using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.Models.MonitorCentre
{
    class ImageInfo : NotificationObject
    {
        public string lng { get; set; }//经度
        public string lat { get; set; }//纬度
        public string address { get; set; }//地址
        //图片路径
        private string imageurl;

        public string imageUrl
        {
            get { return imageurl; }
            set
            {
                imageurl = value;
                this.RaisePropertyChanged("imageUrl");
            }
        }

        public string imageName { get; set; }//图片名称
        public int Sequence { get; set; }//序号
        public string imgData { get; set; }//图片信息
        public string SIM { get; set; }//sim卡号
        public string VehicleId { get; set; }//车牌号
        public string recordtime { get; set; }//记录时间
        public string inserttime { get; set; }//插入时间 
    }
}
