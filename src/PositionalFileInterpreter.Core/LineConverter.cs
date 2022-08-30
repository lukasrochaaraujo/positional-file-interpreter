using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace PositionalFileInterpreter.Core
{
    public static class LineConverter
    {
        private const char Empty = ' ';
        private const char Zero = '0';
        public static string DecimalSeparator { get; set; } = ",";
        public static string HundredSeparator { get; set; } = ".";

        public static T Deserialize<T>(string line) where T : new()
        {
            var objectT = new T();

            LineAttribute objectTLineAttribute;
            RowAttribute objectTRowPropertyAttribute;

            foreach (Attribute objectTAttribute in objectT.GetType().GetCustomAttributes(true))
            {
                objectTLineAttribute = objectTAttribute as LineAttribute;

                if (objectTLineAttribute != null)
                {
                    LineType objectTLineType = objectTLineAttribute.Type;

                    foreach (PropertyInfo objectTPropertyInfo in objectT.GetType().GetProperties())
                    {
                        foreach (Attribute objectTPropertyAttribute in objectTPropertyInfo.GetCustomAttributes(true))
                        {
                            objectTRowPropertyAttribute = objectTPropertyAttribute as RowAttribute;

                            if (objectTRowPropertyAttribute != null)
                            {
                                if (line.Length >= objectTRowPropertyAttribute.Start - 1 + objectTRowPropertyAttribute.Length)
                                {
                                    string lineValue = line.Substring(objectTRowPropertyAttribute.Start - 1, objectTRowPropertyAttribute.Length);

                                    if (string.IsNullOrWhiteSpace(lineValue))
                                        continue;

                                    if (objectTPropertyInfo.PropertyType == typeof(DateTime) && (lineValue.Equals("0000-00-00") || lineValue.Equals("000000") || lineValue.Equals("00000000")))
                                        continue;

                                    if (!objectTRowPropertyAttribute.Decimals.Equals(0))
                                    {
                                        string numberWithoutDecimals = line.Substring(objectTRowPropertyAttribute.Start - 1, objectTRowPropertyAttribute.Length - objectTRowPropertyAttribute.Decimals);
                                        string numberOnlyDecimals = line.Substring((objectTRowPropertyAttribute.Start + objectTRowPropertyAttribute.Length - objectTRowPropertyAttribute.Decimals) - 1, objectTRowPropertyAttribute.Decimals);
                                        lineValue = string.Concat(numberWithoutDecimals, DecimalSeparator, numberOnlyDecimals);
                                    }

                                    if (objectTPropertyInfo.PropertyType == typeof(DateTime) && !string.IsNullOrWhiteSpace(objectTRowPropertyAttribute.Format))
                                    {
                                        objectTPropertyInfo.SetValue(objectT, lineValue.Trim().ToDateTime(objectTRowPropertyAttribute.Format), null);
                                    }
                                    else if (objectTPropertyInfo.PropertyType == typeof(decimal) && !string.IsNullOrWhiteSpace(objectTRowPropertyAttribute.Culture))
                                    {
                                        objectTPropertyInfo.SetValue(objectT, lineValue.Trim().ToDecimal(objectTRowPropertyAttribute.Culture), null);
                                    }
                                    else
                                    {
                                        objectTPropertyInfo.SetValue(objectT, Convert.ChangeType(lineValue.Trim(), objectTPropertyInfo.PropertyType), null);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return objectT;
        }

        public static string Serialize<T>(T objectT) where T : new()
        {
            StringBuilder lineBuffer = null;
            LineAttribute objectTLineAttribute;
            RowAttribute objectTRowPropertyAttribute;
            
            foreach (Attribute attributeClass in objectT.GetType().GetCustomAttributes(true))
            {
                objectTLineAttribute = attributeClass as LineAttribute;

                if (null != objectTLineAttribute)
                {
                    LineType objectTLineType = objectTLineAttribute.Type;

                    lineBuffer = new StringBuilder(string.Empty.PadLeft(objectTLineAttribute.Length));

                    foreach (PropertyInfo propertyInfo in objectT.GetType().GetProperties())
                    {
                        foreach (Attribute attributeProperty in propertyInfo.GetCustomAttributes(true))
                        {
                            objectTRowPropertyAttribute = attributeProperty as RowAttribute;

                            if (null != attributeProperty)
                            {
                                string lineValue = string.Empty.PadLeft(objectTRowPropertyAttribute.Length, Empty);

                                if (propertyInfo.GetValue(objectT) != null)
                                {
                                    if (propertyInfo.PropertyType == typeof(DateTime))
                                    {
                                        var lineFormattedValue = Convert.ToDateTime(propertyInfo.GetValue(objectT));

                                        if (lineFormattedValue != DateTime.MinValue)
                                        {
                                            string mask = string.IsNullOrWhiteSpace(objectTRowPropertyAttribute.Format) ? "yyyyMMdd" : objectTRowPropertyAttribute.Format;
                                            lineValue = string.Format("{0:" + mask + "}", lineFormattedValue).PadLeft(objectTRowPropertyAttribute.Length, Zero).PadLeft(objectTRowPropertyAttribute.Length, Zero);
                                        }
                                        else
                                        {
                                            lineValue = Zero.ToString().PadLeft(objectTRowPropertyAttribute.Length, Zero).PadLeft(objectTRowPropertyAttribute.Length, Zero);
                                        }

                                    }
                                    else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(long))
                                    {
                                        lineValue = string.Format("{0:0}", propertyInfo.GetValue(objectT)).PadLeft(objectTRowPropertyAttribute.Length, Zero);
                                    }
                                    else if (propertyInfo.PropertyType == typeof(decimal))
                                    {
                                        string mask = string.Concat("{0:F", objectTRowPropertyAttribute.Decimals, "}");

                                        if (string.IsNullOrEmpty(objectTRowPropertyAttribute.Culture))
                                            lineValue = string.Format(mask, propertyInfo.GetValue(objectT)).Replace(HundredSeparator, string.Empty).Replace(DecimalSeparator, string.Empty);
                                        else
                                            lineValue = Convert.ToDecimal(propertyInfo.GetValue(objectT)).ToString("F", CultureInfo.CreateSpecificCulture(objectTRowPropertyAttribute.Culture));

                                        lineValue = lineValue.PadLeft(objectTRowPropertyAttribute.Length, Zero);

                                    }
                                    else
                                    {
                                        lineValue = ((string)propertyInfo.GetValue(objectT)).PadRight(objectTRowPropertyAttribute.Length, Empty);
                                    }

                                }

                                int lineStartIndex = objectTRowPropertyAttribute.Start - 1;
                                int lineLength = objectTRowPropertyAttribute.Length;

                                if (objectTRowPropertyAttribute.Decimals > 0)
                                    lineLength++;

                                lineBuffer.Remove(lineStartIndex, lineLength);
                                lineBuffer.Insert(lineStartIndex, lineValue.Substring(0, objectTRowPropertyAttribute.Length));
                            }
                        }
                    }

                }
            }

            return lineBuffer?.ToString() ?? "";
        }
    }
}
