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

        public JsonpResult InitUser(DataGridParam param)
        {
            return new JsonpResult
            {
                Data = _seafood.InitUser(param),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonpResult AddressList(DataGridParam param)
        {
            var data = _seafood.AddressList(param);
            return new JsonpResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonpResult DefaultAddress(DataGridParam param)
        {
            var address = _seafood.DefaultAddress(param);
            if (address == null)
            {
                return new JsonpResult
                {
                    Data = "",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            return new JsonpResult
            {
                Data = address,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public JsonpResult UpdateAddressDefault(DataGridParam param)
        {
            _seafood.UpdateAddressDefault(param);
            return new JsonpResult
            {
                Data =true,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            }; 
        }

        public JsonpResult AddAddress(DataGridParam param)
        {
           var res= _seafood.AddAddress(param);
           return new JsonpResult
            {
                Data = res,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        } 

    }
}