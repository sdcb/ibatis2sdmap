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

        private static Regex reg2 = new Regex(@"#(\w+)#");
        private static Regex reg3 = new Regex(@"\$(\w+)\$");

        public override string Emit()
        {
            var rep1 = Text.Replace("$value$", "#val<>");
            var rep2 = reg2.Replace(rep1, "@$1");
            var rep3 = reg3.Replace(rep2, $"#prop<$1>");
            return rep3;
        }

        public readonly static TextSegment Empty = new TextSegment(new XText(""));
    }
}
