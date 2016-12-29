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
                .Do(file =>
                {
                    XDocument.Load(file)
                        .Descendants($"{{{AppConfig.NsPrefix}}}sqlMap")
                        .SelectMany(SqlItem.Create)
                        .GroupBy(x => x.Namespace)
                        .ToObservable()
                        .Subscribe(x => Save(file, x));
                })
                .Wait();
        }

        public static void Save(string oldfilename, IGrouping<string, SqlItem> v)
        {
            var filename = Path.GetFileNameWithoutExtension(oldfilename);
            var relative = Path.GetDirectoryName(
                FileUtil.GetRelativePath(oldfilename, AppConfig.IBatisXmlDirectory + @"\"));
            var dir = Path.Combine(AppConfig.DestinationDirectory, relative);
            var path = Path.Combine(dir, filename + ".sdmap");

            Directory.CreateDirectory(dir);
            using (var file = new StreamWriter(File.OpenWrite(path)))
            {
                file.WriteLine($"namespace {v.Key}");
                file.WriteLine("{");
                v.Select(x => x.Emit())
                    .ToObservable()
                    .SubscribeOn(TaskPoolScheduler.Default)
                    .Do(file.WriteLine)
                    .Wait();
                file.WriteLine("}");
            }   
        }
    }
}
