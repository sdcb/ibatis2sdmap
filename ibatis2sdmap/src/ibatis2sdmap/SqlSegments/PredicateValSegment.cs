using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class PredicateValSegment : SqlSegment
    {
        public string MacroName { get; }

        public string Property { get; }

        public string Prepend { get; }

        public string CompareValue { get; }

        public IEnumerable<SqlSegment> Segments { get; }

        public PredicateValSegment(XElement xe, string macroName)
        {
            // compareValue
            Property = xe.Attribute("property")?.Value;
            CompareValue = xe.Attribute("compareValue").Value;

            Prepend = xe.Attribute("prepend")?.Value ?? "";
            Segments = xe.Nodes().Select(Create);
            MacroName = macroName;
        }

        public override string Emit()
        {
            return
                $"#{MacroName}<{Property}, @\"{CompareValue}\", sql{{" +
                $"{Prepend} {string.Concat(Segments.Select(x => x.Emit()))}" +
                $"}}>";
        }
    }
}
