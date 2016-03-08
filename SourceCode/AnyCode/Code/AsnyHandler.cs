using System;
using System.Threading;
using System.Web;

namespace AnyCode
{
    public class AsnyHandler : IHttpAsyncHandler
    {
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            //myAsynResult为实现了IAsyncResult接口的类，当不调用cb的回调函数时，该请求不会返回到给客户端，会一直处于连接状态
            myAsynResult asyncResult = new myAsynResult(context, cb, extraData);
            String content = context.Request.Params["content"];
            string[] code=null;
            string isSystem = null,title=null;
            if (context.Request.Params["loginUserType"] != null)
            {
                isSystem = context.Request.Params["loginUserType"];
            }// add by xhg 2012/09/24 PM 用户是否为系统管理员
            string createPeople = null;
            if (context.Request.Params["code[]"] != null)
                code = context.Request.Params["code[]"].Split(',');
            if (context.Request.Params["code"] != null)
                code = context.Request.Params["code"].Split(',');
            if (context.Request.Params["createPeople"] != null)
                createPeople = context.Request.Params["createPeople"];
            if (context.Request.Params["title"] != null)
            {
                title = context.Request.Params["title"];
            }

            //向Message类中添加该消息
            Messages.Instance().AddMessage(title, content, code, createPeople, asyncResult, isSystem);
            return asyncResult;
        }

        #region 不必理会

        public void EndProcessRequest(IAsyncResult result)
        {

        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
        }
        #endregion
    }
    public class myAsynResult : IAsyncResult
    {
        bool _IsCompleted;
        private readonly HttpContext context;
        private readonly AsyncCallback cb;

        public myAsynResult(HttpContext context, AsyncCallback cb, object extraData)
        {
            this.context = context;
            this.cb = cb;
        }

        public string Content { get; set; }

        public string ReceiveId { get; set; }

        #region IAsyncResult接口
        public object AsyncState
        {
            get { return null; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }
        public bool IsCompleted
        {
            get { return _IsCompleted; }
        }




        #endregion



        //在Message类中的添加消息方法中，调用该方法，将消息输入到客户端，从而实现广播的功能
        public void Send(object data)
        {

            context.Response.Write(Content);
            if (cb != null)
            {
                cb(this);
            }
            _IsCompleted = true;
        }
    }

}