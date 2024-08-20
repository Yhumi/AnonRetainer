using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Interface.Style;
using OtterGui.Classes;
using ECommons.Automation.LegacyTaskManager;
using ECommons;
using ECommons.DalamudServices;
using RetainerAnonymiser.UI;
using RetainerAnonymiser.RetainerAddon;

namespace RetainerAnonymiser;

public sealed class RetainerAnonymiser : IDalamudPlugin
{
    public string Name => "AnonRetainer";
    private const string CommandName = "/anonretainer";
    private const int CurrentConfigVersion = 1;

    internal static RetainerAnonymiser P = null!;
    internal PluginUI PluginUi;
    internal WindowSystem ws;
    internal Configuration Config;
    internal TaskManager TM;
    internal TextureCache Icons;

    internal StyleModel Style;
    internal bool StylePushed = false;

    public RetainerAnonymiser(IDalamudPluginInterface pluginInterface)
    {
        ECommonsMain.Init(pluginInterface, this, Module.All);
        P = this;

        //LuminaSheets.Init();
        P.Config = Configuration.Load();
        TM = new();
        TM.TimeLimitMS = 1000;

        if (P.Config.Version != CurrentConfigVersion)
        {
            //P.Config.UpdateConfig();
        }

        ws = new();
        Icons = new(Svc.Data, Svc.Texture);
        Config = P.Config;
        PluginUi = new();

        Svc.Commands.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the AnonRetainer menu.\n" +
            "/anonretainer settings → Opens settings.",
            ShowInHelp = true,
        });

        Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
        Svc.PluginInterface.UiBuilder.OpenConfigUi += DrawSettingsUI;
        Svc.PluginInterface.UiBuilder.OpenMainUi += DrawMainUI;

        Svc.ClientState.Login += OnClientLogin;
        Svc.ClientState.Logout += OnClientLogout;

        Anonymiser.Init();
        if (P.Config.AutomaticallyEnableOnLogin && Svc.ClientState.IsLoggedIn)
            Anonymiser.Enable();

        Svc.Framework.Update += Tick;

        Style = StyleModel.GetFromCurrent()!;
    }

    public void Tick(object _)
    {
        Anonymiser.Tick();
    }

    public void Dispose()
    {
        PluginUi.Dispose();

        Svc.Commands.RemoveHandler(CommandName);
        Svc.PluginInterface.UiBuilder.OpenConfigUi -= DrawSettingsUI;
        Svc.PluginInterface.UiBuilder.Draw -= ws.Draw;
        Svc.PluginInterface.UiBuilder.OpenMainUi -= DrawMainUI;

        Svc.ClientState.Login -= OnClientLogin;
        Svc.ClientState.Logout -= OnClientLogout;

        ws?.RemoveAllWindows();
        ws = null!;

        ECommonsMain.Dispose();
        P = null!;
    }

    private void OnCommand(string command, string args)
    {
        var subcommands = args.Split(' ');

        if (subcommands.Length == 0)
        {
            PluginUi.IsOpen = !PluginUi.IsOpen;
            return;
        }

        var firstArg = subcommands[0];

        // in response to the slash command, just toggle the display status of our main ui
        PluginUi.IsOpen = true;
        PluginUi.OpenWindow = firstArg.ToLower() switch
        {
            "settings" => OpenWindow.Settings,
            _ => OpenWindow.Settings
        };
    }

    private void DrawMainUI()
    {
        PluginUi.OpenWindow = OpenWindow.Settings;
        PluginUi.IsOpen = true;
    }

    private void DrawSettingsUI()
    {
        PluginUi.OpenWindow = OpenWindow.Settings;
        PluginUi.IsOpen = true;
    }

    private void OnClientLogin()
    {
        if (P.Config.AutomaticallyEnableOnLogin)
        {
            Anonymiser.Enable();
        }
    }

    private void OnClientLogout()
    {
        Anonymiser.Disable();
    }
}
