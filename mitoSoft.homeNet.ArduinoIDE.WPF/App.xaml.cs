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
        ShowError(e.Exception, "Unerwarteter Fehler");
        e.Handled = true;
    }

    // Hintergrund-Thread exceptions → app wird beendet
    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;
        var message = ex?.Message ?? e.ExceptionObject?.ToString() ?? "Unbekannter Fehler";

        Dispatcher.Invoke(() =>
            MessageBox.Show(
                $"Ein kritischer Fehler ist aufgetreten:\n\n{message}\n\nDie Anwendung wird beendet.",
                "Kritischer Fehler", MessageBoxButton.OK, MessageBoxImage.Error));
    }

    // Unbeobachtete Task-Exceptions → als behandelt markieren, app bleibt offen
    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        Dispatcher.Invoke(() => ShowError(e.Exception, "Hintergrundfehler"));
    }

    private static void ShowError(Exception exception, string title)
    {
        MessageBox.Show(
            $"Ein unerwarteter Fehler ist aufgetreten:\n\n{exception.Message}",
            title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}