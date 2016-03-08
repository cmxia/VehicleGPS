using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VehicleGPS.Services;

namespace VehicleGPS.Models
{
    class InstructionRight
    {
        private static InstructionRight instance;
        private BasicDataServiceWCF wcfService;
        public List<RightInfo> ListInstructionRight { get; set; }//指令权限列表
        private InstructionRight()
        {
            this.ListInstructionRight = new List<RightInfo>();
            this.wcfService = new BasicDataServiceWCF();
        }
        public static InstructionRight GetInstance()
        {
            if (instance == null)
            {
                instance = new InstructionRight();
            }
            return instance;
        }
        public void RefreshInstructionRightInfo()
        {
            this.wcfService.GetInstructionRightThread();
        }
    }
}
