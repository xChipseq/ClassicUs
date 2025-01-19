using System.Collections.Generic;
using System.Linq;
using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Modifiers;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ClassicUs.Patches;

[HarmonyPatch(typeof(PlayerControl))]
public static class PlayerControlPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControl.FixedUpdate))]
    public static void UpdatePostfix(PlayerControl __instance)
    {
        if (LobbyBehaviour.Instance) return;
        AbilityManager.CustomAbilityButtons.ForEach(button => { if (button.Button && button.Enabled()) button.PlayerFixedUpdate(__instance); });

        var local = PlayerControl.LocalPlayer;
        if (!local) return;
        var localRole = local.GetModdedRole();
        var role = __instance.GetModdedRole();
        if ((AmongUsClient.Instance.IsGameStarted || TutorialManager.InstanceExists) && !HudManager.Instance.IsIntroDisplayed && role != null && (__instance.AmOwner || local.Data.IsDead ||
            (local.GetTeam() == RoleTeam.Impostor && __instance.GetTeam() == RoleTeam.Impostor) ||
            (local.GetTeam() == RoleTeam.Apocalypse && __instance.GetTeam() == RoleTeam.Apocalypse)))
        {
            __instance.cosmetics.nameText.text = role.GetRoleText(__instance) + localRole.GetPrefix(__instance);
            __instance.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1.15f);
        }
        else
        {
            if (localRole != null)
                __instance.cosmetics.nameText.text = __instance.Data.PlayerName + localRole.GetPrefix(__instance);
            else
                __instance.cosmetics.nameText.text = __instance.Data.PlayerName;
            __instance.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -0.2f);
        }

        role?.PlayerFixedUpdate(__instance);
        __instance.GetModifier()?.PlayerFixedUpdate(__instance);
        __instance.GetWinConModifier()?.PlayerFixedUpdate(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerControl.CmdCheckMurder))]
    public static bool CmdCheckMurderPrefix(PlayerControl __instance, ref PlayerControl target)
    {
        __instance.isKilling = true;
        __instance.RpcCustomMurder(target);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControl.CompleteTask))]
    public static void CompleteTaskPostfix(PlayerControl __instance)
    {
        var role = __instance.GetModdedRole();
        bool flag = true;
        if (role != null) flag = role.Config.TasksCountForProgress;
        if (!flag) GameData.Instance.CompletedTasks--;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControl.Die))]
    public static void DiePostfix(PlayerControl __instance, ref DeathReason reason)
    {
        __instance.GetModdedRole()?.OnDeath(reason);
    }
}