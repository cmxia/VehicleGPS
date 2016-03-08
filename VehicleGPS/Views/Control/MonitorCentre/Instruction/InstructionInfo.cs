using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace VehicleGPS.Views.Control.MonitorCentre.Instruction
{
    class InstructionInfo
    {
        public int sequence { get; set; }//序号
        public string id { get; set; }//车辆编号
        public string name { get; set; }//车牌号
        public string parent { get; set; }//所属单位
        public string sim { get; set; }//sim卡号
        public string gpsType { get; set; }
        public string gpsVersion { get; set; }
    }

    //待发指令数据显示的结构
    public class InstructionDataGrid : INotifyPropertyChanged
    {
        private int currentId { set; get; } //序号
        public int CurrentId
        {
            get { return currentId; }
            set
            {
                currentId = value;
                NotifyPropertyChange("CurrentId");
            }
        }

        private string vehicleId { set; get; } //id
        public string VehicleId
        {
            get { return vehicleId; }
            set
            {
                vehicleId = value;
                NotifyPropertyChange("VehicleId");
            }
        }

        private string customerName { set; get; } //单位
        public string CustomerName
        {
            get { return customerName; }
            set
            {
                customerName = value;
                NotifyPropertyChange("CustomerName");
            }
        }

        private string vehicleNumber { set; get; } //车牌号
        public string VehicleNumber
        {
            get { return vehicleNumber; }
            set
            {
                vehicleNumber = value;
                NotifyPropertyChange("VehicleNumber");
            }
        }

        private string sim { set; get; } //SIM卡号
        public string Sim
        {
            get { return sim; }
            set
            {
                sim = value;
                NotifyPropertyChange("Sim");
            }
        }

        private string states { set; get; } //状态
        public string States
        {
            get { return states; }
            set
            {
                states = value;
                NotifyPropertyChange("States");
            }
        }


        private string result { set; get; } //结果
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                NotifyPropertyChange("Result");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
