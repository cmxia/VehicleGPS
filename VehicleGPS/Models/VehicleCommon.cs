using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.DBWCFServices;


namespace VehicleGPS.Models
{
    class VehicleCommon
    {
        #region 权限
        public static string MenuTypeID = "qx00002";
        public static string ReportTypeID = "qx00003";
        /*菜单最大集*/
        public static List<RightInfo> MenuSet = null;
        /*油耗分析权限*/
        public static List<RightInfo> OilSet;
        /*里程分析权限*/
        public static List<RightInfo> MileageSet;
        /*运行分析权限*/
        public static List<RightInfo> RunSet;
        /*告警分析权限*/
        public static List<RightInfo> AlarmSet;
        /*行车记录权限*/
        public static List<RightInfo> RecordSet;
        /*常用报表权限*/
        public static List<RightInfo> CommonSet;
        public static RightInfo InitRightInfo(string name, string typeid)
        {
            RightInfo info = new RightInfo();
            info.ID = "";
            info.Name = name;
            info.TypeID = typeid;
            info.ParentID = "";
            return info;
        }
        public static void InitRightSet()
        {
            if (MenuSet == null)
            {
                MenuSet = new List<RightInfo>();
            }
            MenuSet.Clear();
            //MenuSet.Add(InitRightInfo("监控中心", MenuTypeID));
            //MenuSet.Add(InitRightInfo("调度中心", MenuTypeID));
            //MenuSet.Add(InitRightInfo("信息报表", MenuTypeID));
            //MenuSet.Add(InitRightInfo("系统管理", MenuTypeID));

            OilSet = new List<RightInfo>();
            OilSet.Add(InitRightInfo("油耗统计", ReportTypeID));
            OilSet.Add(InitRightInfo("油耗明细", ReportTypeID));
            OilSet.Add(InitRightInfo("油表查看", ReportTypeID));

            MileageSet = new List<RightInfo>();
            MileageSet.Add(InitRightInfo("里程统计", ReportTypeID));
            MileageSet.Add(InitRightInfo("里程明细", ReportTypeID));
            MileageSet.Add(InitRightInfo("行驶总里程", ReportTypeID));

            RunSet = new List<RightInfo>();
            RunSet.Add(InitRightInfo("离线分析", ReportTypeID));
            RunSet.Add(InitRightInfo("停车统计", ReportTypeID));
            RunSet.Add(InitRightInfo("历史轨迹", ReportTypeID));
            RunSet.Add(InitRightInfo("进出区域明细", ReportTypeID));
            RunSet.Add(InitRightInfo("卸料统计", ReportTypeID));
            RunSet.Add(InitRightInfo("上下线明细", ReportTypeID));
            RunSet.Add(InitRightInfo("未熄火停车", ReportTypeID));
            RunSet.Add(InitRightInfo("ACC分析", ReportTypeID));
            RunSet.Add(InitRightInfo("行车统计", ReportTypeID));
            RunSet.Add(InitRightInfo("CAN数据明细", ReportTypeID));

            AlarmSet = new List<RightInfo>();
            AlarmSet.Add(InitRightInfo("报警明细", ReportTypeID));
            AlarmSet.Add(InitRightInfo("单位报警统计", ReportTypeID));

            RecordSet = new List<RightInfo>();
            RecordSet.Add(InitRightInfo("区域超速汇总", ReportTypeID));
            RecordSet.Add(InitRightInfo("单车区域超速", ReportTypeID));
            RecordSet.Add(InitRightInfo("速度明细", ReportTypeID));
            RecordSet.Add(InitRightInfo("平均速度分析", ReportTypeID));
            RecordSet.Add(InitRightInfo("事故疑点数据", ReportTypeID));

            CommonSet = new List<RightInfo>();
            CommonSet.Add(InitRightInfo("运输明细", ReportTypeID));
            CommonSet.Add(InitRightInfo("日行驶里程", ReportTypeID));
            CommonSet.Add(InitRightInfo("违规作业", ReportTypeID));
            CommonSet.Add(InitRightInfo("无任务离场", ReportTypeID));
            CommonSet.Add(InitRightInfo("超速报警", ReportTypeID));
            CommonSet.Add(InitRightInfo("指令下发历史", ReportTypeID));
        }
        #endregion

        /*车辆在线状态*/
        public static string VSOnlineRun = "行驶中";
        public static string VSOnlineWarn = "报警";
        public static string VSOnlinePark = "停车";
        public static string VSOnlineOff = "离线";
        public static string VSOnlineUnknow = "不确定";

