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
using System.Windows.Media.Animation;

namespace VehicleGPS.Views.Warn
{
    /// <summary>
    /// FloatWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FloatWindow : Window
    {
        public List<TipMsg> tipMsgList = new List<TipMsg>();
        public FloatWindow()
        {
            InitializeComponent();
            this.Top = SystemParameters.FullPrimaryScreenHeight - this.Height + 28;
            this.Left = SystemParameters.FullPrimaryScreenWidth - this.Width;

        }
        /*刷新数据*/
        public void RefreshData()
        {
            try
            {
                lv_TipMsgList.ItemsSource = this.tipMsgList; 
            }
            catch (Exception e)
            {

                throw;
            }
        }
        /*显示浮窗*/
        public void ShowFloat()
        {
            DoubleAnimation heightAnimation = new DoubleAnimation(0, this.Height
                , new Duration(TimeSpan.FromSeconds(0.5)));
            this.BeginAnimation(Window.HeightProperty, heightAnimation);

        }
        /*隐藏浮窗*/
        public void HideFloat()
        {
            DoubleAnimation heightAnimation = new DoubleAnimation(this.Height, 0
                , new Duration(TimeSpan.FromSeconds(0.5)));
            this.BeginAnimation(Window.HeightProperty, heightAnimation);
        }

    }
}
