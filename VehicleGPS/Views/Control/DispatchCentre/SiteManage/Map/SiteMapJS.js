
var m_Map; //地图对象

/*<summary>初始化地图</summary>
*<param name="lng">经度</param>
*<param name="lat">纬度</param>
*<param name="zoom">地图级别</param>
*<return></return>
*/
function InitBaiduMap(city, zoom) {
    m_Map = new BMap.Map("MapDiv");               // 创建Map实例
    m_Map.centerAndZoom(city, zoom);
    m_Map.addControl(new BMap.NavigationControl());   // 添加平移缩放控件 (默认) 
    m_Map.addControl(new BMap.ScaleControl());        // 添加比例尺控件  
    m_Map.addControl(new BMap.OverviewMapControl());  //添加缩略地图控件  
    m_Map.addControl(new BMap.MapTypeControl());      //添加地图类型控件  
    m_Map.enableScrollWheelZoom();                    //启用滚轮放大缩小（地图拖拽）
    m_Map.addEventListener("dblclick", ZoomIn);//双击放大一级
}
//放大地图一级
function ZoomIn() {
    m_Map.zoomIn();
}



/*初始化地图*/
function InitSiteMap() {
    RemoveAllOverlays();
}


/*移除地图所有覆盖物*/
function RemoveAllOverlays() {
    m_Map.clearOverlays();
}
//标注站下的区域和工地
function addCircleByOne(Onelng, Onelat, Oneradius, OneColor, OneName) {
    var Onepoint = new BMap.Point(Onelng, Onelat);
    if (m_Map.getZoom() < 14) {
        m_Map.centerAndZoom(Onepoint, 14);  //设置地图级别为：14
    } else {
        m_Map.setCenter(Onepoint);
    }
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
/*==================================================================================================*/
/*-----------------------------------标区·变量·函数----------------------------------*/
/*add by xiachuangming*/
var circle = null;  //标区对象
var circleLng = null, circleLat = null, circleRadius = null;
/*<summary>为地图添加标区监听事件</summary>
*<param name="Color">区域颜色</param>
*/
function addCircleListener() {
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