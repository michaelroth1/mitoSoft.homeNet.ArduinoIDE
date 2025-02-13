namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

public class YamlFile
{
    public HomeNet.Config HomeNetConfig { get; set; } = null!;

    public Mqtt.Config MqttConfig { get; set; } = null!;
}