using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using SolutionCleaner.Core.Services.Contracts;
using SolutionCleaner.Core.Utils;

namespace SolutionCleaner.Core.Services
{
    public class FileService : IFileService
    {
        public void EnsureDirectoryExists(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public bool ForceDelete(FileInfo fileInfo)
        {
            try
            {
                fileInfo.Delete();
                Thread.Sleep(300);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ForceDelete(DirectoryInfo directoryInfo)
        {
            try
            {
                directoryInfo.Delete(true);
                Thread.Sleep(300);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}