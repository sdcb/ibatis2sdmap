using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class IncludeSegment : SqlSegment
    {
        public string RefId { get; }

        public IncludeSegment(XElement xe)
        {
            RefId = xe.Attribute("refid").Value;
        }

        public override string Emit() => $"#include<{RefId}>";
    }
}
