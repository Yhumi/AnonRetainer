using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using RetainerAnonymiser.RetainerAddon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetainerAnonymiser.UI.Overlays
{
    internal unsafe class AnonymiserOverlay : Window
    {
        internal float Height;
        internal float Width;

        internal bool Enabled = false;

        public AnonymiserOverlay(bool enabled = false) : base("###AnonymiserOverlay", ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing, true)
        {
            RespectCloseHotkey = false;
            IsOpen = true;
            Enabled = enabled;
        }

        public unsafe override void Draw()
        {
            if (!TryGetAddonByName<AtkUnitBase>("RetainerList", out var retainerAddon)) return;

            if (ImGui.Checkbox("Enable Retainer Anonymiser", ref Enabled)) { EnableCheckbox(); };
            
            Height = ImGui.GetWindowSize().Y;
            Width = ImGui.GetWindowSize().X;
        }

        private unsafe void EnableCheckbox()
        {
            Anonymiser.Enabled = Enabled;
            Anonymiser.SetupRetainerList();
        }

        public override bool DrawConditions()
        {
            return TryGetAddonByName<AtkUnitBase>("RetainerList", out var addon)
                    && addon->UldManager.NodeListCount > 20
                    && addon->UldManager.NodeList[0]->IsVisible();
        }
    }
}
