using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class HasPropSegment : PredicateSegment
    {
        public HasPropSegment(XElement xe) : base(xe)
        {
        }

        protected override string GetMacro() => "hasProp";
    }
}
}
