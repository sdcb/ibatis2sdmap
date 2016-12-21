using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap.SqlSegments
{
    public class TextSegment : SqlSegment
    {
        public string Text { get; }

        public TextSegment(XText text)
        {
            Text = text.Value;
        }

        private static Regex pregex = new Regex(@"#(\w+)#");
        private static Regex uregex = new Regex(@"\$(\w+)\$");

        public override string Emit()
        {
            return
                uregex.Replace(
                    pregex.Replace(Text, "@$1"), 
                    $"#prop<$1>");
        }

        public readonly static TextSegment Empty = new TextSegment(new XText(""));
    }
}
