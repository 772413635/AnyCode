using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace AnyCode
{
    public class Sys_JobMessage
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }

        private DateTime _CreateTime;
        public DateTime CreateTime
        {
            get { return _CreateTime.ToLocalTime(); }
            set { _CreateTime = value; }
        }
    }
}