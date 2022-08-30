using System;

namespace PositionalInterpreter.Core
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class RowAttribute : Attribute
    {
        public int Start { get; private set; }
        public int Length { get; private set; }
        public int Decimals { get; private set; }
        public string Format { get; private set; }
        public string Culture { get; private set;  }

        public RowAttribute(int start, int length, string format)
        {
            Start = start;
            Length = length;
            Format = format;
        }

        public RowAttribute(int start, int length, int decimals = 0, string culture = null)
            : this(start, length, format: null)
        {
            Decimals = decimals;
            Culture = culture;
        }
    }
}
