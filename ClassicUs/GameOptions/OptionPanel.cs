using System;
using System.Linq;
using System.Text.Json;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace ClassicUs.GameOptions;

[HarmonyPatch(typeof(HudManager))]
public static class OptionPanel
{
    public static TextMeshPro Panel;
    public static BoxCollider2D Collider;

    public static bool PanelShown = true;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(HudManager.Update))]
    public static void Update(HudManager __instance)
    {
        if (Panel)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool hovered = Collider.bounds.Contains(mouseWorldPosition);

            if (hovered && PanelShown)
            {
                Panel.GetComponent<AspectPosition>().DistanceFromEdge += new Vector3(0, Input.mouseScrollDelta.y * 0.5f);
                float clamped = Math.Clamp(Panel.GetComponent<AspectPosition>().DistanceFromEdge.y, -5f, -0.2f);
                Panel.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(0.07f, clamped);
            }
            else Panel.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(0.07f, 0.5f);

            // Input
            if (Input.GetKeyDown(KeyCode.F1))
            {
                PanelShown = !PanelShown;
            }
            if (AmongUsClient.Instance.AmHost) // Host-only input
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    var next = OptionsManager.ModRoleLists.IndexOf(OptionsManager.CurrentRoleList) + 1;
                    if (next == OptionsManager.ModRoleLists.Count) next = 0;
                    OptionsManager.CurrentRoleList = OptionsManager.ModRoleLists[next];

                    PlayerControl.LocalPlayer.RpcModRoleListChange(OptionsManager.CurrentRoleList);
                    ClassicLogger.Log($"(HOST) Role list change: {OptionsManager.CurrentRoleList.name} (mod)");
                }
            }

            UpdatePanel();
        }
        else
        {
            CreatePanel(__instance);
        }
    }

    public static void UpdatePanel()
    {
        if (AmongUsClient.Instance.IsGameStarted)
        {
            Panel.gameObject.SetActive(false);
            return;
        }
        Panel.gameObject.SetActive(true);
        if (!PanelShown)
        {
            Panel.text = "          <color=grey>(F1 to show game options)</color>";
            return;
        }

        if (!GameManager.Instance.IsHideAndSeek())
        {
            int players = PlayerControl.AllPlayerControls.Count;
            int impostors = GameOptionsManager.Instance.currentGameOptions.GetAdjustedNumImpostors(players);
            var current = OptionsManager.CurrentRoleList;
            var roleList = current;

            string list = "";

            int index = 1;
            foreach (var role in roleList.Impostors)
            {
                if (index > impostors) break;
                if (!ModdedRoleManager.RoleListEntries.ContainsKey(role))
                {
                    if (ModdedRoleManager.RoleTypes.ContainsKey(role))
                    {
                        var tempRole = (ModdedRole)Activator.CreateInstance(ModdedRoleManager.RoleTypes[role]);

                        list += $"            <size=70%>{index}.</size> {tempRole.GetColoredString(tempRole.Name)}\n";
                        index++;
                        if (index > players) break;
                        continue;
                    }
                }

                var roleListEntry = ModdedRoleManager.RoleListEntries[role];

                list += $"            <size=70%>{index}.</size> <color=#{roleListEntry.Color.ToHtmlStringRGBA()}>{roleListEntry.Name}</color>\n";
                index++;
            }
            foreach (var role in roleList.Roles)
            {
                if (!ModdedRoleManager.RoleListEntries.ContainsKey(role))
                {
                    if (ModdedRoleManager.RoleTypes.ContainsKey(role))
                    {
                        var tempRole = (ModdedRole)Activator.CreateInstance(ModdedRoleManager.RoleTypes[role]);

                        list += $"            <size=70%>{index}.</size> {tempRole.GetColoredString(tempRole.Name)}\n";
                        index++;
                        if (index > players) break;
                        continue;
                    }
                }

                var roleListEntry = ModdedRoleManager.RoleListEntries[role];

                list += $"            <size=70%>{index}.</size> <color=#{roleListEntry.Color.ToHtmlStringRGBA()}>{roleListEntry.Name}</color>\n";

                index++;
                if (index > players) break;
            }

            bool host = AmongUsClient.Instance.AmHost;

            Panel.text = @$"
            <size=180%><color=purple>Classic Us</color></size> <size=50%><color=grey>(F1 to hide this menu)</color></size>
            Role list: <color={current.color}><b>{current.name}</b></color> <size=50%><color=grey>{(host ? "(F2 to change)" : "")}</color></size>
            Game options: <i><color=grey>1-15 players</color></i> <size=50%><color=grey>{(host ? "(F3 to change)" : "")}</color></size>

            <size=130%><color=lightblue>Role List:</color></size>
            {impostors} <color=red>Impostor{(impostors > 1 || impostors == 0 ? "s" : "")}</color>

{list}

            <color=red>Bans:</color> <size=50%><color=grey>{(host ? "(F4 to manage bans)" : "")}</color></size>
            <size=70%>1.</size> Trapper
            <size=70%>2.</size> -
            <size=70%>3.</size> -
            <size=70%>4.</size> -
            <size=70%>5.</size> -         
            ";
        }
        else
        {
            Panel.text = @$"
            <size=180%><color=purple>Classic Us</color></size> <size=50%><color=grey>(F1 to hide this menu)</color></size>
            <size=150%><color=blue>Hide and Seek</color></size>

            Always Active Kill Button: {(OptionsManager.HNSBetterKillButton ? "<color=green>On</color>" : "<color=red>Off</color>")}
            <i>Seeker's Kill Button is always highlighted - when used without target gives double the cooldown.</i>

            Move Or Die: {(OptionsManager.HNSMoveOrDie ? "<color=green>On</color>" : "<color=red>Off</color>")}
            <i>After not moving or staying in the same room for too long you self destruct.</i>
            ";
        }

    }

    public static void CreatePanel(HudManager hud)
    {
        // TODO:
        // Add masking with an image and rect mask
        //
        var pingTracker = GameObject.Find("PingTrackerTMP");
        if (pingTracker == null) return;

        var panel = GameObject.Instantiate(pingTracker);
        panel.name = "ClassicUsSettings";
        panel.RemoveComponent<PingTracker>();
        panel.SetActive(true);
        panel.transform.SetParent(hud.transform);

        var aspect = panel.GetComponent<AspectPosition>();
        aspect.Alignment = AspectPosition.EdgeAlignments.LeftTop;
        aspect.DistanceFromEdge = new Vector2(0.07f, -0.2f);

        var tmp = panel.GetComponent<TextMeshPro>();
        // tmp.SetMask(MaskingTypes.MaskHard, new Vector4(0, 0, 50, 3));
        // tmp.EnableMasking();

        Panel = tmp;

        var boxCollider = Panel.gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = Panel.GetRenderedValues();
        boxCollider.offset = Panel.rectTransform.rect.center;
        Collider = boxCollider;
    }
}