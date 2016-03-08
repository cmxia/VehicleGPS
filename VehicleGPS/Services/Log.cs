using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace VehicleGPS.Services
{
    class Log
    {
        /// <summary>
        /// 写日志文件
        /// </summary>
        /// <param name="sMsg"></param>
        public static void WriteLog(string sMsg)
        {
            if (sMsg != "")
            {
                //Random randObj = new Random(DateTime.Now.Millisecond);
                //int file = randObj.Next() + 1;
                string filename = "gpsinfo" + DateTime.Now.ToString("yyyyMMddHH") + ".log";

                FileInfo fi = new FileInfo(Application.StartupPath + "\\log\\" + filename);
                if (!Directory.Exists(Application.StartupPath + "\\log\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\log\\");
                }
                try
                {
                    if (!fi.Exists)
                    {
                        using (StreamWriter sw = fi.CreateText())
                        {
                            sw.WriteLine(DateTime.Now + "\n :" + sMsg + "\n");
                            sw.Close();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = fi.AppendText())
                        {
                            sw.WriteLine(DateTime.Now + "\n :" + sMsg + "\n ");

                            sw.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = fi.AppendText())
                    {
                        sw.WriteLine("gpsinfo" + DateTime.Now.ToString("yyyyMMddHH") + ex.Message);
                        sw.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        /// <param name="logname">日志名称</param>
        public static void WriteLog(string msg, string logname)
        {
            if (msg != "")
            {
                string filename = logname + DateTime.Now.ToString("yyyyMMddHH") + ".log";
                FileInfo fi = new FileInfo(Application.StartupPath + "\\log\\" + filename);
                if (!Directory.Exists(Application.StartupPath + "\\log\\"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "\\log\\");
                }
                try
                {
                    if (!fi.Exists)
                    {
                        using (StreamWriter sw = fi.CreateText())
                        {
                            sw.WriteLine(DateTime.Now + "\n :" + msg + "\n");
                            sw.Close();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = fi.AppendText())
                        {
                            sw.WriteLine(DateTime.Now + "\n :" + msg + "\n");
                            sw.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = fi.AppendText())
                    {
                        sw.WriteLine(DateTime.Now + "\n :" + ex.Message + "\n");
                        sw.Close();
                    }
                }
            }
        }
    }
}
