using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AnyCode.Models.Interfaces;
using AnyCode.Models.Service;
using Common;
using DBlinq;

namespace AnyCode.Controllers
{
    public class SeaFoodController:BaseController
    {
        private readonly ISeaFoodService _seafood;


        public SeaFoodController()
        {
            _seafood = new SeaFoodService(new LinqToDB(), LoginUser);
        }


        public JsonpResult ProductList(DataGridParam param)
        {
            var data = _seafood.ProductList(param);
            return new JsonpResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonpResult IntiUser(DataGridParam param)
        {
            return new JsonpResult
            {
                Data = _seafood.IntiUser(param),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonpResult AddressList(DataGridParam param)
        {
            return new JsonpResult
            {
                Data = null,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}