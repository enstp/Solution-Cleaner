using System.IO;

namespace SolutionCleaner.Core.Services.Contracts
{
    public interface IFileService
    {
        void EnsureDirectoryExists(string dirPath);

        bool ForceDelete(FileInfo fileInfo);

        bool ForceDelete(DirectoryInfo directoryInfo);
    } 
}
