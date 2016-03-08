using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Views.Control.MessCenter
{
    class UniqueStrHelper
    {
        public static string getStrSequence()
        {
            string str = "";
            byte[] buffer = Guid.NewGuid().ToByteArray();
            long long_guid = BitConverter.ToInt64(buffer, 0);
            str = DateTime.Now.ToString("yyyyMMddhhmmss") + randStr(5) + long_guid.ToString();//14+19+5
            return str;
        }

        public static string randStr(int count)
        {
            string allchar = "1,2,3,4,5,6,7,8,9,0";
            string[] allchararray = allchar.Split(',');
            string randomcode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(10);
                if (temp == t)
                {
                    return randStr(count);
                }
                temp = t;
                randomcode += allchararray[t];
            }
            return randomcode;
        }
    }
}
