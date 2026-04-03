using ICSharpCode.AvalonEdit;
using System.Windows.Controls;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Views;

public partial class YamlView : UserControl
{
    public event EventHandler? TextChanged;

    public YamlView()
    {
        InitializeComponent();
        YamlTextEditor.TextChanged += (s, e) => TextChanged?.Invoke(this, EventArgs.Empty);
        YamlTextEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };
        YamlTextEditor.PreviewMouseWheel += (s, e) =>
        {
            YamlTextEditor.ScrollToVerticalOffset(YamlTextEditor.VerticalOffset - e.Delta / 3.0);
            e.Handled = true;
        };
    }

    public string Text
    {
        get => YamlTextEditor.Text;
        set => YamlTextEditor.Text = value;
    }

    public void SetZoomFactor(double zoomFactor)
    {
        YamlTextEditor.FontSize = 12 * zoomFactor;
    }

    public TextEditor GetTextEditor()
    {
        return YamlTextEditor;
    }
}
