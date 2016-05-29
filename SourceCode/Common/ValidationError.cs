using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 验证错误
    /// </summary>
    public class ValidationError
    {
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// 多个验证错误的集合
    /// </summary>
    public class ValidationErrors : List<ValidationError>
    {
        public void Add(string errorMessage)
        {
            base.Add(new ValidationError { ErrorMessage = errorMessage });
        }
    }
}
