using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using System.Data;
using VehicleGPS.Services;
using VehicleGPS.Models.DispatchCentre.TaskManage;
using VehicleGPS.Models;
using VehicleGPS.Models.Login;
using Microsoft.Practices.Prism.Commands;
using VehicleGPS.DBWCFServices;
using Newtonsoft.Json;
using System.Windows;

namespace VehicleGPS.ViewModels.DispatchCentre.TaskManage
{
    class TaskManageViewModel : NotificationObject
    {
        public CVBasicInfo SelectedStation { get; set; }//选择的站点
        public DispatchTaskInfo SelectedTask { get; set; }//选择的区域
        public TaskManageViewModel(CVBasicInfo selectedStation)
        {
            this.SelectedStation = selectedStation;
            this.SelectionChangedCommand = new DelegateCommand<object>(new Action<object>(this.SelectionChangedCommandExecute));
            this.DelTaskCommand = new DelegateCommand(new Action(this.DelTaskCommandExecute));
            this.GetTaskInfo();
            this.QueryCommand = new DelegateCommand(new Action(this.QueryCommandExecute));
        }
        private List<DispatchTaskInfo> listDispatchTaskInfo = new List<DispatchTaskInfo>();
        public List<DispatchTaskInfo> ListDispatchTaskInfo
        {
            get { return listDispatchTaskInfo; }
            set
            {
                listDispatchTaskInfo = value;
                this.RaisePropertyChanged("ListDispatchTaskInfo");
            }
        }

        private string querytext;
        public string QueryText
        {
            get { return querytext; }
            set
            {
                querytext = value;
                this.RaisePropertyChanged("QueryText");
            }
        }

        /*获取选择的任务单*/
        public DelegateCommand<object> SelectionChangedCommand { get; set; }
        private void SelectionChangedCommandExecute(object selectedObject)
        {
            if (selectedObject != null)
            {
                this.SelectedTask = (DispatchTaskInfo)selectedObject;
            }
        }
        /*获取任务单数据*/
        private void GetTaskInfo(string querytext = null)
        {
            string sql = String.Empty;
            if (querytext == null)
            {
                sql = "select * from TranTaskList where  unitId='" + this.SelectedStation.ID + "'";
            }
            else
            {
                sql = "select * from TranTaskList where unitId='" + this.SelectedStation.ID + "' and taskListId like '%" + querytext + "%'";
            }
            this.listDispatchTaskInfo.Clear();//清空数据
            string jsonStr = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
            string jsonTmp = jsonStr.Substring(1, jsonStr.Length - 2);
            List<DispatchTaskInfo> tmp = (List<DispatchTaskInfo>)JsonConvert.DeserializeObject(jsonTmp, typeof(List<DispatchTaskInfo>));
            for (int i = 0; i < tmp.Count; i++)
            {
                tmp[i].Sequence = i + 1;
                //tmp[i].StartTime = Convert.ToDateTime(tmp[i].StartTime).ToString("yyyy/MM/dd");
                //tmp[i].EndTime = Convert.ToDateTime(tmp[i].EndTime).ToString("yyyy/MM/dd");
                tmp[i].taskStatus = VehicleCommon.GetTaskState(tmp[i].taskStatus);
            }
            this.ListDispatchTaskInfo = tmp;
        }
        /*查询任务单*/
        public DelegateCommand QueryCommand { get; set; }
        public void QueryCommandExecute()
        {
            try
            {
                this.GetTaskInfo(this.QueryText.Trim());
            }
            catch (NullReferenceException e)
            {
                MessageBox.Show("请输入查询信息");
            }
        }

        /*本地刷新数据*/
        public void LocalRefreshData()
        {
            List<DispatchTaskInfo> tmp = this.ListDispatchTaskInfo;
            this.ListDispatchTaskInfo = null;
            this.ListDispatchTaskInfo = tmp;
            //this.RaisePropertyChanged("ListDispatchTaskInfo");
        }

        /*删除任务单*/
        public DelegateCommand DelTaskCommand { get; set; }
        public void DelTaskCommandExecute()
        {
            if (this.SelectedTask == null)
            {
                MessageBox.Show("请选择任务单", "提示", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show("确实要删除该任务单的信息吗？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string sql = "delete TranTaskList where taskListId='" + this.SelectedTask.TaskListId + "'";
                string status = VehicleCommon.wcfDBHelper.BexcuteSql(StaticLoginInfo.GetInstance().UserName, sql);
                if (string.Compare(status, "error") != 0)
                {
                    MessageBox.Show("删除成功!", "提示", MessageBoxButton.OK);
                    Dictionary<string, string> instruction = new Dictionary<string, string>();
                    instruction.Add("cmd", "DISPATCH_TYPE");
                    instruction.Add("cmdid", "123_DISPATCH_TYPE");
                    instruction.Add("ID", this.SelectedTask.TaskListId);
                    instruction.Add("type", "TASK");
                    instruction.Add("OPERATETYPE", "3");
                    string insstring = JsonConvert.SerializeObject(instruction);
                    zmq.zmqPackHelper.zmqInstructionsPack(null, insstring);
                    DispatchTaskInfo newInfo = new DispatchTaskInfo();
                    this.listDispatchTaskInfo.Remove(this.SelectedTask);
                    /*重新编号*/
                    for (int i = 0; i < this.listDispatchTaskInfo.Count; i++)
                    {
                        this.listDispatchTaskInfo[i].Sequence = i + 1;
                    }
                    this.LocalRefreshData();
                }
                else
                {
                    MessageBox.Show("删除失败!", "提示", MessageBoxButton.OK);
                }
            }
        }
    }
}

