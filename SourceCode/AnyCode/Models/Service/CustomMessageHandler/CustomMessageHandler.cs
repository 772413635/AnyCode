using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using DBlinq;
using Senparc.Weixin.MP.AppStore;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;

namespace AnyCode.Models.Service
{
    /// <summary>
    /// 自定义MessageHandler
    /// 把MessageHandler作为基类，重写对应请求的处理方法
    /// </summary>
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        private readonly LinqToDB _db;
        private readonly PostModel _postModel;
        private readonly Sys_WebChat_Config _webChat;
        /*
         * 重要提示：v1.5起，MessageHandler提供了一个DefaultResponseMessage的抽象方法，
         * DefaultResponseMessage必须在子类中重写，用于返回没有处理过的消息类型（也可以用于默认消息，如帮助信息等）；
         * 其中所有原OnXX的抽象方法已经都改为虚方法，可以不必每个都重写。若不重写，默认返回DefaultResponseMessage方法中的结果。
         */
        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount, LinqToDB db, Sys_WebChat_Config webChat)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
            _postModel = postModel;
            _db = db;
            _webChat = webChat;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            /* 所有没有被处理的消息会默认返回这里的结果，
             * 因此，如果想把整个微信请求委托出去（例如需要使用分布式或从其他服务器获取请求），
             * 只需要在这里统一发出委托请求，如：
             * var responseMessage = MessageAgent.RequestResponseMessage(agentUrl, agentToken, RequestDocument.ToString());
             * return responseMessage;
             */

            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自AnyCode";
            return responseMessage;
        }


        /// <summary>
        /// 用户关注事件
        /// </summary>
        /// <param name="requestMessage">事件消息</param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            AccessTokenContainer.Register(_postModel.AppId, _webChat.AppSecret);
            var _service = new WebChatService(_db, null);
            _service.CreateMenu(_webChat.Id,_postModel.AppId);//生成微信菜单
            var replyType =
              _db.Sys_WebChat_MsgMap.SingleOrDefault(c => c.Token == _postModel.Token && c.Code == "subscribe");
            if (replyType != null)
            {
                return ReplyMsaage(replyType.MsgType, replyType.MsgOrGroupId, requestMessage);
            }
            return null;
        }

        /// <summary>
        /// 菜单点击事件
        /// </summary>
        /// <param name="requestMessage">事件消息</param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            var msgType = _db.Sys_WebChat_MsgMap.SingleOrDefault(c => c.Code == requestMessage.EventKey);
            if (msgType != null)
            {
                return ReplyMsaage(msgType.MsgType, msgType.MsgOrGroupId, requestMessage);
            }
            //后台未设置则返回默认消息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您点击了【" + requestMessage.EventKey + "】按钮";
            return responseMessage;
        }

        /// <summary>
        /// 扫码等待事件
        /// </summary>
        /// <param name="requestMessage">事件消息</param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = requestMessage.ScanCodeInfo.ScanResult;
            return responseMessage;
        }

        /// <summary>
        /// 文字消息处理
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            return MessageText(requestMessage.Content);
        }

    }
}