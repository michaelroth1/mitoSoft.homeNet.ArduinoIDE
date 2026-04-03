using ICSharpCode.AvalonEdit;

namespace mitoSoft.homeNet.ArduinoIDE;

public interface IEditorView
{
    void ShowFindBar(string searchText);
    TextEditor GetTextEditor();
    void SetZoomFactor(double zoomFactor);
}