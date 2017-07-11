using sdmap.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Xml.Linq;

namespace ibatis2sdmap
{
    class OneConverter
    {
        public static void Convert()
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
            ParseOne(filename);
        }

        private static void ParseOne(string filename)
        {
            var rt = new SdmapCompiler();
            rt.AddSourceCode(File.ReadAllText(filename));

            var sw = new Stopwatch();
            sw.Start();
            var ok = rt.EnsureCompiled();
            sw.Stop();

            Console.WriteLine($"Compiled in: {sw.ElapsedMilliseconds}ms.");
            if (ok.IsFailure)
            {
                Console.WriteLine("Compile failed: " + ok.Error);
            }
            else
            {
                Console.WriteLine("Compile succeed.");
            }
        }

        private static void SaveToOne(IGrouping<string, SqlItem> v)
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
    }
}
