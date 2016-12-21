using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ibatis2sdmap
{
    public static class FileUtil
    {
        public static IObservable<string> EnumerateConfigFiles(string sourceDirectory)
        {
            return Directory
                .EnumerateFiles(sourceDirectory, "*.config", SearchOption.AllDirectories)
                .ToObservable();
        }
    }
}
