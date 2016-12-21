using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class SelectKeySegment : SqlSegment
    {
        public IEnumerable<SqlSegment> Segments { get; }

        public SelectKeySegment(XElement xe)
        {
            // <selectKey property="Id" type="post" resultClass="long">
            // select last_insert_id();
            // </selectKey>
            Segments = xe.Nodes().Select(Create);
        }

        public override string Emit()
        {
            return $" {string.Concat(Segments.Select(x => x.Emit()))} ";
        }
    }
}
