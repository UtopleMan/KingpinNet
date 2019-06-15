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
        public static ValueType Convert(Type type)
        {
            if (type == typeof(bool))
                return ValueType.Bool;
            if (type == typeof(float))
                return ValueType.Float;
            if (type == typeof(DateTime))
                return ValueType.Date;
            if (type == typeof(TimeSpan))
                return ValueType.Duration;
            if (type == typeof(int))
                return ValueType.Int;
            return ValueType.String;
        }
    }
}
