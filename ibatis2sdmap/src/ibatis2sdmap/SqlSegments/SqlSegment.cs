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
                switch (xe.Name.LocalName)
                {
                    case "isPropertyAvailable":
                        return new PredicateSegment(xe, "hasProp");
                    case "isNotPropertyAvailable":
                        return new PredicateSegment(xe, "hasNoProp");
                    case "isNull":
                    case "isNotNull":
                        return new IfSegment(xe, xe.Name.LocalName);
                    case "isEmpty":
                    case "isNotEmpty":
                        return new PredicateSegment(xe, xe.Name.LocalName);
                    case "isEqual":
                    case "isNotEqual":
                    case "isLike":
                    case "isNotLike":
                    case "isLessThan":
                    case "isGreaterEqual":
                    case "isGreaterThan":
                    case "isLessEqual":
                        return new PredicateValSegment(xe, xe.Name.LocalName);
                    case "include":
                        return new IncludeSegment(xe);
                    case "iterate":
                        return new IterateSegment(xe);
                    case "selectKey":
                        return new SelectKeySegment(xe);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(xe.Name.LocalName));
                }
            }
            else if (node is XComment)
            {
                return new CommentSegment((XComment)node);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
