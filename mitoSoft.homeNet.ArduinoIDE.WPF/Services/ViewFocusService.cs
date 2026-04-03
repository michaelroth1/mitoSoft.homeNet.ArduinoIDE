using System.Windows;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Services;

public class ViewFocusService
{
    public FrameworkElement? LastFocusedView { get; private set; }

    public void SetInitialView(FrameworkElement view)
    {
        LastFocusedView = view;
        this.Track(view);
    }

    public void Track(FrameworkElement view)
    {
        view.GotKeyboardFocus += (s, e) => LastFocusedView = view;
    }
}