        /*车辆当前状态*/
        public static string VStateNormal = "正常";
        public static string VStateTemp = "临时";
        public static string VStateMaintain = "维修";

        /*任务状况*/
        public static string TaskNo = "空闲无任务";
        public static string TaskIng = "进行任务中";
        public static string TaskMaintain = "维修不可用";
        public static string TaskOff = "无任务离场";

        /*出行状态标志*/
        public static string TaskDriveInStartPoint = "站点内未发车";
        public static string TaskDriveTransIng = "运输途中";
        public static string TaskDriveReachUnload = "工地内卸料";
        public static string TaskDriveReturnIng = "返程途中";
        public static string TaskDriveReturnStartPoint = "已返回站内";
        public static string TaskDriveBack = "强制回站";
        public enum TaskDriveState
        {
            InStartPoint = 1,
            TransIng,
            ReachUnload,
            ReturnIng,
            ReturnStartPoint
        }
        public static string GetCarsStatus(string carStatus)
        {
            if (carStatus == "1")
            {
                return VehicleCommon.TaskDriveInStartPoint;
            }
            else if (carStatus == "2")
            {
                return VehicleCommon.TaskDriveTransIng;
            }
            else if (carStatus == "3")
            {
                return VehicleCommon.TaskDriveReachUnload;
            }
            else if (carStatus == "4")
            {
                return VehicleCommon.TaskDriveReturnIng;
            }
            else if (carStatus == "5")
            {
                return VehicleCommon.TaskDriveReturnStartPoint;
            }
            else
            {
                return VehicleCommon.TaskDriveBack;
            }
        }
        /*wcf服务类*/
        public static DBHelperClient wcfDBHelper = new DBHelperClient();

        /*获取卸料方式*/
        public static List<string> GetUnloadWay()
        {
            List<string> list = new List<string>();
            list.Add("自卸");
            list.Add("泵送");
            list.Add("塔吊");
            return list;
        }
        /*获取上班类型*/
        public static List<string> GetHolidayType()
        {
            List<string> list = new List<string>();
            list.Add("正常上班");
            list.Add("节假日加班");
            list.Add("周末加班");
            return list;
        }
        #region 根据车辆类型和车辆在线状态获取图片UR（ 监控中心）
        public static string GetVehicleImageURL(string typeID, string onlineState, string datetime = null)
        {
            string imageUrl = VehicleConfig.GetInstance().carImgPath;
            if (typeID == "unknow")
            {
                imageUrl += "offline.png";
            }
            else
            {
                if (datetime != null)
                {
                    try
                    {

                        DateTime dt = DateTime.Parse(datetime);
                        DateTime enddt = DateTime.Now;
                        TimeSpan ts = enddt - dt;
                        if (ts.Days > 0 || ts.Hours > 0 || ts.Minutes > 29)
                        {
                            imageUrl += "offline.png";
                            return imageUrl;
                        }
                    }
                    catch (Exception)
                    {
                        imageUrl += "offline.png";
                        return imageUrl;
                    }
                }

                if (onlineState == VehicleCommon.VSOnlinePark)
                {//停车
                    imageUrl += "park.png";
                }
                else if (onlineState == VehicleCommon.VSOnlineRun)
                {//行驶中
                    imageUrl += "online.png";
                }
                else if (onlineState == VehicleCommon.VSOnlineWarn)
                {//报警
                    imageUrl += "online.png";
                }
                else
                {//离线
                    imageUrl += "offline.png";
                }
            }
            return imageUrl;
        }

        #endregion
        #region 根据车辆类型和车辆在线状态获取图片提示（ 监控中心）
        public static string GetVehicleImageTip(string typeID, string onlineState, string starttime)
        {
            string imageTip;
            if (typeID == "unknow")
            {
                imageTip = "离线";
            }
            else
            {
                if (starttime != null)
                {
                    try
                    {

                        DateTime dt = DateTime.Parse(starttime);
                        DateTime endDt = DateTime.Now;
                        TimeSpan ts = endDt - dt;
                        if (ts.Days > 0 || ts.Hours > 0 || ts.Minutes > 29)
                        {
                            return "离线";
                        }
                    }
                    catch (Exception)
                    {
                        return "离线";
                    }
                }
                if (onlineState == VehicleCommon.VSOnlinePark)
                {//停车
                    imageTip = "停车";
                }
                else if (onlineState == VehicleCommon.VSOnlineRun)
                {//行驶中
                    imageTip = "在线";
                }
                else if (onlineState == VehicleCommon.VSOnlineWarn)
                {//报警

                    imageTip = "在线";
                }
                else
                {//离线
                    imageTip = "离线";
                }
            }
            return imageTip;
        }
        #endregion

