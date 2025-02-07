using System.Diagnostics;
using YamlDotNet.Serialization;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

public class MqttConfig
{
    [YamlMember(Alias = "cover")]
    public List<Cover> Covers { get; set; } = [];

    [YamlMember(Alias = "light")]
    public List<Light> Lights { get; set; } = [];
}

public interface IItem
{
    string Name { get; set; }

    int ControllerId { get; set; }

    string UniqueId { get; set; }
}

[DebuggerDisplay("{Name}")]
public class Cover : IItem
{
    [YamlMember(Alias = "name")]
    public required string Name { get; set; } = null!;

    [YamlMember(Alias = "command_topic")]
    public required string CommandTopic { get; set; } = null!;

    [YamlMember(Alias = "state_topic")]
    public string StateTopic { get; set; } = null!;

    [YamlMember(Alias = "set_position_topic")]
    public string SetPositionTopic { get; set; } = null!;

    [YamlMember(Alias = "position_topic")]
    public string PositionTopic { get; set; } = null!;

    [YamlMember(Alias = "payload_open")]
    public required string PayloadOpen { get; set; } = null!;

    [YamlMember(Alias = "payload_close")]
    public required string PayloadClose { get; set; } = null!;

    [YamlMember(Alias = "payload_stop")]
    public required string PayloadStop { get; set; } = null!;

    [YamlMember(Alias = "position_open")]
    public string PositionOpen { get; set; } = null!;

    [YamlMember(Alias = "position_closed")]
    public string PositionClosed { get; set; } = null!;

    [YamlMember(Alias = "state_open")]
    public string StateOpen { get; set; } = null!;

    [YamlMember(Alias = "state_opening")]
    public string StateOpening { get; set; } = null!;

    [YamlMember(Alias = "state_closed")]
    public string StateClosed { get; set; } = null!;

    [YamlMember(Alias = "state_closing")]
    public string StateClosing { get; set; } = null!;

    [YamlMember(Alias = "state_stopped")]
    public string StateStopped { get; set; } = null!;

    [YamlMember(Alias = "unique_id")]
    public required string UniqueId { get; set; } = null!;

    [YamlMember(Alias = "optimistic")]
    public bool Optimistic { get; set; } = false;

    public int ControllerId { get; set; }

    public int GpioOpen { get; set; }

    public int GpioClose { get; set; }

    public int GpioOpenButton { get; set; }

    public int GpioCloseButton { get; set; }

    public int RunningTime { get; set; }
}

[DebuggerDisplay("{Name}")]
public class Light : IItem
{
    [YamlMember(Alias = "name")]
    public required string Name { get; set; } = null!;

    [YamlMember(Alias = "command_topic")]
    public required string CommandTopic { get; set; } = null!;

    [YamlMember(Alias = "state_topic")]
    public string StateTopic { get; set; } = null!;

    [YamlMember(Alias = "payload_on")]
    public required string PayloadOn { get; set; } = null!;

    [YamlMember(Alias = "payload_off")]
    public required string PayloadOff { get; set; } = null!;

    [YamlMember(Alias = "optimistic")]
    public bool Optimistic { get; set; } = false;

    [YamlMember(Alias = "state_on")]
    public string StateOn { get; set; } = null!;

    [YamlMember(Alias = "state_off")]
    public string StateOff { get; set; } = null!;

    [YamlMember(Alias = "state_toggle")]
    public string StateToggle { get; set; } = null!;

    [YamlMember(Alias = "unique_id")]
    public required string UniqueId { get; set; } = null!;

    public int ControllerId { get; set; }

    public int GpioPin { get; set; }

    public int GpioButton { get; set; }
}