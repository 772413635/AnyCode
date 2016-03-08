using System.Collections.Generic;
using System.Linq;
using Common;
using DBlinq;

namespace AnyCode
{
    public class PrimaryTreeNodeCollection
    {
        private IEnumerable<Sys_Menu> _allEntitys;
        private IEnumerable<Sys_Menu> _fatherTree;
        private IEnumerable<Sys_Menu> _sonTree;

        public bool Bind(IEnumerable<Sys_Menu> allEntitys, ref List<SystemTree> myChildren,string fid )
        {
            _allEntitys = allEntitys;
            _fatherTree = from o in allEntitys
                          where o.ParentId == null
                          select o; //根目录

            if (_fatherTree == null || !_fatherTree.Any())
            {
                return false;
            }

            //填充数据
            foreach (var item in _fatherTree)
            {
                var myTree = new SystemTree { id = item.Id.ToString(), text = item.Name };
                myTree.iconCls = "icon-star";
                myTree.@checked = false;
                myChildren.Add(myTree);
                var sons = from o in allEntitys
                           where o.ParentId == item.Id.ToString()
                           select o;
                if (sons.Any())
                {
                    if (BindSon(sons, myTree, fid))
                    {
                        myTree.state = "open";
                    }
                }


            }
            return true;
        }

        public bool BindSon(IEnumerable<Sys_Menu> entitys, SystemTree fatertree, string fid)
        {
            _sonTree = entitys;
            if (_sonTree != null && _sonTree.Any())
            {
                //填充数据
                foreach (var item in _sonTree)
                {
                    var myTree = new SystemTree { id = item.Id.ToString(), text = item.Name, iconCls = item.Iconic };
                    fatertree.children.Add(myTree);


                    if (!string.IsNullOrWhiteSpace(fid))//不为空,进行功能权限的判断
                    {
                        fatertree.@checked = false;
                        var fids = fid.Split(',');//菜单权限数组

                        var functions = (from tt in fids
                            where tt.Split('[')[0] == fatertree.id
                            select new
                            {
                               fs= tt.Split('[')[1].TrimEnd(']').Split(';')
                            }).SingleOrDefault();//获取该角色针对该菜单的功能权限
                        if (functions != null && functions.fs.Contains(myTree.id))
                        {
                            myTree.@checked = true;
                        }
                        else
                        {
                            myTree.@checked = false;
                        }

                    }


                }
                return true;
            }
            return false;
        }

        public bool BindData(IEnumerable<Sys_Menu> entitys, ref List<SystemTree> myChildren, List<string> pid,string fid)
        {
            _allEntitys = entitys;
            _fatherTree = from o in entitys
                          where o.ParentId == null
                          select o; //根目录

            if (_fatherTree == null || !_fatherTree.Any())
            {
                return false;
            }

            //填充数据
            foreach (var item in _fatherTree)//遍历根目录
            {
                var myTree = new SystemTree { id = item.Id.ToString(), text = item.Name, iconCls = item.Iconic };

                myChildren.Add(myTree);
                var sons = from o in entitys
                           where o.ParentId == item.Id.ToString()
                           select o;//一级子目录
                if (BindDataSon(sons, ref myTree.children, pid, fid))
                {
                    myTree.state = "open";
                }

            }
            return true;
        }

        public bool BindDataSon(IEnumerable<Sys_Menu> entitys, ref List<SystemTree> myChildren, List<string> pid, string fid)
        {
            _sonTree = entitys;
            if (_sonTree != null && _sonTree.Any())
            {
                //填充数据
                foreach (var item in _sonTree)
                {
                    var myTree = new SystemTree
                    {
                        id = item.Id.ToString(),
                        text = item.Name,
                        iconCls = item.Iconic,
                        functions = !string.IsNullOrWhiteSpace(item.Function) ? item.Function.Split(',').GetIntArray() : new []{-1000}
                    };
                    if (pid.Contains(item.Id.ToString()))
                        myTree.@checked = true;
                    else
                        myTree.@checked = false;
                 
                    myChildren.Add(myTree);

                    // 递归调用,获取子菜单的功能模块
                    var db = new GodBuildDB();
                    var currentSon = from c in db.Sys_Function
                                     where myTree.functions.Contains(c.Id)
                                     select new
                                     {
                                         c.Name,
                                         c.Id
                                     };
                    var currentSonT = new List<Sys_Menu>();
                    foreach (var c in currentSon)
                    {
                        currentSonT.Add(new Sys_Menu
                        {
                            Name=c.Name,
                            Id=c.Id,
                            Iconic=null
                        });
                    }
                    if (currentSonT.Any())
                    {
                        myTree.state = "closed";
                        BindSon(currentSonT,myTree, fid);
                    }
                }
                return true;
            }
            return false;
        }


        
    }
}