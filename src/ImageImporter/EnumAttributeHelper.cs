using System;

namespace ImageImporter
{
    public static class EnumAttributeHelper
    {
        public static T GetAttributeOfType<T>(this Enum value) where T:Attribute
        {
            var enumType = value.GetType();
            var memberInfo = enumType.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }
    }
}
