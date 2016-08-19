using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common;
using DBlinq;

namespace AnyCode.Models
{
    /// <summary>
    /// 用户处理类
    /// </summary>
    public class AccountBLL
    {
        //验证用户信息    
        /// <summary>
        /// 验证用户名和密码是否正确
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>是否登录成功</returns>
        public static Sys_User ValidateUser(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }
            var user = (from tt in (new GodBuildDB()).Sys_User
                        where
                            tt.IsDelete == false && tt.Name == userName && tt.Password == MD5.Encrypt(password, 32)
                        select tt).SingleOrDefault();

            return user;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="personName">用户名</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>修改密码是否成功</returns>
        public static bool ChangePassword(string personName, string oldPassword, string newPassword)
        {
            return false;
        }

        #region 储存和读取,删除cookie
        /// <summary>
        /// 储存cookie
        /// </summary>
        /// <param name="name">cookie名</param>
        /// <param name="value">cookie值</param>
        /// <param name="expiresTime">过期时间</param>
        /// <param name="path">储存路径</param>
        public static void SaveCookie(string name, string value, DateTime expiresTime, string path)
        {
            var hc = new HttpCookie(name) { Value = value, Expires = expiresTime, Path = path };
            HttpContext.Current.Response.Cookies.Add(hc);//储存cookie

        }
        /// <summary>
        /// 读取cookie
        /// </summary>
        /// <param name="name">cookie名</param>
        /// <returns></returns>
        public static HttpCookie ReadCookie(string name)
        {
            return HttpContext.Current.Request.Cookies[name];

        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="name">cookie名</param>
        public static void DelteCookie(string name)
        {
            var rhc = ReadCookie("AnyCode");//获取cookie
            if (rhc != null)//如果cookie存在就删除
            {
                var hc = new HttpCookie(name) { Value = rhc.Value, Expires = DateTime.Now.AddMonths(-1), Path = rhc.Path };
                HttpContext.Current.Response.Cookies.Add(hc);//储存cookie
            }
        }

        #endregion
    }

    /// <summary>
    /// 用户信息类
    /// </summary>
    public class UserTicket
    {
        /// <summary>
        /// 已经登录的用户存储在此全局静态列表中
        /// </summary>
        public static List<Sys_User> Users = new List<Sys_User>();

        /// <summary>
        /// 存储或写入在cookie中的用户id
        /// </summary>
        /// <returns></returns>
        public static int Id
        {
            get { return AccountBLL.ReadCookie("AnyCode") == null ? 0 : AccountBLL.ReadCookie("AnyCode").Value.GetInt(); }
            set { AccountBLL.SaveCookie("AnyCode", value.ToString(), DateTime.Now.AddDays(1), "/"); }
        }


        /// <summary>
        /// 存储或写入在cookie中的用户theme
        /// </summary>
        /// <returns></returns>
        public static string Theme
        {
            get { return AccountBLL.ReadCookie("theme") == null ? "" : AccountBLL.ReadCookie("theme").Value; }
            set { AccountBLL.SaveCookie("theme", value, DateTime.Now.AddDays(1), "/"); }
        }

    }



}