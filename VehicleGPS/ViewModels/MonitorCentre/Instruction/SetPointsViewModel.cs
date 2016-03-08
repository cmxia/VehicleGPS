using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using VehicleGPS.ViewModels.MonitorCentre.Instruction;
using VehicleGPS.Models.MonitorCentre;

namespace VehicleGPS.ViewModels.MonitorCentre.Instruction
{
    class SetPointsViewModel : NotificationObject
    {
        //public static List<VehicleGPS.ViewModels.MonitorCentre.Instruction.InstructionViewModel.PointOfLine> pointListStatic = new List<InstructionViewModel.PointOfLine>();
        public SetPointsViewModel(List<PointOfLine> list)
        {
            this.PointList = list;
        }
        private List<PointOfLine> pointList;

        public List<PointOfLine> PointList
        {
            get { return pointList; }
            set
            {
                pointList = value;
                this.RaisePropertyChanged("PointList");
            }
        }

    }
}
