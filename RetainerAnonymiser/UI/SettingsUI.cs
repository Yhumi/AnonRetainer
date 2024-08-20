using Dalamud.Interface.Components;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetainerAnonymiser.UI
{
    internal static unsafe class SettingsUI
    {
        internal static void Draw()
        {
            ImGui.TextWrapped($"Here you can change some of the main settings for RetainerAnonymiser.");

            bool hideRetainerNames = P.Config.HideRetainerNames;
            bool hideRetainerGil = P.Config.HideRetainerGil;
            bool automaticallyEnableOnLogin = P.Config.AutomaticallyEnableOnLogin;

            string retainerAnonymisedName = P.Config.RetainerAnonymisedName;

            ImGui.Separator();

            if (ImGui.CollapsingHeader("General Settings"))
            {
                if (ImGui.Checkbox("Hide Retainer Names", ref hideRetainerNames))
                {
                    P.Config.HideRetainerNames = hideRetainerNames;
                    P.Config.Save();
                }
                ImGuiComponents.HelpMarker($"Replace your retainer names with something anonymous.");

                if (hideRetainerNames)
                {
                    if (ImGui.InputText("Anonymous Retainer Name", ref retainerAnonymisedName, 200, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        P.Config.RetainerAnonymisedName = retainerAnonymisedName;
                        P.Config.Save();
                    }
                    ImGuiComponents.HelpMarker($"The name to replace your retainer names with.");
                }

                if (ImGui.Checkbox("Hide Retainer Gil", ref hideRetainerGil))
                {
                    P.Config.HideRetainerGil = hideRetainerGil;
                    P.Config.Save();
                }
                ImGuiComponents.HelpMarker($"Show your gil across retainers as 0.");

                if (ImGui.Checkbox("Automatically Enable on Login", ref automaticallyEnableOnLogin))
                {
                    P.Config.AutomaticallyEnableOnLogin = automaticallyEnableOnLogin;
                    P.Config.Save();
                }
                ImGuiComponents.HelpMarker($"Should this plugin be enabled by default on login?");
            }
        }
    }
}
