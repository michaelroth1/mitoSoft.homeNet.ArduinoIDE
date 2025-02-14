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

    public int ControllerId { get; set; }

    public int GpioOpen { get; set; }

    public int GpioClose { get; set; }

    public int GpioOpenButton { get; set; }

    public int GpioCloseButton { get; set; }

    public int RunningTime { get; set; }

    public string CommandTopic { get; set; } = "no_topic/command";

    public string StateTopic { get; set; } = "no_topic/state";

    public string SetPositionTopic { get; set; } = "no_topic/command/set";

    public string PositionTopic { get; set; } = "no_topic/state/pos";

    public string PayloadOpen { get; set; } = "up";

    public string PayloadClose { get; set; } = "close";

    public string PayloadStop { get; set; } = "no_message"!;

    public int PositionOpen { get; set; } = 0;

    public int PositionClosed { get; set; } = 100;

    public string StateOpen { get; set; } = "opened";

    public string StateOpening { get; set; } = "opening";

    public string StateClosed { get; set; } = "closed";

    public string StateClosing { get; set; } = "closing";

    public string StateStopped { get; set; } = "stopped";
}

[DebuggerDisplay("{UniqueId}")]
public class Light : IItem
{
    public string Name { get; set; } = null!;

    public bool HasPartner { get; set; } = false;

    public string UniqueId { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int ControllerId { get; set; }

    public int GpioPin { get; set; }

    public int GpioButton { get; set; }

    public string StateOn { get; set; } = "on";

    public string StateOff { get; set; } = "off";

    public string SwitchMode { get; set; } = "button";

    public string CommandTopic { get; set; } = "no_topic/command";

    public string StateTopic { get; set; } = "no_topic/state";

    public string PayloadOn { get; set; } = "on";

    public string PayloadOff { get; set; } = "off";
}