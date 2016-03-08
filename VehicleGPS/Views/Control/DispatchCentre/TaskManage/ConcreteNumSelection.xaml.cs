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
using VehicleGPS.ViewModels.DispatchCentre.TaskManage;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using System.Data;
using VehicleGPS.Services;

namespace VehicleGPS.Views.Control.DispatchCentre.TaskManage
{
    /// <summary>
    /// ConcreteNumSelection.xaml 的交互逻辑
    /// </summary>
    public partial class ConcreteNumSelection : Window
    {
        private TaskAddModDelViewModel parentVM;
        private List<ConcreteInfo> ConcreteList = new List<ConcreteInfo>();
        public ConcreteNumSelection(object vm)
        {
            InitializeComponent();
            this.parentVM = (TaskAddModDelViewModel)vm;
            this.GetConreteData();
        }
        /*获取混凝土标号数据*/
        private void GetConreteData()
        {
            string sql = "select * from Dictionary_Detail where TypeID='zd026'";

            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            if (string.Compare(jsonStr, "error") != 0)
            {
                DataTable dt = new DataTable();
                dt = JsonHelper.JsonToDataTable(jsonStr);
                int count = 0;
                foreach (DataRow row in dt.Rows)
                {
                    ConcreteInfo concreteInfo = new ConcreteInfo();
                    concreteInfo.Sequence = ++count;
                    concreteInfo.Id = row["DicID"].ToString();
                    concreteInfo.Name = row["DicName"].ToString();
                    this.ConcreteList.Add(concreteInfo);
                }

                this.dg_ConcreteList.ItemsSource = null;
                this.dg_ConcreteList.ItemsSource = this.ConcreteList;
            }
            else
            {
                MessageBox.Show("基本类型数据未加载成功", "提示", MessageBoxButton.OK);
            }
        }
        private void close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void confirm_Click(object sender, RoutedEventArgs e)
        {
            string concreteId = "";
            string concreteName = "";
            foreach (ConcreteInfo info in this.ConcreteList)
            {
                if (info.selected == true)
                {
                    concreteId += info.Id + ";";
                    concreteName += info.Name + ";";
                }
            }
            this.parentVM.ConcreteName = concreteName;
            // this.parentVM.ConcreteId = concreteId;
            this.Close();
        }
    }

    public class ConcreteInfo : NotificationObject
    {
        public int Sequence { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    this.RaisePropertyChanged("Selected");
                }
            }
        }
    }
}
