using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using VehicleGPS.Models.DispatchCentre.VehicleDispatch;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Input;
using VehicleGPS.Views.Control.MonitorCentre.VehicleTrack;

namespace VehicleGPS.ViewModels.DispatchCentre.VehicleDispatch
{
    public class DispatchPointViewModel : NotificationObject
    {
        public DispatchPointViewModel()
        {
            /*图片鼠标移近移出事件*/
            this.ImageMouseMove = new DelegateCommand<object>(new Action<object>(this.ImageMouseMoveExecute));
            this.ImageMouseLeave = new DelegateCommand(new Action(this.ImageMouseLeaveExecute));
            this.MouseMovePopup = new DelegateCommand(new Action(this.MouseMovePopupExecute));
            this.MouseLeavePopup = new DelegateCommand(new Action(this.MouseLeavePopupExecute));
            this.ImageMouseDown = new DelegateCommand<object>(new Action<object>(this.ImageMouseDownExecute));
            /*标注点动画，查看重叠的上一部车辆*/
            this.PrePopUp = new DelegateCommand<ListView>(new Action<ListView>(this.PrePopUpExecute));
            /*标注点动画，查看重叠的下一部车辆*/
            this.NextPopUp = new DelegateCommand<ListView>(new Action<ListView>(this.NextPopUpExecute));

            /*车辆跟踪*/
            this.VehicleTrackCommand = new DelegateCommand(new Action(this.VehicleTrackCommandExecute));
        }
        public DelegateCommand VehicleTrackCommand { get; set; }//车辆跟踪
        private void VehicleTrackCommandExecute()
        {
            VehicleTrack win = new VehicleTrack(this.VehicleId);
            win.ShowDialog();
        }
        public string Sim { get; set; }//sim卡号
        public string VehicleNum { get; set; }//车辆编号
        public string VehicleId { get; set; }//车牌号
        public string InnerId { get; set; }//内部编号

        //内部编号的可见性
        private Visibility inneridVisibility;

        public Visibility InnerIdVisibility
        {
            get { return inneridVisibility; }
            set
            {
                inneridVisibility = value;
                this.RaisePropertyChanged("InnerIdVisibility");
            }
        }
        //车牌号的可见性
        private Visibility vehicleIdVisibility;

        public Visibility VehicleIdVisibility
        {
            get { return vehicleIdVisibility; }
            set
            {
                vehicleIdVisibility = value;
                this.RaisePropertyChanged("VehicleIdVisibility");
            }
        }


        public TaskInfo TaskNumberInfo { get; set; }//任务单信息
        /*重叠车辆信息*/
        private List<DispatchVehicleViewModel> listOverlapVehicle = new List<DispatchVehicleViewModel>();
        public List<DispatchVehicleViewModel> ListOverlapVehicle
        {
            get { return listOverlapVehicle; }
            set
            {
                if (listOverlapVehicle != value)
                {
                    listOverlapVehicle = value;
                    this.RaisePropertyChanged("ListOverlapVehicle");
                }
            }
        }
        private int overlapCount;//重叠的个数
        public int OverlapCount
        {
            get { return overlapCount; }
            set
            {
                if (overlapCount != value)
                {
                    overlapCount = value;
                    if (overlapCount >= 99)
                    {//最大99不车重叠
                        overlapCount = 99;
                    }
                    this.RaisePropertyChanged("OverlapCount");
                }
            }
        }
        private Thickness imageMargin = new Thickness(0);
        public Thickness ImageMargin
        {
            get { return imageMargin; }
            set
            {
                if (imageMargin != value)
                {
                    imageMargin = value;
                    this.RaisePropertyChanged("ImageMargin");
                }
            }
        }
        private double marginDistance;//显示距离
        public double MarginDistance
        {
            get { return marginDistance; }
            set
            {
                if (marginDistance != value)
                {
                    marginDistance = value;
                    //this.ImageMargin = new Thickness(this.marginDistance, 0, 0, 0);
                }
            }
        }

