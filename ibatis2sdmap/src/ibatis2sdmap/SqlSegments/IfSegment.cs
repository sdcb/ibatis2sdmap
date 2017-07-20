using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class IfSegment : SqlSegment
    {
        public string MacroName { get; }

        public string Property { get; }

        public string[] Properties { get; }

        public string Prepend { get; }

        public IEnumerable<SqlSegment> Segments { get; }

        public IfSegment(XElement xe, string macroName)
        {
            Property = xe.Attribute("property")?.Value;
            Properties = xe.Attribute("properties")?.Value.Split(',');

            if (Property == null && Properties == null)
                throw new ArgumentException(nameof(xe));

            Prepend = xe.Attribute("prepend")?.Value ?? "";
            Segments = xe.Nodes().Select(Create);
            MacroName = macroName;
        }

        public override string Emit()
        {
            if (Property != null)
            {
                var op = MacroName == "isNull" ? "== null" : "!= null";
                return
                    $"#if({Property} {op}) {{" +
                    $"{Prepend} {string.Concat(Segments.Select(x => x.Emit()))} " +
                    $"}}";
            }
            else
            {
                return string.Join("\r\n", Properties.Select(prop =>
                {
                    var op = MacroName == "isNull" ? "== null" : "!= null";
                    return
                        $"#if({prop} {op}) {{" +
                        $"{Prepend} {string.Concat(Segments.Select(x => x.Emit()))} " +
                        $"}}";
                }));
            }
        }
    }
}
