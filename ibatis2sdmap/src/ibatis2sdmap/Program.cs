using sdmap.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ibatis2sdmap
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //OneConverter.Convert();
            //FolderConverter.Convert();
            var converted = SdmapConverter.IBatisToSdmap(
                File.ReadAllText(@"C:\Users\Public\Nwt\cache\recv\喻毅\SqlMaps\ClientSearch.config"));
            Console.WriteLine(converted);
        }
    }
}
