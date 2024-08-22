using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using ECommons.Automation;
using ECommons.DalamudServices;
using ECommons.Events;
using ECommons.ExcelServices;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuizmoNET;
using PInvoke;
using RetainerAnonymiser.UI.Overlays;
using System;
using System.Collections.Generic;

namespace RetainerAnonymiser.RetainerAddon
{
    internal unsafe static class Anonymiser
    {
        internal static AnonymiserOverlay? Overlay = null;
        internal static bool Enabled = false;

        internal static bool IsSetup = false;

        public static Dictionary<int, string> RetainerNames = new Dictionary<int, string>();
        public static Dictionary<int, string> RetainerGilValues = new Dictionary<int, string>();

        public static void Init()
        {
            Overlay = new();
            P.ws.AddWindow(Overlay);
            Svc.AddonLifecycle.RegisterListener(AddonEvent.PreDraw, ["RetainerList"], OnRetainerListEvent);
            Svc.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, ["RetainerList"], OnRetainerListEvent);
        }

        public static void Dispose()
        {
            Safe(() => Svc.AddonLifecycle.UnregisterListener(AddonEvent.PreDraw, ["RetainerList"], OnRetainerListEvent));
            Safe(() => Svc.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, ["RetainerList"], OnRetainerListEvent));
        }

        public static void Enable()
        {
            Enabled = true;
            if (Overlay != null)
                Overlay.Enabled = true;
        }

        public static void Disable()
        {
            Enabled = false;
            if (Overlay != null)
                Overlay.Enabled = false;
        }

        public static void Tick()
        {
            if (Overlay == null) return;

            if (TryGetAddonByName<AtkUnitBase>("RetainerList", out var retainerAddon))
            {
                if (retainerAddon->X != 0 || retainerAddon->Y != 0)
                {
                    Overlay.Position = new(retainerAddon->X + retainerAddon->GetScaledWidth(true) - Overlay.Width, retainerAddon->Y - Overlay.Height);
                }
            }
        }

        public static unsafe void OnRetainerListEvent(AddonEvent type, AddonArgs args)
        {
            if (!IsSetup && type == AddonEvent.PreDraw)
            {
                RetainerNames = new Dictionary<int, string>();
                RetainerGilValues = new Dictionary<int, string>();
                SetupRetainerList();
                IsSetup = true;
            }

            if (IsSetup && type == AddonEvent.PreFinalize)
                IsSetup = false;
        }

        public static unsafe void SetupRetainerList()
        {
            if (!TryGetAddonByName<AtkUnitBase>("RetainerList", out var retainerAddon)) return;
            
            if (Enabled)
            {
                if (P.Config.HideRetainerNames)
                {
                    StoreAndHideRetainerNames(retainerAddon);
                }

                if (P.Config.HideRetainerGil)
                {
                    StoreAndHideRetainerGil(retainerAddon);
                }
            }

            if (!Enabled)
            {
                if (RetainerNames.Count > 0)
                {
                    RestoreRetainerNames(retainerAddon);
                }
                    

                if (RetainerGilValues.Count > 0)
                {
                    RestoreRetainerGil(retainerAddon);
                }
            }
        }

        public static unsafe void StoreAndHideRetainerNames(AtkUnitBase* retainerAddon)
        {
            var retainers = retainerAddon->UldManager.NodeList[2]->GetAsAtkComponentList();
            if (retainers != null)
            {
                for (int i = 1; i < 11; i++)
                {
                    var retainer = retainers->UldManager.NodeList[i]->GetComponent();
                    if (retainer == null) continue;

                    var retainerName = retainer->UldManager.NodeList[13]->GetAsAtkTextNode();
                    if (retainerName != null)
                    {
                        RetainerNames[i] = retainerName->NodeText.ExtractText();
                        retainerName->SetText($"{P.Config.RetainerAnonymisedName} {i}");
                    }
                }
            }
        }

        public static unsafe void StoreAndHideRetainerGil(AtkUnitBase* retainerAddon)
        {
            var retainers = retainerAddon->UldManager.NodeList[2]->GetAsAtkComponentList();
            if (retainers != null)
            {
                for (int i = 1; i < 11; i++)
                {
                    var retainer = retainers->UldManager.NodeList[i]->GetComponent();
                    if (retainer == null) continue;

                    var retainerGil = retainer->UldManager.NodeList[7]->GetAsAtkTextNode();
                    if (retainerGil != null)
                    {
                        RetainerGilValues[i] = retainerGil->NodeText.ExtractText();
                        retainerGil->SetText($"-");
                    }
                }
            }
        }

        public static unsafe void RestoreRetainerNames(AtkUnitBase* retainerAddon)
        {
            var retainers = retainerAddon->UldManager.NodeList[2]->GetAsAtkComponentList();
            if (retainers != null)
            {
                for (int i = 1; i < 11; i++)
                {
                    var retainer = retainers->UldManager.NodeList[i]->GetComponent();
                    if (retainer == null) continue;

                    var retainerName = retainer->UldManager.NodeList[13]->GetAsAtkTextNode();
                    if (retainerName != null)
                    {
                        retainerName->SetText($"{RetainerNames[i]}");
                    }
                }
            }
        }

        public static unsafe void RestoreRetainerGil(AtkUnitBase* retainerAddon)
        {
            var retainers = retainerAddon->UldManager.NodeList[2]->GetAsAtkComponentList();
            if (retainers != null)
            {
                for (int i = 1; i < 11; i++)
                {
                    var retainer = retainers->UldManager.NodeList[i]->GetComponent();
                    if (retainer == null) continue;

                    var retainerGil = retainer->UldManager.NodeList[7]->GetAsAtkTextNode();
                    if (retainerGil != null)
                    {
                        retainerGil->SetText($"{RetainerGilValues[i]}");
                    }
                }
            }
        }
    }
}
