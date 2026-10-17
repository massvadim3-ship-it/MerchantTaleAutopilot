# Merchant's Tale Autopilot

A Dalamud plugin that turns **The Merchant's Tale** variant dungeon (FFXIV 7.45) into a checklist you can follow in-game.

13 Corvosi Folklore Records, each broken into ordered steps. Pick a record, do the highlighted step, tick it off. Progress persists between sessions.

It does **not** move your character, press buttons, or fight. It is a notepad with better memory.

---

## Install

The plugin is distributed through a custom Dalamud repository.

1. In game, run `/xlsettings`
2. Go to **Experimental** -> **Custom Plugin Repositories**
3. Paste this URL and press `+`:

   ```
   https://raw.githubusercontent.com/massvadim3-ship-it/MerchantTaleAutopilot/main/repo.json
   ```

4. Save, then `/xlplugins` -> search **Merchant's Tale Autopilot** -> Install

Until a release is published, install it as a dev plugin instead (see [Build](#build)).

## Usage

| Command | Effect |
|---|---|
| `/mta` | Toggle the window |
| `/mta next` | Tick the current step (rolls over to the next uncleared record) |
| `/mta reset` | Reset the selected record |
| `/mta id` | Print the current territory id |
| `/mta add` | Add the current territory to the auto-open list |
| `/mta auto` | Toggle auto-open on/off |
| `/mta config` | Open settings |

**Auto-open:** territory ids are not hardcoded, because guessing them is how plugins break on patch day. Stand inside the dungeon once, run `/mta add`, and from then on the window pops up whenever you zone in.

**Progress** is stored in `pluginConfigs/MerchantTaleAutopilot.json` and is account-wide, not per character. "Clear all progress" in settings wipes all 13 records.

## Routes

| Route | First fork | Records | Bosses |
|---|---|---|---|
| Left - Residential | Apple / non-apple | 1-4 | Genie of the Lamp or Rukhkh -> Pari of Plenty |
| Right - Market | Seaweed / beach walk | 5-8 (+ secret 13) | Genie of the Lamp -> Darya the Sea-maid |
| Middle - Gate | Mushrooms / floating rocks | 9-12 | Rukhkh -> Lone Swordmaster |

The secret record (13) needs the ritual: right route, seaweed, turn right into the Shimmering Stage, `/bow`, say `I am returned!` in Say chat, then step the panels Coral -> Conch -> Starfish -> Pearl. All of it before crossing the Tangled Seaweed Barrier.

## Build

Requirements: **.NET 10 SDK** and Dalamud dev libraries at `%APPDATA%\XIVLauncher\addon\Hooks\dev\`.

```bash
git clone https://github.com/massvadim3-ship-it/MerchantTaleAutopilot.git
cd MerchantTaleAutopilot
dotnet build -c Release
```

Output lands in `MerchantTaleAutopilot/bin/Release/net10.0-windows/`. If your dev libs live elsewhere:

```bash
dotnet build -c Release -p:DalamudLibPath="D:\dev\dalamud\"
```

### Load as a dev plugin

1. `/xlsettings` -> **Experimental** -> **Dev Plugin Locations** -> add the full path to `bin/Release/net10.0-windows/`
2. `/xlplugins` -> **Dev Tools** -> **Load Plugin** -> *Merchant's Tale Autopilot*

## Release

Tag and push; the GitHub Action builds, packages `latest.zip` and attaches it to the release.

```bash
git tag v1.0.0
git push origin v1.0.0
```

Bump `AssemblyVersion` in `MerchantTaleAutopilot/MerchantTaleAutopilot.json` and `repo.json` before tagging, otherwise Dalamud will not offer the update.

## Project layout

```
MerchantTaleAutopilot/
  MerchantTaleAutopilot.csproj
  MerchantTaleAutopilot.json     plugin manifest
  Plugin.cs                      entry point, commands, territory watcher
  Configuration.cs               saved progress
  Data/Records.cs                the 13 records and their steps
  Windows/MainWindow.cs          routes + reference UI
  Windows/ConfigWindow.cs        settings
repo.json                        custom repository manifest
.github/workflows/build.yml      CI: build, package, release on tag
```

## Troubleshooting

**Dalamud refuses to load it.** The manifest declares `"DalamudApiLevel": 15`, matching Dalamud 15 (FFXIV 7.45). Check yours with `/xldev` -> **Dalamud API Level** and set the same number in both JSON files.

**`CS1705: Dalamud uses System.Runtime 10.0.0.0`.** You are building against .NET 9. The plugin targets `net10.0-windows`; install the .NET 10 SDK or let `setup-dotnet` pull `10.0.x`.

**`CS0246: ImGuiNET could not be found`.** Dalamud 15 dropped the old ImGui.NET binding. The plugin already uses the replacement:

```csharp
using Dalamud.Bindings.ImGui;
```

If you add new UI code, import that namespace, not `ImGuiNET`.

## Credits

Route data compiled from community guides for patch 7.45. FFXIV and all related assets are property of Square Enix.

## License

MIT - see [LICENSE](LICENSE).
