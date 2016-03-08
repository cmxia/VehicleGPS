using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using VehicleGPS.Models;

namespace VehicleGPS.Views.Control.DispatchCentre
{
    /// <summary>
    ///XmlUtilC 的摘要说明
    /// </summary>
    public class XmlUtilSL
    {
        public static string vehicleStatesOnlineRun = "行驶中";
        public static string vehicleStatesOnlineWarn = "报警";
        public static string vehicleStatesOnlinePase = "停车";
        public static string vehicleStatesOffOnlineBlue = "离线";
        public static string vehicleStatesOffOnline = "从未上传";

        /*
        * 函数名称：DeserializeGpsInfoList(string xml)
        * 函数功能：反序列化GpsInfoList
        * 函数参数： xml 字符串
        * 作者：wangyi
        *	时间：2013/09/25
        */
        public GPSInfo DeserializeGpsInfo(string xml)
        {
            GPSInfo gpsInfo = new GPSInfo();
            using (XmlReader xReader = XmlReader.Create(new StringReader(xml)))
            {
                if (!xReader.ReadToFollowing("GpsInfo"))
                    return null;
                gpsInfo.Sim = xReader.GetAttribute("a1") == null ? "" : xReader.GetAttribute("a1");
                gpsInfo.Longitude = chargeLngData(xReader.GetAttribute("a2"));
                gpsInfo.Latitude = chargeLatData(xReader.GetAttribute("a3"));
                gpsInfo.Altitude = xReader.GetAttribute("a4") == null ? "" : xReader.GetAttribute("a4");
                gpsInfo.GpsStatus = xReader.GetAttribute("a5") == null ? "" : xReader.GetAttribute("a5");
                gpsInfo.Speed = xReader.GetAttribute("a6") == null ? "" : xReader.GetAttribute("a6");
                gpsInfo.DevSpeed = xReader.GetAttribute("a7") == null ? "" : xReader.GetAttribute("a7");
                gpsInfo.Mileage = xReader.GetAttribute("a8") == null ? "" : xReader.GetAttribute("a8");
                gpsInfo.OilVolumn = xReader.GetAttribute("a9") == null ? "" : xReader.GetAttribute("a9");
                gpsInfo.Direction = xReader.GetAttribute("b0") == "正北" ? "" : translateDirection(xReader.GetAttribute("b0"));
                gpsInfo.Datetime = chargeDateTime(xReader.GetAttribute("b1"));
                gpsInfo.OnlineStates = getVehicleOnlinestates(gpsInfo.Speed, gpsInfo.Datetime, gpsInfo);
                gpsInfo.isCheckedFlag = false;

                gpsInfo.Idtime = xReader.GetAttribute("b2") == null ? "" : xReader.GetAttribute("b2");
                gpsInfo.Accstatus = xReader.GetAttribute("b3") == null ? "" : xReader.GetAttribute("b3");
                gpsInfo.Workstatus = xReader.GetAttribute("b4") == null ? "" : xReader.GetAttribute("b4");
                gpsInfo.Llsecret = xReader.GetAttribute("b5") == null ? "" : xReader.GetAttribute("b5");
                gpsInfo.Gpsmode = xReader.GetAttribute("b6") == null ? "" : xReader.GetAttribute("b6");
                gpsInfo.Oilwaystatus = xReader.GetAttribute("b7") == null ? "" : xReader.GetAttribute("b7");
                gpsInfo.Vcstatus = xReader.GetAttribute("b8") == null ? "" : xReader.GetAttribute("b8");
                gpsInfo.Vdstatus = xReader.GetAttribute("b9") == null ? "" : xReader.GetAttribute("b9");
                gpsInfo.Fdstatus = xReader.GetAttribute("c0") == null ? "" : xReader.GetAttribute("c0");
                gpsInfo.Bdstatus = xReader.GetAttribute("c1") == null ? "" : xReader.GetAttribute("c1");
                gpsInfo.Enginestatus = xReader.GetAttribute("c2") == null ? "" : xReader.GetAttribute("c2");
                gpsInfo.Conditionerstatus = xReader.GetAttribute("c3") == null ? "" : xReader.GetAttribute("c3");
                gpsInfo.Brakestatus = xReader.GetAttribute("c4") == null ? "" : xReader.GetAttribute("c4");
                gpsInfo.Ltstatus = xReader.GetAttribute("c5") == null ? "" : xReader.GetAttribute("c5");
                gpsInfo.Rtstatus = xReader.GetAttribute("c6") == null ? "" : xReader.GetAttribute("c6");
                gpsInfo.Farlstatus = xReader.GetAttribute("c7") == null ? "" : xReader.GetAttribute("c7");
                gpsInfo.Nearlstatus = xReader.GetAttribute("c8") == null ? "" : xReader.GetAttribute("c8");
                gpsInfo.Pnstatus = xReader.GetAttribute("c9") == null ? "" : xReader.GetAttribute("c9");
                gpsInfo.Shakestatus = xReader.GetAttribute("d0") == null ? "" : xReader.GetAttribute("d0");
                gpsInfo.Hornstatus = xReader.GetAttribute("d1") == null ? "" : xReader.GetAttribute("d1");
                gpsInfo.Protectstatus = xReader.GetAttribute("d2") == null ? "" : xReader.GetAttribute("d2");
                gpsInfo.Loadstatus = xReader.GetAttribute("d3") == null ? "" : xReader.GetAttribute("d3");
                gpsInfo.Busstatus = xReader.GetAttribute("d4") == null ? "" : xReader.GetAttribute("d4");
                gpsInfo.Gsmstatus = xReader.GetAttribute("d5") == null ? "" : xReader.GetAttribute("d5");
                gpsInfo.Gpsstatus = xReader.GetAttribute("d6") == null ? "" : xReader.GetAttribute("d6");
                gpsInfo.Lcstatus = xReader.GetAttribute("d7") == null ? "" : xReader.GetAttribute("d7");
                gpsInfo.Ffstatus = xReader.GetAttribute("d8") == null ? "" : xReader.GetAttribute("d8");
                gpsInfo.Bfstatus = xReader.GetAttribute("d9") == null ? "" : xReader.GetAttribute("d9");
                gpsInfo.Gpsantstatus = xReader.GetAttribute("e0") == null ? "" : xReader.GetAttribute("e0");
                gpsInfo.Soswarn = xReader.GetAttribute("e1") == null ? "" : xReader.GetAttribute("e1");
                gpsInfo.Overspeedwarn = xReader.GetAttribute("e2") == null ? "" : xReader.GetAttribute("e2");
                gpsInfo.Tiredwarn = xReader.GetAttribute("e3") == null ? "" : xReader.GetAttribute("e3");
                gpsInfo.Prewarn = xReader.GetAttribute("e4") == null ? "" : xReader.GetAttribute("e4");
                gpsInfo.Gnssfatal = xReader.GetAttribute("e5") == null ? "" : xReader.GetAttribute("e5");
                gpsInfo.Gnssantwarn = xReader.GetAttribute("e6") == null ? "" : xReader.GetAttribute("e6");
                gpsInfo.Gnssshortwarn = xReader.GetAttribute("e7") == null ? "" : xReader.GetAttribute("e7");
                gpsInfo.Lowvolwarn = xReader.GetAttribute("e8") == null ? "" : xReader.GetAttribute("e8");
                gpsInfo.Highvolwarn = xReader.GetAttribute("e9") == null ? "" : xReader.GetAttribute("e9");
                gpsInfo.Outagewarn = xReader.GetAttribute("f0") == null ? "" : xReader.GetAttribute("f0");
                gpsInfo.Lcdfatalwarn = xReader.GetAttribute("f1") == null ? "" : xReader.GetAttribute("f1");
                gpsInfo.Ttsfatalwarn = xReader.GetAttribute("f1") == null ? "" : xReader.GetAttribute("f2");
                gpsInfo.Camerafatalwarn = xReader.GetAttribute("f3") == null ? "" : xReader.GetAttribute("f3");
                gpsInfo.Vediolosewarn = xReader.GetAttribute("f4") == null ? "" : xReader.GetAttribute("f4");
                gpsInfo.Vedioshelterwarn = xReader.GetAttribute("f5") == null ? "" : xReader.GetAttribute("f5");
                gpsInfo.Accumtimeout = xReader.GetAttribute("f6") == null ? "" : xReader.GetAttribute("f6");
                gpsInfo.Stoptimeout = xReader.GetAttribute("f7") == null ? "" : xReader.GetAttribute("f7");
                gpsInfo.Inoutareawarn = xReader.GetAttribute("f8") == null ? "" : xReader.GetAttribute("f8");
                gpsInfo.Inoutlinewarn = xReader.GetAttribute("f9") == null ? "" : xReader.GetAttribute("f9");
                gpsInfo.Drivingtimewarn = xReader.GetAttribute("g0") == null ? "" : xReader.GetAttribute("g0");
                gpsInfo.Deviatewarn = xReader.GetAttribute("g1") == null ? "" : xReader.GetAttribute("g1");
                gpsInfo.Vssfatalwarn = xReader.GetAttribute("g2") == null ? "" : xReader.GetAttribute("g2");
                gpsInfo.Oilexceptionwarn = xReader.GetAttribute("g3") == null ? "" : xReader.GetAttribute("g3");
                gpsInfo.Vehiclestolenwarn = xReader.GetAttribute("g4") == null ? "" : xReader.GetAttribute("g4");
                gpsInfo.Illignitewarn = xReader.GetAttribute("g5") == null ? "" : xReader.GetAttribute("g5");
                gpsInfo.Illmovewarn = xReader.GetAttribute("g6") == null ? "" : xReader.GetAttribute("g6");
                gpsInfo.Crashwarn = xReader.GetAttribute("g7") == null ? "" : xReader.GetAttribute("g7");
                gpsInfo.Sdexceptionwarn = xReader.GetAttribute("g8") == null ? "" : xReader.GetAttribute("g8");
                gpsInfo.Robwarn = xReader.GetAttribute("g9") == null ? "" : xReader.GetAttribute("g9");
                gpsInfo.Sleeptimewarn = xReader.GetAttribute("h0") == null ? "" : xReader.GetAttribute("h0");
                gpsInfo.Illtimedrivingwarn = xReader.GetAttribute("h1") == null ? "" : xReader.GetAttribute("h1");
                gpsInfo.Overstationwarn = xReader.GetAttribute("h2") == null ? "" : xReader.GetAttribute("h2");
                gpsInfo.Ilopendoorwarn = xReader.GetAttribute("h3") == null ? "" : xReader.GetAttribute("h3");
                gpsInfo.Protectwarn = xReader.GetAttribute("h4") == null ? "" : xReader.GetAttribute("h4");
                gpsInfo.Trimmingwarn = xReader.GetAttribute("h5") == null ? "" : xReader.GetAttribute("h5");
                gpsInfo.Passwdwarn = xReader.GetAttribute("h6") == null ? "" : xReader.GetAttribute("h6");
                gpsInfo.Prohibitmovewarn = xReader.GetAttribute("h7") == null ? "" : xReader.GetAttribute("h7");
                gpsInfo.Illstopwarn = xReader.GetAttribute("h8") == null ? "" : xReader.GetAttribute("h8");
                gpsInfo.GPSMileage = xReader.GetAttribute("h9") == null ? "" : xReader.GetAttribute("h9");
            }
            return gpsInfo;
        }

