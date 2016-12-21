using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class PredicateSegment : SqlSegment
    {
        public string MacroName { get; }

        public string Property { get; }

        public string Prepend { get; }

        public IEnumerable<SqlSegment> Segments { get; }

        public PredicateSegment(XElement xe, string macroName)
        {
            Property = xe.Attribute("property").Value;
            Prepend = xe.Attribute("prepend")?.Value ?? "";
            Segments = xe.Nodes().Select(Create);
            MacroName = macroName;
        }

        public override string Emit()
        {
            return
                $"#{MacroName}<{Property}, sql{{" +
                $"{Prepend} {string.Concat(Segments.Select(x => x.Emit()))}" +
                $"}}>";
        }
    }
}
