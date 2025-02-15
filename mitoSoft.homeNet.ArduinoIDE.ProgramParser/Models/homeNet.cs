using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using YamlDotNet.Serialization;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;

public class Config
{
    [YamlMember(Alias = "cover")]
    public List<Cover> Covers { get; set; } = [];

    [YamlMember(Alias = "light")]
    public List<Light> Lights { get; set; } = [];

    [YamlMember(Alias = "controller")]
    public List<Controller> Controllers { get; set; } = [];
}

/*
- name: "MilaController"
  unique_id: 123 
  ip: "192,168,2,200"
  mac: "0xA8, 0x61, 0x0A, 0xAE, 0x16, 0x3D"
 */
[DebuggerDisplay("{Name}-{IPAddress}")]
public class Controller
{
    [YamlMember(Alias = "name")]
    public required string Name { get; set; } = null!;

    [YamlMember(Alias = "subscribed_topic")]
    public required string SubscribedTopic { get; set; } = null!;

    [YamlMember(Alias = "unique_id")]
    public required int UniqueId { get; set; }

    [YamlMember(Alias = "ip")]
    public required string IPAddress { get; set; } = null!;

    [YamlMember(Alias = "mac")]
    public required string MacAddress { get; set; } = null!;

    [YamlMember(Alias = "broker")]
    public required string BrokerIPAddress { get; set; } = null!;

    [YamlMember(Alias = "gpio_mode")]
    public required string GpioMode { get; set; } = null!;

    [YamlMember(Alias = "additional_declaration")]
    public string AdditionalDeclaration { get; set; } = null!;

    [YamlMember(Alias = "additional_setup")]
    public string AdditionalSetup { get; set; } = null!;

    [YamlMember(Alias = "additional_code")]
    public string AdditionalCode { get; set; } = null!;

    public override string ToString()
    {
        return this.Name;
    }
}

[DebuggerDisplay("{UniqueId}")]
public class Cover
{
    [YamlMember(Alias = "unique_id")]
    [Required]
    public string UniqueId { get; set; } = null!;

    [YamlMember(Alias = "description")]
    public string Description { get; set; } = null!;

    [YamlMember(Alias = "controller_id")]
    [Required]
    public int ControllerId { get; set; } = 1;

    [YamlMember(Alias = "gpio_open")]
    [Required]
    public int GpioOpen { get; set; } = 1;

    [YamlMember(Alias = "gpio_close")]
    [Required]
    public int GpioClose { get; set; } = 2;

    [YamlMember(Alias = "gpio_in_open")]
    public int GpioOpenButton { get; set; } = -1;

    [YamlMember(Alias = "gpio_in_close")]
    public int GpioCloseButton { get; set; } = -1;

    [YamlMember(Alias = "running_time")]
    [Required]
    public int RunningTime { get; set; } = 15000;
}

[DebuggerDisplay("{UniqueId}")]
public class Light
{
    [YamlMember(Alias = "unique_id")]
    [Required]
    public string UniqueId { get; set; } = null!;

    [YamlMember(Alias = "description")]
    public string Description { get; set; } = null!;

    [YamlMember(Alias = "controller_id")]
    [Required]
    public int ControllerId { get; set; } = 1;

    [YamlMember(Alias = "gpio_pin")]
    [Required]
    public int GpioPin { get; set; } = 1;

    [YamlMember(Alias = "gpio_button")]
    public int GpioButton { get; set; } = -1;

    [YamlMember(Alias = "state_on")]
    public string StateOn { get; set; } = "on";

    [YamlMember(Alias = "state_off")]
    public string StateOff { get; set; } = "off";

    [YamlMember(Alias = "switch_mode")]
    [Required]
    public string SwitchMode { get; set; } = "button";
}