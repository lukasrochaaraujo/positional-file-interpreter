using System;
using System.Globalization;

namespace PositionalFileInterpreter.Core
{
    public static class LineConverterExtensions
    {
        public static DateTime ToDateTime(this string value, string format = "yyyy-MM-dd")
        {
            return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        }

        public static decimal ToDecimal(this string value, string culture)
        {
            if (value.Contains("\0"))
                value = "0";

            return decimal.Parse(value, NumberStyles.Any, new CultureInfo(culture));
        }
    }
}
