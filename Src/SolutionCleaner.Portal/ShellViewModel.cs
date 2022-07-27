using System;
using System.IO;
using System.Reflection;
using Prism.Commands;
using SolutionCleaner.Core.Constants;
using SolutionCleaner.Core.Models;
using SolutionCleaner.Core.Services.Contracts;
using SolutionCleaner.Core.Utils;

namespace SolutionCleaner
{
    public class ShellViewModel : BaseViewModel<ShellViewModel>
    {
        private readonly ICleaner _cleaner;
        private readonly IReporter _reporter;
        private readonly ILocalStorage _localStorage;

        #region Storage Fields

        private string title;
        private string directory;
        private string fileExtensions;
        private string directoryNames;
        private string resultMessage;

        #endregion

        public ShellViewModel(ICleaner cleaner, IReporter reporter, ILocalStorage localStorage)
        {
            _cleaner = cleaner;
            _reporter = reporter;
            _localStorage = localStorage;

            Title = $"Solution Cleaner {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}";
            Directory = _localStorage.ReadString(StorageConstants.CurrentDirectory)?.Trim() ?? 
                        AppDomain.CurrentDomain.BaseDirectory;
            FileExtensions = ".csproj.user; ";
            DirectoryNames = "bin; obj; ";
            CleanCommand = new DelegateCommand<object>(CleanFiles);
        }

        #region Business Properties

        public static readonly BusinessProperty<string> TitleProperty = RegisterProperty(x => x.Title);
        public static readonly BusinessProperty<string> DirectoryProperty = RegisterProperty(x => x.Directory);
        public static readonly BusinessProperty<string> FileExtensionsProperty = RegisterProperty(x => x.FileExtensions);
        public static readonly BusinessProperty<string> DirectoryNamesProperty = RegisterProperty(x => x.DirectoryNames);
        public static readonly BusinessProperty<string> ResultMessageProperty = RegisterProperty(x => x.ResultMessage);

        #endregion

        #region Public Properties

        public string Title
        {
            get => title;
            set => PropertySetter(TitleProperty, ref title, value);
        }
        public string Directory
        {
            get => directory;
            set => PropertySetter(DirectoryProperty, ref directory, value);
        }
        public string DirectoryNames
        {
            get => directoryNames;
            set => PropertySetter(DirectoryNamesProperty, ref directoryNames, value);
        }
        public string FileExtensions
        {
            get => fileExtensions;
            set => PropertySetter(FileExtensionsProperty, ref fileExtensions, value);
        }

        public string ResultMessage
        {
            get => resultMessage;
            set => PropertySetter(ResultMessageProperty, ref resultMessage, value);
        }

        #endregion

        #region Commands

        public DelegateCommand<object> CleanCommand { get; private set; }

        #endregion

        #region Command Actions
        
        private void CleanFiles(object parameter)
        {
            ResultMessage = string.Empty;
            var dirs = DirectoryNames?.SplitBy(";");
            var flExtensions = FileExtensions?.SplitBy(";");
            var directoryInfo = new DirectoryInfo(Directory);
            if (flExtensions == null || !directoryInfo.Exists)
            {
                _reporter.ReportException(new Exception("Input values are not valid!"));
            }
            else
            {
	            _localStorage.WriteString(StorageConstants.CurrentDirectory, Directory);
                int reportedFaultedFiles = 0, reportedFaultedDirectories = 0;
                int reportedSuccessFiles = 0, reportedSuccessDirectories = 0;
                _cleaner.RecursiveFilesClean(directoryInfo, flExtensions, ref reportedSuccessFiles, ref reportedFaultedFiles);
                _cleaner.RecursiveDirectoriesClean(directoryInfo, dirs, ref reportedSuccessDirectories, ref reportedFaultedDirectories);
                if (reportedFaultedFiles > 0)
                {
                    _reporter.ReportException(new Exception($"Operation completed, but {reportedFaultedFiles} files could not be deleted!"));
                }
                if (reportedFaultedDirectories > 0)
                {
                    _reporter.ReportException(new Exception($"Operation completed, but {reportedFaultedDirectories} folders could not be deleted!"));
                }

                ResultMessage = $"{reportedSuccessFiles} files deleted. {reportedSuccessDirectories} folders deleted";
            }
        }

        #endregion
    }
}
