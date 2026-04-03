using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.WPF.Models;
using System.Text;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Services;

public class GpioOverviewService
{
    public List<ControllerGpioOverview> GenerateOverview(string yamlContent)
    {
        try
        {
            var parser = new YamlParser(yamlContent);
            var yaml = parser.Deserialize();

            var overviews = new List<ControllerGpioOverview>();

            foreach (var controller in yaml.HomeNetConfig.Controllers)
            {
                var overview = new ControllerGpioOverview
                {
                    ControllerName = controller.Name,
                    ControllerId = controller.UniqueId,
                    IPAddress = controller.IPAddress ?? "N/A",
                    MacAddress = controller.MacAddress ?? "N/A",
                    Items = []
                };

                var usedGpios = new HashSet<int>();

                // Add Covers
                var covers = yaml.HomeNetConfig.Covers
                    .Where(c => c.ControllerId == controller.UniqueId)
                    .OrderBy(c => c.GpioOpen);

                foreach (var cover in covers)
                {
                    var gpioPins = new List<string>();
                    
                    gpioPins.Add($"Open: {cover.GpioOpen}");
                    gpioPins.Add($"Close: {cover.GpioClose}");
                    
                    usedGpios.Add(cover.GpioOpen);
                    usedGpios.Add(cover.GpioClose);

                    if (cover.GpioOpenButton > 0)
                    {
                        gpioPins.Add($"Btn↑: {cover.GpioOpenButton}");
                        usedGpios.Add(cover.GpioOpenButton);
                    }

                    if (cover.GpioCloseButton > 0)
                    {
                        gpioPins.Add($"Btn↓: {cover.GpioCloseButton}");
                        usedGpios.Add(cover.GpioCloseButton);
                    }

                    var name = ExtractFriendlyName(cover.UniqueId);
                    
                    overview.Items.Add(new GpioOverviewItem
                    {
                        Type = "Cover",
                        Name = name,
                        Description = cover.Description ?? "",
                        GpioPins = string.Join(" | ", gpioPins),
                        AdditionalInfo = $"Time: {cover.RunningTime}ms"
                    });
                }

                // Add Lights
                var lights = yaml.HomeNetConfig.Lights
                    .Where(l => l.ControllerId == controller.UniqueId)
                    .OrderBy(l => l.GpioPin);

                foreach (var light in lights)
                {
                    var gpioPins = new List<string>();
                    
                    gpioPins.Add($"Pin: {light.GpioPin}");
                    usedGpios.Add(light.GpioPin);

                    if (light.GpioButton > 0)
                    {
                        gpioPins.Add($"Btn: {light.GpioButton}");
                        usedGpios.Add(light.GpioButton);
                    }

                    var name = ExtractFriendlyName(light.UniqueId);

                    overview.Items.Add(new GpioOverviewItem
                    {
                        Type = "Light",
                        Name = name,
                        Description = light.Description ?? "",
                        GpioPins = string.Join(" | ", gpioPins),
                        AdditionalInfo = $"Mode: {light.SwitchMode}"
                    });
                }

                overview.TotalGpioCount = usedGpios.Count;
                overviews.Add(overview);
            }

            return overviews;
        }
        catch
        {
            return [];
        }
    }

    private string ExtractFriendlyName(string uniqueId)
    {
        // Extract friendly name from unique_id like "arduino_shutter_eg_kuche_schneider"
        if (uniqueId.StartsWith("arduino_shutter_"))
        {
            return uniqueId.Replace("arduino_shutter_", "").Replace("_", " ").ToUpper();
        }
        else if (uniqueId.StartsWith("arduino_light_"))
        {
            return uniqueId.Replace("arduino_light_", "").Replace("_", " ").ToUpper();
        }

        return uniqueId.Replace("_", " ");
    }

    public string GenerateDocumentation(string yamlContent)
    {
        var overviews = GenerateOverview(yamlContent);

        if (overviews.Count == 0)
        {
            return "Keine Controller mit GPIOs gefunden.\n\nBitte stellen Sie sicher, dass Ihr YAML-File gültige homeNet-Controller mit Cover- oder Light-Konfigurationen enthält.";
        }

        var sb = new StringBuilder();
        sb.AppendLine("================================================================================");
        sb.AppendLine("               GPIO DOKUMENTATION - SCHALTSCHRANK ÜBERSICHT");
        sb.AppendLine("================================================================================");
        sb.AppendLine();

        foreach (var overview in overviews)
        {
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine($"CONTROLLER: {overview.ControllerName} (ID: {overview.ControllerId})");
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine($"IP-Adresse:      {overview.IPAddress}");
            sb.AppendLine($"MAC-Adresse:     {overview.MacAddress}");
            sb.AppendLine($"Verwendete GPIOs: {overview.TotalGpioCount}");
            sb.AppendLine();

            if (overview.Items.Count == 0)
            {
                sb.AppendLine("  Keine Geräte konfiguriert");
                sb.AppendLine();
                continue;
            }

            // Group by type
            var covers = overview.Items.Where(i => i.Type == "Cover").ToList();
            var lights = overview.Items.Where(i => i.Type == "Light").ToList();

            if (covers.Count > 0)
            {
                sb.AppendLine("  ROLLLÄDEN / JALOUSIEN:");
                sb.AppendLine("  " + new string('-', 76));
                sb.AppendLine($"  {"Name",-30} {"GPIO Pins",-30} {"Info",-16}");
                sb.AppendLine("  " + new string('-', 76));

                foreach (var cover in covers)
                {
                    sb.AppendLine($"  {cover.Name,-30} {cover.GpioPins,-30} {cover.AdditionalInfo,-16}");
                }
                sb.AppendLine();
            }

            if (lights.Count > 0)
            {
                sb.AppendLine("  LICHTER:");
                sb.AppendLine("  " + new string('-', 76));
                sb.AppendLine($"  {"Name",-30} {"GPIO Pins",-30} {"Info",-16}");
                sb.AppendLine("  " + new string('-', 76));

                foreach (var light in lights)
                {
                    sb.AppendLine($"  {light.Name,-30} {light.GpioPins,-30} {light.AdditionalInfo,-16}");
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("================================================================================");
        sb.AppendLine($"Erstellt am: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        sb.AppendLine("================================================================================");

        return sb.ToString();
    }
}
