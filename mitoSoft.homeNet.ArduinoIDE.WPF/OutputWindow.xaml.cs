using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace mitoSoft.homeNet.ArduinoIDE.WPF;

public partial class OutputWindow : Window
{
    public OutputWindow(string title, string content)
    {
        InitializeComponent();
        Title = title;
        OutputTextBox.Text = content;
    }

    private void CopyButton_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(OutputTextBox.Text))
        {
            Clipboard.SetText(OutputTextBox.Text);
            MessageBox.Show("Content copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Arduino files (*.ino)|*.ino|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = Title + ".ino"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            File.WriteAllText(saveFileDialog.FileName, OutputTextBox.Text);
            MessageBox.Show($"File saved to: {saveFileDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