        #region 判断获取的gps格式，并且对不正确的进行修改

        public static string chargeLngData(string lng)
        {
            if (lng == null)
                return "null";

            double lngd = 0;
            if ((lng.Length > 3) && (!lng.Substring(3, 1).Equals(".")))
            {
                lng = lng.Replace(".", "");
                lng = lng.Trim();
                if (lng.Length > 3)
                {
                    lng = lng.Substring(0, 3) + "." + lng.Substring(3);
                }
            }

            if (double.TryParse(lng, out lngd))
            {
                if (lngd < -180 || lngd > 180)
                    return "null";
            }
            else
            {
                return "null";
            }

            return lng;
        }

        public static string chargeLatData(string lat)
        {
            if (lat == null)
                return "null";

            double latd = 0;

            if ((lat.Length > 2) && (!lat.Substring(2, 1).Equals(".")))
            {
                lat = lat.Replace(".", "");
                lat = lat.Trim();
                if (lat.Length > 2)
                {
                    lat = lat.Substring(0, 2) + "." + lat.Substring(2);
                }
            }

            if (double.TryParse(lat, out latd))
            {
                if (latd < -90 || latd > 90)
                    return "null";
            }
            else
            {
                return "null";
            }

            return lat;
        }

        public string chargeDateTime(string dateTime)
        {
            if ((dateTime != null) && (dateTime.Length == 18))
            {
                dateTime = dateTime.Substring(0, 10) + " " + dateTime.Substring(10);
            }
            else if (dateTime.Length != 19)
            {
                dateTime = DateTime.Now.ToString();
            }
            return dateTime;
        }

