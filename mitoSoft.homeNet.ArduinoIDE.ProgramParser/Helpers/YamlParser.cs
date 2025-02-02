using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.ProgramParser.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class YamlParser
{
    private string _yamlText;
    private List<string> _items = [];

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
        IDeserializer deserializer = GetDeserializer();

        var homeNetData = deserializer.Deserialize<Dictionary<string, HomeNetConfig>>(_yamlText);
        var homeNetConfig = homeNetData["homeNet"]; // Extrahiere nur den "homeNet"-Teil

        return homeNetConfig.Controllers;
    }

    public MqttConfig Parse(int controllerId)
    {
        IDeserializer deserializer = GetDeserializer();

        MqttConfig mqttConfig = GetMergedMqttConfig(deserializer);

        return new MqttConfig()
        {
            Covers = mqttConfig.Covers.Where(c => c.ControllerId == controllerId).ToList(),
            Lights = mqttConfig.Lights.Where(l => l.ControllerId == controllerId).ToList(),
        };
    }

    public string CheckYaml()
    {
        var warnings = new List<string>();

        IDeserializer deserializer = GetDeserializer();

        MqttConfig mqttConfig = GetMergedMqttConfig(deserializer);

        var gpios = mqttConfig.Covers
            .SelectMany(c => new[] { c.GpioOpen, c.GpioClose, c.GpioOpenButton, c.GpioCloseButton }) // Alle GPIO-Werte sammeln
            .Concat(mqttConfig.Lights.SelectMany(l => new[] { l.GpioPin, l.GpioButton })) // Light GPIOs hinzufügen
            .Where(gpio => gpio > 0) // Falls es ungültige Werte gibt, ignorieren
            .OrderBy(gpio => gpio) // Sortieren
            .ToList();

        var duplicates = gpios.FindDuplicates();

        foreach (var duplicate in duplicates)
        {
            foreach (var cover in mqttConfig.Covers)
            {
                if (cover.GpioClose == duplicate
                 || cover.GpioOpen == duplicate
                 || cover.GpioCloseButton == duplicate
                 || cover.GpioOpenButton == duplicate)
                {
                    warnings.Add($"In cover '{cover.Name}' a duplicate gpio is used: {duplicate}");
                }
            }
            foreach (var light in mqttConfig.Lights )
            {
                if (light.GpioPin  == duplicate
                 || light.GpioButton == duplicate)
                {
                    warnings.Add($"In light '{light.Name}' a duplicate gpio is used: {duplicate}");
                }
            }
        }

        warnings.Insert(0, $"{warnings.Count} warnings found:");

        return string.Join('\n', warnings.ToArray());
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

    private MqttConfig GetMergedMqttConfig(IDeserializer deserializer)
    {
        var mqttData = deserializer.Deserialize<Dictionary<string, MqttConfig>>(_yamlText);
        var mqttConfig = mqttData["mqtt"];// Extrahiere nur den "mqtt"-Teil

        var homeNetData = deserializer.Deserialize<Dictionary<string, HomeNetConfig>>(_yamlText);
        var homeNetConfig = homeNetData["homeNet"]; // Extrahiere nur den "homeNet"-Teil

        // für jede mqttConfig muss auch eine homeNetConfig vorhanden sein
        CheckCovers(mqttConfig, homeNetConfig);
        CheckLights(mqttConfig, homeNetConfig);

        foreach (var cover in mqttConfig.Covers)
        {
            var homeNetCover = homeNetConfig.Covers.Single(h => h.UniqueId == cover.UniqueId);
            cover.GpioOpen = homeNetCover.GpioOpen;
            cover.GpioOpenButton = homeNetCover.GpioOpenButton;
            cover.GpioCloseButton = homeNetCover.GpioCloseButton;
            cover.GpioClose = homeNetCover.GpioClose;
            cover.RunningTime = homeNetCover.RunningTime;
            cover.ControllerId = homeNetCover.ControllerId;
        }

        foreach (var light in mqttConfig.Lights)
        {
            var homeNetLight = homeNetConfig.Lights.Single(h => h.UniqueId == light.UniqueId);
            light.GpioPin = homeNetLight.GpioPin;
            light.GpioButton = homeNetLight.GpioButton;
            light.ControllerId = homeNetLight.ControllerId;
        }

        return mqttConfig;
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