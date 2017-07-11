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
    class FolderConverter
    {
        public static void Convert()
        {
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
                        .Subscribe(x => SaveToFolder(file, x));
                });

            Console.WriteLine("Parsing...");
            ParseFolder(AppConfig.DestinationDirectory);
        }

        private static void ParseFolder(string folderPathName)
        {
            var rt = new SdmapCompiler();
            foreach (var file in Directory.GetFiles(folderPathName, "*.sdmap", SearchOption.AllDirectories))
            {
                rt.AddSourceCode(File.ReadAllText(file));
            }

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



        private static void SaveToFolder(string oldfilename, IGrouping<string, SqlItem> v)
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
