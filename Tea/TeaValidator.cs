using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly : InternalsVisibleTo("TeaUnitTests")]

namespace Tea
{
    internal class TeaValidator
    {
        public ValidationAttribute Attribute { get; set; }

        public string PropertyName { get; set; }

        public TeaValidator(ValidationAttribute attribute, string propertyName)
        {
            Attribute = attribute;
            PropertyName = propertyName;
        }

        public void ValidateRequired(object obj)
        {
            if (Attribute != null && Attribute.Required)
            {
                if (obj == null)
                {
                    throw new ArgumentException(string.Format("{0} is required.", PropertyName));
                }
            }
        }

        public void ValidateRegex(object obj)
        {
            if (Attribute != null && !string.IsNullOrEmpty(Attribute.Pattern) && obj != null)
            {
                Match match = Regex.Match(obj.ToString(), Attribute.Pattern);
                if (!match.Success)
                {
                    throw new ArgumentException(string.Format("{0} is not match {1}", PropertyName, Attribute.Pattern));
                }
            }
        }


        public void ValidateMaxLength(object obj)
        {
            bool result = true;
            if(Attribute != null && Attribute.MaxLength>0)
            {
                if (typeof(IList).IsAssignableFrom(obj.GetType()))
                {
                    result = ((IList)obj).Count <= Attribute.MaxLength;
                }
                else
                {
                    result = obj.ToString().Length <= Attribute.MaxLength;
                }
            }

            if (!result)
            {
                throw new ArgumentException(string.Format("{0} is exceed max-length: {1}", PropertyName, Attribute.MaxLength));
            }
        }
    }
}
