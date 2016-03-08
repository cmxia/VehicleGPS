
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

/*<summary>在地图设置标志</summary>
*<param name="lng">经度</param>
*<param name="lat">纬度</param>
*<param name="cont">点击标注后弹窗内容,为""则不设置事件</param>
*<return></return>
*/
function SetMarker(lng, lat, cont) {
    RemoveAllOverlays();
    var point = new BMap.Point(lng, lat);
    var t_Marker = new BMap.Marker(point);
    var t_InfoWindow = new BMap.InfoWindow(cont, { width: 280, height: 120 }); // 创建信息窗口对象    
    if (cont != "") {
        t_Marker.addEventListener("click", function () {
            this.openInfoWindow(t_InfoWindow);      // 打开信息窗口 
        })
    }
    m_Map.addOverlay(t_Marker);
    t_Marker.openInfoWindow(t_InfoWindow); // 打开信息窗口 
}

/*移除地图所有覆盖物*/
function RemoveAllOverlays() {
    m_Map.clearOverlays();
}

/*超速路段*
*<param name="lngs">经度</param>
*<param name="lats">纬度</param>
*<param name="conts">点击标注后弹窗内容</param>
*<return></return>
*/
function OverSpeedTrack(lngs, lats, conts) {

    RemoveAllOverlays()
    var myLngs = new Array();
    var myLats = new Array();
    var myConts = new Array();

    myLngs = lngs.split("$");
    myLats = lats.split("$");
    myConts = conts.split("$");

    var m_Points = []; //所有点
    for (var i = 0; i < myLngs.length; i++) {
        var point = new BMap.Point(myLngs[i], myLats[i]);
        var cont = myConts[i];
        m_Points.push(point);
        var marker = addMarker(point, cont);
        m_Map.addOverlay(marker);
    }
    m_Map.centerAndZoom(new BMap.Point(myLngs[0], myLats[0]), 15);
    var polyline = new BMap.Polyline(m_Points, { strokeColor: "#5b849e", strokeWeight: 5, strokeStyle: "solid" });
    m_Map.addOverlay(polyline);
}

addMarker = function (point, cont) {
    var _marker = new BMap.Marker(point);
    _marker.addEventListener("click", function (e) {
        var infoWindow = new BMap.InfoWindow(cont, { width: 250, height: 100 }); // 创建信息窗口对象  
        this.openInfoWindow(infoWindow);
    });
    return _marker;
}
