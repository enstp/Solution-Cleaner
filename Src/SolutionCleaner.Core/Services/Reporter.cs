using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using SolutionCleaner.Core.Services.Contracts;
using SolutionCleaner.Core.Utils;

namespace SolutionCleaner.Core.Services 
{
    public class Reporter : IReporter
    {
        private readonly IFileService _fileService;

        public Reporter(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void ReportSuccess(string successMessage)
        {
            Utility.DispatchIfNotOnUiThread(() => MessageBox.Show(successMessage));
        }

        public void ReportException(Exception exception)
        {
            string logFile = LogError(exception);
            List<Exception> exceptions = ReverseFlattenException(exception);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("The following errors has occurred : ");

            foreach (Exception e in exceptions)
            {
                sb.AppendLine("Message: " + e.Message);
                sb.AppendLine("--------------------------");
                sb.AppendLine("StackTrace: " + e.StackTrace);
                sb.AppendLine();
            }

            ReportErrorMessage(sb.ToString());
        }

        private string LogError(Exception exception)
        {
            Assembly caller = Assembly.GetEntryAssembly();
            Process thisProcess = Process.GetCurrentProcess();

            string logFile = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt";

            _fileService.EnsureDirectoryExists("Logs");

            using (StreamWriter sw = new StreamWriter(Path.Combine("Logs", logFile)))
            {
                sw.WriteLine("==============================================================================");
                sw.WriteLine(caller.FullName);
                sw.WriteLine("------------------------------------------------------------------------------");
                sw.WriteLine("Application Information");
                sw.WriteLine("------------------------------------------------------------------------------");
                sw.WriteLine("Program      : " + caller.Location);
                sw.WriteLine("Time         : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                sw.WriteLine("User         : " + Environment.UserName);
                sw.WriteLine("Computer     : " + Environment.MachineName);
                sw.WriteLine("OS           : " + Environment.OSVersion);
                sw.WriteLine("Culture      : " + CultureInfo.CurrentCulture.Name);
                sw.WriteLine("Processors   : " + Environment.ProcessorCount);
                sw.WriteLine("Working Set  : " + Environment.WorkingSet);
                sw.WriteLine("Framework    : " + Environment.Version);
                sw.WriteLine("Run Time     : " + (DateTime.Now - Process.GetCurrentProcess().StartTime));
                sw.WriteLine("------------------------------------------------------------------------------");
                sw.WriteLine("Exception Information");
                sw.WriteLine("------------------------------------------------------------------------------");
                sw.WriteLine("Source       : " + exception.Source?.Trim());
                sw.WriteLine("Method       : " + exception.TargetSite?.Name);
                sw.WriteLine("Type         : " + exception.GetType());
                sw.WriteLine("Error        : " + GetExceptionStack(exception));
                sw.WriteLine("Stack Trace  : " + exception.StackTrace?.Trim());
                sw.WriteLine("------------------------------------------------------------------------------");
                sw.WriteLine("Loaded Modules");
                sw.WriteLine("------------------------------------------------------------------------------");
                foreach (ProcessModule module in thisProcess.Modules)
                {
                    try
                    {
                        sw.WriteLine(module.FileName + " | " + module.FileVersionInfo.FileVersion + " | " + module.ModuleMemorySize);
                    }
                    catch (FileNotFoundException)
                    {
                        sw.WriteLine("File Not Found: " + module);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                sw.WriteLine("------------------------------------------------------------------------------");
                sw.WriteLine(logFile);
                sw.WriteLine("==============================================================================");
            }

            return logFile;
        }

        private string GetExceptionStack(Exception e)
        {
            StringBuilder message = new StringBuilder();
            message.Append(e.Message);
            while (e.InnerException != null)
            {
                e = e.InnerException;
                message.Append(Environment.NewLine);
                message.Append(e.Message);
            }

            return message.ToString();
        }

        private List<Exception> ReverseFlattenException(Exception ex)
        {
            List<Exception> exceptions = new List<Exception>();
            while (ex != null)
            {
                exceptions.Add(ex);
                ex = ex.InnerException;
            }
            exceptions.Reverse();
            return exceptions;
        }

        private void ReportErrorMessage(string message)
        {
            Utility.DispatchIfNotOnUiThread(() => MessageBox.Show(message));
        }
    }
}