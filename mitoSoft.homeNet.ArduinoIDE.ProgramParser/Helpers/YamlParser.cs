using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.ProgramParser.Extensions;
using YamlDotNet.Serialization;

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
        this.DeleteIgnorableItems();

        this.Truncate("!include");

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties() // Ignoriert Alexa
            .Build();

        var homeNetData = deserializer.Deserialize<Dictionary<string, HomeNetConfig>>(_yamlText);
        var homeNetConfig = homeNetData["homeNet"]; // Extrahiere nur den "homeNet"-Teil

        return homeNetConfig.Controllers;
    }

    public MqttConfig Parse(int controllerId)
    {
        this.DeleteIgnorableItems();

        this.Truncate("!include");

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties() // Ignoriert Alexa
            .Build();

        var mqttData = deserializer.Deserialize<Dictionary<string, MqttConfig>>(_yamlText);
        var mqttConfig = mqttData["mqtt"];// Extrahiere nur den "mqtt"-Teil

        var homeNetData = deserializer.Deserialize<Dictionary<string, HomeNetConfig>>(_yamlText);
        var homeNetConfig = homeNetData["homeNet"]; // Extrahiere nur den "homeNet"-Teil

        // für jede mqttConfig muss auch eine arduinoConfig vorhanden sein
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
            var homeNetCover = homeNetConfig.Lights.Single(h => h.UniqueId == light.UniqueId);
            light.GpioPin = homeNetCover.GpioPin;
            light.GpioButton = homeNetCover.GpioButton;
            light.ControllerId = homeNetCover.ControllerId;
        }

        return new MqttConfig()
        {
            Covers = mqttConfig.Covers.Where(c => c.ControllerId == controllerId).ToList(),
            Lights = mqttConfig.Lights.Where(l => l.ControllerId == controllerId).ToList(),
        };
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