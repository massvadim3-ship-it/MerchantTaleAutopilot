using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using MerchantTaleAutopilot.Data;

namespace MerchantTaleAutopilot.Windows;

public sealed class MainWindow : Window, IDisposable
{
    private static readonly Vector4 ColGold = new(0.79f, 0.66f, 0.42f, 1.00f);
    private static readonly Vector4 ColMuted = new(0.55f, 0.58f, 0.64f, 1.00f);
    private static readonly Vector4 ColBody = new(0.86f, 0.88f, 0.92f, 1.00f);
    private static readonly Vector4 ColDone = new(0.42f, 0.45f, 0.50f, 1.00f);
    private static readonly Vector4 ColTeal = new(0.31f, 0.70f, 0.65f, 1.00f);
    private static readonly Vector4 ColBlue = new(0.42f, 0.62f, 0.85f, 1.00f);
    private static readonly Vector4 ColRed = new(0.88f, 0.55f, 0.51f, 1.00f);
    private static readonly Vector4 ColPurple = new(0.61f, 0.50f, 0.83f, 1.00f);

    private readonly Plugin plugin;

    public MainWindow(Plugin plugin)
        : base("Merchant's Tale Autopilot###MtaMain")
    {
        this.plugin = plugin;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(680, 420),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue),
        };

        Size = new Vector2(900, 620);
        SizeCondition = ImGuiCond.FirstUseEver;
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        DrawHeader();
        ImGui.Separator();

        if (ImGui.BeginTabBar("##mtaTabs"))
        {
            if (ImGui.BeginTabItem("Routes"))
            {
                DrawRoutesTab();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Reference"))
            {
                DrawReferenceTab();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    // ---------------------------------------------------------------- header

    private void DrawHeader()
    {
        var done = RouteData.Records.Count(r => IsRecordDone(r.Id));

        ImGui.TextColored(ColGold, $"{done} / 13 records");
        ImGui.SameLine();
        ImGui.TextDisabled("|");
        ImGui.SameLine();
        ImGui.TextDisabled("Space-free zone: tick a step when it is done, or /mta next");
        ImGui.Spacing();
        ImGui.ProgressBar(done / 13f, new Vector2(-1, 6), string.Empty);
    }

    // ----------------------------------------------------------------- routes

    private void DrawRoutesTab()
    {
        var cfg = plugin.Configuration;
        var avail = ImGui.GetContentRegionAvail();

        if (!cfg.CompactMode)
        {
            ImGui.BeginChild("##recordList", new Vector2(252, avail.Y), true);

            foreach (var (groupName, ids) in RouteData.Groups)
            {
                ImGui.TextDisabled(groupName);
                foreach (var id in ids)
                {
                    var rec = RouteData.Get(id);
                    var done = IsRecordDone(id);
                    var label = $"{(done ? "[x]" : "[  ]")} {id:00}  {rec.Title}";

                    if (ImGui.Selectable(label, cfg.SelectedRecord == id))
                    {
                        cfg.SelectedRecord = id;
                        cfg.Save();
                    }
                }

                ImGui.Spacing();
            }

            ImGui.EndChild();
            ImGui.SameLine();
        }

        ImGui.BeginChild("##recordDetail", new Vector2(0, avail.Y), false);
        DrawRecord(RouteData.Get(cfg.SelectedRecord));
        ImGui.EndChild();
    }

    private void DrawRecord(RecordData rec)
    {
        var cfg = plugin.Configuration;

        ImGui.TextColored(ColMuted, $"RECORD {rec.Id:00} - {rec.Route}");
        ImGui.TextColored(ColGold, rec.Title);
        ImGui.Spacing();

        ImGui.PushTextWrapPos(0f);
        ImGui.TextColored(ColMuted, rec.Brief);
        ImGui.PopTextWrapPos();

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        var current = CurrentIndex(rec);

        for (var i = 0; i < rec.Steps.Length; i++)
        {
            var step = rec.Steps[i];
            var key = Configuration.StepKey(rec.Id, i);
            var done = cfg.CompletedSteps.Contains(key);
            var isCurrent = i == current && !done;

            ImGui.PushID(i);

            var textColor = done ? ColDone : (isCurrent ? ColGold : ColBody);
            ImGui.PushStyleColor(ImGuiCol.Text, textColor);

            var ticked = done;
            if (ImGui.Checkbox("##done", ref ticked))
            {
                if (ticked)
                {
                    cfg.CompletedSteps.Add(key);
                }
                else
                {
                    cfg.CompletedSteps.Remove(key);
                }

                cfg.Save();
            }

            ImGui.SameLine();
            ImGui.PopStyleColor();

            ImGui.TextColored(TagColor(step.Kind), TagLabel(step.Kind));
            ImGui.SameLine();

            ImGui.PushStyleColor(ImGuiCol.Text, textColor);
            ImGui.PushTextWrapPos(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X);
            ImGui.TextWrapped(step.Text);
            ImGui.PopTextWrapPos();
            ImGui.PopStyleColor();

            ImGui.PopID();
            ImGui.Spacing();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        if (ImGui.Button("Complete step", new Vector2(150, 0)))
        {
            CompleteCurrentStep();
        }

        ImGui.SameLine();
        if (ImGui.Button("Mark record done", new Vector2(150, 0)))
        {
            MarkRecordDone(rec.Id);
        }

        ImGui.SameLine();
        if (ImGui.Button("Reset record", new Vector2(150, 0)))
        {
            ResetRecord(rec.Id);
        }
    }

    // -------------------------------------------------------------- reference

    private void DrawReferenceTab()
    {
        ImGui.TextColored(ColGold, "Unlock");
        ImGui.PushTextWrapPos(0f);
        ImGui.TextWrapped("Finish the Endwalker main scenario, then speak with Osmon in Old Sharlayan (X:11.9, Y:13.3). " +
                          "Take the sidequest \"A Spellbinding Read\" from Shallow Moor at (X:12, Y:13.5) and clear the dungeon once. " +
                          "Afterwards re-queue from Duty -> V&C Dungeon Finder.");
        ImGui.TextColored(ColRed, "The duty syncs everyone to Lv 100 / item level 765 and ignores materia. Fill every gear slot.");
        ImGui.PopTextWrapPos();

        ImGui.Spacing();
        ImGui.TextColored(ColGold, "Route split");
        if (ImGui.BeginTable("##split", 3, ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("Route", ImGuiTableColumnFlags.WidthFixed, 150);
            ImGui.TableSetupColumn("First fork", ImGuiTableColumnFlags.WidthFixed, 190);
            ImGui.TableSetupColumn("Records / bosses");
            ImGui.TableHeadersRow();

            Row("Left - Residential", "Apple / non-apple", "1-4 | Genie or Rukhkh -> Pari of Plenty");
            Row("Right - Market", "Seaweed / beach walk", "5-8 (+13) | Genie -> Darya the Sea-maid");
            Row("Middle - Gate", "Mushrooms / float rocks", "9-12 | Rukhkh -> Lone Swordmaster");

            ImGui.EndTable();
        }

        ImGui.Spacing();
        ImGui.TextColored(ColGold, "Boss mechanics");

        Collapsing("Genie of the Lamp",
            "Fabulous Firecrackers: glowing arm = half-room cleave, dull arm = 60 degree cone. Stand on the dull side.",
            "Spectacular Sparks: two turret sets alternate line AoEs. Start in front of the second set, rotate to the first after it fires.",
            "A Sailor's Tale + Chart Course: three airship lanes, the switch forces an outer lane, one safe spot remains.",
            "Fanning Flame: two alternating cone sets, start on the second and rotate to the first.");

        Collapsing("Rukhkh",
            "Sphere of Sand / Sandplume / Sand Burst: three spheres in a triangle, cones decide which grow, grown ones explode wider.",
            "Banishing Mist: boss or spheres go invisible, track positions before they vanish.",
            "Biting Scratch: huge frontal cone, move to flank or rear.");

        Collapsing("Pari of Plenty",
            "Bleed border appears early, do not hug the edge.",
            "Fireflight + Sun Circlet: dashes three lines then a full ring AoE, follow the dash end point.",
            "Left/Right Fireflight: rotating half-room cleave. Second arrow matches the first -> swap sides. Inverted colour -> stay.",
            "Charming Baubles: cross AoEs from red gems, usually layered with something else.");

        Collapsing("Darya the Sea-maid",
            "Fall-off border at the start.",
            "Echoed Serenade: sheet music shows add firing order, memorise it before it fades.",
            "Swimming in the Air: seven crystals detonate at once, two corners are safe.",
            "Receding / Encroaching Twin Tides: close-then-ring or ring-then-close, second hit has no telegraph.");

        Collapsing("Lone Swordmaster",
            "Malefic Quartering: a debuff marks one (later two) quadrants around you as vulnerable.",
            "Will of the Underworld: half-room cleave, keep your red quadrant away from the origin.",
            "Crusher of Lions: four 90 degree AoEs from centre, same logic.",
            "Unyielding Will: L-shaped AoE per player, step through the telegraph to flip it if it clips your weak side.");

        Collapsing("Deadly Dandan (secret)",
            "Devour: two charge lines rotate then dash, being eaten kills you on the later Spit.",
            "Stinging Tentacle: two spinning line AoEs, read the spin and pre-position at the stop point.",
            "Tidal Guillotine: boss dives under and erupts, underwater position marks the centre.",
            "Strewn Bubbles: bind on contact, then a 6s Bubble Gaol and a 10s fall-off.");

        ImGui.Spacing();
        ImGui.TextColored(ColGold, "Rewards");
        ImGui.PushTextWrapPos(0f);
        ImGui.TextWrapped("All 13 records -> achievement \"A Storied Collection\" -> The Faces We Wear - Comfortable Eye Mask. " +
                          "No mount from this dungeon. Potsherds exchange at Trisassant, Old Sharlayan (X:12, Y:13.4): " +
                          "glamour pieces 6 / 9 / 12 each, Saving Face and the Worthy Pursuits roll 9 each.");
        ImGui.TextDisabled("Advanced: Memolivia (11.9, 13.3), Lv100 i760 boss rush. Criterion: Osmon, after clearing all three bosses in one Advanced run.");
        ImGui.PopTextWrapPos();
    }

    private static void Row(string a, string b, string c)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(a);
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(b);
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(c);
    }

    private static void Collapsing(string title, params string[] lines)
    {
        if (!ImGui.CollapsingHeader(title))
        {
            return;
        }

        ImGui.Indent();
        ImGui.PushTextWrapPos(0f);
        foreach (var line in lines)
        {
            ImGui.Bullet();
            ImGui.SameLine();
            ImGui.TextWrapped(line);
        }

        ImGui.PopTextWrapPos();
        ImGui.Unindent();
    }

    // ---------------------------------------------------------------- helpers

    private static string TagLabel(StepKind kind)
    {
        return kind switch
        {
            StepKind.Pick => "PICK",
            StepKind.Do => "DO",
            StepKind.Fight => "FIGHT",
            StepKind.Avoid => "AVOID",
            StepKind.Talk => "TALK",
            _ => "FINISH",
        };
    }

    private static Vector4 TagColor(StepKind kind)
    {
        return kind switch
        {
            StepKind.Pick => ColGold,
            StepKind.Do => ColBlue,
            StepKind.Fight => ColRed,
            StepKind.Avoid => ColRed,
            StepKind.Talk => ColPurple,
            _ => ColTeal,
        };
    }

    public bool IsStepDone(int recordId, int index)
    {
        return plugin.Configuration.CompletedSteps.Contains(Configuration.StepKey(recordId, index));
    }

    public bool IsRecordDone(int recordId)
    {
        var cfg = plugin.Configuration;
        if (cfg.CompletedRecords.Contains(recordId))
        {
            return true;
        }

        var rec = RouteData.Get(recordId);
        for (var i = 0; i < rec.Steps.Length; i++)
        {
            if (!IsStepDone(recordId, i))
            {
                return false;
            }
        }

        return true;
    }

    private int CurrentIndex(RecordData rec)
    {
        for (var i = 0; i < rec.Steps.Length; i++)
        {
            if (!IsStepDone(rec.Id, i))
            {
                return i;
            }
        }

        return rec.Steps.Length - 1;
    }

    public void CompleteCurrentStep()
    {
        var cfg = plugin.Configuration;
        var rec = RouteData.Get(cfg.SelectedRecord);
        var index = CurrentIndex(rec);

        if (IsStepDone(rec.Id, index))
        {
            // Everything ticked - jump to the next record that still has work.
            var next = RouteData.Records.FirstOrDefault(r => !IsRecordDone(r.Id));
            if (next != null)
            {
                cfg.SelectedRecord = next.Id;
                cfg.Save();
            }

            return;
        }

        cfg.CompletedSteps.Add(Configuration.StepKey(rec.Id, index));
        cfg.Save();
    }

    public void ResetCurrentRecord()
    {
        ResetRecord(plugin.Configuration.SelectedRecord);
    }

    private void ResetRecord(int recordId)
    {
        var cfg = plugin.Configuration;
        var rec = RouteData.Get(recordId);

        for (var i = 0; i < rec.Steps.Length; i++)
        {
            cfg.CompletedSteps.Remove(Configuration.StepKey(recordId, i));
        }

        cfg.CompletedRecords.Remove(recordId);
        cfg.Save();
    }

    private void MarkRecordDone(int recordId)
    {
        var cfg = plugin.Configuration;
        var rec = RouteData.Get(recordId);

        for (var i = 0; i < rec.Steps.Length; i++)
        {
            cfg.CompletedSteps.Add(Configuration.StepKey(recordId, i));
        }

        cfg.CompletedRecords.Add(recordId);
        cfg.Save();
    }
}
