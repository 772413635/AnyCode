using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AnyCode
{
    public class Sys_PerformLog
    {

        public ObjectId _id { get; set; }
        public string Ip { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Params { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }

        private DateTime _CreateTime;

        public DateTime CreateTime
        {
            get { return _CreateTime.ToLocalTime(); }
            set { _CreateTime=value; }
        }
    }
}