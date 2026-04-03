using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.Models;

namespace mitoSoft.homeNet.ArduinoIDE.Services;

public static class DocumentationService
{
    public static List<ControllerGpioOverview> GenerateOverview(string yamlContent)
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

    private static string ExtractFriendlyName(string uniqueId)
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
}