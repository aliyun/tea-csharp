using System;

namespace Tea
{
    public class NameInMapAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Example { get; set; }

        public string Regexp { get; set; }

    }
}
