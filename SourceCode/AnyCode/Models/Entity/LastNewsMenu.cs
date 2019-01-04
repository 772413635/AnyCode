using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnyCode.Models.Entity
{
    public class LastNewsMenu
    {
        public string title { get; set; }
        public string code { get; set; }
        public List<LastNewsMenu> child { get; set; }

        public override bool Equals(object obj)
        {
            var obj2 = (LastNewsMenu)obj;
            if (this.title == obj2.title && this.code == obj2.code)
            {
                var res = true;
                if (this.child == obj2.child)
                    res = true;
                else if (this.child == null && obj2.child != null)
                    res = false;
                else if (this.child != null && obj2.child == null)
                    res = false;
                else if (this.child.Count != obj2.child.Count)
                    res = false;
                else
                    for (var i = 0; i < obj2.child.Count; i++)
                    {
                        res = this.child[i].Equals(obj2.child[i]);
                    }
                return res;
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            return this.title.GetHashCode() + this.code.GetHashCode();
        }
    }
}