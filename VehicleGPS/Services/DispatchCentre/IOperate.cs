using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Services.DispatchCentre
{
    public interface IOperate<T>
    {
        List<T> ReadAllInfo();//获取全部Info
        bool AddInfo(T info);//添加一个Info
        bool ModInfo(T info);//修改一个Info
        bool DelInfo(T info);//删除一个Info
    }
}
