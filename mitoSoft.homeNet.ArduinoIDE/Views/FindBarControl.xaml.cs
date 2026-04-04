using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace mitoSoft.homeNet.ArduinoIDE.Views;

public partial class FindBarControl : UserControl
{
    public event EventHandler<string>? FindNextRequested;
    public event EventHandler? CloseRequested;

    public string SearchText
    {
        get => SearchTextBox.Text;
        set => SearchTextBox.Text = value;
    }

    public FindBarControl()
    {
        InitializeComponent();
        IsVisibleChanged += (s, e) =>
        {
            if ((bool)e.NewValue)
                Dispatcher.BeginInvoke(DispatcherPriority.Input, () =>
                {
                    Keyboard.Focus(SearchTextBox);
                    SearchTextBox.SelectAll();
                });
        };
    }

    public void Show(string initialText)
    {
        SearchTextBox.Text = initialText;
        Visibility = Visibility.Visible;
    }

    public void SetNotFoundState(bool notFound)
    {
        SearchTextBox.Background = notFound
            ? new SolidColorBrush(Color.FromRgb(255, 200, 200))
            : Brushes.White;
    }

    private void FindNextButton_Click(object sender, RoutedEventArgs e)
    {
        FindNextRequested?.Invoke(this, SearchTextBox.Text);
        this.FocusSearchBox();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            FindNextRequested?.Invoke(this, SearchTextBox.Text);
            this.FocusSearchBox();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
            e.Handled = true;
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        SetNotFoundState(false);
    }

    private void FocusSearchBox()
    {
        Dispatcher.BeginInvoke(DispatcherPriority.Input, () =>
        {
            Keyboard.Focus(SearchTextBox);
        });
    }
}
