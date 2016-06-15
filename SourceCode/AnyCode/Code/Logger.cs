using System;
using System.Text;
using log4net;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace AnyCode
{
    /// <summary>
    /// log4net logger
    /// </summary>
    public class Logger
    {
        //private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog log = LogManager.GetLogger("FastLogger");

        public static void Log(string msg)
        {
            try
            {
                log.Error(msg);
            }
            catch
            { }
        }

        public static void Log(string localtion, string msg)
        {
            try
            {
                log.Debug(localtion + " :: " + msg);
            }
            catch
            { }
        }

        public static void Log(Exception ex)
        {
            try
            {
                log.Debug(GetErrorCode(ex)
                    + "\r\n"
                    + GetErrorCode(ex.GetBaseException()));
            }
            catch
            { }
        }

        public static void Log(string localtion, Exception ex)
        {
            try
            {
                log.Debug(localtion + " :: " + GetErrorCode(ex)
                    + "\r\n"
                    + GetErrorCode(ex.GetBaseException()));
            }
            catch
            { }
        }

        private static string GetErrorCode(Exception ex)
        {
            if (ex == null)
                return "";

            StringBuilder error = new StringBuilder();

            error.AppendLine("Class Name :: " + ((ex.TargetSite == null || ex.TargetSite.DeclaringType == null) ? "null" : ex.TargetSite.DeclaringType.FullName));

            error.AppendLine("Method Name :: " + (ex.TargetSite == null ? "null" : ex.TargetSite.Name));

            error.AppendLine("Message :: " + (ex.Message == null ? "null" : ex.Message));

            error.AppendLine("StackTrace :: " + (ex.StackTrace == null ? "null" : ex.StackTrace));

            error.AppendLine("Source :: " + (ex.Source == null ? "null" : ex.Source));

            error.AppendLine("InnerException :: " + (ex.InnerException == null ? "null" : ex.InnerException.ToString()));

            return error.ToString();
        }
    }
}