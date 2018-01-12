using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ibatis2sdmap
{
    public static class SdmapConverter
    {
        public static string IBatisToSdmap(string ibatisCode)
        {
            var namespaces = XDocument.Parse(ibatisCode)
                .Descendants($"{{{AppConfig.NsPrefix}}}sqlMap")
                .SelectMany(SqlItem.Create)
                .GroupBy(x => x.Namespace)
                .Select(ns =>
                {
                    var sqls = string.Join("\r\n", ns.Select(sql => sql.Emit()));
                    return $"namespace {ns.Key} {{\r\n{sqls}\r\n}}";
                });
            return string.Join("\r\n\r\n", namespaces);
        }
    }
}
