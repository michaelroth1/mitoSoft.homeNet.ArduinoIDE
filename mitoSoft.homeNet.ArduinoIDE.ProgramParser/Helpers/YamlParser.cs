using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using HomeNet = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;
using Mqtt = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Mqtt;
using Merge = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Merge;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class YamlParser(string yaml)
{
    public string _yamlText = yaml;

    public HomeNet.Controller GetController(string controllerName)
    {
        var yaml = this.Deserialize();

        var controller = yaml.HomeNetConfig.Controllers.Single(c => c.Name == controllerName);

        return controller!;
    }

    public Merge.Config Parse(int controllerId)
    {
        var yaml = this.Deserialize();

        var homeNet = yaml.HomeNetConfig.GetByControllerId(controllerId);

        return homeNet.MergeMqttConfig(yaml.MqttConfig);
    }

    public string AddHomeNetElements()
    {
        var yaml = this.Deserialize();

        var newHomeNetConfig = new HomeNet.Config();

        // für jede mqttConfig muss auch eine arduinoConfig vorhanden sein
        foreach (var cover in yaml.MqttConfig.Covers)
        {
            var homeNetCover = yaml.HomeNetConfig.Covers.SingleOrDefault(h => h.UniqueId == cover.UniqueId);
            if (homeNetCover == null)
            {
                newHomeNetConfig.Covers.Add(new HomeNet.Cover()
                {
                    Description = cover.UniqueId,
                    UniqueId = cover.UniqueId,
                });
            }
        }

        foreach (var light in yaml.MqttConfig.Lights)
        {
            var homeNetLight = yaml.HomeNetConfig.Lights.SingleOrDefault(h => h.UniqueId == light.UniqueId);
            if (homeNetLight == null)
            {
                newHomeNetConfig.Lights.Add(new HomeNet.Light()
                {
                    Description = light.UniqueId,
                    UniqueId = light.UniqueId,
                });
            }
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(NullNamingConvention.Instance)  // ERZWINGT die Verwendung der Alias-Namen!
            .Build();

        var yamlOutput = serializer.Serialize(new Dictionary<string, HomeNet.Config> { { "homeNet", newHomeNetConfig } });

        yamlOutput = yamlOutput.Replace("controller: []", "");
        yamlOutput = yamlOutput.Replace("cover: []", "");
        yamlOutput = yamlOutput.Replace("light: []", "");

        return yamlOutput;
    }

    public YamlFile Deserialize()
    {
        IDeserializer deserializer = GetDeserializer();

        var mqttData = deserializer.Deserialize<Dictionary<string, Mqtt.Config>>(_yamlText);
        var homeNetData = deserializer.Deserialize<Dictionary<string, HomeNet.Config>>(_yamlText);

        var yaml = new YamlFile()
        {
            HomeNetConfig = homeNetData["homeNet"],

            MqttConfig = mqttData["mqtt"],
        };

        return yaml;
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
        this.Truncate("!include");

        var deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties() // Ignoriert Alexa
            .Build();
        return deserializer;
    }
}