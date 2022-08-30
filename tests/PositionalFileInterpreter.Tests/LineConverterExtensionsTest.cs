using PositionalFileInterpreter.Core;
using System;
using Xunit;

namespace PositionalFileInterpreter.Tests
{
    public class LineConverterExtensionsTest
    {
        [Theory]
        [InlineData("2022-01-01")]
        [InlineData("2021-11-10")]
        [InlineData("1995-05-19")]
        public void ConvertToDateTimeUsingDefaultFormat(string value)
        {
            //arrange & act
            DateTime convertedValue = value.ToDateTime();

            //assert
            Assert.Equal(value, convertedValue.ToString("yyyy-MM-dd"));
        }

        [Theory]
        [InlineData("2022-01-01", "yyyy-MM-dd")]
        [InlineData("202111", "yyyyMM")]
        [InlineData("19-05-1995", "dd-MM-yyyy")]
        public void ConvertToDateTimeUsingSpecifiedFormat(string value, string format)
        {
            //arrange & act
            DateTime convertedValue = value.ToDateTime(format);

            //assert
            Assert.Equal(value, convertedValue.ToString(format));
        }

        [Theory]
        [InlineData("2.000,00", "pt-br", 2000)]
        [InlineData("1,999.99", "en-us", 1999.99)]
        [InlineData("0,99", "pt-br", 0.99)]
        public void ConvertToDecimalWithoutNullChar(string value, string culture, decimal expectedValue)
        {
            //arrange & act
            decimal convertedValue = value.ToDecimal(culture);

            //assert
            Assert.Equal(expectedValue, convertedValue);
        }

        [Theory]
        [InlineData("\0\0\0", "pt-br", 0)]
        [InlineData("\0", "pt-br", 0)]
        [InlineData("\0\0\0\0", "pt-br", 0)]
        public void ConvertToDecimalWithNullChar(string value, string culture, decimal expectedValue)
        {
            //arrange & act
            decimal convertedValue = value.ToDecimal(culture);

            //assert
            Assert.Equal(expectedValue, convertedValue);
        }
    }
}
