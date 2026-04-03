namespace mitoSoft.homeNet.ArduinoIDE.WPF.Models;

public class GpioOverviewItem
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string GpioPins { get; set; } = string.Empty;
    public string AdditionalInfo { get; set; } = string.Empty;
}

public class ControllerGpioOverview
{
    public string ControllerName { get; set; } = string.Empty;
    public int ControllerId { get; set; }
    public string IPAddress { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public List<GpioOverviewItem> Items { get; set; } = [];
    
    public int TotalGpioCount { get; set; }
}
