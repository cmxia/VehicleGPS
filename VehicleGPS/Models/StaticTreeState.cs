using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VehicleGPS.Models
{
    /*此类用于判定车辆和用户基本信息以及车辆详细信息的加载情况*/
    enum LoadingState
    {
        NOLOADING,//还未加载
        LOADING,//正在加载
        LOADCOMPLETE,//加载完成
        LOADDINGFAIL//加载失败
    };
    enum OnlineState
    {
        Online,//在线
        Offline,//掉线
    };
    static class StaticTreeState
    {
        public static OnlineState ClientIsOnline = OnlineState.Offline;//客户端是否在线
        public static LoadingState VehicleBasicInfo = LoadingState.NOLOADING;//车辆关键信息加载状态
        public static LoadingState RegionBasicInfo = LoadingState.NOLOADING;//车辆关键信息加载状态
        public static LoadingState ClientBasicInfo = LoadingState.NOLOADING;//用户关键信息加载状态
        public static LoadingState VehicleGPSInfo = LoadingState.NOLOADING;//车辆详细GPS信息加载状态
        public static LoadingState VehicleAllBasicInfo = LoadingState.NOLOADING;//车辆全部基础信息加载状态
        public static LoadingState BasicTypeInfo = LoadingState.NOLOADING;//所有类型数据
        public static LoadingState RigthInfo = LoadingState.NOLOADING;//所有权限类型数据
        public static LoadingState InstructionInfo = LoadingState.NOLOADING;//所有指令权限数据
        public static LoadingState WarnSettinInfo = LoadingState.NOLOADING;//所有需要显示的报警类型
        public static LoadingState WarnInfo = LoadingState.LOADCOMPLETE;//所有需要显示的报警类型
        public static LoadingState MessageInfo = LoadingState.LOADCOMPLETE;//消息是否已经准备好
        public static LoadingState ClusterReady = LoadingState.LOADCOMPLETE;//聚类完成
        public static LoadingState SendText = LoadingState.LOADCOMPLETE;//发送短语列表准备好
        public static LoadingState ReceivedText = LoadingState.LOADCOMPLETE;//接收短语列表准备好
        public static LoadingState CmdStatus = LoadingState.LOADCOMPLETE;//指令状态列表准备好
        public static LoadingState DownLoadComplete = LoadingState.LOADING;//下载文件完成

        public static LoadingState StopInfoLoad = LoadingState.LOADING;//停车数据加载完成
        public static LoadingState OverSpeedInfoLoad = LoadingState.LOADING;//超速数据加载完成
        public static LoadingState GpsWarnInfoLoad = LoadingState.LOADING;//报警数据加载完成
        public static LoadingState GpsOnlineInfoLoad = LoadingState.LOADING;//上下线数据加载完成
        public static LoadingState StopAddrLoad = LoadingState.LOADING;//停车数据地址加载完成
        public static LoadingState OverSpeedAddrLoad = LoadingState.LOADING;//超速数据地址加载完成
        public static LoadingState GpsWarnAddrLoad = LoadingState.LOADING;//报警数据地址加载完成
        public static LoadingState GpsOnlineAddrLoad = LoadingState.LOADING;//上下线数据地址加载完成
        public static LoadingState GpsDetailAddrLoad = LoadingState.LOADING;//上下线数据地址加载完成


        public static LoadingState DispatchTreeLoad = LoadingState.LOADING;//调度树形可用
        public static LoadingState DispatchCenter = LoadingState.LOADING;//调度中心可用
        public static LoadingState DispatchCenterPage = LoadingState.LOADCOMPLETE;//调度中心分页完成可用
        public static LoadingState DispatchTreeRefresh = LoadingState.LOADING;//调度树形刷新

        public static object VehicleBasicMutex = new object();
        public static object ClientBasicMutex = new object();
        public static object RegionBasicMutex = new object();
        public static object VehicleGPSInfoMutex = new object();
        public static object VeicleAllBasicMutex = new object();
        public static object BasicTypeMutex = new object();
        public static object TreeMutex = new object();


        /*树是否已经创建*/
        public static bool RealTimeTreeContruct = false;
        public static bool DispatchTreeContruct = false;
        public static bool ReportTreeContruct = false;
        /*是否正在刷新数据*/
        public static bool IsRefreshRealTimeData = false;
        /*历史轨迹数据是否已经准备好*/
        public static bool TrackPlayDataIsReady = false;

        public static LoadingState CVBaseInfo = LoadingState.NOLOADING;//加载车辆查询数据
        public static object CVBaseMutex = new object();

        /*视图是否已经创建*/
        public static bool RealTimeViewContruct = false;
        public static bool VehicleDispatchViewContruct = false;
        public static bool RealTimeMapViewContruct = false;
    }
}
