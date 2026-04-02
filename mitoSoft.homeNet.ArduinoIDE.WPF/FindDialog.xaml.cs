using System.Windows;
using System.Windows.Input;

namespace mitoSoft.homeNet.ArduinoIDE.WPF;

public partial class FindDialog : Window
{
    public string SearchText { get; private set; } = string.Empty;

    public FindDialog(string initialSearchText)
    {
        InitializeComponent();
        SearchTextBox.Text = initialSearchText;
        SearchTextBox.Focus();
        SearchTextBox.SelectAll();
    }

    private void FindButton_Click(object sender, RoutedEventArgs e)
    {
        SearchText = SearchTextBox.Text;
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            FindButton_Click(sender, e);
        }
    }
}
