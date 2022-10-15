﻿using HarmonyLib;
using System;
using System.Collections.Generic;

namespace BetterLogicOverlay.LogicSettingDisplay
{
    [HarmonyPatch(typeof(GeneratedBuildings), nameof(GeneratedBuildings.LoadGeneratedBuildings))]
    class ApplySettingsToDefs
    {
        static void Postfix()
        {
            foreach (var def in Assets.BuildingDefs)
            {
                var go = def.BuildingComplete;

                // If there are no logic ports: skip this def.
                // The LogicPorts fields are not reliable; some configs don't set them, but still yield a LogicPorts component.
                if (!go.GetComponent<LogicPorts>() && !go.GetComponent<LogicGateBase>())
                    continue;

                // Add the correct setting component for the building.
                foreach (var (building, setting) in buildingToLabelMap)
                {
                    if (building != null && go.GetComponent(building))
                    {
                        go.AddComponent(setting);
                        break;
                    }
                }
            }
        }

        // Maps a component on the def to the correct type of setting to add.
        // Processed first to last, so interface fallbacks should be after specific implementations.
        static readonly List<(Type building, Type setting)> buildingToLabelMap = new List<(Type, Type)>()
        {
            // IThresholdSwitch
            (typeof(ConduitTemperatureSensor), typeof(ThresholdSwitchSetting.ConduitTemp)),
            (typeof(LogicDiseaseSensor), typeof(ThresholdSwitchSetting.Germs)),
            (typeof(ConduitDiseaseSensor), typeof(ThresholdSwitchSetting.Germs)),
            (typeof(LogicCritterCountSensor), typeof(ThresholdSwitchSetting.CritterCount)),
            (typeof(IThresholdSwitch), typeof(ThresholdSwitchSetting)),

            // ISliderControl
            (AccessTools.TypeByName("WirelessSignalReceiver"), typeof(SliderControlSetting.WirelessSignalRecieverSetting)),
            (AccessTools.TypeByName("WirelessSignalEmitter"), typeof(SliderControlSetting.WirelessSignalEmitterSetting)),
            (typeof(ISliderControl), typeof(SliderControlSetting)),

            // General categories
            (typeof(IActivationRangeTarget), typeof(ActivationRangeTargetSetting)),
            (typeof(ILogicRibbonBitSelector), typeof(LogicRibbonBitSelectorSetting)),

            // Specific buildings
            (typeof(Filterable), typeof(ConduitElementSensorSetting)),
            (typeof(LogicElementSensor), typeof(LogicElementSensorSetting)),
            (typeof(LogicCounter), typeof(LogicCounterSetting)),
            (typeof(LogicTimerSensor), typeof(LogicTimerSensorSetting)),
            (typeof(LogicTimeOfDaySensor), typeof(LogicTimeOfDaySensorSetting)),
            (typeof(LogicBroadcaster), typeof(LogicBroadcasterSetting)),
            (typeof(LogicBroadcastReceiver), typeof(LogicBroadcasterSetting.Receiver)),

            // Specific mod buildings
            (typeof(TreeFilterable), typeof(SolidElementSetting))
        };

    }
}
