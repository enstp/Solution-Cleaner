using System;

namespace SolutionCleaner.Core.Services.Contracts
{
    public interface IReporter
    {
        void ReportSuccess(string successMessage);

        void ReportException(Exception exception);
    }
}