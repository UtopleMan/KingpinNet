using System;

namespace KingpinNet
{
    public enum ValueType
    {
        String,
        Bool,
        Url,
        Int,
        Ip,
        Enum,
        Duration,
        Tcp,
        Float,
        Date
    }

    public static class ValueTypeConverter
    {
        internal static ValueType Convert(Type type)
        {
            if (type == typeof(Int32) || type == typeof(Int16) || type == typeof(Int64))
                return ValueType.Int;
            if (type == typeof(DateTime))
                return ValueType.Date;
            if (type == typeof(TimeSpan))
                return ValueType.Duration;
            if (type == typeof(float) || type == typeof(decimal))
                return ValueType.Float;
            if (type == typeof(bool))
                return ValueType.Bool;
            if (type == typeof(Uri))
                return ValueType.Url;
            if (type.IsEnum)
                return ValueType.Enum;
            return ValueType.String;
        }
    }
}
