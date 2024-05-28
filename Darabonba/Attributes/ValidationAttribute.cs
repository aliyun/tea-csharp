using System;

namespace Darabonba
{
    public class ValidationAttribute : Attribute
    {
        public string Pattern { get; set; }

        public int MaxLength { get; set; }

        public bool Required { get; set; }

        public int MinLength { get; set; }

        public double Maximun { get; set; }

        public double Minimum { get; set; }
    }
}
