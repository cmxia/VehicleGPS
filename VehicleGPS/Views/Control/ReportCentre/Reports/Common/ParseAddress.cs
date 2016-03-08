using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using VehicleGPS.Models;
using Newtonsoft.Json;

namespace VehicleGPS.Views.Control.ReportCentre.Reports.Common
{
    class ParseAddress
    {
        private int parseThreadNum = 20;
        private int parseAllCount = 0;
        private object counterMonitor = new object();//地理和坐标解析计数互斥量
        private int timeCounter = 0;//地理和坐标解析计数器
        public List<PointAddr> pointAddrList;
        public ParseAddress(List<double> lngList,List<double> latList)
        {
            this.pointAddrList = new List<PointAddr>();
            this.parseAllCount = lngList.Count;
            for (int i = 0; i < lngList.Count; i++)
            {
                PointAddr pa = new PointAddr();
                pa.lat = latList[i].ToString();
                pa.lng = lngList[i].ToString();
                this.pointAddrList.Add(pa);
            }
        }

        public List<PointAddr> BeginParse()
        {
            /*启动地理解析线程*/
            int oneThreadNum = 0;
            if (this.parseAllCount < this.parseThreadNum)
            {
                this.parseThreadNum = 1;
                oneThreadNum = this.parseAllCount;
            }
            else
            {
                oneThreadNum = this.parseAllCount / this.parseThreadNum;
            }
            for (int i = 0; i < this.parseThreadNum; i++)
            {
                int startIndex = i * oneThreadNum;
                int endIndex = (i + 1) * oneThreadNum;
                Thread parseThread = new Thread(delegate() { this.ParseAddressThread(startIndex, endIndex); });
                parseThread.Start();
            }
            if (this.parseAllCount % this.parseThreadNum != 0)
            {
                int startIndex = this.parseThreadNum * oneThreadNum;
                int endIndex = this.parseAllCount;
                Thread parseThread = new Thread(delegate() { this.ParseAddressThread(startIndex, endIndex); });
                parseThread.Start();
            }
            int lastCounter = 0;
            int sameTimes = 0;
            while (this.timeCounter != this.parseAllCount)
            {
                Thread.Sleep(500);
                if (lastCounter == this.timeCounter)
                {
                    if (++sameTimes == 4)
                    {
                        break;
                    }
                }
                else
                {
                    lastCounter = this.timeCounter;
                    sameTimes = 0;
                }
            }
            return this.pointAddrList;
        }

        // 定时获取去的最新gps信息根据经纬度解析地址
        private void ParseAddressThread(int startIndex, int endIndex)
        {
            /*逆地址解析*/
            WebClient webClient = new WebClient();
            webClient.Encoding = System.Text.Encoding.UTF8;
            VehicleGPS.Services.BusinessDataServiceWEB.BaiDuPoint bdPoint = new VehicleGPS.Services.BusinessDataServiceWEB.BaiDuPoint();
            VehicleGPS.Services.BusinessDataServiceWEB.BaiDuAddr bdAddr = new VehicleGPS.Services.BusinessDataServiceWEB.BaiDuAddr();
            Uri endpoint;
            for (int i = startIndex; i < endIndex; i++)
            {
                try
                {
                    PointAddr pa = this.pointAddrList[i];
                    string lat = pa.lat;
                    string lng = pa.lng;
                    /*坐标转换*/
                    endpoint = new Uri(VehicleConfig.GetInstance().URL_POINTTRANS + "&x=" + lng + "&y=" + lat);
                    string pointStr = webClient.DownloadString(endpoint);
                    if (pointStr != "")
                    {
                        bdPoint = JsonConvert.DeserializeObject<VehicleGPS.Services.BusinessDataServiceWEB.BaiDuPoint>(pointStr);
                        if (bdPoint.error == "0")
                        {
                            byte[] buf;
                            buf = Convert.FromBase64String(bdPoint.x);
                            pa.lng = Encoding.UTF8.GetString(buf, 0, buf.Length);
                            buf = Convert.FromBase64String(bdPoint.y);
                            pa.lat = Encoding.UTF8.GetString(buf, 0, buf.Length);
                        }
                    }
                    /*地址解析*/
                    endpoint = new Uri(VehicleConfig.GetInstance().URL_PARSEADDRESS + "&location=" + lat + "," + lng + "&output=json");
                    string addrStr = webClient.DownloadString(endpoint);
                    if (addrStr != "")
                    {
                        bdAddr = JsonConvert.DeserializeObject<VehicleGPS.Services.BusinessDataServiceWEB.BaiDuAddr>(addrStr);
                        if (bdAddr.status == "0")
                        {
                            pa.addr = bdAddr.result.formatted_address;
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    Monitor.Enter(this.counterMonitor);
                    this.timeCounter++;
                    Monitor.Exit(this.counterMonitor);
                }
            }
        }
    }
    public class PointAddr
    {
        public string lat;
        public string lng;
        public string addr;
    }
}
