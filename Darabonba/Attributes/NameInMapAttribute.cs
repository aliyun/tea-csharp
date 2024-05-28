using System;

namespace Darabonba
{
    public class NameInMapAttribute : Attribute
    {
        public string Name { get; set; }

        public NameInMapAttribute(string name)
        {
            Name = name;
        }

    }
}
