using System;
using System.Text.RegularExpressions;

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
                else if (obj is string && obj.ToString() == "")
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

    }
}
