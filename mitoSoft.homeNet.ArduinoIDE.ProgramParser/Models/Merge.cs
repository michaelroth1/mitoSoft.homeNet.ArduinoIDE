using System.Diagnostics;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Merge;

public class Config
{
    public List<Cover> Covers { get; set; } = [];

    public List<Light> Lights { get; set; } = [];
}

public interface IItem
{
    string Name { get; set; }

    bool HasPartner { get; set; }

    string UniqueId { get; set; }

    string Description { get; set; }

    int ControllerId { get; set; }
}


[DebuggerDisplay("{UniqueId}")]
public class Cover : IItem
{
    public string Name { get; set; } = null!;

    public bool HasPartner { get; set; } = false;

    public string UniqueId { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int ControllerId { get; set; } = 1;

    public int GpioOpen { get; set; } = 1;

    public int GpioClose { get; set; } = 2;

    public int GpioOpenButton { get; set; } = 3;

    public int GpioCloseButton { get; set; } = 4;

    public int RunningTime { get; set; } = 15000;

    public string CommandTopic { get; set; } = "_no_topic/command";

    public string StateTopic { get; set; } = "_no_topic/state";

    public string SetPositionTopic { get; set; } = "_no_topic/command/set";

    public string PositionTopic { get; set; } = "_no_topic/state/pos";

    public string PayloadOpen { get; set; } = "_no_payload";

    public string PayloadClose { get; set; } = "_no_payload";

    public string PayloadStop { get; set; } = "_no_payload"!;

    public int PositionOpen { get; set; } = 0;

    public int PositionClosed { get; set; } = 100;

    public string StateOpen { get; set; } = "_no_state";

    public string StateOpening { get; set; } = "_no_state";

    public string StateClosed { get; set; } = "_no_state";

    public string StateClosing { get; set; } = "_no_state";

    public string StateStopped { get; set; } = "_no_state";
}

[DebuggerDisplay("{UniqueId}")]
public class Light : IItem
{
    public string Name { get; set; } = null!;

    public bool HasPartner { get; set; } = false;

    public string UniqueId { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int ControllerId { get; set; } = 1;

    public int GpioPin { get; set; } = 1;

    public int GpioButton { get; set; } = 2;

    public string StateOn { get; set; } = "_no_state";

    public string StateOff { get; set; } = "_no_state";

    public string SwitchMode { get; set; } = "button";

    public string CommandTopic { get; set; } = "_no_topic/command";

    public string StateTopic { get; set; } = "_no_topic/state";

    public string PayloadOn { get; set; } = "_no_payload";

    public string PayloadOff { get; set; } = "_no_payload";
}