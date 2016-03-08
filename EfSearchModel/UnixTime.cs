using System;

namespace EfSearchModel
{
    public class UnixTime
    {
        private static DateTime _baseTime = new DateTime(1970,1,1);

        /// <summary>
        /// 将unixtime转换为.NET的DateTime
        /// </summary>
        /// <param name="timeStamp">秒数</param>
        /// <returns>转换后的时间</returns>
        public static DateTime FromUnixTime(long timeStamp)
        {
            return new DateTime((timeStamp + 8 * 60 * 60) * 10000000 + _baseTime.Ticks);
            //return BaseTime.AddSeconds(timeStamp);
            //return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timeStamp);
        }

        /// <summary>
        /// 将.NET的DateTime转换为unix time
        /// </summary>
        /// <param name="dateTime">待转换的时间</param>
        /// <returns>转换后的unix time</returns>
        public static long FromDateTime(DateTime dateTime)
        {
            return (dateTime.Ticks - _baseTime.Ticks) / 10000000 - 8 * 60 * 60;
            //return (dateTime.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks) / 10000000;
        }
    }
}
