using ICSharpCode.AvalonEdit;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Views;

public interface IEditorView
{
    void ShowFindBar(string searchText);
    TextEditor GetTextEditor();
    void SetZoomFactor(double zoomFactor);
}