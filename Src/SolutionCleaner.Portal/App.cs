using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Prism.Ioc;
using Prism.Unity;
using SolutionCleaner.Core.Services;
using SolutionCleaner.Core.Services.Contracts;
using SolutionCleaner.Core.Utils;

namespace SolutionCleaner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>  
    public class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Current.DispatcherUnhandledException += AppDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;

            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IFileService, FileService>();
            containerRegistry.Register<ILocalStorage, LocalStorage>();
            containerRegistry.Register<IReporter, Reporter>();
            containerRegistry.Register<ICleaner, Cleaner>();

        }

        protected override Window CreateShell() => new ShellView();

        #region Global Error Handlers

        private void TaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Utility.DispatchIfNotOnUiThread(() =>
            {
                try
                {
                    Container.Resolve<IReporter>().ReportException(e.Exception);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Container.Resolve<IReporter>().ReportException(ex);
            }
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Utility.DispatchIfNotOnUiThread(() =>
            {
                try
                {
                    e.Handled = true;
                    Container.Resolve<IReporter>().ReportException(e.Exception);
                }
                catch
                {
                    MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Handled = false;
                    Current.Shutdown(1);
                }
            });
        }

        #endregion
    }
}