        #region 根据车辆类型和车辆任务状态获取图片URL（调度中心）
        /*根据车辆类型和车辆任务状态获取图片URL*/
        public static string GetTaskImageURL(CVBasicInfo nodeInfo)
        {
            string OnLineState = nodeInfo.OnlineStatus;
            string imageUrl = VehicleConfig.GetInstance().carImgPath;

            if (OnLineState == VehicleCommon.VSOnlinePark)
            {//在线熄火

                imageUrl += "park.png";
            }
            else if (OnLineState == VehicleCommon.VSOnlineRun)
            {//在线

                imageUrl += "online.png";
            }
            else
            {//离线                
                imageUrl += "offline.png";
            }
            return imageUrl;
        }
        #endregion

        /*获取调度车辆实时显示图片*/
        public static string GetDispatchImageURL(string typeID, string back)
        {
            string imageUrl = VehicleConfig.GetInstance().carImgPath;
            if (back == VehicleCommon.TaskDriveReturnIng)
            {
                if (typeID == "cl00001")//搅拌车
                {
                    imageUrl += "concreteCarOnline_64.png";
                }
                else if (typeID == "cl00002")//泵车
                {
                    imageUrl += "pumpCarOnline_64.png";
                }
                else if (typeID == "cl00003")//工具车
                {//未选定
                    imageUrl += "concreteCarOnline_64.png";
                }
                else if (typeID == "cl00004")//服务车
                {
                    imageUrl += "ServiceCarOnLine_64.png";
                }
                else
                {//如果都不是默认为搅拌车
                    imageUrl += "concreteCarOnline_64.png";
                }
            }
            else
            {
                if (typeID == "cl00001")//搅拌车
                {
                    imageUrl += "concreteCarOnlineTo_64.png";
                }
                else if (typeID == "cl00002")//泵车
                {
                    imageUrl += "pumpCarOnlineTo_64.png";
                }
                else if (typeID == "cl00003")//工具车
                {//未选定
                    imageUrl += "concreteCarOnlineTo_64.png";
                }
                else if (typeID == "cl00004")//服务车
                {
                    imageUrl += "ServiceCarOnLineTo_64.png";
                }
                else
                {//如果都不是默认为搅拌车
                    imageUrl += "concreteCarOnlineTo_64.png";
                }
            }

            return imageUrl;
        }

        /*任务单的执行状态*/
        public static string GetTaskState(string num)
        {
            try
            {
                if (num.Equals("1"))
                {
                    return "正在执行";
                }
                else if (num.Equals("2"))
                {
                    return "执行完成";
                }
                else if (num.Equals("3"))
                {
                    return "等待执行";
                }
                return "";
            }
            catch (NullReferenceException e)
            {
                return "";
            }
        }
        public static string GetTaskStateReverse(string text)
        {
            if (text.Equals("正在执行"))
            {
                return "1";
            }
            else if (text.Equals("执行完成"))
            {
                return "2";
            }
            else if (text.Equals("等待执行"))
            {
                return "3";
            }
            return "";
        }
        /*获取方向图片*/
        public static string GetDirectionImageUrl(string direction, string onlineState)
        {
            //正北 正西 正南 正东 西北 西南  东南 东北
            string imageStr = VehicleConfig.GetInstance().directionImgPath;
            if (onlineState == VehicleCommon.VSOnlineRun)
            {
                if (direction.Equals("正北"))
                {
                    imageStr += "movenorth.png";
                }
                else if (direction.Equals("正西"))
                {
                    imageStr += "movewest.png";
                }
                else if (direction.Equals("正南"))
                {
                    imageStr += "movesouth.png";
                }
                else if (direction.Equals("正东"))
                {
                    imageStr += "moveeast.png";
                }
                else if (direction.Equals("西北"))
                {
                    imageStr += "movenorthwest.png";
                }
                else if (direction.Equals("西南"))
                {
                    imageStr += "movesouthwest.png";
                }
                else if (direction.Equals("东南"))
                {
                    imageStr += "movesoutheast.png";
                }
                else if (direction.Equals("东北"))
                {
                    imageStr += "movenortheast.png";
                }
            }
            else
            {
                imageStr += "park.png";
            }
            return imageStr;
        }

