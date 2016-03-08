
var m_Map; //地图对象

/*<summary>初始化地图</summary>
*<param name="lng">经度</param>
*<param name="lat">纬度</param>
*<param name="zoom">地图级别</param>
*<return></return>
*/
function InitBaiduMap(city, zoom) {
    if (m_Map == null) {
        m_Map = new BMap.Map("MapDiv");               // 创建Map实例
    }
    m_Map.centerAndZoom(city, zoom);
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


var m_MarkerCluster = null; //标注点聚合
var m_Markers = []; //标注点数组
var m_InfoWindow = []; //标注点弹窗
var m_carNumber = []; //车牌号
var m_IsWindowOpen = []; //是否打开了信息窗口
/*<summary>在地图设置标志</summary>
*<param name="lng">经度</param>
*<param name="lat">纬度</param>
*<param name="cont">点击标注后弹窗内容,为""则不设置事件</param>
*<return></return>
*/
function SetMarker(name, lng, lat, cont, iconUrl) {

    var myIcon = new BMap.Icon(iconUrl, new BMap.Size(25, 25));
    var point = new BMap.Point(lng, lat);
    var t_Marker = new BMap.Marker(point, { icon: myIcon });
    var label = new BMap.Label(name);
    label.setOffset(new BMap.Size(-17, 25));
    label.setStyle({ color: "white", backgroundColor: "#007898", border: "none", textAlign: "center", width: "60px" });
    t_Marker.setLabel(label);
    t_Marker.setTitle(name);

    var t_InfoWindow = new BMap.InfoWindow(cont, { width: 300, height: 140, offset: new BMap.Size(0, -15) }); // 创建信息窗口对象    
    if (cont != "") {
        t_Marker.addEventListener("click", function () {
            m_Map.openInfoWindow(t_InfoWindow, point);      // 打开信息窗口 
        })
        t_Marker.addEventListener("mouseover", function () {
            t_Marker.setTop(true);
        })
        t_Marker.addEventListener("mouseout", function () {
            t_Marker.setTop(false);
        })
    }
    m_carNumber.push(name);
    m_Markers.push(t_Marker);
    m_InfoWindow.push(t_InfoWindow);
    m_IsWindowOpen.push(false);
}
/*设置标注数组为聚合*/
function SetMarkerCluster() {
    m_MarkerCluster = new BMapLib.MarkerClusterer(m_Map, { markers: m_Markers });
    //m_Map.reset();
}

/*-------添加标注点--------*/
function AddMarkers() {
    for (var i = 0; i < m_Markers.length; i++) {
        m_Map.addOverlay(m_Markers[i]);
    }
}

/*初始化实时监控地图*/
function InitRealTimeMap() {
    RemoveMarkerClusterer();
    RemoveMarkers();
    RemoveInfoWindows();
    RemoveCarNumbers();
    RemoveIsWindowOpen();
    RemoveAllOverlays();
}
/*移除标注窗口*/
function RemoveInfoWindows() {
    for (var i = 0; i < m_InfoWindow.length; i++) {
        var infoWindow = m_InfoWindow.pop();
        delete infoWindow;
    }
    m_InfoWindow = [];
}
function RemoveCarNumbers() {
    m_carNumber = [];
}
function RemoveIsWindowOpen() {
    m_IsWindowOpen = [];
}
/*移除标注*/
function RemoveMarkers() {
    for (var i = 0; i < m_Markers.length; i++) {
        var marker = m_Markers.pop();
        delete marker;
    }
    m_Markers = [];
}
/*移除聚合*/
function RemoveMarkerClusterer() {
    if (m_MarkerCluster != null) {
        m_MarkerCluster.removeMarkers(m_Markers);
        delete m_MarkerCluster;
        m_MarkerCluster = null;
    }
    //   m_Map.reset();
}
/*移除地图所有覆盖物*/
function RemoveAllOverlays() {
    m_Map.clearOverlays();
}

/*获得一个marker的焦点*/
function FocusMarker(name) {
    for (var i = 0; i < m_carNumber.length; i++) {
        if (name == m_carNumber[i]) {
            var marker = m_Markers[i];
            var point = marker.getPosition();
            var infoWondow = m_InfoWindow[i];
            m_Map.panTo(point);
            m_Map.openInfoWindow(infoWondow, point);
            break;
        }
    }
}
/*修改焦点*/
function ModMarker(name, lng, lat, cont, iconUrl) {
    for (var i = 0; i < m_carNumber.length; i++) {
        if (name == m_carNumber[i]) {
            var marker = m_Markers[i];
            var myIcon = new BMap.Icon(iconUrl, new BMap.Size(25, 25));
            var point = marker.getPosition();
            point.lat = lat;
            point.lng = lng;
            marker.setIcon(myIcon);
            var label = new BMap.Label(name);
            label.setOffset(new BMap.Size(-17, 25));
            label.setStyle({ color: "white", backgroundColor: "#007898", border: "none", textAlign: "center", width: "60px" });
            marker.setLabel(label);
            marker.setTitle(name);
            var infoWindow = m_InfoWindow[i];
            infoWindow.setContent(cont);
            break;
        }
    }
}
/*==================================================================================================*/
/*-----------------------------------标区·变量·函数----------------------------------*/
/*add by xiachuangming*/
var circle = null;  //标区对象
var circleLng = null, circleLat = null, circleRadius = null;
/*<summary>为地图添加标区监听事件</summary>
*<param name="Color">区域颜色</param>
*/
function addCircleListener() {
    //    if (m_Map.getZoom() < 14) {
    //        m_Map.setZoom(14); //设置地图级别为：14
    //    }
    m_Map.addEventListener("click", addCircle);
}
//在地图上添加一个圆：可编辑；并移除地图的标区监听事件
function addCircle(e) {
    var point = new BMap.Point(e.point.lng, e.point.lat);
    circle = new BMap.Circle(point, 200, {
        strokeColor: '#f00',         //圆边线颜色
        fillColor: '#f00',           //填充颜色
        fillOpacity: 0.35,          //填充透明度
        strokeWeight: 2              //边线宽度
    });
    m_Map.addOverlay(circle);
    circle.enableEditing();
    m_Map.removeEventListener("click", addCircle);
}
//移除点击监听事件
function removeclicklistener() {
    m_Map.removeEventListener("click", addCircle);
}
//返回圆的圆心经纬度和半径
function returnLngLatRadius() {
    if (circle == null) {
        return null;
    } else {
        circleLng = circle.getCenter().lng;
        circleLat = circle.getCenter().lat;
        circleRadius = circle.getRadius();
        return circleLng + ";" + circleLat + ";" + circleRadius;
    }
}
function removeCircleByAdd() {
    m_Map.removeOverlay(circle);
}
/*---------------------------------------------------------------------------------------------*/
//添加一个圆形覆盖物
function addCircleByOne(Onelng, Onelat, Oneradius, OneColor, OneName) {
    if (m_Map == null) {
        m_Map = new BMap.Map("MapDiv");               // 创建Map实例
    }
    var Onepoint = new BMap.Point(Onelng, Onelat);
    //alert(Onelng + "||" + Onelat + "||" + Oneradius + "||" + OneColor + "||" + OneName);
    var circle = new BMap.Circle(Onepoint, Oneradius, {
        strokeColor: OneColor,     //圆边线颜色
        fillColor: OneColor,       //填充颜色
        fillOpacity: 0.35,      //填充透明度
        strokeWeight: 2  //边线宽度
    });
    var label = new BMap.Label(OneName, { offset: new BMap.Size(-30, 0), position: Onepoint });
    label.setStyle({ color: "white", backgroundColor: "#ff7851", border: "none", textAlign: "center" });
    m_Map.addOverlay(label);
    m_Map.addOverlay(circle);
}
/*获得一个marker的焦点*/
function FocusMarker(lng, lat) {
    for (var i = 0; i < m_Markers.length; i++) {
        var marker = m_Markers[i];
        var point = marker.getPosition();
        var infoWondow = m_InfoWindow[i];
        if (point.lng == lng && point.lat == lat) {
            //m_Map.zoomTo(20);
            m_Map.panTo(point);
            m_Map.openInfoWindow(infoWondow, point);
            break;
        }
    }
}


var overlays = [];
var infoWindows = []; //标注点弹窗

//回调获得覆盖物信息
var overlaycomplete = function (e) {
    overlays.push(e.overlay);
    var result = "";
    var point = "";
    result = "<p>";
    e.label = null;

    if (e.drawingMode == BMAP_DRAWING_MARKER) {
        result += '<br> 图形：' + "点";
        result += '<br> 坐标：<' + e.overlay.getPosition().lng + ',' + e.overlay.getPosition().lat + '>';
        point = new BMap.Point(e.overlay.getPosition().lng, e.overlay.getPosition().lng);
    }
    if (e.drawingMode == BMAP_DRAWING_CIRCLE) {
        result += '<br> 图形：' + "圆形";
        result += '<br> 半径（米）：' + e.overlay.getRadius();
        result += '<br> 中心点：<' + e.overlay.getCenter().lng + "," + e.overlay.getCenter().lat + '>';
        result += '<br> 面积（平方米）：' + e.calculate;
        point = new BMap.Point(e.overlay.getCenter().lng, e.overlay.getCenter().lat);
    }
    if (e.drawingMode == BMAP_DRAWING_POLYLINE) {
        result += '<br> 图形：' + "直线";
        result += '<br> 点个数：' + e.overlay.getPath().length;

        result += '<br> 直线长度（米）：' + e.calculate;
        point = new BMap.Point(e.overlay.getPath()[0].lng, e.overlay.getPath()[0].lat);
    }
    if (e.drawingMode == BMAP_DRAWING_POLYGON) {
        result += '<br> 图形：' + "多边形";
        result += '<br> 点个数：' + e.overlay.getPath().length;
        result += '<br> 面积（平方米）：' + e.calculate;
        point = new BMap.Point(e.overlay.getPath()[0].lng, e.overlay.getPath()[0].lat);
    }
    if (e.drawingMode == BMAP_DRAWING_RECTANGLE) {
        result += '<br> 图形：' + "矩形";
        result += '<br> 点个数：' + e.overlay.getPath().length;

        result += '<br> 面积（平方米）：' + e.calculate;
        point = new BMap.Point(e.overlay.getPath()[0].lng, e.overlay.getPath()[0].lat);
    }
    result += "</p>";
    //    $("showOverlayInfo").style.display = "none";
    //    $("panel").innerHTML += result; //将绘制的覆盖物信息结果输出到结果面板
    var _infoWindow = new BMap.InfoWindow(result, { width: 300, height: 140, offset: new BMap.Size(0, -15) }); // 创建信息窗口对象 ;

    m_Map.openInfoWindow(_infoWindow, point);
    infoWindows.push(_infoWindow);
};

var styleOptions = {
    strokeColor: "red",    //边线颜色。
    fillColor: "red",      //填充颜色。当参数为空时，圆形将没有填充效果。
    strokeWeight: 3,       //边线的宽度，以像素为单位。
    strokeOpacity: 0.8,    //边线透明度，取值范围0 - 1。
    fillOpacity: 0.6,      //填充的透明度，取值范围0 - 1。
    strokeStyle: 'solid' //边线的样式，solid或dashed。
}

//实例化鼠标绘制工具
var drawingManager;


function OpenDrawingManager() {
    drawingManager = new BMapLib.DrawingManager(m_Map, {
        isOpen: true, //是否开启绘制模式
        enableDrawingTool: true, //是否显示工具栏
        enableCalculate: true,
        drawingToolOptions: {
            anchor: BMAP_ANCHOR_BOTTOM_RIGHT, //位置
            offset: new BMap.Size(5, 5), //偏离值
            scale: 0.8, //工具栏缩放比例
            drawingModes:
           [
            BMAP_DRAWING_CIRCLE,
            BMAP_DRAWING_POLYLINE,
            BMAP_DRAWING_POLYGON,
            BMAP_DRAWING_RECTANGLE
         ]
        },
        circleOptions: styleOptions, //圆的样式
        polylineOptions: styleOptions, //线的样式
        polygonOptions: styleOptions, //多边形的样式
        rectangleOptions: styleOptions //矩形的样式
    });
    //添加鼠标绘制工具监听事件，用于获取绘制结果
    drawingManager.addEventListener('overlaycomplete', overlaycomplete);
}

function CloseDrawingManager() {

    if (drawingManager != null) {

        drawingManager.close();
    }
    drawingManager = null;
    for (var i = 0; i < overlays.length; i++) {
        m_Map.removeOverlay(overlays[i]);
    }
    overlays.length = 0;
    infoWindows = [];

}