        #endregion

        private string translateDirection(string direction)
        {
            string tdr = "正北"; //正北 正西 正南 正东 西北 西南  东南 东北
            double tdi = 0;
            if (double.TryParse(direction, out tdi))
            {
                if (tdi == 0)
                {
                    tdr = "正北";
                }
                else if (tdi < 90)
                {
                    tdr = "东北";
                }
                else if (tdi == 90)
                {
                    tdr = "正东";
                }
                else if (tdi < 180)
                {
                    tdr = "东南";
                }
                else if (tdi == 180)
                {
                    tdr = "正南";
                }
                else if (tdi < 270)
                {
                    tdr = "西南";
                }
                else if (tdi == 270)
                {
                    tdr = "正西";
                }
                else if (tdi < 360)
                {
                    tdr = "西北";
                }
            }

            return tdr;
        }

        private bool getTrackVehicleWarnShow(GPSInfo gpsInfo)
        {
            string normal = "正常";
            if ((gpsInfo.Soswarn != null) && (!gpsInfo.Soswarn.Equals(normal)) && (!gpsInfo.Soswarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Overspeedwarn != null) && (!gpsInfo.Overspeedwarn.Equals(normal)) && (!gpsInfo.Overspeedwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Tiredwarn != null) && (!gpsInfo.Tiredwarn.Equals(normal)) && (!gpsInfo.Tiredwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Prewarn != null) && (!gpsInfo.Prewarn.Equals(normal)) && (!gpsInfo.Prewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Gnssfatal != null) && (!gpsInfo.Gnssfatal.Equals(normal)) && (!gpsInfo.Gnssfatal.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Gnssantwarn != null) && (!gpsInfo.Gnssantwarn.Equals(normal)) && (!gpsInfo.Gnssantwarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Gnssshortwarn != null) && (!gpsInfo.Gnssshortwarn.Equals(normal)) && (!gpsInfo.Gnssshortwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Lowvolwarn != null) && (!gpsInfo.Lowvolwarn.Equals(normal)) && (!gpsInfo.Lowvolwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Highvolwarn != null) && (!gpsInfo.Highvolwarn.Equals(normal)) && (!gpsInfo.Highvolwarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Outagewarn != null) && (!gpsInfo.Outagewarn.Equals(normal)) && (!gpsInfo.Outagewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Lcdfatalwarn != null) && (!gpsInfo.Lcdfatalwarn.Equals(normal)) && (!gpsInfo.Lcdfatalwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Ttsfatalwarn != null) && (!gpsInfo.Ttsfatalwarn.Equals(normal)) && (!gpsInfo.Ttsfatalwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Camerafatalwarn != null) && (!gpsInfo.Camerafatalwarn.Equals(normal)) && (!gpsInfo.Camerafatalwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Vediolosewarn != null) && (!gpsInfo.Vediolosewarn.Equals(normal)) && (!gpsInfo.Vediolosewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Vedioshelterwarn != null) && (!gpsInfo.Vedioshelterwarn.Equals(normal)) && (!gpsInfo.Vedioshelterwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Accumtimeout != null) && (!gpsInfo.Accumtimeout.Equals(normal)) && (!gpsInfo.Accumtimeout.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Stoptimeout != null) && (!gpsInfo.Stoptimeout.Equals(normal)) && (!gpsInfo.Stoptimeout.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Inoutareawarn != null) && (!gpsInfo.Inoutareawarn.Equals(normal)) && (!gpsInfo.Inoutareawarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Inoutlinewarn != null) && (!gpsInfo.Inoutlinewarn.Equals(normal)) && (!gpsInfo.Inoutlinewarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Drivingtimewarn != null) && (!gpsInfo.Drivingtimewarn.Equals(normal)) && (!gpsInfo.Drivingtimewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Deviatewarn != null) && (!gpsInfo.Deviatewarn.Equals(normal)) && (!gpsInfo.Deviatewarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Vssfatalwarn != null) && (!gpsInfo.Vssfatalwarn.Equals(normal)) && (!gpsInfo.Vssfatalwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Oilexceptionwarn != null) && (!gpsInfo.Oilexceptionwarn.Equals(normal)) && (!gpsInfo.Oilexceptionwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Vehiclestolenwarn != null) && (!gpsInfo.Vehiclestolenwarn.Equals(normal)) && (!gpsInfo.Vehiclestolenwarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Illignitewarn != null) && (!gpsInfo.Illignitewarn.Equals(normal)) && (!gpsInfo.Illignitewarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Illmovewarn != null) && (!gpsInfo.Illmovewarn.Equals(normal)) && (!gpsInfo.Illmovewarn.Equals("")))
            {
                return true;
            }
            if ((gpsInfo.Crashwarn != null) && (!gpsInfo.Crashwarn.Equals(normal)) && (!gpsInfo.Crashwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Sdexceptionwarn != null) && (!gpsInfo.Sdexceptionwarn.Equals(normal)) && (!gpsInfo.Sdexceptionwarn.Equals("")))
            {
                return true;
            }


            if ((gpsInfo.Robwarn != null) && (!gpsInfo.Robwarn.Equals(normal)) && (!gpsInfo.Robwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Sleeptimewarn != null) && (!gpsInfo.Sleeptimewarn.Equals(normal)) && (!gpsInfo.Sleeptimewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Illtimedrivingwarn != null) && (!gpsInfo.Illtimedrivingwarn.Equals(normal)) && (!gpsInfo.Illtimedrivingwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Overstationwarn != null) && (!gpsInfo.Overstationwarn.Equals(normal)) && (!gpsInfo.Overstationwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Ilopendoorwarn != null) && (!gpsInfo.Ilopendoorwarn.Equals(normal)) && (!gpsInfo.Ilopendoorwarn.Equals("")))
            {
                return true;
            }
            if ((gpsInfo.Protectwarn != null) && (!gpsInfo.Protectwarn.Equals(normal)) && (!gpsInfo.Protectwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Trimmingwarn != null) && (!gpsInfo.Trimmingwarn.Equals(normal)) && (!gpsInfo.Trimmingwarn.Equals("")))
            {
                return true;
            }
            if ((gpsInfo.Passwdwarn != null) && (!gpsInfo.Passwdwarn.Equals(normal)) && (!gpsInfo.Passwdwarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Prohibitmovewarn != null) && (!gpsInfo.Prohibitmovewarn.Equals(normal)) && (!gpsInfo.Prohibitmovewarn.Equals("")))
            {
                return true;
            }

            if ((gpsInfo.Illstopwarn != null) && (!gpsInfo.Illstopwarn.Equals(normal)) && (!gpsInfo.Illstopwarn.Equals("")))
            {
                return true;
            }
            return false;
        }

