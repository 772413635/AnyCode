using System;
using System.IO;
using System.Text;
using System.Web;
using DBlinq;

namespace AnyCode
{
    public class LogClassModels
    {
        public static void WriteServiceLog(string logType, string message, string menu, string result, string ServiceId)
        {
            try
            {
                Sys_Log sysLog = new Sys_Log();
                sysLog.CreateTime = DateTime.Now;
                sysLog.Ip = GetIP();
                sysLog.Message = message;
                if (HttpContext.Current != null && HttpContext.Current.Session["user"] != null)
                {
                    var person = (Sys_User)HttpContext.Current.Session["user"];
                    sysLog.PersonId = person.Name;
                }
                else
                {
                    sysLog.PersonId = "未登录用户";
                }
                //sysLog.ServiceId = ServiceId;
                sysLog.State = logType;
                sysLog.Result = result;
                sysLog.MenuId = menu;
                sysLog.Id = Guid.NewGuid().ToString();
                //using (SysLogService sysLogService = new SysLogService())
                //{
                //    ValidationErrors validationErrors = new ValidationErrors();
                //    sysLogService.Add(ref validationErrors, sysLog);
                //    return;
                //}
            }
            catch (Exception ep)
            {
                try
                {
                    string path = @"~/mylog.txt";
                    string txtPath = HttpContext.Current.Server.MapPath(path);//获取绝对路径
                    using (StreamWriter sw = new StreamWriter(txtPath, true, Encoding.Default))
                    {
                        sw.WriteLine((ep.Message + "|" + logType + "|" + message + "|" + "MySession.Get(MySessionKey.UserID)" + "|" + GetIP() + DateTime.Now));
                        sw.Close();
                    }
                }
                catch {
                }
            }

        }
        public static string GetIP()
        {
            string ip = string.Empty;
            try
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null) // 服务器， using proxy
                    {
                        //得到真实的客户端地址
                        ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]; // Return real client IP.
                    }
                    else//如果没有使用代理服务器或者得不到客户端的ip not using proxy or can't get the Client IP
                    {

                        //得到服务端的地址要判断  System.Web.HttpContext.Current 为空的情况

                        ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; //While it can't get the Client IP, it will return proxy IP.

                    }
                }
            }
            catch (Exception ep)
            {
                ip = "没有正常获取IP，" + ep.Message;
            }

            return ip;
        }

        public void Dispose()
        {

        }
    }
}