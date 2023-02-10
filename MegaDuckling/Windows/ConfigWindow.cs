using System;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace MegaDuckling.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    public ConfigWindow(MegaDuckling plugin) : base(
        "Mega Duckling Config",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var ModelIDValue = this.Configuration.ModelID;
        var ModelScaleValue = this.Configuration.ModelScale;
        var ZoneChangeWaitTimeInSeconds = this.Configuration.ZoneChangeWaitTimeInSeconds;
        var LoginWaitTimeInSeconds = this.Configuration.LoginWaitTimeInSeconds;

        ImGui.Text("Model ID to enlarge:");
        if (ImGui.InputInt("##Object Model ID", ref ModelIDValue, 0))
        {
            this.Configuration.ModelID = ModelIDValue;
            this.Configuration.Save();
        }
        ImGui.Spacing();

        ImGui.Text("Zone Change Delay:");
        if (ImGui.InputFloat("##ZoneDelayValue", ref ZoneChangeWaitTimeInSeconds))
        {
            this.Configuration.ZoneChangeWaitTimeInSeconds = ZoneChangeWaitTimeInSeconds;
            this.Configuration.Save();
        }
        ImGui.Spacing();

        ImGui.Text("Login Delay:");
        if (ImGui.InputFloat("##LoginDelayValue", ref LoginWaitTimeInSeconds))
        {
            this.Configuration.LoginWaitTimeInSeconds = LoginWaitTimeInSeconds;
            this.Configuration.Save();
        }
        ImGui.Spacing();

        ImGui.Text("Multiplier:");
        if (ImGui.InputFloat("##MultiplierFloat", ref ModelScaleValue))
        {
            if (ModelScaleValue >= 0.1)
            {
                this.Configuration.ModelScale = ModelScaleValue;
                this.Configuration.Save();
            }
            else if (ModelScaleValue <= 0)
            {
                this.Configuration.ModelScale = 0.1f;
                this.Configuration.Save();
            }
        }

        if (ImGui.Button("Run Enlarger Manually"))
        {
            MegaDuckling.Plugin.DuckyEnlarger(null, 0);
        }
    }
}
