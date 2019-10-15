using System;

namespace Tea
{
    public class NameInMapAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description{get;set;}

        public string Example{get;set;}

        public NameInMapAttribute()
        {

        }

        public NameInMapAttribute(string Name):this(Name,null)
        {
            
        }

        public NameInMapAttribute(string Name,string Description):this(Name,Description,null)
        {
            
        }

        public NameInMapAttribute(string Name,string Description,string Example)
        {
            this.Name=Name;
            this.Description=Description;
            this.Example=Example;
        }
    }
}