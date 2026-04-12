using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using Merge = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Merge;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class ProgramTextBuilder(string controllerName,
                                string ip,
                                string mac,
                                string brokerIp,
                                string brokerUserName,
                                string brokerPassword,
                                string gpioOutputMode,
                                string subscribedTopic,
                                string additionalDeclaration,
                                string additionalSetup,
                                string additionalCode)
{
    private readonly string _controllerName = controllerName;
    private readonly string _ip = ip ?? "0.0.0.0";
    private readonly string _mac = mac ?? "0x00, 0x00, 0x00, 0x00, 0x00, 0x00";
    private readonly string _brokerIp = brokerIp ?? "0.0.0.0";
    private readonly string _brokerUserName = brokerUserName ?? "";
    private readonly string _brokerPassword = brokerPassword ?? "";
    private readonly string _gpioOutputMode = gpioOutputMode ?? "STANDARD";
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

        this.SetMqttInfo();

        this.CleanUp();

        return _program;
    }

    private void SetHeaderInfo()
    {
        _program = _program.Replace("##controllerName##", _controllerName);
        _program = _program.Replace("##ip##", _ip.GetArduinoIPFormat());
        _program = _program.Replace("##mac##", _mac.GetArduinoSignaturFormat());
        _program = _program.Replace("##brokerIp##", _brokerIp.GetArduinoIPFormat());
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
            cover.ReplaceDescription();
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
            var coverLoop = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverLoop.txt");

            if (!cover.CommandTopic.StartsWith("_no_"))
            {
                coverDeclaration = coverDeclaration.Replace($"///hasnocommandtopic: ", "");
                coverSetup = coverSetup.Replace($"///hasnocommandtopic: ", "");
                coverTemplate = coverTemplate.Replace($"///hasnocommandtopic: ", "");
                coverLoop = coverLoop.Replace($"///hasnocommandtopic: ", "");
            }
            if (cover.GpioCloseButton > 0)
            {
                coverDeclaration = coverDeclaration.Replace($"///hasnodownbutton: ", "");
                coverSetup = coverSetup.Replace($"///hasnodownbutton: ", "");
                coverTemplate = coverTemplate.Replace($"///hasnodownbutton: ", "");
                coverLoop = coverLoop.Replace($"///hasnodownbutton: ", "");
            }
            if (cover.GpioOpenButton > 0)
            {
                coverDeclaration = coverDeclaration.Replace($"///hasnoupbutton: ", "");
                coverSetup = coverSetup.Replace($"///hasnoupbutton: ", "");
                coverTemplate = coverTemplate.Replace($"///hasnoupbutton: ", "");
                coverLoop = coverLoop.Replace($"///hasnoupbutton: ", "");
            }
            if (!cover.PayloadStop.StartsWith("_no_"))
            {
                coverDeclaration = coverDeclaration.Replace($"///hasnostoppayload: ", "");
                coverSetup = coverSetup.Replace($"///hasnostoppayload: ", "");
                coverTemplate = coverTemplate.Replace($"///hasnostoppayload: ", "");
                coverLoop = coverLoop.Replace($"///hasnostoppayload: ", "");
            }
            if (!cover.SetPositionTopic.StartsWith("_no_"))
            {
                coverDeclaration = coverDeclaration.Replace($"///hasnosetTopic: ", "");
                coverSetup = coverSetup.Replace($"///hasnosetTopic: ", "");
                coverTemplate = coverTemplate.Replace($"///hasnosetTopic: ", "");
                coverLoop = coverLoop.Replace($"///hasnosetTopic: ", "");
            }
            if (!cover.StateTopic.StartsWith("_no_"))
            {
                coverDeclaration = coverDeclaration.Replace($"///hasnostatetopic: ", "");
                coverSetup = coverSetup.Replace($"///hasnostatetopic: ", "");
                coverTemplate = coverTemplate.Replace($"///hasnostatetopic: ", "");
                coverLoop = coverLoop.Replace($"///hasnostatetopic: ", "");
            }
            if (!cover.PositionTopic.StartsWith("_no_"))
            {
                coverDeclaration = coverDeclaration.Replace($"///hasnopositiontopic: ", "");
                coverSetup = coverSetup.Replace($"///hasnopositiontopic: ", "");
                coverTemplate = coverTemplate.Replace($"///hasnopositiontopic: ", "");
                coverLoop = coverLoop.Replace($"///hasnopositiontopic: ", "");
            }

            var properties = ReflectionHelper.GetAllProperties(cover);
            foreach (var prop in properties.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
            {
                coverDeclaration = coverDeclaration.Replace($"##{prop.Key}##", prop.Value);
                coverSetup = coverSetup.Replace($"##{prop.Key}##", prop.Value);
                coverTemplate = coverTemplate.Replace($"##{prop.Key}##", prop.Value);
                coverLoop = coverLoop.Replace($"##{prop.Key}##", prop.Value);
            }

            _program = _program.Replace("##coverDeclaration##", coverDeclaration);
            _program = _program.Replace("##coverSetup##", coverSetup);
            _program = _program.Replace("##cover##", coverTemplate);
            _program = _program.Replace("##coverLoop##", coverLoop);
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
            light.ReplaceDescription();
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
            var lightLoop = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightLoop.txt");

            if (light.GpioButton > 0)
            {
                lightDeclaration = lightDeclaration.Replace($"///hasnobutton: ", "");
                lightSetup = lightSetup.Replace($"///hasnobutton: ", "");
                lightTemplate = lightTemplate.Replace($"///hasnobutton: ", "");
                lightLoop = lightLoop.Replace($"///hasnobutton: ", "");
            }
            if (light.SwitchMode.ToLower().Trim() == "button")
            {
                lightDeclaration = lightDeclaration.Replace($"///hasnobuttonmode: ", "");
                lightSetup = lightSetup.Replace($"///hasnobuttonmode: ", "");
                lightTemplate = lightTemplate.Replace($"///hasnobuttonmode: ", "");
                lightLoop = lightLoop.Replace($"///hasnobuttonmode: ", "");
            }
            if (light.SwitchMode.ToLower().Trim() == "switch")
            {
                lightDeclaration = lightDeclaration.Replace($"///hasnoswitchmode: ", "");
                lightSetup = lightSetup.Replace($"///hasnoswitchmode: ", "");
                lightTemplate = lightTemplate.Replace($"///hasnoswitchmode: ", "");
                lightLoop = lightLoop.Replace($"///hasnoswitchmode: ", "");
            }
            if (!light.CommandTopic.StartsWith("_no_"))
            {
                lightDeclaration = lightDeclaration.Replace($"///hasnocommandtopic: ", "");
                lightSetup = lightSetup.Replace($"///hasnocommandtopic: ", "");
                lightTemplate = lightTemplate.Replace($"///hasnocommandtopic: ", "");
                lightLoop = lightLoop.Replace($"///hasnocommandtopic: ", "");
            }
            if (!light.StateTopic.StartsWith("_no_"))
            {
                lightDeclaration = lightDeclaration.Replace($"///hasnostatetopic: ", "");
                lightSetup = lightSetup.Replace($"///hasnostatetopic: ", "");
                lightTemplate = lightTemplate.Replace($"///hasnostatetopic: ", "");
                lightLoop = lightLoop.Replace($"///hasnostatetopic: ", "");
            }

            var properties = ReflectionHelper.GetAllProperties(light);
            foreach (var prop in properties.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
            {
                lightDeclaration = lightDeclaration.Replace($"##{prop.Key}##", prop.Value);
                lightSetup = lightSetup.Replace($"##{prop.Key}##", prop.Value);
                lightTemplate = lightTemplate.Replace($"##{prop.Key}##", prop.Value);
                lightLoop = lightLoop.Replace($"##{prop.Key}##", prop.Value);
            }

            _program = _program.Replace("##lightDeclaration##", lightDeclaration);
            _program = _program.Replace("##lightSetup##", lightSetup);
            _program = _program.Replace("##light##", lightTemplate);
            _program = _program.Replace("##lightLoop##", lightLoop);
        }
        catch (Exception ex)
        {
            throw new Exception($"{light.Name}: {ex.Message}");
        }
    }

    private void SetGpioMode()
    {
        _program = _program.Replace("##GpioOutputMode##", _gpioOutputMode);
    }

    private void SetMqttInfo()
    {
        if (_ip.IsValidIPAddress()
            && _brokerIp.IsValidIPAddress()
            && _subscribedTopic.DontStartsWith("_no_topic")
            && _mac.IsValidMacAddress())
        {
            //hasmqtt = true -> this means delete ///hasnomqtt:
            _program = _program.Replace("///hasnomqtt: ", "");
        }
    }

    private void CleanUp()
    {
        _program = _program.CleanKeyWord("##coverDeclaration##");
        _program = _program.CleanKeyWord("##coverSetup##");
        _program = _program.CleanKeyWord("##cover##");
        _program = _program.CleanKeyWord("##coverLoop##");
        _program = _program.CleanKeyWord("##lightDeclaration##");
        _program = _program.CleanKeyWord("##lightSetup##");
        _program = _program.CleanKeyWord("##light##");
        _program = _program.CleanKeyWord("##lightLoop##");
        _program = _program.CleanKeyWord("##additionalDeclaration##");
        _program = _program.CleanKeyWord("##additionalSetup##");
        _program = _program.CleanKeyWord("##additionalCode##");
        _program = _program.RemoveDoubleEmptyRows();
    }

    public void Check()
    {
        if (_program.Contains("##"))
        {
            throw new InvalidOperationException("Error in Program - still containing a '##' char.");
        }
    }
}