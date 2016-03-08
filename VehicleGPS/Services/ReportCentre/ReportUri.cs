using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Services.ReportCentre
{
  public class ReportUri
    {
       public static readonly string webServerUri = "http://59.69.101.3:8888/VEHICLEGPS/";//Web 服务器地址

       //信息报表根据车辆编号查询，  WEB 参考如下

       /// <summary>
       /// ========运行分析========
       /// （stopWithAccOn------未熄火停车分析  //刘雅梦(web)
       /// （stop-------停车分析）    //刘雅梦(web)
       /// （acc-------ACC分析）    //刘雅梦(web)
       /// （moningAnalysis------行车分析）   //刘雅梦(web)
       /// （inOutRegionReport-------进出区域分析） //刘雅梦(web)
       ///（ onlineOfflineReport--------上下线分析）   //刘雅梦(web)
       /// （historyTrack----------历史轨迹） //王娜(web)
       /// （offLineDetails-------离线分析）  //刘雅梦(web)

       /// ========油耗分析========
       /// （oilStatistic-----油耗统计）//匡俐(web）
       /// （oilDetails------油耗明细）//夏创铭（web）
       /// （oilChecked------油表查看）//武林林(web)

       /// ========告警统计========(3(2))
       /// （warnDetail----报警明细）//李浩文(web)
       /// （unitWarnReport-----单位报警统计）//李浩文(web)

       /// ========行车记录========
       /// （speedDetail-----速度明细）   //武林林(web)
       /// （overSpeedInAreaReport-----单车区域超速）//俞哲(web)
       /// （overSpeedTotalReport----区域超速汇总） //俞哲(web)

       /// ========里程分析========
       /// mileageReport---里程统计  //李浩文(web)
       /// mileageDetail----里程明细  //李浩文(web)
       /// totalMileage----行驶总里程  //李浩文(web)
       /// </summary>
       #region 油耗分析
         
       public static readonly string getOilListUri = webServerUri + "Reports/getOilList.ashx";//油耗明细
       public static readonly string getOilStatisticUri=webServerUri + "Reports/getOilStatistic.ashx"; //油耗统计        
       public static readonly string getCurrentOilUri = webServerUri + "Reports/getCurrentOil.ashx";//油表查看地址       
       #endregion
       #region 行车记录
       public static readonly string getSpeedDetails = webServerUri + "Reports/getSpeedDetails.ashx";//速度明细
       //............开发中.................//平均速度分析
       //............开发中.................//事故疑点数据
       public static readonly string getOverSpeedVehicleUri = webServerUri + "Reports/GetOverSpeedVehicle.ashx"; //单车区域超速  区域超速汇总
       //public static readonly string getOverSpeedVehicleUri = webServerUri + "Reports/GetOverSpeedVehicle.ashx"; //区域超速汇总

	    #endregion
       #region 告警分析
       public static readonly string getUnitWarnUri = webServerUri + "Reports/getUnitWarn.ashx";//单位报警
       public static readonly string getWarnDetailsUri = webServerUri + "Reports/getWarnDetails.ashx"; //报警明细
       #endregion
       #region 运行分析
       public static readonly string getAccAnalysisUri = webServerUri + "Reports/AccAnalysisData.ashx";//acc分析
       public static readonly string getOfflineVehicleUri = webServerUri + "Reports/GetOfflineVehicle.ashx";//离线分析
       public static readonly string getParkingWithAccOnDataUri = webServerUri + "Reports/ParkingWithAccOnData.ashx"; //停车统计  //未熄火停车
       public static readonly string getHistoryTracksUri = webServerUri + "Reports/GetHistoryTracks.ashx";//历史轨迹
       public static readonly string  getRegionInOutReportDataUri = webServerUri + "Reports/RegionInOutReportData.ashx";//进出区域明细
       //............开发中..................//卸料统计
      public static readonly string getOnlineOfflineReportDataUri = webServerUri + "Reports/OnlineOfflineReportData.ashx";//上下线明细
      //public static readonly string  getParkingWithAccOnDataUri = webServerUri + "Reports/ParkingWithAccOnData.ashx";//未熄火停车
      public static readonly string getMovingAnalysisDataUri = webServerUri + "Reports/MovingAnalysisData.ashx"; //行车统计
      //............开发中.................//Can数据明细
       #endregion
       #region 里程分析
        public static readonly string getVehicleMileageTotalUri=webServerUri + "Reports/getVehicleMileageTotal.ashx";//   行驶总里程
        public static readonly string getVehicleMileageDetailUri=webServerUri + "Reports/getVehicleMileageDetail.ashx";  // 里程明细
        public static readonly string  getVehicleMileageByDayUri= webServerUri + "Reports/getVehicleMileageByDay.ashx"; // 里程统计
       #endregion
       public static void get()
       {

       }
    }
}
