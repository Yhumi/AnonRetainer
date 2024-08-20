using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ECommons;
using ECommons.DalamudServices;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RetainerAnonymiser.UI
{
    internal unsafe class PluginUI : Window
    {
        private bool visible = false;
        public OpenWindow OpenWindow { get; set; }

        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        public PluginUI() : base($"{P.Name} {P.GetType().Assembly.GetName().Version}###RetainerAnonymiser")
        {
            this.RespectCloseHotkey = false;
            this.SizeConstraints = new()
            {
                MinimumSize = new(250, 100),
                MaximumSize = new(9999, 9999)
            };
            P.ws.AddWindow(this);
        }

        public override void PreDraw()
        {
        }

        public override void PostDraw()
        {
        }

        public void Dispose()
        {
        }

        public override void Draw()
        {
            var region = ImGui.GetContentRegionAvail();
            var itemSpacing = ImGui.GetStyle().ItemSpacing;
            var topLeftSideHeight = region.Y;

            ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(5f.Scale(), 0));

            try
            {
                using (var table = ImRaii.Table($"RetainerAnonymiserTableContainer", 2, ImGuiTableFlags.Resizable))
                {
                    if (!table)
                        return;

                    ImGui.TableSetupColumn("##LeftColumn", ImGuiTableColumnFlags.WidthFixed, ImGui.GetWindowWidth() / 2);
                    ImGui.TableNextColumn();

                    var regionSize = ImGui.GetContentRegionAvail();
                    ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0.5f, 0.5f));

                    using (var leftChild = ImRaii.Child($"###RetainerAnonymiserLeftSide", regionSize with { Y = topLeftSideHeight }, false, ImGuiWindowFlags.NoDecoration))
                    {
                        var imagePath = Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName!, "Images/wetried.png");

                        if (ThreadLoadImageHandler.TryGetTextureWrap(imagePath, out var logo))
                        {
                            ImGuiEx.LineCentered("###RetainerAnonymiserLogo", () =>
                            {
                                ImGui.Image(logo.ImGuiHandle, new(125f.Scale(), 125f.Scale()));
                            });
                        }

                        ImGui.Spacing();
                        ImGui.Separator();

                        if (ImGui.Selectable("Settings", OpenWindow == OpenWindow.Settings))
                        {
                            OpenWindow = OpenWindow.Settings;
                        }

                        ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 25f);
                        if (ImGui.Selectable($"About", OpenWindow == OpenWindow.About))
                        {
                            OpenWindow = OpenWindow.About;
                        };
                    }

                    ImGui.PopStyleVar();

                    ImGui.PopStyleVar();
                    ImGui.TableNextColumn();
                    using (var rightChild = ImRaii.Child($"###RetainerAnonymiserRightSide", Vector2.Zero, false))
                    {
                        switch (OpenWindow)
                        {
                            case OpenWindow.Settings:
                                SettingsUI.Draw();
                                break;
                            case OpenWindow.About:
                                AboutUI.Draw();
                                break;
                            case OpenWindow.None:
                            default:
                                break;
                        };
                    }
                }
            }
            catch (Exception ex) 
            {
                ex.Log();
            }
        }
    }

    public enum OpenWindow
    {
        None = 0,
        Settings = 1,
        About = 2
    }
}
