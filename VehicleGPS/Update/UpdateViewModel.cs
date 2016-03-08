using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Timers;
using VehicleGPS.Models;

namespace VehicleGPS.Update
{
    class UpdateViewModel : NotificationObject
    {
        UpdateView parentWin = null;
        public UpdateViewModel(UpdateView win)
        {
            this.parentWin = win;
            this.ConfirmCommand = new DelegateCommand(new Action(this.ConfirmCommandExecute));
            promptindex = 0;
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(RefreshPrompt);
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();
        }
        private string prompt;

        public string Prompt
        {
            get { return prompt; }
            set
            {
                prompt = value;
                this.RaisePropertyChanged("Prompt");
            }
        }
        public int promptindex { get; set; }
        public string[] prompttext = { ".", "..", "...", "....", ".....", "......" };
        public DelegateCommand ConfirmCommand { get; set; }
        private void ConfirmCommandExecute()
        {
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(RefreshPrompt);
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();
        }
        private void RefreshPrompt(object sender, EventArgs e)
        {
            this.Prompt = "正在下载中" + prompttext[((promptindex++) % 6)];
            if (StaticTreeState.DownLoadComplete == LoadingState.LOADCOMPLETE)
            {
                parentWin.Dispatcher.BeginInvoke((Action)delegate()
                {
                    parentWin.Close();
                });
            }
        }
    }
}
