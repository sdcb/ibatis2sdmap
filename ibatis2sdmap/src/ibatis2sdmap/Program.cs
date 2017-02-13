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
            var filename = Path.Combine(AppConfig.DestinationDirectory, "one.sdmap");
            File.Delete(filename);

            Console.WriteLine("Transforming...");
            Directory.CreateDirectory(AppConfig.DestinationDirectory);
            FileUtil.EnumerateConfigFiles(AppConfig.IBatisXmlDirectory)
                .Subscribe(file =>
                {
                    XDocument.Load(file)
                        .Descendants($"{{{AppConfig.NsPrefix}}}sqlMap")
                        .SelectMany(SqlItem.Create)
                        .GroupBy(x => x.Namespace)
                        .ToObservable()
                        .Subscribe(x => SaveToOne(x));
                });

            Console.WriteLine("Parsing...");
            var rt = new SdmapCompiler();
            rt.AddSourceCode(File.ReadAllText(filename));
            var ok = rt.EnsureCompiled();
            if (ok.IsFailure)
            {
                Console.WriteLine("Compile failed: " + ok.Error);
            }
            else
            {
                Console.WriteLine("Compile succeed.");
            }
        }

        public static void SaveToOne(IGrouping<string, SqlItem> v)
        {
            var filename = Path.Combine(AppConfig.DestinationDirectory, "one.sdmap");
            using (var file = new StreamWriter(File.Open(filename, FileMode.Append)))
            {
                file.WriteLine($"namespace {v.Key}");
                file.WriteLine("{");
                v.Select(x => x.Emit())
                    .ToObservable()
                    .SubscribeOn(TaskPoolScheduler.Default)
                    .Do(file.WriteLine)
                    .Wait();
                file.WriteLine("}\r\n\r\n");
            }
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
