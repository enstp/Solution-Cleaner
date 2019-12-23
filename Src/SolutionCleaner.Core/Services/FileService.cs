using System.IO;
using SolutionCleaner.Core.Services.Contracts;

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
                directoryInfo.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}