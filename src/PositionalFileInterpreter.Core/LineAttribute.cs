using System;

namespace PositionalFileInterpreter.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LineAttribute : Attribute
    {
        public LineType Type { get; private set; }
        public int Length { get; private set; }

        public LineAttribute(LineType type)
        {
            Type = type;
        }

        public LineAttribute(LineType type, int length) : this(type)
        {
            Length = length;
        }
    }
}
