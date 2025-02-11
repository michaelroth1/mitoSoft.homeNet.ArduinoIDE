﻿using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.ProgramParser.Extensions;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class ProgramTextBuilder
{
    private readonly string _controllerName;
    private readonly string _ip;
    private readonly string _mac;
    private readonly string _brokerIp;
    private readonly string _gpioMode;
    private readonly string _subscribedTopic;
    private readonly string _additionalSetup;
    private readonly string _additionalDeclaration;
    private readonly string _additionalCode;
    private string _program;

    public ProgramTextBuilder(string controllerName,
                              string ip,
                              string mac,
                              string brokerIp,
                              string gpioMode,
                              string subscribedTopic,
                              string additionalDeclaration,
                              string additionalSetup,
                              string additionalCode)
    {
        _controllerName = controllerName;
        _ip = ip;
        _mac = mac;
        _brokerIp = brokerIp;
        _gpioMode = gpioMode;
        _subscribedTopic = subscribedTopic;
        _additionalDeclaration = additionalDeclaration;
        _additionalSetup = additionalSetup;
        _additionalCode = additionalCode;
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
        _program = _program.ReplaceWithWhiteSpaces(0, "##additionalDeclaration##", _additionalDeclaration);
        _program = _program.ReplaceWithWhiteSpaces(2,"##additionalSetup##", _additionalSetup);
        _program = _program.ReplaceWithWhiteSpaces(2, "##additionalCode##", _additionalCode);
    }

    private void SetCoverInfo(IList<Cover> covers)
    {
        foreach (var cover in covers)
        {             
            var coverDeclaration = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverDeclaration.txt");
            var coverSetup = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverSetup.txt");
            var coverTemplate = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.CoverTemplate.txt");

            var properties = ReflectionHelper.GetAllProperties(cover);
            foreach (var prop in properties)
            {
                coverDeclaration = coverDeclaration.Replace($"##{prop.Key}##", prop.Value);
                coverSetup = coverSetup.Replace($"##{prop.Key}##", prop.Value);
                coverTemplate = coverTemplate.Replace($"##{prop.Key}##", prop.Value);
            }

            _program = _program.Replace("##coverDeclaration##", coverDeclaration);
            _program = _program.Replace("##coverSetup##", coverSetup);
            _program = _program.Replace("##cover##", coverTemplate);
        }
    }

    private void SetLightInfo(IList<Light> lights)
    {
        foreach (var light in lights)
        {
            var lightDeclaration = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightDeclaration.txt");
            var lightSetup = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightSetup.txt");
            var lightTemplate = FileHelper.ReadResourceFile("mitoSoft.homeNet.ArduinoIDE.ProgramParser.Templates.LightTemplate.txt");
            
            var properties = ReflectionHelper.GetAllProperties(light);
            foreach (var prop in properties)
            {
                lightDeclaration = lightDeclaration.Replace($"##{prop.Key}##", prop.Value);
                lightSetup = lightSetup.Replace($"##{prop.Key}##", prop.Value);
                lightTemplate = lightTemplate.Replace($"##{prop.Key}##", prop.Value);
            }

            _program = _program.Replace("##lightDeclaration##", lightDeclaration);
            _program = _program.Replace("##lightSetup##", lightSetup);
            _program = _program.Replace("##light##", lightTemplate);
        }
    }

    private void CleanUp()
    {   
        _program = _program.Replace("##coverDeclaration##", "");
        _program = _program.Replace("##coverSetup##", "");
        _program = _program.Replace("##cover##", "");
        _program = _program.Replace("##lightDeclaration##", "");
        _program = _program.Replace("##lightSetup##", "");
        _program = _program.Replace("##light##", "");

        _program = _program.Replace("##additionalDeclaration##", "");
        _program = _program.Replace("##additionalSetup##", "");
        _program = _program.Replace("##additionalCode##", "");

        _program = _program.RemoveDoubleEmptyRows();
        _program = _program.CleanEmptyRows();
        _program = _program.Replace("\n\n  }", "\n  }");
        _program = _program.Replace("\n\n}", "\n}");
    }

    private void Check()
    {
        if (_program.Contains("##"))
        {
            throw new InvalidOperationException("Error in Program - still containing a '##' char.");
        }
    }
}