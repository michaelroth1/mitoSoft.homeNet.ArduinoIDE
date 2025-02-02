using YamlDotNet.Serialization;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

public class HomeNetConfig
{
    [YamlMember(Alias = "cover")]
    public List<HomeNetCover> Covers { get; set; } = [];

    [YamlMember(Alias = "light")]
    public List<HomeNetLight> Lights { get; set; } = [];

    [YamlMember(Alias = "controller")]
    public required List<HomeNetController> Controllers { get; set; } = [];
}

/*
- name: "MilaController"
  unique_id: 123 
  ip: "192,168,2,200"
  mac: "0xA8, 0x61, 0x0A, 0xAE, 0x16, 0x3D"
 */
public class HomeNetController
{
    [YamlMember(Alias = "name")]
    public required string Name { get; set; } = null!;

    [YamlMember(Alias = "unique_id")]
    public required int UniqueId { get; set; }

    [YamlMember(Alias = "ip")]
    public required string IPAddress { get; set; } = null!;

    [YamlMember(Alias = "mac")]
    public required string MacAddress { get; set; } = null!;

    public override string ToString()
    {
        return this.Name;
    }
}

public class HomeNetCover
{
    [YamlMember(Alias = "unique_id")]
    public required string UniqueId { get; set; } = null!;

    [YamlMember(Alias = "controller_id")]
    public required int ControllerId { get; set; }

    [YamlMember(Alias = "gpio_open")]
    public required int GpioOpen { get; set; }

    [YamlMember(Alias = "gpio_close")]
    public required int GpioClose { get; set; }

    [YamlMember(Alias = "gpio_in_open")]
    public required int GpioOpenButton { get; set; }

    [YamlMember(Alias = "gpio_in_close")]
    public required int GpioCloseButton { get; set; }

    [YamlMember(Alias = "running_time")]
    public required int RunningTime { get; set; }
}

public class HomeNetLight
{
    [YamlMember(Alias = "unique_id")]
    public required string UniqueId { get; set; } = null!;

    [YamlMember(Alias = "controller_id")]
    public required int ControllerId { get; set; }

    [YamlMember(Alias = "gpio_pin")]
    public required int GpioPin { get; set; }

    [YamlMember(Alias = "gpio_button")]
    public required int GpioButton { get; set; }
}