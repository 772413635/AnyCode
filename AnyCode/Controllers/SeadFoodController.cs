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
    public class SeadFoodController:BaseController
    {
        private readonly ISeadFoodService _seadfood;


        public SeadFoodController()
        {
            _seadfood = new SeadFoodService(new LinqToDB(), LoginUser);
        }


        [HttpPost]
        public JsonpResult ProductList(DataGridParam param)
        {
            var data = _seadfood.ProductList(param);
            return new JsonpResult
            {
                Data = data
            };
        }
    }
}