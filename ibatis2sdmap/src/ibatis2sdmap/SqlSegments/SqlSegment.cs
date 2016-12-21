using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public abstract class SqlSegment
    {
        public abstract string Emit();

        public static SqlSegment Create(XNode node)
        {
            if (node is XText)
            {
                return new TextSegment((XText)node);
            }
            else if (node is XElement)
            {
                var xe = (XElement)node;
                if (xe.Name.LocalName == "include")
                {
                    return new IncludeSegment(xe);
                }
                else if (xe.Name.LocalName == "isPropertyAvailable")
                {
                    return new HasPropSegment(xe);
                }
                else if (xe.Name.LocalName == "isNotNull")
                {
                    return new PredicateSegment(xe, "isNotNull");
                }
                else if (xe.Name.LocalName == "isNotEmpty")
                {
                    return new PredicateSegment(xe, "isNotEmpty");
                }
                else if (xe.Name.LocalName == "isEmpty")
                {
                    return new PredicateSegment(xe, "isEmpty");
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(xe.Name.LocalName));
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
