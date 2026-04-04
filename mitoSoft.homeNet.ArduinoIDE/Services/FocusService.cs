using System.Windows;

namespace mitoSoft.homeNet.ArduinoIDE.Services;

public class FocusService
{
    public FrameworkElement? FocusedView { get; private set; }

    public void SetFocusedView(FrameworkElement view)
    {
        FocusedView = view;
    }

    public void Track(FrameworkElement view)
    {
        view.GotKeyboardFocus += (s, e) => this.SetFocusedView(view);
    }
}