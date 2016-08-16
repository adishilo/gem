using System;
using System.Configuration;
using System.Threading;
using System.Windows;
using NLog;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace GemGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        // Taken from http://stackoverflow.com/questions/21789899/how-to-create-single-instance-wpf-application-that-restores-the-open-window-when
        // in order to make a singleton WPF application:
        private static readonly Mutex s_mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");
        private static MainWindow s_mainWindow = null;

        App()
        {
            InitializeComponent();
        }

        [STAThread]
        static void Main()
        {
            try
            {
                if (s_mutex.WaitOne(TimeSpan.Zero, true))
                {
                    s_logger.Info("----- Starting GemGui -----");

                    // Register Unhandled-exception handler:
                    AppDomain.CurrentDomain.UnhandledException += AppExceptionHandler;

                    var app = new App();
                    s_mainWindow = new MainWindow();
                    app.Run(s_mainWindow);
                    s_mutex.ReleaseMutex();
                }
                else
                {
                    // Not using 'Xceed.Wpf.Toolkit.MessageBox' because its having a problem calculating the owner window
                    // Since it is not yet displayed/instantiated. This is probably a bug of Xceed.Wpf.ToolKit version 2.9.0.0
                    System.Windows.MessageBox.Show("GEM Application is already open.", "GEM", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                    s_logger.Info("GEM application is already open - terminating.");
                }
            }
            catch (ConfigurationErrorsException)
            {
                throw; // Cannot recover
            }
            catch (Exception ex)
            {
                InnerAppExceptionHandler(ex);
            }
        }

        private static void AppExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var ex = args.ExceptionObject as Exception;

            if (ex != null)
            {
                InnerAppExceptionHandler(ex);
            }
            else
            {
                s_logger.Error($"Unhandled exception thrown, caught by AppDomain, but the given exception object is not an exception.");
            }
        }

        private static void InnerAppExceptionHandler(Exception ex)
        {
            if (ex is TypeInitializationException && ex.InnerException is ConfigurationErrorsException)
            {
                MessageBox.Show(ex.InnerException.Message, "GEM", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }

            s_logger.Error(ex, "*** GemGui Unhandled exception caught.");
        }
    }
}
