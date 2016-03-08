using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace VehicleGPS.ViewModels.AutoComplete
{
    class AutoCompleteItem : NotificationObject
    {
        public string ID { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        private string sim;
        public string SIM
        {
            get { return sim; }
            set
            {
                if (sim != value)
                {
                    sim = value;
                    this.RaisePropertyChanged("SIM");
                }
            }
        }

        private string namesim;

        public string NameSim
        {
            get { return namesim; }
            set
            {
                namesim = value;
                this.RaisePropertyChanged("NameSim");
            }
        }

    }
}
