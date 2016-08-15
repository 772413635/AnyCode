using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Common;
using DBlinq;
using AnyCode.Models.Interfaces;
using EfSearchModel;
using Common;
using EfSearchModel.Model;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Menu;

namespace AnyCode.Models.Service
{
    public class WebChatService : BaseService, IWebChatService
    {
        private XDocument doc;
        public WebChatService(LinqToDB db, Sys_User loginUser)
            : base(db, loginUser)
        {

        }

        /// <summary>
        /// 微信公众号账号列表
        /// </summary>
        /// <param name="param">查询对象</param>
        /// <returns></returns>
        public JqueryGridObject WxCfgList(DataGridParam param)
        {
            var qm = param.Query.GetModel();
            if (!LoginUser.IsSystem)
            {
                qm.Items.Add(new ConditionItem
                {
                    Field = "CreatePerson",
                    Method = QueryMethod.Equal,
                    Value = LoginUser.Id
                });
            }
            var tquery = _db.Sys_WebChat_Config.Where(qm).OrderBy(param.SortName + " " + param.SortOrder);
            var count = tquery.Count();
            var query = tquery.Skip((param.Page - 1)*param.RP).Take(param.RP);
            foreach (var q in query)
            {
                AccessTokenContainer.Register(q.AppId, q.AppSecret);
            }
            return new JqueryGridObject
            {
                DataGridParam = param,
                total = count,
                Query = query
            };

            
        }

        //删除微信账号
        public string DeleteAccount<T>(string id) where T : class, new()
        {
            return DeleteData<T>(id);
        }

        public string CreateOrUpdate<T>(string pkName, dynamic pkValue, dynamic initialValue, T InsertEntity, dynamic updateEntity) where T : class, new()
        {
            return DataControl(pkName, pkValue, initialValue, InsertEntity, updateEntity);
        }

        public AjaxValidateResult SingleToken(AjaxValidate ajaxValidate)
        {
            var ExtraDatas = ajaxValidate.ExtraData.Split(',');
            var singleData = _db.Sys_WebChat_Config.Any(c => c.Token == ajaxValidate.FieldValue);//通过token查询数据
            if (singleData)//数据存在
            {
                if (ExtraDatas[1] != "0") //修改
                {
                    var editModel = _db.Sys_WebChat_Config.Single(c => c.Id == Int32.Parse(ExtraDatas[1]));
                    if (editModel.Token == ajaxValidate.FieldValue)//token值未修改，则也通过验证
                    {
                        return new AjaxValidateResult
                        {
                            ErrorFieldId = ExtraDatas[0],
                            Status = true,
                            Msg = "此Token可以使用",
                        };
                    }
                    else//说明修改后的token值不是原来值
                    {
                        return new AjaxValidateResult
                        {
                            ErrorFieldId = ExtraDatas[0],
                            Status = false,
                            Msg = "此Token已经存在",
                        };
                    }
                }
                else//新增
                {
                    return new AjaxValidateResult
                    {
                        ErrorFieldId = ExtraDatas[0],
                        Status = false,
                        Msg = "此Token已经存在",
                    };
                }

            }
            else
            {
                return new AjaxValidateResult
                {
                    ErrorFieldId = ExtraDatas[0],
                    Status = true,
                    Msg = "此Token可以使用",
                };
            }
        }

        public string DelteMenu(string menuid)
        {
            return DeleteData<Sys_WebChat_Menu>(menuid);
        }

        public string ReName(int menuid, string newName)
        {
            return DataControl<Sys_WebChat_Menu>("Id", menuid, 0, null, new
            {
                Name = newName
            });
        }

        public string AddFartherMenu(string name, int cfgId)
        {
            return DataControl("Id", 0, 0, new Sys_WebChat_Menu
            {
                Name = name,
                CfgId = cfgId
            }, null);
        }

        public string UpdateFartherMenu(int id)
        {
            return DataControl("Id", id, 0, new Sys_WebChat_Menu(), new
            {
                Type = new int?(),
                Key = "",
                Url = "",
                Media_id = ""
            });
        }

        public string AddChildrenMenu(string name, int cfgId, int fid)
        {
            return DataControl("Id", 0, 0, new Sys_WebChat_Menu
            {
                Name = name,
                CfgId = cfgId,
                Fid = fid,
                Type = 1
            }, null);
        }

        public string SetChildrenBtn(int id, int type, string val)
        {
            dynamic model = null;
            switch ((ButtonType)type)
            {
                case ButtonType.view:
                    {
                        model = new
                        {
                            Key = "",
                            Url = val,
                            Type = type

                        };
                    }
                    break;
                default://暂时这样写，后续会做修改
                    {
                        model = new
                        {
                            Key = val,
                            Url = "",
                            Type = type
                        };
                    }
                    break;
            }

            return DataControl<Sys_WebChat_Menu>("Id", id, 0, null, model);
        }

        public dynamic Menu(int configId)
        {
            var menuList = _db.Sys_WebChat_V_Menu.Where(c => c.cfgid == configId).ToList();//获得所有菜单数据
            var firstNoSecondMenu = menuList.Where(c => c.typeid != null && c.fid == null).ToList();//一级菜单(没有二级菜单)
            var firstMenu = menuList.Where(c => c.typeid == null && c.fid == null).ToList();//一级父菜单(有二级菜单)
            var buttonList = firstNoSecondMenu;//weixin菜单
            foreach (var fm in firstMenu)
            {
                var secondMenu = menuList.OrderBy(c=>c.sort).Where(c => c.fid == fm.id);//获取子菜单
                fm.sub_button = secondMenu;
                buttonList.Add(fm);
            }
            var menuModel = new
            {
                button = buttonList.OrderBy(c=>c.sort)
            };
            return menuModel;
        }

        public WxJsonResult CreateMenu(int cfgid, string appId)
        {
            var menuModel = Menu(cfgid);
            return CommonApi.CreateMenu(appId, (object)menuModel);
        }

        public JqueryGridObject AutoReplyList(DataGridParam param)
        {
            var tquery = _db.Sys_WebChat_V_MsgMap.Where(param.Query.GetModel());
            var count = tquery.Count();
            var query = tquery.Skip((param.Page - 1)*param.RP).Take(param.RP);
            return new JqueryGridObject
            {
                DataGridParam = param,
                total = count,
                Query = query
            };
        }
    }
}