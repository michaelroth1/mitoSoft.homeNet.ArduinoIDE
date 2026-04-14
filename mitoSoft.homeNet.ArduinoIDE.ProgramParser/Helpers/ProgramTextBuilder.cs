using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
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
    private string _program = FileHelper.ReadResourceFile("Program.txt");

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
            var templates = new TemplateSet()
            {
                Declaration = FileHelper.ReadResourceFile("CoverDeclaration.txt"),
                Setup = FileHelper.ReadResourceFile("CoverSetup.txt"),
                MainTemplate = FileHelper.ReadResourceFile("CoverTemplate.txt"),
                Loop = FileHelper.ReadResourceFile("CoverLoop.txt"),
            };

            if (!cover.CommandTopic.StartsWith("_no_"))
                templates.EnableFeature("///hasnocommandtopic: ");

            if (cover.GpioCloseButton > 0)
                templates.EnableFeature("///hasnodownbutton: ");

            if (cover.GpioOpenButton > 0)
                templates.EnableFeature("///hasnoupbutton: ");

            if (!cover.PayloadStop.StartsWith("_no_"))
                templates.EnableFeature("///hasnostoppayload: ");

            if (!cover.SetPositionTopic.StartsWith("_no_"))
                templates.EnableFeature("///hasnosetTopic: ");

            if (!cover.StateTopic.StartsWith("_no_"))
                templates.EnableFeature("///hasnostatetopic: ");

            if (!cover.PositionTopic.StartsWith("_no_"))
                templates.EnableFeature("///hasnopositiontopic: ");

            ReflectionHelper.GetAllProperties(cover)
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .ToList()
                .ForEach(prop => templates.ReplaceInAll($"##{prop.Key}##", prop.Value));

            _program = _program.Replace("##coverDeclaration##", templates.Declaration);
            _program = _program.Replace("##coverSetup##", templates.Setup);
            _program = _program.Replace("##cover##", templates.MainTemplate);
            _program = _program.Replace("##coverLoop##", templates.Loop);
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
            var templates = new TemplateSet()
            {
                Declaration = FileHelper.ReadResourceFile("LightDeclaration.txt"),
                Setup = FileHelper.ReadResourceFile("LightSetup.txt"),
                MainTemplate = FileHelper.ReadResourceFile("LightTemplate.txt"),
                Loop = FileHelper.ReadResourceFile("LightLoop.txt"),
            };

            if (light.GpioButton > 0)
                templates.EnableFeature("///hasnobutton: ");

            if (light.SwitchMode.ToLower().Trim() == "button")
                templates.EnableFeature("///hasnobuttonmode: ");

            if (light.SwitchMode.ToLower().Trim() == "switch")
                templates.EnableFeature("///hasnoswitchmode: ");

            if (!light.CommandTopic.StartsWith("_no_"))
                templates.EnableFeature("///hasnocommandtopic: ");

            if (!light.StateTopic.StartsWith("_no_"))
                templates.EnableFeature("///hasnostatetopic: ");

            ReflectionHelper.GetAllProperties(light)
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .ToList()
                .ForEach(prop => templates.ReplaceInAll($"##{prop.Key}##", prop.Value));

            _program = _program.Replace("##lightDeclaration##", templates.Declaration);
            _program = _program.Replace("##lightSetup##", templates.Setup);
            _program = _program.Replace("##light##", templates.MainTemplate);
            _program = _program.Replace("##lightLoop##", templates.Loop);
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