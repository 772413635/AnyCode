using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DBlinq
{
    public class MongoDbHelper
    {
        private static MongoServer _server;

        public static  MongoServer Server
        {
            get
            {
                if (_server == null)
                {
                    _server = MongoServer.Create(Properties.Settings.Default.MongoDBConnectionString);
                    _server.Connect();
                }
                else
                {
                    if (_server.State == MongoServerState.Disconnected)
                    {
                        _server.Connect();
                    }
                }
                
                return _server;
            }
        }

        public static MongoDatabase DataBase
        {
            get { return Server.GetDatabase("AnyCode"); }
        }
    }
}
