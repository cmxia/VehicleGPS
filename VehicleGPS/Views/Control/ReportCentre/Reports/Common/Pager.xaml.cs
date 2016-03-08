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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VehicleGPS.Views.Control.ReportCentre.Reports.Common
{

    public delegate void PagerIndexChangingEvent(int pageIndex, EventArgs e);

    /// <summary>
    /// Pager.xaml 的交互逻辑
    /// </summary>
    public partial class Pager : UserControl
    {
        public Pager(int count,int pageSize)
        {
            InitializeComponent();
            this.PageSize = pageSize;
            this.Count = count;
        }

        public event PagerIndexChangingEvent PageIndexChanging;

        /// <summary>
        /// 当前页索引
        /// </summary>
        private int pageIndex = 0;

        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }
        /// <summary>
        /// 总页数
        /// </summary>
        private int pageCount;
        /// <summary>
        /// 一页个数
        /// </summary>
        private int pageSize;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }
        //数据个数
        private int count;

        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                if (count == 0)
                    pageCount = 0;
                else
                    pageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
                txtCount.Text = pageCount.ToString();
                txtAll.Text = count.ToString();
                Init(null);
            }
        }

        //到指定页数
        public void ToPage(int index)
        {
            PageIndex = index;
            Init(new EventArgs());
        }

        void Init(EventArgs e)
        {
            try
            {
                InitButton();
                int temp = pageIndex + 1;
                if (pageCount == 0)
                    txtCurrent.Text = "0";
                else
                    txtCurrent.Text = temp.ToString();
                if (e != null)
                {
                    PageIndexChanging(pageIndex, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void InitButton()
        {
            this.FirstButton.IsEnabled = true;
            this.PreButton.IsEnabled = true;
            this.NextButton.IsEnabled = true;
            this.LastButton.IsEnabled = true;
            //this.FirstButton.Background = Brushes.Black;
            //this.PreButton.Background = Brushes.Black;
            //this.NextButton.Background = Brushes.Black;
            //this.LastButton.Background = Brushes.Black;

            //总共一页
            if (pageCount < 2)
            {
                this.FirstButton.IsEnabled = false;
                this.PreButton.IsEnabled = false;
                this.NextButton.IsEnabled = false;
                this.LastButton.IsEnabled = false;
                //this.FirstButton.Background = Brushes.Red;
                //this.PreButton.Background = Brushes.Red;
                //this.NextButton.Background = Brushes.Red;
                //this.LastButton.Background = Brushes.Red;
                return;
            }

            //第一页
            if (pageIndex == 0)
            {
                this.FirstButton.IsEnabled = false;
                this.PreButton.IsEnabled = false;
                //this.FirstButton.Background = Brushes.Red;
                //this.PreButton.Background = Brushes.Red;
                return;
            }

            //最后一页
            if (pageIndex + 1 == pageCount)
            {
                this.NextButton.IsEnabled = false;
                this.LastButton.IsEnabled = false;
                //this.NextButton.Background = Brushes.Red;
                //this.LastButton.Background = Brushes.Red;
            }
        }


        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            pageIndex = 0;
            Init(e);
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreButton_Click(object sender, RoutedEventArgs e)
        {
            --pageIndex;
            Init(e);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ++pageIndex;
            Init(e);
        }

        /// <summary>
        /// 尾页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LastButton_Click(object sender, RoutedEventArgs e)
        {
            pageIndex = pageCount - 1;
            Init(e);
        }
    }
}
