using System;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using MerchantTaleAutopilot.Windows;

namespace MerchantTaleAutopilot;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/mta";

    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    private readonly ICommandManager commandManager;
    private readonly IClientState clientState;
    private readonly IChatGui chatGui;
    private readonly IFramework framework;
    private readonly IPluginLog log;

    private uint lastTerritory;

    public Configuration Configuration { get; }
    public WindowSystem WindowSystem { get; } = new("MerchantTaleAutopilot");

    public MainWindow MainWindow { get; }
    public ConfigWindow ConfigWindow { get; }

    public string Name => "Merchant's Tale Autopilot";

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        IClientState clientState,
        IChatGui chatGui,
        IFramework framework,
        IPluginLog log)
    {
        PluginInterface = pluginInterface;
        this.commandManager = commandManager;
        this.clientState = clientState;
        this.chatGui = chatGui;
        this.framework = framework;
        this.log = log;

        Configuration = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        MainWindow = new MainWindow(this);
        ConfigWindow = new ConfigWindow(this);

        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(ConfigWindow);

        commandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Toggle the route window. Subcommands: next, reset, id, add, auto, config",
            ShowInHelp = true,
        });

        pluginInterface.UiBuilder.Draw += WindowSystem.Draw;
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        lastTerritory = clientState.TerritoryType;
        framework.Update += OnFrameworkUpdate;

        log.Information("[MTA] loaded");
    }

    public void Dispose()
    {
        framework.Update -= OnFrameworkUpdate;
        commandManager.RemoveHandler(CommandName);

        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;

        WindowSystem.RemoveAllWindows();
        MainWindow.Dispose();
        ConfigWindow.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        switch (args.Trim().ToLowerInvariant())
        {
            case "":
            case "toggle":
                MainWindow.Toggle();
                break;

            case "next":
                MainWindow.CompleteCurrentStep();
                break;

            case "reset":
                MainWindow.ResetCurrentRecord();
                break;

            case "id":
                chatGui.Print($"[MTA] Current territory id: {CurrentTerritoryId()}");
                break;

            case "add":
                AddCurrentTerritory();
                break;

            case "auto":
                Configuration.AutoOpen = !Configuration.AutoOpen;
                Configuration.Save();
                chatGui.Print($"[MTA] Auto-open is now {(Configuration.AutoOpen ? "on" : "off")}.");
                break;

            case "config":
                ConfigWindow.Toggle();
                break;

            default:
                chatGui.Print("[MTA] Unknown subcommand. Try: next, reset, id, add, auto, config");
                break;
        }
    }

    public uint CurrentTerritoryId()
    {
        return clientState.TerritoryType;
    }

    public void AddCurrentTerritory()
    {
        var id = CurrentTerritoryId();
        if (Configuration.AutoOpenTerritories.Add(id))
        {
            Configuration.Save();
            chatGui.Print($"[MTA] Added territory {id} to the auto-open list.");
        }
        else
        {
            chatGui.Print($"[MTA] Territory {id} is already tracked.");
        }
    }

    /// <summary>
    /// Polled rather than event-driven on purpose: the TerritoryChanged delegate signature
    /// has changed between Dalamud API levels, polling cannot break on that.
    /// </summary>
    private void OnFrameworkUpdate(IFramework fw)
    {
        var id = CurrentTerritoryId();
        if (id == lastTerritory)
        {
            return;
        }

        lastTerritory = id;

        if (Configuration.AutoOpen && Configuration.AutoOpenTerritories.Contains(id))
        {
            MainWindow.IsOpen = true;
        }
    }

    private void ToggleMainUi()
    {
        MainWindow.Toggle();
    }

    private void ToggleConfigUi()
    {
        ConfigWindow.Toggle();
    }
}
