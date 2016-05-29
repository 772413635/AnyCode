using System.Collections.Generic;

namespace Common
{
    public class SystemTree
    {
        public string id;
        public string text;
        public string iconCls;
        public bool @checked = false;
        public string state;
        public List<SystemTree> children = new List<SystemTree>();
        public int[] functions;
    }

    public class SeatTeamTree
    {
        public string id;
        public string text;
        public string Cid;
        public string TeamId;
        public string iconCls;
        public bool @checked = false;
        public string sort;
        public string state;
        public string type;
        public List<SeatTeamTree> children = new List<SeatTeamTree>();
    }

    public class  CompanyTree
    {
        public string id;
        public string text;
        public string iconCls;
        public bool @checked = false;
        public string state;
        public string sort;
    }

    public class SeatModelTree
    {
        public string id;
        public string text;
        public string iconCls;
        public bool @checked = false;
        public string state;
        public string sort;
        public List<SeatModelTree> children = new List<SeatModelTree>();
    }
   
    public class IvrTree
    {
        public int id;
        public string name;
        public string code;
        public string iconCls;
        public bool @checked = false;
        public string state;
        public string sort;
        public string istransf;
        public string team;
        public List<IvrTree> children = new List<IvrTree>();
    }
}
