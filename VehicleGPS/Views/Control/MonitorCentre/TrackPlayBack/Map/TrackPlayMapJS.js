
var m_Map; //地图对象
/*<summary>初始化地图</summary>
*<param name="lng">经度</param>
*<param name="lat">纬度</param>
*<param name="zoom">地图级别</param>
*<return></return>
*/
function InitBaiduMap(lng, lat, zoom) {
    m_Map = new BMap.Map("MapDiv");               // 创建Map实例
    m_Map.centerAndZoom(new BMap.Point(lng, lat), zoom);
    m_Map.addControl(new BMap.NavigationControl());   // 添加平移缩放控件  
    m_Map.addControl(new BMap.ScaleControl());        // 添加比例尺控件  
    m_Map.addControl(new BMap.OverviewMapControl());  //添加缩略地图控件  
    m_Map.addControl(new BMap.MapTypeControl());      //添加地图类型控件  
    m_Map.enableScrollWheelZoom();                    //启用滚轮放大缩小
    m_Map.addEventListener("dblclick", ZoomIn);
}
//放大地图一级
function ZoomIn() {
    m_Map.zoomIn();
}




/*根据点Point设置轨迹路径*/
function SetTrack() {
    var polyline = new BMap.Polyline(m_Points, {strokeColor:"#5b849e",strokeWeight:5,strokeStyle:"solid"});
    m_Map.addOverlay(polyline);
    for (var i = 0; i < m_Markers.length; i++) {
        m_Map.addOverlay(m_Markers[i]);
    }
}

var m_Points = [];//所有点
var m_Markers = []; //个别标注点数组
var m_InfoWindow = []; //个别标注点弹窗
var m_MarkerInterval = 10;//在历史轨迹中间隔几个放一个方向标
/*初始化轨迹地图*/
function InitTrackPlayMap() {
    RemoveInfoWindows();
    RemoveMarkers();
    RemovePoints();
    RemoveAllOverlays();
}
/*初始化轨迹回放数据*
*<param name="lng">经度</param>
*<param name="lat">纬度</param>
*<param name="cont">点击标注后弹窗内容</param>
*<return></return>
*/
function InitTrackPlayData(lngs, lats, conts, icons) {

    var myLngs = new Array();
    var myLats = new Array();
    var myConts = new Array();
    var myIcons = new Array();

    myLngs = lngs.split("$");
    myLats = lats.split("$");
    myConts = conts.split("$");
    myIcons = icons.split("$");
    
    for (var i = 0; i < myLngs.length; i++) {
        var point = new BMap.Point(myLngs[i], myLats[i]);
        var cont = myConts[i];
        m_Points.push(point);
        if (i == 0 || i % m_MarkerInterval == 0 || i == myLngs.length - 1) {
            var myIcon = new BMap.Icon(myIcons[i], new BMap.Size(25, 25));
            var marker = new BMap.Marker(point, { icon: myIcon });

//            var infoWindow = new BMap.InfoWindow(cont, { width: 250, height: 100, offset: new BMap.Size(0, -15) }); // 创建信息窗口对象    
//            marker.addEventListener("click", function () {
//                this.openInfoWindow(infoWindow);      // 打开信息窗口 
//            })
            //m_InfoWindow.push(infoWindow);
            m_Markers.push(marker);
        }
    }
    m_Map.centerAndZoom(new BMap.Point(myLngs[0], myLats[0]), 15);
    SetTrack();
}
/*移动标注*/
var m_MoveMarker = null;
var m_MoveInfoWindow = null;
/*设置移动标注*/
function SetMoveMarker(lng, lat, cont, iconUrl) {
    if (m_MoveMarker != null) {
        m_Map.removeOverlay(m_MoveMarker);
        delete m_MoveMarker;
        m_MoveMarker = null;
    }
    var myIcon = new BMap.Icon(iconUrl, new BMap.Size(25, 25));
    var myPoint = new BMap.Point(lng, lat);
    m_MoveMarker = new BMap.Marker(myPoint, {icon:myIcon});
    m_Map.addOverlay(m_MoveMarker);
    m_Map.setCenter(myPoint);
}
/*设置标注并显示消息框*/
function SetMoveMarkerAndShowInfoWindow(lng, lat, cont, iconUrl) {
    if (m_MoveMarker != null) {
        m_Map.removeOverlay(m_MoveMarker);
        delete m_MoveMarker;
        m_MoveMarker = null;
    }
    if (m_MoveInfoWindow != null) {
        m_Map.removeOverlay(m_MoveInfoWindow);
        delete m_MoveInfoWindow;
        m_MoveInfoWindow = null;
    }
    var myIcon = new BMap.Icon(iconUrl, new BMap.Size(25, 25));
    var myPoint = new BMap.Point(lng, lat);
    m_MoveMarker = new BMap.Marker(myPoint, { icon: myIcon });
    var infoWindow = new BMap.InfoWindow(cont, { width: 280, height: 120, offset: new BMap.Size(0, -15) }); // 创建信息窗口对象    
    m_Map.openInfoWindow(infoWindow, myPoint);      // 打开信息窗口 
    m_Map.addOverlay(m_MoveMarker);
    m_Map.setCenter(myPoint);
}
/*移除轨迹点*/
function RemovePoints() {
    for (var i = 0; i < m_Points.length; i++) {
        var point = m_Points.pop();
        delete point;
    }
    m_Points = [];
}
/*移除标注窗口*/
function RemoveInfoWindows() {
    for (var i = 0; i < m_InfoWindow.length; i++) {
        var infoWindow = m_InfoWindow.pop();
        delete infoWindow;
    }
    m_InfoWindow = [];
}
/*移除标注*/
function RemoveMarkers() {
    for (var i = 0; i < m_Markers.length; i++) {
        var marker = m_Markers.pop();
        delete marker;
    }
    m_Markers = [];
}
/*移除地图所有覆盖物*/
function RemoveAllOverlays() {
    m_Map.clearOverlays();
}