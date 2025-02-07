using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

internal class MqttConfigHelper
{
    private readonly MqttConfig _mqtt;

    public MqttConfigHelper(MqttConfig mqttConfig)
    {
        _mqtt = mqttConfig;
    }

    public MqttConfig GetConfigByControllerId(int controllerId)
    {
        return new MqttConfig()
        {
            Covers = this._mqtt.Covers.Where(c => c.ControllerId == controllerId).ToList(),
            Lights = this._mqtt.Lights.Where(l => l.ControllerId == controllerId).ToList(),
        };
    }

}