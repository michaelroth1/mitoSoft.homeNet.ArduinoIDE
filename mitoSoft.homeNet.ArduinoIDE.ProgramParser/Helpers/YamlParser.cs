using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class YamlParser
{
    public string _yamlText;
    private List<string> _items = [];
    private object mqttConfig;

    public YamlParser(string yaml)
    {
        _yamlText = yaml;
    }

    public YamlParser IgnorableItem(string item)
    {
        _items.Add(item);
        return this;
    }

    private void DeleteIgnorableItems()
    {
        foreach (var item in _items)
        {
            _yamlText = _yamlText.Replace(item, "");
        }
    }

    public IList<HomeNetController> ParseHomeNetControllers()
    {
        try
        {
            IDeserializer deserializer = GetDeserializer();

            var yaml = deserializer.Deserialize<Dictionary<string, HomeNetConfig>>(_yamlText);
            if (yaml != null)
            {
                var homeNetConfig = yaml!["homeNet"]; // Extrahiere nur den "homeNet"-Teil
                return homeNetConfig.Controllers;
            }
            else
            {
                return [];
            }
        }
        catch
        {
            return [];
        }
    }

    public HomeNetConfig Parse(int controllerId)
    {
        IDeserializer deserializer = GetDeserializer();

        var homeNetConfig = GetMergedMqttConfig(deserializer);

        return new HomeNetConfig()
        {
            Covers = homeNetConfig.Covers.Where(c => c.ControllerId == controllerId).ToList(),
            Lights = homeNetConfig.Lights.Where(l => l.ControllerId == controllerId).ToList(),
        };
    }

    public string CheckYaml()
    {
        var errors = new List<string>();

        IDeserializer deserializer = GetDeserializer();

        var homeNetConfig = GetMergedMqttConfig(deserializer);

        var gpios1 = homeNetConfig.Covers
            .SelectMany(c => new[] {
                $"{c.ControllerId}-{c.GpioOpen}",
                $"{c.ControllerId}-{c.GpioClose}",
                $"{c.ControllerId}-{c.GpioOpenButton}",
                $"{c.ControllerId}-{c.GpioCloseButton}" });// Alle GPIO-Werte sammeln

        var gpios2 = homeNetConfig.Lights
            .SelectMany(l => new[] {
                $"{l.ControllerId}-{l.GpioPin}",
                $"{l.ControllerId}-{l.GpioButton}" });

        var merged = gpios1.Concat(gpios2); // GPIOs merging

        var gpios = merged.Where(gpio => Convert.ToInt16(gpio.Split('-')[1]) > 0) // Falls es ungültige Werte gibt, ignorieren
            .OrderBy(gpio => gpio) // Sortieren
            .ToList();

        var duplicates = gpios.FindDuplicates();

        foreach (var duplicate in duplicates)
        {
            foreach (var cover in homeNetConfig.Covers)
            {
                if ($"{cover.ControllerId}-{cover.GpioClose}" == duplicate
                 || $"{cover.ControllerId}-{cover.GpioOpen}" == duplicate
                 || $"{cover.ControllerId}-{cover.GpioCloseButton}" == duplicate
                 || $"{cover.ControllerId}-{cover.GpioOpenButton}" == duplicate)
                {
                    errors.Add($"WARNING: In controller '{cover.ControllerId}', cover '{cover.Name}' a duplicate gpio is used: {duplicate}");
                }
            }
            foreach (var light in homeNetConfig.Lights)
            {
                if ($"{light.ControllerId}-{light.GpioPin}" == duplicate
                 || $"{light.ControllerId}-{light.GpioButton}" == duplicate)
                {
                    errors.Add($"\"WARNING: In controller '{light.ControllerId}', light '{light.Name}' a duplicate gpio is used: {duplicate}");
                }
            }
        }

        return string.Join('\n', errors.ToArray());
    }

    public string AddHomeNetElements()
    {
        IDeserializer deserializer = GetDeserializer();

        var mqttData = deserializer.Deserialize<Dictionary<string, MqttConfig>>(_yamlText);
        var mqttConfig = mqttData["mqtt"];// Extrahiere nur den "mqtt"-Teil

        var homeNetData = deserializer.Deserialize<Dictionary<string, HomeNetConfig>>(_yamlText);
        var homeNetConfig = homeNetData["homeNet"]; // Extrahiere nur den "homeNet"-Teil

        var newHomeNetConfig = new HomeNetConfig();

        // für jede mqttConfig muss auch eine arduinoConfig vorhanden sein
        foreach (var cover in mqttConfig.Covers)
        {
            var homeNetCover = homeNetConfig.Covers.SingleOrDefault(h => h.UniqueId == cover.UniqueId);
            if (homeNetCover == null)
            {
                newHomeNetConfig.Covers.Add(new HomeNetCover()
                {
                    ControllerId = 1,
                    GpioClose = 1,
                    GpioOpen = 2,
                    GpioCloseButton = 3,
                    GpioOpenButton = 4,
                    RunningTime = 15000,
                    UniqueId = cover.UniqueId,
                });
            }
        }

        foreach (var light in mqttConfig.Lights)
        {
            var homeNetLight = homeNetConfig.Lights.SingleOrDefault(h => h.UniqueId == light.UniqueId);
            if (homeNetLight == null)
            {
                newHomeNetConfig.Lights.Add(new HomeNetLight()
                {
                    ControllerId = 1,
                    GpioButton = 1,
                    GpioPin = 2,
                    UniqueId = light.UniqueId,
                });
            }
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)  // ERZWINGT die Verwendung der Alias-Namen!
            .Build();

        var yamlOutput = serializer.Serialize(new Dictionary<string, HomeNetConfig> { { "homeNet", newHomeNetConfig } });

        yamlOutput = yamlOutput.Replace("controller: []", "");
        yamlOutput = yamlOutput.Replace("cover: []", "");
        yamlOutput = yamlOutput.Replace("light: []", "");

        return yamlOutput;
    }

    private HomeNetConfig GetMergedMqttConfig(IDeserializer deserializer)
    {
        var mqttData = deserializer.Deserialize<Dictionary<string, MqttConfig>>(_yamlText);
        var mqttConfig = mqttData["mqtt"];// Extrahiere nur den "mqtt"-Teil

        var homeNetData = deserializer.Deserialize<Dictionary<string, HomeNetConfig>>(_yamlText);
        var homeNetConfig = homeNetData["homeNet"]; // Extrahiere nur den "homeNet"-Teil

        // für jede mqttConfig muss auch eine homeNetConfig vorhanden sein
        CheckCovers(mqttConfig, homeNetConfig);
        CheckLights(mqttConfig, homeNetConfig);

        foreach (var homeNetCover in homeNetConfig.Covers)
        {
            var mqttCover = mqttConfig.Covers.SingleOrDefault(h => h.UniqueId == homeNetCover.UniqueId);

            homeNetCover.Name = mqttCover?.Name ?? homeNetCover.UniqueId;
            homeNetCover.CommandTopic = mqttCover?.CommandTopic ?? (new HomeNetCover()).CommandTopic;
            homeNetCover.StateTopic = mqttCover?.StateTopic ?? (new HomeNetCover()).StateTopic;
            homeNetCover.SetPositionTopic = mqttCover?.SetPositionTopic ?? (new HomeNetCover()).SetPositionTopic;
            homeNetCover.PositionTopic = mqttCover?.PositionTopic ?? (new HomeNetCover()).PositionTopic;
            homeNetCover.PayloadOpen = mqttCover?.PayloadOpen ?? (new HomeNetCover()).PayloadOpen;
            homeNetCover.PayloadClose = mqttCover?.PayloadClose ?? (new HomeNetCover()).PayloadClose;
            homeNetCover.PayloadStop = mqttCover?.PayloadStop ?? (new HomeNetCover()).PayloadStop;
            homeNetCover.PositionOpen = mqttCover?.PositionOpen ?? (new HomeNetCover()).PositionOpen;
            homeNetCover.PositionClosed = mqttCover?.PositionClosed ?? (new HomeNetCover()).PositionClosed;
            homeNetCover.StateOpen = mqttCover?.StateOpen ?? (new HomeNetCover()).StateOpen;
            homeNetCover.StateOpening = mqttCover?.StateOpening ?? (new HomeNetCover()).StateOpening;
            homeNetCover.StateClosed = mqttCover?.StateClosed ?? (new HomeNetCover()).StateClosed;
            homeNetCover.StateClosing = mqttCover?.StateClosing ?? (new HomeNetCover()).StateClosing;
            homeNetCover.StateStopped = mqttCover?.StateStopped ?? (new HomeNetCover()).StateStopped;
        }

        foreach (var homeNetLight in homeNetConfig.Lights)
        {
            var mqttLight = mqttConfig.Lights.SingleOrDefault(h => h.UniqueId == homeNetLight.UniqueId);

            homeNetLight.Name = mqttLight?.Name! ?? homeNetLight.UniqueId;
            homeNetLight.CommandTopic = mqttLight?.CommandTopic ?? (new HomeNetLight()).CommandTopic;
            homeNetLight.StateTopic = mqttLight?.StateTopic ?? (new HomeNetLight()).StateTopic;
            homeNetLight.PayloadOn = mqttLight?.PayloadOn ?? (new HomeNetLight()).PayloadOn;
            homeNetLight.PayloadOff = mqttLight?.PayloadOff ?? (new HomeNetLight()).PayloadOff;
        }

        return homeNetConfig;
    }

    private void Truncate(string keyword)
    {
        var lines = _yamlText.Split('\n');
        var result = new List<string>();

        foreach (var line in lines)
        {
            result.Add(line.TruncateText(keyword));
        }

        _yamlText = string.Join("\n", result);
    }

    private IDeserializer GetDeserializer()
    {
        this.DeleteIgnorableItems();

        this.Truncate("!include");

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties() // Ignoriert Alexa
            .Build();
        return deserializer;
    }

    private static void CheckCovers(MqttConfig mqttConfig, HomeNetConfig homeNetConfig)
    {
        try
        {
            CheckKeys(mqttConfig.Covers.Select(c => c.UniqueId).ToList(),
                      homeNetConfig.Covers.Select(c => c.UniqueId).ToList());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Missing cover key: {ex.Message}");
        }
    }

    private static void CheckLights(MqttConfig mqttConfig, HomeNetConfig homeNetConfig)
    {
        try
        {
            CheckKeys(mqttConfig.Lights.Select(c => c.UniqueId).ToList(),
                      homeNetConfig.Lights.Select(c => c.UniqueId).ToList());
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Missing light key: {ex.Message}");
        }
    }

    private static void CheckKeys(List<string> mqttKeys, List<string> arduinoKeys)
    {
        foreach (var mqttKey in mqttKeys)
        {
            if (!arduinoKeys.Select(k => k.Trim()).Contains(mqttKey.Trim()))
            {
                throw new InvalidOperationException($"'{mqttKey.Trim()}' not specified in homeNet devices.");
            }
        }
    }
}