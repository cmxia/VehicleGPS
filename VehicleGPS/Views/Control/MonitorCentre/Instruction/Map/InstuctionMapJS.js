
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


/*移除地图所有覆盖物*/
function RemoveAllOverlays() {
    m_Map.clearOverlays();
}

var overlays = [];
var infoWindows = []; //标注点弹窗
var Lines = ""; //折线
var Rects = ""; //矩形
var PolyGons = ""; //多边形
var circle = ""; //圆

function getLine() {
    return Lines;
}
function getCircle() {
    return circle;
}
function getRect() {
    return Rects;
}
function getPoly() {
    return PolyGons;
}
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
        circle = "";
        circle += e.overlay.getRadius() + ';' + e.overlay.getCenter().lng + ";" + e.overlay.getCenter().lat + ";" + e.calculate;
        point = new BMap.Point(e.overlay.getCenter().lng, e.overlay.getCenter().lat);
    }
    if (e.drawingMode == BMAP_DRAWING_POLYLINE) {
        result += '<br> 图形：' + "直线";
        result += '<br> 点个数：' + e.overlay.getPath().length;
        var points = e.overlay.getPath();
        Lines = "";
        for (var i = 0; i < points.length; i++) {
            Lines += points[i].lng + ',' + points[i].lat + ';';
        }
        Lines += points.length + ';' + e.calculate;
        result += '<br> 直线长度（米）：' + e.calculate;
        point = new BMap.Point(e.overlay.getPath()[0].lng, e.overlay.getPath()[0].lat);
    }
    if (e.drawingMode == BMAP_DRAWING_POLYGON) {
        result += '<br> 图形：' + "多边形";
        result += '<br> 点个数：' + e.overlay.getPath().length;
        result += '<br> 面积（平方米）：' + e.calculate;
        var points = e.overlay.getPath();
        PolyGons = "";
        for (var i = 0; i < points.length; i++) {
            PolyGons += points[i].lng + ',' + points[i].lat + ';';
        }
        PolyGons += points.length + ';' + e.calculate;
        point = new BMap.Point(e.overlay.getPath()[0].lng, e.overlay.getPath()[0].lat);
    }
    if (e.drawingMode == BMAP_DRAWING_RECTANGLE) {
        result += '<br> 图形：' + "矩形";
        result += '<br> 点个数：' + e.overlay.getPath().length;

        result += '<br> 面积（平方米）：' + e.calculate;
        var points = e.overlay.getPath();
        Rects = "";
        for (var i = 0; i < points.length; i = i + 2) {
            Rects += points[i].lng + ',' + points[i].lat + ';';
        }
        Rects += e.calculate;
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
//打开折线画图工具
function OpenDrawPolyLineManager() {
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
            BMAP_DRAWING_POLYLINE
           ]
        },
        circleOptions: styleOptions, //圆的样式
        polylineOptions: styleOptions, //线的样式
        polygonOptions: styleOptions, //多边形的样式
        rectangleOptions: styleOptions //矩形的样式
    });
    drawingManager.setDrawingMode(BMAP_DRAWING_POLYLINE);
    //添加鼠标绘制工具监听事件，用于获取绘制结果
    drawingManager.addEventListener('overlaycomplete', overlaycomplete);
}
//打开圆形画图工具
function OpenDrawCircleManager() {
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
            BMAP_DRAWING_CIRCLE
           ]
        },
        circleOptions: styleOptions, //圆的样式
        polylineOptions: styleOptions, //线的样式
        polygonOptions: styleOptions, //多边形的样式
        rectangleOptions: styleOptions //矩形的样式
    });
    drawingManager.setDrawingMode(BMAP_DRAWING_CIRCLE);
    //添加鼠标绘制工具监听事件，用于获取绘制结果
    drawingManager.addEventListener('overlaycomplete', overlaycomplete);
}
//打开多边形画图窗口
function OpenDrawPolyGonManager() {
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
            BMAP_DRAWING_POLYGON
           ]
        },
        circleOptions: styleOptions, //圆的样式
        polylineOptions: styleOptions, //线的样式
        polygonOptions: styleOptions, //多边形的样式
        rectangleOptions: styleOptions //矩形的样式
    });
    drawingManager.setDrawingMode(BMAP_DRAWING_POLYGON);
    //添加鼠标绘制工具监听事件，用于获取绘制结果
    drawingManager.addEventListener('overlaycomplete', overlaycomplete);
}
//打开矩形画图窗口
function OpenDrawRectManager() {
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
            BMAP_DRAWING_RECTANGLE
           ]
        },
        circleOptions: styleOptions, //圆的样式
        polylineOptions: styleOptions, //线的样式
        polygonOptions: styleOptions, //多边形的样式
        rectangleOptions: styleOptions //矩形的样式
    });
    drawingManager.setDrawingMode(BMAP_DRAWING_RECTANGLE);
    //添加鼠标绘制工具监听事件，用于获取绘制结果
    drawingManager.addEventListener('overlaycomplete', overlaycomplete);
}
//打开所有类型的覆盖物画图
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