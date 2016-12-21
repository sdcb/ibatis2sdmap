using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class CommentSegment : SqlSegment
    {
        public string Comment { get; }

        public CommentSegment(XComment xc)
        {
            Comment = xc.Value;
        }

        public override string Emit()
        {
            return $"/* {Comment} */";
        }
    }
}
