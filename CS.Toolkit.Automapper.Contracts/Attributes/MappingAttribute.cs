using System;

namespace CS.Toolkit.Automapper.Contracts.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MappingAttribute : Attribute
    {
        public string TargetPropertyPath { get; }
        public MappingAttribute()
        {                
        }

        public MappingAttribute(string targetPropertyPath) : this()
        {
            TargetPropertyPath = targetPropertyPath;
        }

        public MappingAttribute(string targetPropertyPath, Type converterType) : this(targetPropertyPath)
        {
            
        }
    }
}
