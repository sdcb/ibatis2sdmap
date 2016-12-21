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
            Directory.CreateDirectory(AppConfig.DestinationDirectory);
            FileUtil.EnumerateConfigFiles(AppConfig.IBatisXmlDirectory)
                .Select(XDocument.Load)
                .SelectMany(x => x.Descendants($"{{{AppConfig.NsPrefix}}}sqlMap"))
                .SelectMany(SqlItem.Create)
                .GroupBy(x => x.Namespace)
                .Subscribe(Save);
        }

        public static void Save(IGroupedObservable<string, SqlItem> v)
        {
            var filename = Path.Combine(AppConfig.DestinationDirectory, v.Key + ".sdmap");
            var file = new StreamWriter(File.OpenWrite(filename));
            file.WriteLine($"namespace {v.Key}");
            file.WriteLine("{");
            v.Select(x => x.Emit())
                .SubscribeOn(ImmediateScheduler.Instance)
                .Subscribe(file.WriteLine, () =>
                {
                    file.WriteLine("}");
                    file.Flush();
                    file.Dispose();
                });
        }

        public static void Print(SqlItem item)
        {
            Console.WriteLine(item.Emit());
        }
    }
}
