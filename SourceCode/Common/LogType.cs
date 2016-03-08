using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
     public static class LogType
    {
        /// <summary>
        /// 未定义
        /// </summary>
        [DataMember]
        public static string None { get { return "未定义"; } }
        /// <summary>
        /// 捕获的异常
        /// </summary>
        [DataMember]
        public static string Exception { get { return "捕获的异常"; } }
        /// <summary>
        /// 数据库的异常
        /// </summary>
        [DataMember]
        public static string DataBase { get { return "数据库的异常"; } }
        /// <summary>
        /// 出错信息
        /// </summary>
        [DataMember]
        public static string Error { get { return "出错信息"; } }
        /// <summary>
        /// 操作信息
        /// </summary>
        [DataMember]
        public static string Operation { get { return "操作信息"; } }
        /// <summary>
        /// 系统调用
        /// </summary>
        [DataMember]
        public static string System { get { return "系统"; } }
        /// <summary>
        /// 其他系统调用的服务
        /// </summary>
        [DataMember]
        public static string AnotherSystem { get { return "其他系统调用"; } }
    }
}
