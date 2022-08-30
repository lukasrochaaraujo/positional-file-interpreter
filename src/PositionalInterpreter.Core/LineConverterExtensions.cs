using System;
using System.Globalization;

namespace PositionalInterpreter.Core
{
    public static class LineConverterExtensions
    {
        public static DateTime ToDateTime(this string value, string format = "yyyy-MM-dd")
        {
            return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        }

        public static decimal ToDecimal(this string value, string culture)
        {
            string nullChar = "\0";

            if (value.Contains(nullChar))
                value = "0";

            return decimal.Parse(value, NumberStyles.Any, new CultureInfo(culture));
        }
    }
}
