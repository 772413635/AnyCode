using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Data.Linq;
using System.Web.Script.Serialization;
using AnyCode.Models.Interfaces;
using Common;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using IResponseMessageBase = Senparc.Weixin.MP.Entities.IResponseMessageBase;

namespace AnyCode.Models.Service
{
    public partial class CustomMessageHandler
    {


        //微信回复消息
        private IResponseMessageBase ReplyMsaage(int msgType, int msgId, Senparc.Weixin.MP.Entities.RequestMessageBase requestMessage)
        {
            switch ((ResponseMsgType)msgType)
            {
                case ResponseMsgType.Text:
                    {
                        var text = _db.Sys_WebChat_MsgText.SingleOrDefault(c => c.Id == msgId);
                        var responseMessage = CreateResponseMessage<ResponseMessageText>();
                        if (text != null)
                        {

                            responseMessage.Content = ReplaceWeiXinText(text.Content, requestMessage);//文本内容
                        }
                        return responseMessage;


                    }

                case ResponseMsgType.MultipleNews:
                    {
                        var news = _db.Sys_WebChat_MsgNews.Where(c => c.Group == msgId).OrderByDescending(c => c.Sort);
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        if (news.Any())
                        {
                            foreach (var n in news)
                            {
                                responseMessage.Articles.Add(new Article()
                                {
                                    Title = n.Title,
                                    Description = n.Description,
                                    PicUrl = n.PicUrl,
                                    Url = ReplaceWeiXinText(n.Url, requestMessage)
                                });
                            }

                        }
                        return responseMessage;
                    }
                case ResponseMsgType.News:
                    {
                        var news = _db.Sys_WebChat_MsgNews.SingleOrDefault(c => c.Id == msgId);
                        var responseMessage = CreateResponseMessage<ResponseMessageNews>();
                        if (news != null)
                        {
                            responseMessage.Articles.Add(new Article()
                            {
                                Title = news.Title,
                                Description = news.Description,
                                PicUrl = news.PicUrl,
                                Url = ReplaceWeiXinText(news.Url, requestMessage)
                            });

                        }
                        return responseMessage;
                    }
                default:
                    return null;
            }
        }



        //文本处理方法
        private IResponseMessageBase MessageText(string text)
        {
            var response = CreateResponseMessage<ResponseMessageText>();
            if (text.IndexOf(":", StringComparison.Ordinal) > -1)//有命令:值这样的消息类型
            {
                var commandText = text.Split(':');
                var autoReplyMap = _db.Sys_WebChat_AutoReplyMap.Single(c => c.Command == commandText[0]);//获取命令回复配置表
                if (autoReplyMap.MsgOrGroupId == null)//处理特定的命令消息
                {
                    var replyContent = "";
                    switch (commandText[0])
                    {
                        case "pwd":
                            {
                                replyContent = GetPwdCommandReply(commandText[1]);
                            }
                            break;
                        case "text":
                            {
                                replyContent = GetTextCommandReply(commandText[1]);
                            }
                            break;
                    }
                    response.Content = replyContent;
                    return response;
                }
                return ReplyMsaage(autoReplyMap.MsgTypeId, (int)autoReplyMap.MsgOrGroupId,null);
            }
            response.Content = "暂时不知道怎么回复你啊";
            return response;
        }

        private string GetTextCommandReply(string commandValue)
        {
            var regex = new Regex(@"<[^>]*>|\t+|\s+");
            var regex2 = new Regex(@"∫+");
            var stext = _db.Sys_Text.FirstOrDefault(c => c.Title.Contains(commandValue));
            var replyContent = stext == null ? "找不到回复" : regex2.Replace(regex.Replace(stext.Content, ReplaceHtml), ReplaceHtml).Trim('∫').Replace("∫", "\r\n");
            return replyContent;
        }

        private string GetPwdCommandReply(string commandValue)
        {
            var pwdList = _db.Sys_Password.Where(c => c.Name.Contains(commandValue.Trim())).ToList();
            var replyContent = "";
            foreach (var p in pwdList)
            {
                //查询数据的添加人
                var addUser = p.CreatePerson==null?null:_db.Sys_User.SingleOrDefault(c=>c.Id==p.CreatePerson) ;
                if (addUser != null)
                {
                    //获取加密token，解密
                    p.UserName = MeDes.uncMe(p.UserName, addUser.UserToken);
                    p.Password = MeDes.uncMe(p.Password, addUser.UserToken);
                }
                replyContent += string.Format("{0}:\r\n{1}\r\n{2}\r\n{3}\r\n", p.Name, p.UserName, p.Password, p.Remark);



            }
            if (string.IsNullOrWhiteSpace(replyContent))
            {
                return "找不到回复";
            }
            return replyContent.TrimEnd(new[] { '\r', '\n' });

        }

        private string ReplaceHtml(Match match)
        {
            if (match.ToString().IndexOf("\t") > -1 || match.ToString().IndexOf(@"\s") > -1 || match.ToString().IndexOf("∫") > -1)
            {
                return "∫";
            }
            return "";
        }

        //替换占位符
        private string ReplaceWeiXinText(string text, Senparc.Weixin.MP.Entities.RequestMessageBase requestMessage)
        {
            text = text.Replace(":FromUserName", requestMessage.FromUserName);
            text = text.Replace(":ToUserName", requestMessage.ToUserName);
            return text;
        }


    }
}