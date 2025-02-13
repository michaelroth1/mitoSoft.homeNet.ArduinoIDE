using System.Diagnostics;
using YamlDotNet.Serialization;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Mqtt;

public class Config
{
    [YamlMember(Alias = "cover")]
    public List<Cover> Covers { get; set; } = [];

    [YamlMember(Alias = "light")]
    public List<Light> Lights { get; set; } = [];
}

[DebuggerDisplay("{Name}")]
public class Cover 
{
    [YamlMember(Alias = "name")]
    public required string Name { get; set; } = null!;

    [YamlMember(Alias = "command_topic")]
    public required string CommandTopic { get; set; } = null!;

    [YamlMember(Alias = "state_topic")]
    public string StateTopic { get; set; } = "no_topic/state";

    [YamlMember(Alias = "set_position_topic")]
    public string SetPositionTopic { get; set; } = "no_topic/command/pos";

    [YamlMember(Alias = "position_topic")]
    public string PositionTopic { get; set; } = "no_topic/state/pos";

    [YamlMember(Alias = "payload_open")]
    public required string PayloadOpen { get; set; } = null!;

    [YamlMember(Alias = "payload_close")]
    public required string PayloadClose { get; set; } = null!;

    [YamlMember(Alias = "payload_stop")]
    public required string PayloadStop { get; set; } = null!;

    [YamlMember(Alias = "position_open")]
    public string PositionOpen { get; set; } = "100";

    [YamlMember(Alias = "position_closed")]
    public string PositionClosed { get; set; } = "0";

    [YamlMember(Alias = "state_open")]
    public string StateOpen { get; set; } = "opened";

    [YamlMember(Alias = "state_opening")]
    public string StateOpening { get; set; } = "opening";

    [YamlMember(Alias = "state_closed")]
    public string StateClosed { get; set; } = "closed";

    [YamlMember(Alias = "state_closing")]
    public string StateClosing { get; set; } = "closing";

    [YamlMember(Alias = "state_stopped")]
    public string StateStopped { get; set; } = "stopped";

    [YamlMember(Alias = "unique_id")]
    public required string UniqueId { get; set; } = null!;

    [YamlMember(Alias = "optimistic")]
    public bool Optimistic { get; set; } = false;
}

[DebuggerDisplay("{Name}")]
public class Light
{
    [YamlMember(Alias = "name")]
    public required string Name { get; set; } = null!;

    [YamlMember(Alias = "command_topic")]
    public required string CommandTopic { get; set; } = null!;

    [YamlMember(Alias = "state_topic")]
    public string StateTopic { get; set; } = "no_topic/state";

    [YamlMember(Alias = "payload_on")]
    public required string PayloadOn { get; set; } = null!;

    [YamlMember(Alias = "payload_off")]
    public required string PayloadOff { get; set; } = null!;

    [YamlMember(Alias = "optimistic")]
    public bool Optimistic { get; set; } = false;

    [YamlMember(Alias = "unique_id")]
    public required string UniqueId { get; set; } = null!;
}