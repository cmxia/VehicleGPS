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
using System.Timers;
using System.Windows.Threading;
namespace VehicleGPS.Models.Dialog
{
    /// <summary>
    /// PopupBorder.xaml 的交互逻辑
    /// </summary>
    public partial class PopupBorder : Window
    {
        private Timer timer = new Timer(3000);
        public PopupBorder(string msg, bool isRightDownOrNot)
        {
            InitializeComponent();

            this.txtMessage.Text = msg;
            this.Height = 70;
            if (null != msg)
            {
                this.Width = 25 * msg.Length;
            }
            else
            {
                this.Width = 200;
            }
            if (true == isRightDownOrNot)
            {

                isRdPosition();
            }

        }
        public PopupBorder(string msg, int width, int height, bool isRightDownOrNot)
        {
            InitializeComponent();
            this.txtMessage.Text = msg;
            this.Height = height;
            this.Width = width;
            if (true == isRightDownOrNot)
            {

                isRdPosition();
            }
        }
        public void poupClosed()
        {
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
            {
                this.Hide();
            });
        }
        public void isRdPosition()
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;//设置为默认或者自定义位置。            
            Left = SystemParameters.FullPrimaryScreenWidth - SystemParameters.MinimizedWindowWidth - this.Width + 150;
            Top = SystemParameters.FullPrimaryScreenHeight - SystemParameters.MinimumWindowHeight;
        }


    }
}
