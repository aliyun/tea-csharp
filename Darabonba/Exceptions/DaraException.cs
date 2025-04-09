using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Tea;

namespace Darabonba.Exceptions
{
    public class DaraException : TeaException
    {
        private string customCode;
        private string customMessage;
        private Dictionary<string, object> customData;
        private int customStatusCode;
        private string customDescription;
        private Dictionary<string, object> customAccessDeniedDetail;
        
        public DaraException() : base(new Dictionary<string, object>())
        {
        }
        
        public new string Code
        {
            get { return customCode ?? base.Code; }
            set { customCode = value; SetInternalField("code", value); }
        }
        
        public new string Message
        {
            get { return customMessage ?? base.Message; }
            set { customMessage = value; SetInternalField("message", value); }
        }
        
        public new Dictionary<string, object> Data
        {
            get { return customData ?? base.DataResult; }
            set { customData = value; SetInternalField("data", value); }
        }
        
        public new int StatusCode
        {
            get { return customStatusCode != 0 ? customStatusCode : base.StatusCode; }
            set { customStatusCode = value; SetInternalField("statusCode", value); }
        }
        
        public new string Description
        {
            get { return customDescription ?? base.Description; }
            set { customDescription = value; SetInternalField("description", value); }
        }
        
        public new Dictionary<string, object> AccessDeniedDetail
        {
            get { return customAccessDeniedDetail ?? base.AccessDeniedDetail; }
            set { customAccessDeniedDetail = value; SetInternalField("accessDeniedDetail", value); }
        }
        
        // 使用反射设置TeaException中的私有字段
        private void SetInternalField(string fieldName, object value)
        {
            var field = typeof(TeaException).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(this, value);
            }
        }
        
        public DaraException(IDictionary dict) : base(dict)
        {
            Message = base.Message;
            Code = base.Code;
            StatusCode = base.StatusCode;
            Description = base.Description;
            AccessDeniedDetail = base.AccessDeniedDetail;
            Data = base.DataResult;
        }
    }
}