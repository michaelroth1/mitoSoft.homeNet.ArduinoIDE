using System.Windows;

namespace mitoSoft.homeNet.ArduinoIDE.Services;

public class FocusService
{
    public FrameworkElement? FocusedView { get; private set; }
    public IEditorView? FocusedEditorView => FocusedView as IEditorView;

    public void SetInitialView(FrameworkElement view)
    {
        FocusedView = view;
        this.Track(view);
    }

    public void Track(FrameworkElement view)
    {
        view.GotKeyboardFocus += (s, e) => FocusedView = view;
    }
}