        #region 图片鼠标移近移出事件
        /*是否显示Popup*/
        private bool showPopup = false;
        public bool ShowPopup
        {
            get { return showPopup; }
            set
            {
                if (showPopup != value)
                {
                    showPopup = value;
                    this.RaisePropertyChanged("ShowPopup");
                }
            }
        }
        private int PopupWinHeight = 240;//冒泡窗口高度
        private int popupVerticalOffset;//冒泡窗口垂直偏移量
        public int PopupVerticalOffset
        {
            get { return popupVerticalOffset; }
            set
            {
                if (popupVerticalOffset != value)
                {
                    popupVerticalOffset = value;
                    this.RaisePropertyChanged("PopupVerticalOffset");
                }
            }
        }
        private Visibility downVisibility;//冒泡窗口箭头朝下
        public Visibility DownVisibility
        {
            get { return downVisibility; }
            set
            {
                if (downVisibility != value)
                {
                    downVisibility = value;
                    this.RaisePropertyChanged("DownVisibility");
                }
            }
        }
        private Visibility upVisibility;//冒泡窗口箭头朝上
        public Visibility UpVisibility
        {
            get { return upVisibility; }
            set
            {
                if (upVisibility != value)
                {
                    upVisibility = value;
                    this.RaisePropertyChanged("UpVisibility");
                }
            }
        }
        public DelegateCommand<object> ImageMouseDown { get; set; }
        private void ImageMouseDownExecute(object gridObject)
        {
            Grid grid = (Grid)gridObject;
            /*调整冒泡窗口显示位置*/
            Point clickPoint = grid.PointFromScreen(new Point(0, 0));
            if (clickPoint.Y * -1 < this.PopupWinHeight)
            {
                this.DownVisibility = Visibility.Collapsed;
                this.UpVisibility = Visibility.Visible;
                this.PopupVerticalOffset = 130;
            }
            else
            {
                this.DownVisibility = Visibility.Visible;
                this.UpVisibility = Visibility.Collapsed;
                this.PopupVerticalOffset = -130;
            }
            this.ShowPopup = !this.ShowPopup;
        }
        public DelegateCommand<object> ImageMouseMove { get; set; }
        private void ImageMouseMoveExecute(object gridObject)
        {
            if (this.ShowPopup)
            {
                return;
            }
            this.ImageMouseDownExecute(gridObject);
        }
        public DelegateCommand ImageMouseLeave { get; set; }
        private void ImageMouseLeaveExecute()
        {
            this.MouseLeavePopupExecute();
        }

        public DelegateCommand MouseMovePopup { get; set; }
        private void MouseMovePopupExecute()
        {

        }
        public DelegateCommand MouseLeavePopup { get; set; }
        private void MouseLeavePopupExecute()
        {
            this.ShowPopup = false;
        }
        #endregion

        #region 标注点动画，上一步和下一步车辆
        public DelegateCommand<ListView> PrePopUp { get; set; }
        private int CurrentIndex = 0;//当前popup显示的项的索引
        private void PrePopUpExecute(ListView lv)
        {
            if (this.listOverlapVehicle.Count == 1 || CurrentIndex == 0)
            {
                return;
            }
            /*动画切换到上一张*/
            Thickness from = new Thickness(CurrentIndex * 350 * -1, 0, 0, 0);
            Thickness to = new Thickness((CurrentIndex - 1) * 350 * -1, 0, 0, 0);
            ThicknessAnimation ta = new ThicknessAnimation(from, to, new Duration(TimeSpan.FromSeconds(0.5)));
            lv.BeginAnimation(ListView.MarginProperty, ta);
            this.CurrentIndex--;
        }
        public DelegateCommand<ListView> NextPopUp { get; set; }
        private void NextPopUpExecute(ListView lv)
        {
            if (this.listOverlapVehicle.Count == 1 || this.CurrentIndex == this.listOverlapVehicle.Count - 1)
            {
                return;
            }
            /*动画切换到下一张*/
            Thickness from = new Thickness(CurrentIndex * 350 * -1, 0, 0, 0);
            Thickness to = new Thickness((CurrentIndex + 1) * 350 * -1, 0, 0, 0);
            ThicknessAnimation ta = new ThicknessAnimation(from, to, new Duration(TimeSpan.FromSeconds(0.5)));
            lv.BeginAnimation(ListView.MarginProperty, ta);
            this.CurrentIndex++;
        }
        #endregion
    }
}
