namespace mitoSoft.homeNet.ArduinoIDE.Services;

public class SettingsService
{
    public void LoadWindowSettings(out double width, out double height, out double zoomFactor)
    {
        width = Properties.Settings.Default.WindowWidth;
        height = Properties.Settings.Default.WindowHeight;
        zoomFactor = Properties.Settings.Default.ZoomFactor;
    }

    public void SaveWindowSettings(double width, double height, double zoomFactor)
    {
        Properties.Settings.Default.WindowWidth = width;
        Properties.Settings.Default.WindowHeight = height;
        Properties.Settings.Default.ZoomFactor = zoomFactor;
        Properties.Settings.Default.Save();
    }

    public string GetLastOpenedYamlFile()
    {
        return Properties.Settings.Default.LastOpenedYamlFile;
    }

    public void SaveLastOpenedYamlFile(string filePath)
    {
        Properties.Settings.Default.LastOpenedYamlFile = filePath;
        Properties.Settings.Default.Save();
    }
}
