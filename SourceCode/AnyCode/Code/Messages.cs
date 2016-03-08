using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DBlinq;

namespace AnyCode
{
    public class Messages : BaseController
    {
        //记录所有请求的客户端
        readonly List<myAsynResult> clients = new List<myAsynResult>();

        #region 实现该类的单例
        private static readonly Messages _Instance = new Messages();
        private Messages()
        {
        }
        public static Messages Instance()
        {
            return _Instance;
        }
        #endregion

        public void AddMessage(string title, string content, string[] code, string createPeople, myAsynResult asyncResult, string isSystem)//string[] company
        {
            //当传入的内容为"-1"时，表示为建立连接请求，即为了维持一个从客户端到服务器的连接而建立的连接
            //此时将该连接保存到 List<myAsynResult> clients中，待再有消息发送过来时，该连接将会被遍历，并且会将该连接输出内容后，结束该连接
            if (content == "-1")
            {
                string userCode = code[0];

                asyncResult.ReceiveId = userCode;
                clients.Add(asyncResult);
            }
            else if (content == "-2")
            {
                string userId = code[0];
                asyncResult.ReceiveId = userId;
                var queryResult = (from a in Db.Sys_WebNews
                                   join b in Db.Sys_WebNewsCode on a.Id equals b.NewId
                                   where b.Code == userId.GetInt() && b.IsRead == false//code为用户ID
                                   select new
                                   {
                                       NewsCount = a.News
                                   }).Distinct().Count();
                asyncResult.Content = queryResult.ToString();
                asyncResult.Send(null);
            }
            else
            {
                try
                {
                    //插入数据库
                    Sys_WebNews oneNews = new Sys_WebNews
                    {
                        Title = title,
                        News = content,
                        CreateTime = DateTime.Now,
                        CreatePerson = createPeople
                    };
                    foreach (string st in code)
                    {
                        Sys_WebNewsCode oneNewCode = new Sys_WebNewsCode { NewId = oneNews.Id, Code = st.GetInt(), IsRead = false };
                        oneNews.Sys_WebNewsCode.Add(oneNewCode);
                    }
                    Db.InsertOnSubmit(oneNews);

                    //遍历所有已缓存的client,并将当前内容输出到客户端
                    foreach (myAsynResult result in clients)
                    {
                        foreach (string str in code)
                        {
                            if (str == result.ReceiveId)
                            {
                                result.Content = "<h4>" + title + "</h4>" + content;
                                result.Send("接收");
                            }
                            break;
                        }
                    }

                    //将当前请求的内容输出到客户端
                    asyncResult.Content = "发送成功！";

                    asyncResult.Send(null);

                    Db.SubmitChanges();
                }
                catch
                {
                    //将当前请求的内容输出到客户端
                    asyncResult.Content = "发送失败！";
                    asyncResult.Send(null);
                }

                //清空所有缓存
                clients.Clear();

            }
        }


        /*
                private bool CheckCompId(string code)
                {
                    bool result = false;//默认不存在
                    foreach (var client in clients)
                    {
                        if (client.Cid == code)
                        {
                            return true;
                        }
                    }
                    return result;
                }
        */
    }
}