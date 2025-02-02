using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class ProgramTextBuilder
{
    private readonly string _controllerName;
    private readonly string _ip;
    private readonly string _mac;
    private readonly string _brokerIp;
    private readonly string _gpioMode;
    private readonly string _subscribedTopic;
    private string _program;

    public ProgramTextBuilder(string controllerName,
                              string ip,
                              string mac,
                              string brokerIp,
                              string gpioMode,
                              string subscribedTopic)
    {
        _controllerName = controllerName;
        _ip = ip;
        _mac = mac;
        _brokerIp = brokerIp;
        _gpioMode = gpioMode;
        _subscribedTopic = subscribedTopic;
        _program = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.Program.txt");
    }

    public string Build(MqttConfig config)
    {
        this.SetHeaderInfo();

        this.SetCoverInfo(config.Covers);

        this.SetLightInfo(config.Lights);

        this.SetGpioMode();

        this.CleanUp();

        this.Check();

        return _program;
    }

    private void SetGpioMode()
    {
        _program = _program.Replace("##GpioMode##", _gpioMode);
    }

    private void SetHeaderInfo()
    {
        _program = _program.Replace("##controllerName##", _controllerName);
        _program = _program.Replace("##ip##", _ip);
        _program = _program.Replace("##mac##", _mac);
        _program = _program.Replace("##brokerIp##", _brokerIp);
        _program = _program.Replace("##date##", DateTime.Now.ToString());
        _program = _program.Replace("##author##", "build with mitoSoft.ArduinoIDE");
        _program = _program.Replace("##subscribedTopic##", _subscribedTopic);
    }

    private void SetCoverInfo(IList<Cover> covers)
    {
        foreach (var cover in covers)
        {
            var coverTemplate = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.Cover.txt");
            var coverSetup = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverSetup.txt");
            var coverInit = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverReferenceRun.txt");

            var properties = ReflectionHelper.GetAllProperties(cover);
            foreach (var prop in properties)
            {
                coverSetup = coverSetup.Replace($"##{prop.Key}##", prop.Value);
                coverInit = coverInit.Replace($"##{prop.Key}##", prop.Value);
                coverTemplate = coverTemplate.Replace($"##{prop.Key}##", prop.Value);
            }

            _program = _program.Replace("##coverSetup##", coverSetup);
            _program = _program.Replace("##coverReferenceRun##", coverInit);
            _program = _program.Replace("##cover##", coverTemplate);
        }
    }

    private void SetLightInfo(IList<Light> lights)
    {
        foreach (var light in lights)
        {
            var lightTemplate = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.Light.txt");
            var lightSetup = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightSetup.txt");
            var lightInit = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightInit.txt");

            var properties = ReflectionHelper.GetAllProperties(light);
            foreach (var prop in properties)
            {
                lightTemplate = lightTemplate.Replace($"##{prop.Key}##", prop.Value);
                lightSetup = lightSetup.Replace($"##{prop.Key}##", prop.Value);
                lightInit = lightInit.Replace($"##{prop.Key}##", prop.Value);
            }

            _program = _program.Replace("##light##", lightTemplate);
            _program = _program.Replace("##lightSetup##", lightSetup);
            _program = _program.Replace("##lightInit##", lightInit);
        }
    }

    private void CleanUp()
    {
        _program = _program.Replace("##cover##", "");
        _program = _program.Replace("##light##", "");
        _program = _program.Replace("##coverSetup##", "");
        _program = _program.Replace("##lightSetup##", "");
        _program = _program.Replace("##coverReferenceRun##", "");
        _program = _program.Replace("##lightInit##", "");
    }

    private void Check()
    {
        if (_program.Contains("##"))
        {
            throw new InvalidOperationException("Error in Program - still containing a '##' char.");
        }
    }
}