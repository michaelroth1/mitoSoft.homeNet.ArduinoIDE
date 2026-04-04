using ICSharpCode.AvalonEdit;

namespace mitoSoft.homeNet.ArduinoIDE.Contracts;

public interface IEditorView
{
    void ShowFindBar(string searchText);

    TextEditor GetTextEditor();
    
    void SetZoomFactor(double zoomFactor);
}