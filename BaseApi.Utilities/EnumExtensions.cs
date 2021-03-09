using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaseApi.Utilities
{
    public static class EnumExtensions
    {
        public static string GetStringValue(this Enum value)
        {
            var retVal = default(string);

            if (value != null)
            {
                Type type = value.GetType();
                FieldInfo fieldInfo = type.GetField(value.ToString());
                var attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                retVal = attribs.Length > 0 ? attribs[0].StringValue : null;
            }

            return retVal;
        }


        public static bool HasImplicitConversion(this Type baseType, Type targetType)
        {
            var retVal = baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                .Any(mi =>
                {
                    ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                    return pi != null && pi.ParameterType == baseType;
                });

            return retVal;
        }
    }
}
