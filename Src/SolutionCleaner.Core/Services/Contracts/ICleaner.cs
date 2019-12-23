using System.Collections.Generic;
using System.IO;

namespace SolutionCleaner.Core.Services.Contracts
{
    public interface ICleaner
    {
        void RecursiveFilesClean(DirectoryInfo directory, IEnumerable<string> fileExtensions, ref int faultedFiles);

        void RecursiveDirectoriesClean(DirectoryInfo directory, IEnumerable<string> fileExtensions, ref int faultedDirectories);
    }
}
