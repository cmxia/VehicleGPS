using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;

namespace VehicleGPS.Services.ReportCentre
{
  public sealed  class ReportTest
    {
        public string result;
        public string getCurrentOil()
        {
            string uri = ReportUri.getCurrentOilUri;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowWriteStreamBuffering = true;
            request.BeginGetRequestStream(new AsyncCallback(RequestReady), request);
            return null;
        }
        private void RequestReady(IAsyncResult asyncResult)
        {
            HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
            StreamWriter postStream = new StreamWriter(request.EndGetRequestStream(asyncResult));
            List<string> listData = new List<string>();
            listData.Add("V201310190612477948");
            listData.Add("V201310190612475704");
            listData.Add("V201311241121122Mx5");
            listData.Add("V20131124112113yaox");
            listData.Add("V201310190612479704");
            listData.Add("V20131124112113RfEe");
            listData.Add("V20131124112113JYDK");
            //传入参数说明
            string postData = JsonConvert.SerializeObject(listData);
            postStream.Write("{0}={1}&{2}={3}&{4}={5}&", "Request",
                postData, "startTime", DateTime.Now.AddMonths(0).ToString("yyyy-M-d"), "endTime", DateTime.Now.AddMonths(0).ToString("yyyy-M-d"));

            postStream.Close();
            postStream.Dispose();
            request.BeginGetResponse(new AsyncCallback(ResponseReady), request);

        }
        void ResponseReady(IAsyncResult asyncResult)
        {
            HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
            WebResponse response = request.EndGetResponse(asyncResult) as WebResponse; //同步线程上下文

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream);
                string reStr = reader.ReadToEnd();
                result = reStr;
                Thread mythread = new Thread(new ThreadStart(ShowResult));
                mythread.Start();
            }
        }
        private void ShowResult()
        {
            Log.WriteLog(result);

        }
        //private delegate void InvokeDelegate(string res);
    }
}
