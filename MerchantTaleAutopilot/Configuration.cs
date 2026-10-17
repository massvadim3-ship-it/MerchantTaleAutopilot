using System;
using System.Collections.Generic;
using Dalamud.Configuration;

namespace MerchantTaleAutopilot;

[Serializable]
public sealed class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    /// <summary>Currently selected record id (1-13).</summary>
    public int SelectedRecord { get; set; } = 1;

    /// <summary>Completed steps, keyed as "recordId:stepIndex".</summary>
    public HashSet<string> CompletedSteps { get; set; } = new();

    /// <summary>Records marked as fully cleared.</summary>
    public HashSet<int> CompletedRecords { get; set; } = new();

    /// <summary>Open the window automatically when entering one of the tracked territories.</summary>
    public bool AutoOpen { get; set; } = true;

    /// <summary>Territory ids that count as "inside The Merchant's Tale". Add yours with /mta add.</summary>
    public HashSet<uint> AutoOpenTerritories { get; set; } = new();

    /// <summary>Compact mode: hide the record list and show only the active record.</summary>
    public bool CompactMode { get; set; }

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }

    public static string StepKey(int recordId, int stepIndex)
    {
        return recordId + ":" + stepIndex;
    }
}
