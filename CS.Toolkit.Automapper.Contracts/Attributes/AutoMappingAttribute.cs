using System;

namespace CS.Toolkit.Automapper.Contracts.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class AutoMappingAttribute : Attribute
    {
        public string SourceTypeName { get; }
        public string TargetTypeName { get; }
        public AutoMappingAttribute(Type sourceType, Type targetType)
        {
            SourceTypeName = sourceType.FullName;
            TargetTypeName = targetType.FullName;
        }
    }
}
