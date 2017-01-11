using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class IterateSegment : SqlSegment
    {
        public string Property { get; }

        public string Open { get; }

        public string Close { get; }

        public string Conjunction { get; }

        public IEnumerable<SqlSegment> Segments { get; }

        public IterateSegment(XElement xe)
        {
            // // <iterate property="Districts" open="(" close=")" conjunction="," xmlns="http://ibatis.apache.org/mapping">#Districts[]#</iterate>
            Property = xe.Attribute("property")?.Value;
            Open = xe.Attribute("open").Value;
            Close = xe.Attribute("close").Value;
            Conjunction = xe.Attribute("conjunction").Value;
            Segments = xe.Nodes().Select(Create);
        }

        public override string Emit()
        {
            if (Property != null)
            {
                return $"@{Property}";
            }
            else
            {
                return
                    $"{Open}#iterate<'{Conjunction}', sql{{\r\n" +
                    $"{string.Concat(Segments.Select(x => x.Emit()))}" +
                    $"}}>{Close}";
            }
        }
    }
}
