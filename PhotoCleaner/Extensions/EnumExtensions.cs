using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PhotoCleaner.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription<TEnum>(this TEnum enumValue) 
            where TEnum : struct
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return enumValue.ToString();
        }
    }
}
