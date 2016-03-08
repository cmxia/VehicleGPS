using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleGPS.Services
{
    public static class TelHelper
    {
        //判断电话号码是否合法
        public static bool isTelRight(string telstr)
        {
            bool T_F = false;
            if (!isNumber(telstr))
            {
                T_F = false;
            }
            else
            {
                if (telstr.Length != 11 && telstr.Length != 8)
                {
                    T_F = false;
                }
                else {
                    T_F = true;
                }
            }
            return T_F;
        }
        //判断字符串s是否为数字
        private static bool isNumber(string s)
        {
            int Flag = 0;
            char[] str = s.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsNumber(str[i]))
                {
                    Flag++;
                }
                else
                {
                    Flag = -1;
                    break;
                }
            }
            if (Flag > 0)
            {
                return true;
            }
            else
            {
                return false;

            }
        }
    }
}
