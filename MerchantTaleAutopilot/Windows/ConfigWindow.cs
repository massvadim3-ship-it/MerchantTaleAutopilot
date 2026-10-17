using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace MerchantTaleAutopilot.Windows;

public sealed class ConfigWindow : Window, IDisposable
{
    private static readonly Vector4 ColGold = new(0.79f, 0.66f, 0.42f, 1.00f);
    private static readonly Vector4 ColMuted = new(0.55f, 0.58f, 0.64f, 1.00f);

    private readonly Plugin plugin;

    public ConfigWindow(Plugin plugin)
        : base("Merchant's Tale Autopilot - Settings###MtaConfig")
    {
        this.plugin = plugin;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(420, 300),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };

        Size = new Vector2(520, 420);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        var cfg = plugin.Configuration;

        ImGui.TextColored(ColGold, "Window");
        ImGui.Spacing();

        var compact = cfg.CompactMode;
        if (ImGui.Checkbox("Compact mode (hide the record list)", ref compact))
        {
            cfg.CompactMode = compact;
            cfg.Save();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        ImGui.TextColored(ColGold, "Auto-open");
        ImGui.Spacing();

        var auto = cfg.AutoOpen;
        if (ImGui.Checkbox("Open the window when entering a tracked territory", ref auto))
        {
            cfg.AutoOpen = auto;
            cfg.Save();
        }

        ImGui.TextColored(ColMuted, $"Current territory: {plugin.CurrentTerritoryId()}");

        if (ImGui.Button("Add current territory"))
        {
            plugin.AddCurrentTerritory();
        }

        ImGui.Spacing();

        if (cfg.AutoOpenTerritories.Count == 0)
        {
            ImGui.TextDisabled("No territories tracked yet. Stand inside the dungeon and press the button above.");
        }
        else
        {
            ImGui.TextDisabled("Tracked territories:");
            foreach (var id in cfg.AutoOpenTerritories.ToArray())
            {
                ImGui.PushID((int)id);
                ImGui.TextUnformatted(id.ToString());
                ImGui.SameLine();
                if (ImGui.SmallButton("remove"))
                {
                    cfg.AutoOpenTerritories.Remove(id);
                    cfg.Save();
                }

                ImGui.PopID();
            }
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        ImGui.TextColored(ColGold, "Progress");
        ImGui.Spacing();

        if (ImGui.Button("Clear all progress"))
        {
            cfg.CompletedSteps.Clear();
            cfg.CompletedRecords.Clear();
            cfg.SelectedRecord = 1;
            cfg.Save();
        }

        ImGui.SameLine();
        ImGui.TextDisabled($"{cfg.CompletedSteps.Count} steps ticked");

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        ImGui.TextColored(ColMuted, "This plugin only displays a checklist.");
        ImGui.TextColored(ColMuted, "It never sends input, moves your character, or acts in combat.");
    }
}
