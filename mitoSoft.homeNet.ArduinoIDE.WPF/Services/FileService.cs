using Microsoft.Win32;
using System.IO;

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

    public string? ShowSaveDialog(string filter, string defaultFileName, string? currentFilePath = null)
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = filter,
            FileName = !string.IsNullOrEmpty(currentFilePath)
                ? Path.GetFileName(currentFilePath)
                : defaultFileName
        };

        if (!string.IsNullOrEmpty(currentFilePath))
        {
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(currentFilePath);
        }

        return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
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
