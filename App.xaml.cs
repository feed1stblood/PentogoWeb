using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using PentagoWeb.ViewModel;
using PentagoWeb.Model;
using PentagoWeb.Model.AI;
using PentagoWeb.Model.Board;
using System.Diagnostics;
using System.Threading;

namespace PentagoWeb
{
    public partial class App : Application
    {
        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
            UnhandledException += Application_UnhandledException;



            InitializeComponent();
            //System.Diagnostics.Debug.WriteLine(-450.0%360.0);
            

        }

        int i, j;
        string [,] result = new string[6,6];
        AutoResetEvent mutex = new AutoResetEvent(false);
        private void Application_Startup(object sender, StartupEventArgs e)
        {



            RootVisual = new PageGame();
            DispatcherHelper.Initialize();

            //for (int i = 0; i < 10; i++)
            //{
            //    new Tournament().Start();
            //    Debug.WriteLine("");
            //}
        }

        public void GameOver(Status winner)
        {
            switch (winner.State)
            {
                case Status.StateEnum.white:
                    result[i, j] = "←";
                    break;
                case Status.StateEnum.black:
                    result[i, j] = "↑";
                    break;
                case Status.StateEnum.empty:
                    result[i, j] = "♢";
                    break;
            }
            Debug.WriteLine(result[i, j]);
            mutex.Set();
        }

        

        private void Application_Exit(object sender, EventArgs e)
        {
            ViewModelLocator.Cleanup();
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    ReportErrorToDOM(e);
                });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
