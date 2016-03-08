using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Services
{
    public static class IDHelper
    {
        public static Random rd = new Random();
        /// <summary>
        /// 生成图片的名称
        /// </summary>
        /// <param name="SIM"></param>
        /// <returns></returns>
        public static string GetImageName(string SIM) {
            return "img" + SIM + IDHelper.rd.Next(1000, 9999) + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
        /// <summary>
        /// 生成区域的ID
        /// </summary>
        /// <returns></returns>
        public static string GetRegionID()
        {
            return "CQ" + DateTime.Now.ToString("yyyyMMddHHmmss") + IDHelper.rd.Next(1000,9999);//厂区
        }
        /// <summary>
        /// 生成工地的ID
        /// </summary>
        /// <returns></returns>
        public static string GetSiteID()
        {
            return "GD" + DateTime.Now.ToString("yyyyMMddHHmmss") + IDHelper.rd.Next(1000, 9999);//工地
        }
        /// <summary>
        /// 生成任务单ID
        /// </summary>
        /// <returns></returns>
        public static string GetTaskID()
        {
            return "RW" + DateTime.Now.ToString("yyyyMMddHHmmss") + IDHelper.rd.Next(1000, 9999);
        }
        /// <summary>
        /// 生成派车单ID
        /// </summary>
        /// <returns></returns>
        public static string GetTaskDetailID()
        {
            return "RW_DTAIL" + DateTime.Now.ToString("yyyyMMddHHmmss") + IDHelper.rd.Next(1000, 9999);
        }
        /// <summary>
        /// 生成工地或区域的ID
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetRegionOrBuildingID(string type)
        {
            if (type == "cq")
            {
                return "CQ" + DateTime.Now.ToString("yyyyMMddHHmmss") + IDHelper.rd.Next(1000, 9999);//厂区
            }
            else
            {
                return "GD" + DateTime.Now.ToString("yyyyMMddHHmmss") + IDHelper.rd.Next(1000, 9999);//工地
            }

        }
    }
}
