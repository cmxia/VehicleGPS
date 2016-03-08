using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VehicleGPS.Views.Control.DispatchCentre.VehicleDispatch
{
    public delegate void MouseEnter(object sender, MouseEventArgs e);
    public delegate void MouseMove(object sender, MouseEventArgs e);
    public delegate void MouseLeave(object sender, MouseEventArgs e);
    /// <summary>
    /// 
    /// CarPositionControlDown.xaml 的交互逻辑
    /// </summary>
    public partial class CarPositionControlDown : Window
    {
        private string startPoint;  //起始点
        private string endPoint;  //结束点
        private string sim;  //sim卡号
        private string wholeDistance;  //总距离
        private string transedDistance;  //已运输距离

        #region 属性

        public string StartPoint
        {
            get
            {
                return startPoint;
            }
            set
            {
                startPoint = value;
            }
        }

        public string EndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                endPoint = value;
            }
        }

        public string SIM
        {
            get
            {
                return sim;
            }
            set
            {
                sim = value;
            }
        }

        public string WholeDistance
        {
            get
            {
                return wholeDistance;
            }
            set
            {
                wholeDistance = value;
            }
        }

        public string TransedDistance
        {
            get
            {
                return transedDistance;
            }
            set
            {
                transedDistance = value;
            }
        }

        #endregion

        private MouseEnter onMouseEnter;
        private MouseMove onMouseMove;
        private MouseLeave onMouseLeave;

        public CarPositionControlDown(MouseEnter MouseEnterEvent, MouseMove MouseMoveEvent, MouseLeave MouseLeaveEvent, string SIM)
        {
            // 为初始化变量所必需
            InitializeComponent();

            this.SIM = SIM;
            this.onMouseEnter += MouseEnterEvent;
            this.onMouseMove += MouseMoveEvent;
            this.onMouseLeave += MouseLeaveEvent;

            this.MouseEnter += new MouseEventHandler(CarPositionControlDown_MouseEnter);
            this.MouseMove += new MouseEventHandler(CarPositionControlDown_MouseMove);
            this.MouseLeave += new MouseEventHandler(CarPositionControlDown_MouseLeave);
        }

        void CarPositionControlDown_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.onMouseEnter != null)
                onMouseEnter(this, e);
        }

        void CarPositionControlDown_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.onMouseMove != null)
                onMouseMove(this, e);
        }

        void CarPositionControlDown_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.onMouseLeave != null)
                onMouseLeave(this, e);
        }
    }
}
