using System;

namespace Tea
{
    public class ValidationAttribute : Attribute
    {
        public string Pattern { get; set; }

        public int MaxLength { get; set; }

        public bool Required { get; set; }
    }
}
