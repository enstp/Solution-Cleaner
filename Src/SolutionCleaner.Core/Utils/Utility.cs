using System;
using System.Windows;
using System.Windows.Threading;

namespace SolutionCleaner.Core.Utils
{
    public static class Utility
    {
        public static void DispatchIfNotOnUiThread(Action action)
        {
            if (GetDispatcher().CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                GetDispatcher().BeginInvoke(DispatcherPriority.Background, action);
            }
        }

        public static Dispatcher GetDispatcher()
        {
            return Application.Current.Dispatcher;
        }
    }
}