        private string getVehicleOnlinestates(string speed, string dtOldStr, GPSInfo gpsInfo)
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtOld = new DateTime();

            if (dtOldStr.Length < 10)
            {
                return vehicleStatesOffOnlineBlue;
            }

            int index4 = dtOldStr.LastIndexOf("-");
            int index3 = -1;
            if (index4 > 0)
            {
                index3 = dtOldStr.Substring(0, index4).LastIndexOf("-");
                if (index3 > 0)
                {
                    dtOldStr = dtOldStr.Substring(0, index3) + ":" + dtOldStr.Substring(index3 + 1, index4 - index3 - 1) + ":" + dtOldStr.Substring(index4 + 1);
                    if (DateTime.TryParse(dtOldStr, out dtOld))
                    {
                        TimeSpan ts = dtNow - dtOld;
                        if (ts.Hours > VehicleConfig.GetInstance().LONG_TIME_OFF_ONLINE_SPAN_HOURS)
                        {
                            return vehicleStatesOffOnlineBlue;
                        }

                        if (getTrackVehicleWarnShow(gpsInfo))
                        {
                            return vehicleStatesOnlineWarn;
                        }

                        if ((speed == null) || (speed.Length == 0))
                        {
                            return vehicleStatesOnlinePase;
                        }
                        try
                        {
                            double ds = double.Parse(speed);
                            if (ds > 0)
                                return vehicleStatesOnlineRun;
                        }
                        catch (Exception ex)
                        {
                            return vehicleStatesOnlinePase;
                        }
                        return vehicleStatesOnlinePase;
                    }
                }
            }
            return vehicleStatesOffOnlineBlue;
        }


    }
}
