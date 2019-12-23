using System;
using System.IO;
using System.Reflection;
using Prism.Commands;
using SolutionCleaner.Core.Models;
using SolutionCleaner.Core.Services.Contracts;
using SolutionCleaner.Core.Utils;

namespace SolutionCleaner
{
    public class ShellViewModel : BaseViewModel<ShellViewModel>
    {
        private readonly ICleaner _cleaner;
        private readonly IReporter _reporter;

        #region Storage Fields

        private string title;
        private string directory;
        private string fileExtensions;
        private string directoryNames;

        #endregion

        public ShellViewModel(ICleaner cleaner, IReporter reporter)
        {
            _cleaner = cleaner;
            _reporter = reporter;

            Title = $"Solution Cleaner {Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}";
            Directory = AppDomain.CurrentDomain.BaseDirectory;
            FileExtensions = ".csproj.user; ";
            DirectoryNames = "bin; obj; ";
            CleanCommand = new DelegateCommand<object>(CleanFiles);
        }

        #region Business Properties

        public static readonly BusinessProperty<string> TitleProperty = RegisterProperty(x => x.Title);
        public static readonly BusinessProperty<string> DirectoryProperty = RegisterProperty(x => x.Directory);
        public static readonly BusinessProperty<string> FileExtensionsProperty = RegisterProperty(x => x.FileExtensions);
        public static readonly BusinessProperty<string> DirectoryNamesProperty = RegisterProperty(x => x.DirectoryNames);

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

        #endregion

        #region Commands

        public DelegateCommand<object> CleanCommand { get; private set; }

        #endregion

        #region Command Actions
        
        private void CleanFiles(object parameter)
        {
            var dirs = DirectoryNames?.SplitBy(";");
            var flExtensions = FileExtensions?.SplitBy(";");
            var directoryInfo = new DirectoryInfo(Directory);
            if (flExtensions == null || !directoryInfo.Exists)
            {
                _reporter.ReportException(new Exception("Input values are not valid!"));
            }
            else
            {
                int reportedFaultedFiles = 0, reportedFaultedDirectories = 0;
                _cleaner.RecursiveFilesClean(directoryInfo, flExtensions, ref reportedFaultedFiles);
                _cleaner.RecursiveDirectoriesClean(directoryInfo, dirs, ref reportedFaultedDirectories);
                if (reportedFaultedFiles > 0)
                {
                    _reporter.ReportException(new Exception($"Operation completed, but {reportedFaultedFiles} files could not be deleted!"));
                }
                if (reportedFaultedDirectories > 0)
                {
                    _reporter.ReportException(new Exception($"Operation completed, but {reportedFaultedDirectories} folders could not be deleted!"));
                }
            }
        }

        #endregion
    }
}
