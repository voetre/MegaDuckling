using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using MegaDuckling.Windows;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;

namespace MegaDuckling
{
    public class MegaDuckling : IDalamudPlugin
    {
        public string Name => "Mega Duckling";
        private const string DuckyConfigCommand = "/duckyconfig";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("MegaDuckling");
        public static MegaDuckling Plugin { get; private set; }

        public MegaDuckling(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            DalamudApi.Initialize(this, pluginInterface);

            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);
            WindowSystem.AddWindow(new ConfigWindow(this));
            Plugin = this;

            this.CommandManager.AddHandler(DuckyConfigCommand, new CommandInfo(DuckyConfig)
            {
                HelpMessage = "Open the Mega Ducky config."
            });

            DalamudApi.ClientState.TerritoryChanged += DuckyEnlarger;
            DalamudApi.ClientState.Login += LoginEventHandler;
            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            DuckyEnlarger(null, 0);
        }

        private void LoginEventHandler(object sender, EventArgs args)
        {
            DalamudApi.Framework.RunOnTick(() =>
            {
                Plugin.DuckyEnlarger(null, 0);
            },
            TimeSpan.FromSeconds(this.Configuration.LoginWaitTimeInSeconds));
        }

        public unsafe void DuckyEnlarger(object sender, ushort e)
        {
            DalamudApi.Framework.RunOnTick(() => {
                foreach (var o in DalamudApi.ObjectTable)
                {
                    if (o is PlayerCharacter) continue;
                    var obj = (GameObject*)o.Address;
                    var d = (Character*)o.Address;
                    if (d->ModelCharaId == this.Configuration.ModelID)
                    {
                        obj->Scale = this.Configuration.ModelScale;
                        d->ModelScale = this.Configuration.ModelScale;
                        obj->RenderFlags = 2;
                        if (Dalamud.SafeMemory.ReadString(o.Address + 48) == "Ugly Duckling")
                        {
                            Dalamud.SafeMemory.WriteString(o.Address + 48, "Mega Duckling");
                        }
                        DalamudApi.Framework.RunOnTick(() => obj->RenderFlags = 0, default, 2);
                        return;
                    }
                }
            }
            , TimeSpan.FromSeconds(this.Configuration.ZoneChangeWaitTimeInSeconds));
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            DalamudApi.ClientState.TerritoryChanged -= DuckyEnlarger;
            DalamudApi.ClientState.Login -= LoginEventHandler;
            this.PluginInterface.UiBuilder.Draw -= DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;
            this.CommandManager.RemoveHandler(DuckyConfigCommand);
        }

        private void DuckyConfig(string command, string args)
        {
            WindowSystem.GetWindow("Mega Duckling Config").IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            WindowSystem.GetWindow("Mega Duckling Config").IsOpen = true;
        }
    }
}
