using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using SolutionCleaner.Core.Services.Contracts;

namespace SolutionCleaner.Core.Services
{
    public class FileService : IFileService
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DeleteFile(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool RemoveDirectory(string path);

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
                // fileInfo.Delete(); // slowest solution
                DeleteFile(fileInfo.FullName);
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
                // directoryInfo.Delete(true); // slowest solution
                RemoveDirectory(directoryInfo.FullName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}