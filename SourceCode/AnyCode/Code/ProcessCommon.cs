using System.Collections.Generic;
using System.Linq;

namespace AnyCode
{
    public class ProcessCommon
    {
        /// <summary>
        /// 创建同步对象
        /// </summary>
        public static object SyncObj = new object();

        /// <summary>
        /// 创建进程通道
        /// </summary>
        private static IDictionary<string, int> ProcessStatus { get; set; }

        public ProcessCommon()
        {
            if (ProcessStatus == null)
            {
                ProcessStatus = new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// 增加状态值
        /// </summary>
        /// <param name="id">Guid</param>
        public void AddStatus(string id)
        {

            lock (SyncObj)     //确保当一个线程位于代码的临界区时，另一个线程不会进入该临界区
            {
                ProcessStatus.Add(id, 0);  // 
            }
        }

        /// <summary>
        /// 删除状态值
        /// </summary>
        /// <param name="id">Guid</param>
        public void RemoveStatus(string id)
        {
            lock (SyncObj)
            {
                ProcessStatus.Remove(id);
            }
        }

        /// <summary>
        /// 获取状态值
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns></returns>
        public string GetStatus(string id)
        {
            lock (SyncObj)
            {
                if (ProcessStatus.Keys.Count(p => p == id) == 1)
                {
                    return ProcessStatus[id].ToString();
                }
                return "";

            }
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="currentid">进度条返回要获取值的GUID</param>
        /// <param name="totalid">进度条返回总值的GUID</param>
        /// <returns></returns>
        public string ProcessRunningAction(string currentid, string totalid)
        {
            return "";
        }

    }
}