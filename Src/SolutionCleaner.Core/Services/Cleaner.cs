using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolutionCleaner.Core.Services.Contracts;

namespace SolutionCleaner.Core.Services
{
    public class Cleaner : ICleaner
    {
        private readonly IFileService _fileService;

        public Cleaner(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void RecursiveFilesClean(DirectoryInfo directory, IEnumerable<string> fileExtensions, ref int faultedFiles)
        {
            foreach (var directoryInfoChild in directory.GetDirectories())
            {
                RecursiveFilesClean(directoryInfoChild, fileExtensions, ref faultedFiles);
            }

            foreach (var fileInfo in directory.GetFiles())
            {
                if (fileExtensions.Contains($"*{fileInfo.Extension}"))
                {
                    if (!_fileService.ForceDelete(fileInfo))
                    {
                        ++faultedFiles;
                    }
                }
            }
        }

        public void RecursiveDirectoriesClean(DirectoryInfo directory, IEnumerable<string> directoryNames, ref int faultedDirectories)
        {
            foreach (var directoryInfoChild in directory.GetDirectories())
            {
                RecursiveDirectoriesClean(directoryInfoChild, directoryNames, ref faultedDirectories);
            }

            foreach (var directoryInfo in directory.GetDirectories())
            {
                if (directoryNames.Contains(directoryInfo.Name))
                {
                    if (!_fileService.ForceDelete(directoryInfo))
                    {
                        ++faultedDirectories;
                    }
                }
            }
        }
    }
}