        /*轨迹回放割点方向标（静态方向标）*/
        public static string GetStaticDirectionImageUrl(string direction)
        {
            string imageUrl = VehicleConfig.GetInstance().directionImgPath;
            if (direction == "正东")
            {
                imageUrl = imageUrl + "east.png";
            }
            else if (direction == "正南")
            {
                imageUrl = imageUrl + "south.png";
            }
            else if (direction == "正西")
            {
                imageUrl = imageUrl + "west.png";
            }
            else if (direction == "正北")
            {
                imageUrl = imageUrl + "north.png";
            }
            else if (direction == "东北")
            {
                imageUrl = imageUrl + "northeast.png";
            }
            else if (direction == "东南")
            {
                imageUrl = imageUrl + "southeast.png";
            }
            else if (direction == "西北")
            {
                imageUrl = imageUrl + "northwest.png";
            }
            else if (direction == "西南")
            {
                imageUrl = imageUrl + "southwest.png";
            }
            return imageUrl;
        }

        #region 生成序列号
        /*获取年月日时分秒*/
        public static string getStrSequence()
        {
            string str = "";
            byte[] buffer = Guid.NewGuid().ToByteArray();
            long long_guid = BitConverter.ToInt64(buffer, 0);
            str = DateTime.Now.ToString("yyyyMMddhhmmss") + randStr(5) + long_guid.ToString();//14+19+5
            return str;
        }
        /*获取随机数*/
        public static string randStr(int count)
        {
            string allchar = "1,2,3,4,5,6,7,8,9,0";
            string[] allchararray = allchar.Split(',');
            string randomcode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(10);
                if (temp == t)
                {
                    return randStr(count);
                }
                temp = t;
                randomcode += allchararray[t];
            }
            return randomcode;
        }
        #endregion

        #region 计算两点直线距离
        private const double EARTH_RADIUS = 6378.137;//地球半径
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
            Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        #endregion

        #region 地图infowindow窗口信息html
        /*生成显示标注信息的Html
         <param name="licenseNum">车牌号</param>
         * <param name="SIM">SIM卡号</param>
         * <param name="uploadTime">上传时间</param>
         * <param name="speed">速度</param>
         * <param name="direction">行驶方向</param>
         * <param name="curlocation">当前位置</param>
         */
        public static string GetHtml(string licenseNum, string onlineStatus, string uploadTime, string speed, string direction, string curlocation)
        {
            string ret = "";
            ret += "<table style='font-family:verdana;font-size:12;color:black;word-warp:break-word;word-break:break-all;wordbreak:breakall'>";
            ret += "<tr><td>车牌号：</td><td>";
            ret += licenseNum + "</td></tr>";
            ret += "<tr><td>在线状态：</td><td>";
            ret += onlineStatus + "</td></tr>";
            ret += "<tr><td>上传时间：</td><td>";
            ret += uploadTime + "</td></tr>";
            ret += "<tr><td>速度：</td><td>";
            ret += speed + "</td></tr>";
            ret += "<tr><td>行驶方向：</td><td>";
            ret += direction + "</td></tr>";
            ret += "<tr><td>当前位置：</td><td width='210'>";
            ret += curlocation + "</td></tr>";
            ret += "</table>";
            return ret;
        }
        #endregion

        /*获取方向*/
        public static string translateDirection(string direction)
        {
            //正北 正西 正南 正东 西北 西南  东南 东北
            double dir = 0;
            if (!double.TryParse(direction, out dir))
                return "正北";
            if (dir > 22.5 && dir < 90 - 22.5)
            {
                return "东北";
            }
            else if (dir >= 90 - 22.5 && dir <= 90 + 22.5)
            {
                return "正东";
            }
            else if (dir > 90 + 22.5 && dir < 180 - 22.5)
            {
                return "东南";
            }
            else if (dir >= 180 - 22.5 && dir <= 180 + 22.5)
            {
                return "正南";
            }
            else if (dir > 180 + 22.5 && dir < 270 - 22.5)
            {
                return "西南";
            }
            else if (dir >= 270 - 22.5 && dir <= 270 + 22.5)
            {
                return "正西";
            }
            else if (dir > 270 + 22.5 && dir < 360 - 22.5)
            {
                return "西北";
            }
            else if (dir >= 360 - 22.5 && dir <= 360 || dir >= 0 && dir <= 22.5)
            {
                return "正北";
            }
            else
            {
                return "正北";
            }
        }

    }
}
