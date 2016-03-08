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

var prePoint = null;//前一个点
var m_Points = [];//所有点
var m_InfoWindow = []; //标注点弹窗
//设置标注
function SetMarker(lng, lat, cont, icon) {
    var myIcon = new BMap.Icon(icon, new BMap.Size(25, 25));
    var point = new BMap.Point(lng, lat);
    var marker = new BMap.Marker(point, { icon: myIcon });
//    var label = new BMap.Label(name);
//    label.setOffset(new BMap.Size(-17, 25));
//    label.setStyle({ color: "white", backgroundColor: "#007898", border: "none" });
//    marker.setLabel(label);
//    marker.setTitle(name);

    var infoWindow = new BMap.InfoWindow(cont, { width: 280, height: 120, offset: new BMap.Size(0, -15) }); // 创建信息窗口对象    
    marker.addEventListener("click", function () {
        m_Map.openInfoWindow(infoWindow, point);      // 打开信息窗口 
    })
    marker.addEventListener("mouseover", function () {
        marker.setTop(true);
    })
    marker.addEventListener("mouseout", function () {
        marker.setTop(false);
    })
    m_Points.push(point);
    m_InfoWindow.push(infoWindow);
    m_Map.addOverlay(marker);
    m_Map.centerAndZoom(point, 17);
    /*画轨迹*/
    if (prePoint != null) {
        var points = new Array();
        points.push(prePoint);
        points.push(point);
        var polyline = new BMap.Polyline(points, { strokeColor: "#5b849e", strokeWeight: 5, strokeStyle: "solid" });
        m_Map.addOverlay(polyline);
    }
    prePoint = point;
}

/*获得一个marker的焦点*/
function FocusMarker(lng, lat) {
    for (var i = 0; i < m_Points.length; i++) {
        var point = m_Points[i];
        var infoWondow = m_InfoWindow[i];
        if (point.lng == lng && point.lat == lat) {
            m_Map.zoomTo(13);
            m_Map.panTo(point);
            m_Map.openInfoWindow(infoWondow,point);
            break;
        }
    }
}

/*移除地图所有覆盖物*/
function RemoveAllOverlays() {
    prePoint = null;
    m_Map.clearOverlays();
}