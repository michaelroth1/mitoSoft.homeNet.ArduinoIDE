using HomeNet = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;
using Mqtt = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Mqtt;
using Merge = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Merge;
using System.Text;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions
{
    public static class MergedConfigExtensions
    {
        public static IList<Merge.IItem> GetItems(this Merge.Config config)
        {
            var items = new List<Merge.IItem>();

            items.AddRange(config.Covers);
            items.AddRange(config.Lights);

            return items;
        }

        public static string GetPartnerWarnings(this Merge.Config merged)
        {
            var warnings = merged
                .GetItems()
                .Where(c => c.HasPartner == false)
                .Select(c => $"'{c.Name.Trim()}' not specified in Mqtt section.")
                .ToList();

            return string.Join("\n", warnings);
        }

        public static string GetGpioWarnings(this Merge.Config merged)
        {
            var errors = new List<string>();

            var gpioCovers = merged.Covers
                .SelectMany(c => new[] { c.GpioOpen, c.GpioClose, c.GpioOpenButton, c.GpioCloseButton });// Alle GPIO-Werte sammeln

            var gpioLights = merged.Lights
                .SelectMany(l => new[] { l.GpioPin, l.GpioButton });

            var gpioMerged = gpioCovers.Concat(gpioLights); // GPIOs merging

            var gpios = gpioMerged.Where(gpio => gpio > 0)  // Falls es ungültige Werte gibt, ignorieren
                .OrderBy(gpio => gpio)
                .ToList();

            var duplicates = gpios.FindDuplicates();

            foreach (var duplicate in duplicates)
            {
                foreach (var cover in merged.Covers)
                {
                    if (cover.GpioClose == duplicate                     )
                    {
                        errors.Add($"WARNING: In controller '{cover.ControllerId}', cover '{cover.Name}' a duplicate gpio is used: {duplicate} GpioClose.");
                    }
                    if ( cover.GpioOpen == duplicate)
                    {
                        errors.Add($"WARNING: In controller '{cover.ControllerId}', cover '{cover.Name}' a duplicate gpio is used: {duplicate} GpioOpen.");
                    }
                    if (cover.GpioCloseButton == duplicate)
                    {
                        errors.Add($"WARNING: In controller '{cover.ControllerId}', cover '{cover.Name}' a duplicate gpio is used: {duplicate} GpioCloseButton.");
                    }
                    if (cover.GpioOpenButton == duplicate)
                    {
                        errors.Add($"WARNING: In controller '{cover.ControllerId}', cover '{cover.Name}' a duplicate gpio is used: {duplicate} GpioOpenButton.");
                    }
                }
                foreach (var light in merged.Lights)
                {
                    if (light.GpioPin == duplicate)
                    {
                        errors.Add($"WARNING: In controller '{light.ControllerId}', light '{light.Name}' a duplicate gpio is used: {duplicate} GpioPin.");
                    }
                    if (light.GpioButton == duplicate)
                    {
                        errors.Add($"WARNING: In controller '{light.ControllerId}', light '{light.Name}' a duplicate gpio is used: {duplicate} GpioButton.");
                    }
                }
            }

            return string.Join('\n', errors);
        }
    }
}