using HomeNet = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.HomeNet;
using Mqtt = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Mqtt;
using Merge = mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models.Merge;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;

public static class HomeNetConfigExtensions
{
    public static HomeNet.Config GetByControllerId(this HomeNet.Config config, int controllerId)
    {
        return new HomeNet.Config()
        {
            Covers = config.Covers.Where(c => c.ControllerId == controllerId).ToList(),
            Lights = config.Lights.Where(l => l.ControllerId == controllerId).ToList(),
        };
    }

    public static Merge.Config MergeMqttConfig(this HomeNet.Config homeNetConfig, Mqtt.Config mqttConfig)
    {
        var mergedCovers = new List<Merge.Cover>();
        foreach (var homeNetCover in homeNetConfig.Covers)
        {
            var merged = new Merge.Cover
            {
                UniqueId = homeNetCover.UniqueId,
                HasPartner = false,
                Description = homeNetCover.Description,
                RunningTime = homeNetCover.RunningTime,
                GpioClose = homeNetCover.GpioClose,
                GpioOpen = homeNetCover.GpioOpen,
                GpioCloseButton = homeNetCover.GpioCloseButton,
                GpioOpenButton = homeNetCover.GpioOpenButton,
                ControllerId = homeNetCover.ControllerId,
            };

            mergedCovers.Add(merged);

            var mqttCover = mqttConfig.Covers.SingleOrDefault(h => h.UniqueId == homeNetCover.UniqueId);

            if (mqttCover == null)
            {
                merged.Name = homeNetCover.UniqueId;
                merged.HasPartner = false;
                continue;
            }
            merged.Name = mqttCover.Name;
            merged.HasPartner = true;
            merged.CommandTopic = mqttCover.CommandTopic;
            merged.StateTopic = mqttCover.StateTopic;
            merged.SetPositionTopic = mqttCover.SetPositionTopic;
            merged.PositionTopic = mqttCover.PositionTopic;
            merged.PayloadOpen = mqttCover.PayloadOpen;
            merged.PayloadClose = mqttCover.PayloadClose;
            merged.PayloadStop = mqttCover.PayloadStop;
            merged.PositionOpen = mqttCover.PositionOpen;
            merged.PositionClosed = mqttCover.PositionClosed;
            merged.StateOpen = mqttCover.StateOpen;
            merged.StateOpening = mqttCover.StateOpening;
            merged.StateClosed = mqttCover.StateClosed;
            merged.StateClosing = mqttCover.StateClosing;
            merged.StateStopped = mqttCover.StateStopped;
        }

        var mergedLights = new List<Merge.Light>();
        foreach (var homeNetLight in homeNetConfig.Lights)
        {
            var merged = new Merge.Light
            {
                UniqueId = homeNetLight.UniqueId,
                HasPartner = false,
                GpioButton = homeNetLight.GpioButton,
                GpioPin = homeNetLight.GpioPin,
                Description = homeNetLight.Description,
                ControllerId = homeNetLight.ControllerId,
                StateOn = homeNetLight.StateOn,
                StateOff = homeNetLight.StateOff,
            };

            mergedLights.Add(merged);

            var mqttLight = mqttConfig.Lights.SingleOrDefault(h => h.UniqueId == homeNetLight.UniqueId);

            if (mqttLight == null)
            {
                merged.Name = homeNetLight.UniqueId;
                merged.HasPartner = false;
                continue;
            }
            merged.Name = mqttLight.Name;
            merged.HasPartner = true;
            merged.CommandTopic = mqttLight.CommandTopic;
            merged.StateTopic = mqttLight.StateTopic;
            merged.PayloadOn = mqttLight.PayloadOn;
            merged.PayloadOff = mqttLight.PayloadOff;
        }

        return new Merge.Config()
        {
            Covers = mergedCovers,
            Lights = mergedLights,
        };
    }
}