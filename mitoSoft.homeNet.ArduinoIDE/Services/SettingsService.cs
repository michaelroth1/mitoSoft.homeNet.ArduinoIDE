using mitoSoft.homeNet.ArduinoIDE.Properties;

namespace mitoSoft.homeNet.ArduinoIDE.Services;

public class SettingsService
{
    private readonly Settings _properties;

    public SettingsService()
    {
        _properties = Properties.Settings.Default;
    }

    public void LoadWindowSettings(out double width, out double height, out double zoomFactor)
    {
        width = _properties.WindowWidth;
        height = _properties.WindowHeight;
        zoomFactor = _properties.ZoomFactor;
    }

    public void SaveWindowSettings(double width, double height, double zoomFactor)
    {
        _properties.WindowWidth = width;
        _properties.WindowHeight = height;
        _properties.ZoomFactor = zoomFactor;
        _properties.Save();
    }

    public string GetLastOpenedYamlFile() =>
        _properties.LastOpenedYamlFile;

    public void SaveLastOpenedYamlFile(string filePath)
    {
        _properties.LastOpenedYamlFile = filePath;
        _properties.Save();
    }
}
