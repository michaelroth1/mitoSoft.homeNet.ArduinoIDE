using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Services;

public class FileService
{
    public string? OpenYamlFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "YAML files (*.yaml;*.yml)|*.yaml;*.yml|All files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            return openFileDialog.FileName;
        }

        return null;
    }

    public string? SaveYamlFile(string currentFilePath)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*",
            FileName = !string.IsNullOrEmpty(currentFilePath) ? Path.GetFileName(currentFilePath) : "config.yaml"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            return saveFileDialog.FileName;
        }

        return null;
    }

    public void SaveOutputFile(string controllerName, string content)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Arduino files (*.ino)|*.ino|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = controllerName + ".ino"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            File.WriteAllText(saveFileDialog.FileName, content);
            MessageBox.Show($"File saved to: {saveFileDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public void WriteFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }
}
