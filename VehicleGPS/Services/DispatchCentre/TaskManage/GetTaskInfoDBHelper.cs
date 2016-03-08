using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using System.Data;
using VehicleGPS.DBWCFServices;

namespace VehicleGPS.Services.DispatchCentre.TaskManage
{
    public static class GetTaskInfoDBHelper
    {
        /// <summary>
        /// 获取工地或区域的信息
        /// </summary>
        /// <param name="stationid">站点ID</param>
        /// <param name="type">获取的类型：1-工地；6-区域</param>
        /// <returns></returns>
        public static List<CVBasicInfo> GetSiteOrRegionList(string stationid, int type)
        {
            List<CVBasicInfo> list = new List<CVBasicInfo>();
            string selecteditems = "ZONEID,ZONENAME";
            string sql = "select " + selecteditems + " from inforegion where type='xq0000" + type + "' and CustomerID='" + stationid + "'";
            DBHelperClient client = new DBHelperClient();
            string username = StaticLoginInfo.GetInstance().UserName;
            string status = client.BexcuteSql(username, sql);
            client.Close();
            if (string.Compare(status, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(status);
                int x = 0;
                if (dt != null)
                {
                    x = dt.Rows.Count;
                }
                for (int i = 0; i < x; i++)
                {
                    CVBasicInfo cvb = new CVBasicInfo();
                    cvb.Name = dt.Rows[i]["ZONENAME"].ToString();
                    cvb.ID = dt.Rows[i]["ZONEID"].ToString();
                    list.Add(cvb);
                }
            }
            return list;
        }
        /// <summary>
        /// 获取混凝土标号
        /// </summary>
        /// <returns></returns>
        public static List<BasicTypeInfo> GetConcreteNumList()
        {
            List<BasicTypeInfo> concretenumlist = new List<BasicTypeInfo>();
            DBHelperClient client = new DBHelperClient();
            string username = StaticLoginInfo.GetInstance().UserName;
            string sql = "select TypeID,TypeName,VehicleGroupName from BasicTypes where TypeIdentity='bh' and SettingId='" + username + "'";
            string status = client.BexcuteSql(username, sql);
            client.Close();
            if (string.Compare(status, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(status);
                int x = 0;
                if (dt != null)
                {
                    x = dt.Rows.Count;
                }
                for (int i = 0; i < x; i++)
                {
                    BasicTypeInfo basic = new BasicTypeInfo();
                    basic.GroupName = dt.Rows[i]["VehicleGroupName"].ToString();
                    basic.TypeID = dt.Rows[i]["TypeID"].ToString();
                    basic.TypeName = dt.Rows[i]["TypeName"].ToString();
                    concretenumlist.Add(basic);
                }
            }
            return concretenumlist;
        }
    }
}
