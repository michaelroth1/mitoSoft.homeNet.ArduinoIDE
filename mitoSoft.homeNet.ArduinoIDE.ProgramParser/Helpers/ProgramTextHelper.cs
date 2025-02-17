using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using Merge = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Merge;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class ProgramTextBuilder(string controllerName,
                          string ip,
                          string mac,
                          string brokerIp,
                          string brokerUserName,
                          string brokerPassword,
                          string gpioMode,
                          string subscribedTopic,
                          string additionalDeclaration,
                          string additionalSetup,
                          string additionalCode)
{
    private readonly string _controllerName = controllerName;
    private readonly string _ip = ip ?? "0, 0, 0, 0";
    private readonly string _mac = mac ?? "0x00, 0x00, 0x00, 0x00, 0x00, 0x00";
    private readonly string _brokerIp = brokerIp ?? "0, 0, 0, 0";
    private readonly string _brokerUserName = brokerUserName ?? "";
    private readonly string _brokerPassword = brokerPassword ?? "";
    private readonly string _gpioMode = gpioMode;
    private readonly string _subscribedTopic = subscribedTopic ?? "_no_topic/to_subcribe/#";
    private readonly string _additionalSetup = additionalSetup ?? "///hasnoadditionalsetup";
    private readonly string _additionalDeclaration = additionalDeclaration ?? "///hasnoadditionaldeclaration";
    private readonly string _additionalCode = additionalCode ?? "///hasnoadditionalcode";
    private string _program = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.Program.txt");

    public string Build(Merge.Config config)
    {
        this.SetHeaderInfo();

        this.SetCoverInfo(config.Covers);

        this.SetLightInfo(config.Lights);

        this.SetGpioMode();

        this.CleanUp();

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
        _program = _program.Replace("##brokerUserName##", _brokerUserName);
        _program = _program.Replace("##brokerPassword##", _brokerPassword);
        _program = _program.Replace("##date##", DateTime.Now.ToString());
        _program = _program.Replace("##author##", "build with mitoSoft.ArduinoIDE");
        _program = _program.Replace("##subscribedTopic##", _subscribedTopic);
        _program = _program.Replace("##additionalDeclaration##", _additionalDeclaration);
        _program = _program.Replace("##additionalSetup##", _additionalSetup, 2);
        _program = _program.Replace("##additionalCode##", _additionalCode, 2);
    }

    private void SetCoverInfo(IList<Merge.Cover> covers)
    {
        foreach (var cover in covers)
        {
            this.TrySetCoverInfo(cover);
        }
    }

    private void TrySetCoverInfo(Merge.Cover cover)
    {
        try
        {
            var coverDeclaration = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverDeclaration.txt");
            var coverSetup = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverSetup.txt");
            var coverTemplate = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverTemplate.txt");

            if (!cover.CommandTopic.StartsWith("_no_"))
            {
                coverTemplate = coverTemplate.Replace($"///hasnocommandtopic: ", "");
            }
            if (cover.GpioCloseButton > 0)
            {
                coverTemplate = coverTemplate.Replace($"///hasnodownbutton: ", "");
                coverDeclaration = coverDeclaration.Replace($"///hasnodownbutton: ", "");
            }
            if (cover.GpioOpenButton > 0)
            {
                coverTemplate = coverTemplate.Replace($"///hasnoupbutton: ", "");
                coverDeclaration = coverDeclaration.Replace($"///hasnoupbutton: ", "");
            }
            if (!cover.PayloadStop.StartsWith("_no_"))
            {
                coverTemplate = coverTemplate.Replace($"///hasnostoppayload: ", "");
            }
            if (!cover.SetPositionTopic.StartsWith("_no_"))
            {
                coverTemplate = coverTemplate.Replace($"///hasnosetTopic: ", "");
            }
            if (!cover.StateTopic.StartsWith("_no_"))
            {
                coverTemplate = coverTemplate.Replace($"///hasnostatetopic: ", "");
            }
            if (!cover.PositionTopic.StartsWith("_no_"))
            {
                coverTemplate = coverTemplate.Replace($"///hasnopositiontopic: ", "");
            }

            var properties = ReflectionHelper.GetAllProperties(cover);
            foreach (var prop in properties.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
            {
                coverDeclaration = coverDeclaration.Replace($"##{prop.Key}##", prop.Value);
                coverSetup = coverSetup.Replace($"##{prop.Key}##", prop.Value);
                coverTemplate = coverTemplate.Replace($"##{prop.Key}##", prop.Value);
            }

            if (coverTemplate.Contains("##Description##"))
            {
                coverTemplate = coverTemplate.Replace($"##Description##", $"Cover {cover.Name}");
            }

            _program = _program.Replace("##coverDeclaration##", coverDeclaration);
            _program = _program.Replace("##coverSetup##", coverSetup);
            _program = _program.Replace("##cover##", coverTemplate);
        }
        catch (Exception ex)
        {
            throw new Exception($"{cover.Name}: {ex.Message}");
        }
    }

    private void SetLightInfo(IList<Merge.Light> lights)
    {
        foreach (var light in lights)
        {
            this.TrySetLightInfo(light);
        }
    }

    private void TrySetLightInfo(Merge.Light light)
    {
        try
        {
            var lightDeclaration = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightDeclaration.txt");
            var lightSetup = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightSetup.txt");
            var lightTemplate = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightTemplate.txt");

            if (light.GpioButton > 0)
            {
                lightTemplate = lightTemplate.Replace($"///hasnobutton: ", "");
                lightDeclaration = lightDeclaration.Replace($"///hasnobutton: ", "");
            }
            if (light.SwitchMode.ToLower().Trim() == "button")
            {
                lightTemplate = lightTemplate.Replace($"///hasnobuttonmode: ", "");
            }
            if (light.SwitchMode.ToLower().Trim() == "switch")
            {
                lightTemplate = lightTemplate.Replace($"///hasnoswitchmode: ", "");
            }
            if (!light.CommandTopic.StartsWith("_no_"))
            {
                lightTemplate = lightTemplate.Replace($"///hasnocommandtopic: ", "");
            }
            if (!light.StateTopic.StartsWith("_no_"))
            {
                lightTemplate = lightTemplate.Replace($"///hasnostatetopic: ", "");
            }

            var properties = ReflectionHelper.GetAllProperties(light);
            foreach (var prop in properties.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
            {
                lightDeclaration = lightDeclaration.Replace($"##{prop.Key}##", prop.Value);
                lightSetup = lightSetup.Replace($"##{prop.Key}##", prop.Value);
                lightTemplate = lightTemplate.Replace($"##{prop.Key}##", prop.Value);
            }

            if (lightTemplate.Contains("##Description##"))
            {
                lightTemplate = lightTemplate.Replace($"##Description##", $"Light {light.Name}");
            }

            _program = _program.Replace("##lightDeclaration##", lightDeclaration);
            _program = _program.Replace("##lightSetup##", lightSetup);
            _program = _program.Replace("##light##", lightTemplate);
        }
        catch (Exception ex)
        {
            throw new Exception($"{light.Name}: {ex.Message}");
        }
    }

    private void CleanUp()
    {
        if (_ip.IsValidIPAddress()
           && _brokerIp.IsValidIPAddress()
           && _subscribedTopic.StartsWith("_no_topic")
           && _mac.IsValidMacAddress())
        {
            //hasmqtt = true -> this means delete ///hasnomqtt:
            _program = _program.Replace("///hasnomqtt: ", "");
        }

        _program = _program.CleanKeyWord("##coverDeclaration##");
        _program = _program.CleanKeyWord("##coverSetup##");
        _program = _program.CleanKeyWord("##cover##");
        _program = _program.CleanKeyWord("##lightDeclaration##");
        _program = _program.CleanKeyWord("##lightSetup##");
        _program = _program.CleanKeyWord("##light##");
        _program = _program.CleanKeyWord("##additionalDeclaration##");
        _program = _program.CleanKeyWord("##additionalSetup##");
        _program = _program.CleanKeyWord("##additionalCode##");
    }

    public void Check()
    {
        if (_program.Contains("##"))
        {
            throw new InvalidOperationException("Error in Program - still containing a '##' char.");
        }
    }
}