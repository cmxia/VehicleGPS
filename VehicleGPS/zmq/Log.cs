using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WpfApplication1.zmq
{
    class Log
    {
        public static void WriteLog(string fileLogName, string sMsg)
        {
            if (sMsg != "" && fileLogName != "")
            {
                //Random randObj = new Random(DateTime.Now.Millisecond);
                //int file = randObj.Next() + 1;
                string filename = DateTime.Now.ToString("yyyyMMddHH") + fileLogName + ".log";
                try
                {
                    FileInfo fi = new FileInfo("C:\\Log\\" + filename);
                    if (!fi.Exists)
                    {
                        using (StreamWriter sw = fi.CreateText())
                        {
                            sw.WriteLine(DateTime.Now + "\n" + sMsg + "\n");
                            sw.Close();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = fi.AppendText())
                        {
                            sw.WriteLine(DateTime.Now + "\n" + sMsg + "\n");
                            sw.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
