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
    public class Program
    {
        public static void Main(string[] args)
        {
            //OneConverter.Convert();
            FolderConverter.Convert();
        }
    }
}
