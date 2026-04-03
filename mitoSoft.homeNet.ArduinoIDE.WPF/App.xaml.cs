using Res = mitoSoft.homeNet.ArduinoIDE.WPF.Properties.Resources;
using System.Windows;
using System.Windows.Threading;

namespace mitoSoft.homeNet.ArduinoIDE.WPF;

public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    // UI-Thread exceptions → app bleibt offen
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ShowError(e.Exception, Res.App_ErrorTitle);
        e.Handled = true;
    }

    // Hintergrund-Thread exceptions → app wird beendet
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        var message = ex?.Message ?? e.ExceptionObject?.ToString() ?? "Unbekannter Fehler";

        Dispatcher.Invoke(() =>
            MessageBox.Show(
                string.Format(Res.App_CriticalError, message),
                Res.App_CriticalErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error));
    }

    // Unbeobachtete Task-Exceptions → als behandelt markieren, app bleibt offen
    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        Dispatcher.Invoke(() => ShowError(e.Exception, Res.App_BackgroundErrorTitle));
    }

    private static void ShowError(Exception exception, string title)
    {
        MessageBox.Show(
            string.Format(Res.App_UnexpectedError, exception.Message),
            title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}