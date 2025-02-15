using System.Diagnostics;
using YamlDotNet.Serialization;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models
{
    namespace Mqtt
    {
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
            public string StateTopic { get; set; } = "_no_topic/state";

            [YamlMember(Alias = "set_position_topic")]
            public string SetPositionTopic { get; set; } = "_no_topic/command/pos";

            [YamlMember(Alias = "position_topic")]
            public string PositionTopic { get; set; } = "_no_topic/state/pos";

            [YamlMember(Alias = "payload_open")]
            public required string PayloadOpen { get; set; } = "_no_payload"!;

            [YamlMember(Alias = "payload_close")]
            public required string PayloadClose { get; set; } = "_no_payload"!;

            [YamlMember(Alias = "payload_stop")]
            public required string PayloadStop { get; set; } = "_no_payload"!;

            [YamlMember(Alias = "position_open")]
            public int PositionOpen { get; set; } = 0;

            [YamlMember(Alias = "position_closed")]
            public int PositionClosed { get; set; } = 100;

            [YamlMember(Alias = "state_open")]
            public string StateOpen { get; set; } = "_no_state";

            [YamlMember(Alias = "state_opening")]
            public string StateOpening { get; set; } = "_no_state";

            [YamlMember(Alias = "state_closed")]
            public string StateClosed { get; set; } = "_no_state";

            [YamlMember(Alias = "state_closing")]
            public string StateClosing { get; set; } = "_no_state";

            [YamlMember(Alias = "state_stopped")]
            public string StateStopped { get; set; } = "_no_state";

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
            public required string CommandTopic { get; set; } = "_no_topic/command";

            [YamlMember(Alias = "state_topic")]
            public string StateTopic { get; set; } = "_no_topic/state";

            [YamlMember(Alias = "payload_on")]
            public required string PayloadOn { get; set; } = "_no_payload";

            [YamlMember(Alias = "payload_off")]
            public required string PayloadOff { get; set; } = "_no_payload";

            [YamlMember(Alias = "optimistic")]
            public bool Optimistic { get; set; } = false;

            [YamlMember(Alias = "unique_id")]
            public required string UniqueId { get; set; } = null!;
        }
    }
}