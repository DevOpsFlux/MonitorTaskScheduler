/*-------------------------------------------------------
'	프로그램명	: Task Scheduler Monitor
'	작성자		: DevOpsFlux
'	작성일		: 2019-01-17
'	설명		: 작업 스케줄러 모니터링
' -------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Script.Serialization;

using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Win32.TaskScheduler;

namespace TaskWebNew
{
    public partial class Index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strKind = Request["k"]; // k : XML k :JSON
            string strPathKey = Request["pk"]; // pk : Batch (BatchAKind/BatchBKind)

            if (String.IsNullOrEmpty(strKind))
            {
                strKind = "JSON";
            }
            if (String.IsNullOrEmpty(strPathKey))
            {
                strPathKey = "Batch";
            }

            GetTask(strKind, strPathKey);
        }

        public struct TaskInfo
        {
            public string Name;
            public string State;
            public string Enabled;
            public string LastTaskResult;
            public string LastRunTime;
            public string NextRunTime;
            //public string strPath;
            public string RunningTime;
        }

        public void GetTask(string strKind, string strPathKey)
        {
            try
            {
                List<TaskInfo> listTask = new List<TaskInfo>();

                var ts = TaskService.Instance;

                int i = 0;
                foreach (var t in ts.FindAllTasks(new Regex(".*"), true))
                {
                    // Batch Path : "\\BatchDev\\TestBatchFlux"

                    TaskInfo ti;
                    //if(i < 200)
                    if ("Batch" == t.Path.Substring(1, 5))
                    {
                        if (t.Enabled)
                        {
                            ti.Name = t.Name;
                            ti.State = t.State.ToString();
                            ti.Enabled = t.Enabled.ToString();
                            ti.LastTaskResult =
                                string.Format("0x{0:x}", t.LastTaskResult); // 267045(10진수) -> 0x41325(16진수) 
                            //ti.LastTaskResult = t.LastTaskResult.ToString();                    // 0 : 실행성공 https://docs.microsoft.com/en-us/windows/desktop/taskschd/task-scheduler-error-and-success-constants
                            ti.LastRunTime = t.LastRunTime.ToString("yyyy-MM-dd HH:mm:ss");
                            ti.NextRunTime = t.NextRunTime.ToString("yyyy-MM-dd HH:mm:ss");
                            //ti.strPath = t.Path.ToString();
                            ti.RunningTime = GetRunningTime(ti.State, ti.LastRunTime);
                            

                            listTask.Add(ti);
                        }
                    }
                    //i += 1;
                    //Response.Write(t.Name + "<br>");
                }

                if (strKind == "JSON")
                {
                    Response.ContentType = "text/json";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Write(SerializeToJson(listTask));
                } else if (strKind == "XML")
                {
                    Response.ContentType = "application/xml";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Write(SerializeToXml(listTask));
                }
                else
                {
                    Response.ContentType = "text/json";
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Write(SerializeToJson(listTask));
                }

                //Response.ContentType = "text/javascript";
                //Response.ContentEncoding = Encoding.UTF8;
                //Response.Write(SerializeToJson(listTask));

                //Response.ContentType = "application/xml";
                //Response.ContentEncoding = Encoding.UTF8;
                //Response.Write(SerializeToXml2(listTask));

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        protected string SerializeToXml2<T>(T t)
        {
            string strXml = "";
            XmlSerializer xs = new XmlSerializer(typeof(T));

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    xs.Serialize(ms, t);
                    strXml = Encoding.UTF8.GetString(ms.ToArray());
                }

            }
            catch (Exception ex)
            {
                strXml = ex.ToString();
            }

            return strXml;
        }

        public static string SerializeToXml(object obj)
        {
            XDocument doc = new XDocument();
            using (XmlWriter xmlWriter = doc.CreateWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(xmlWriter, obj);
                xmlWriter.Close();
            }
            return doc.ToString();
        }

        public static string SerializeToJson(object obj)
        {
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(obj);
            return json.ToString();
        }

        private string GetRunningTime(string strState, string strLastRunTime)
        {
            string strRunningTime = "0";
            string CheckDate = strLastRunTime;
            string NowDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (strState=="Running" && !String.IsNullOrEmpty(strLastRunTime))
            {
                DateTime StartDate = Convert.ToDateTime(CheckDate);
                DateTime EndDate = Convert.ToDateTime(NowDate);

                TimeSpan dateDiff = EndDate - StartDate;
                strRunningTime = dateDiff.ToString();
            }

            return strRunningTime;
        }
    }
}