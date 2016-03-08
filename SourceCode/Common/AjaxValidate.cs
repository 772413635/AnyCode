using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class AjaxValidate
    {
        /// <summary>
        /// 验证字段名
        /// </summary>
        public string FieldId { get; set; }
        /// <summary>
        /// 用户输入值
        /// </summary>
        public string FieldValue { get; set; }

        public string ExtraData { get; set; }

        public string ExtraDataDynamic { get; set; }
    }

    public class AjaxValidateResult
    {
        /// <summary>
        /// 验证字段id
        /// </summary>
        public string ErrorFieldId { get; set; }
        /// <summary>
        /// 验证结果，true为验证通过
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 验证消息
        /// </summary>
        public string Msg { get; set; }
    }
}
