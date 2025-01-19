using System.Collections.Generic;
using System.Linq;
using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Modifiers;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ClassicUs.Patches;

[HarmonyPatch(typeof(HudManager))]
public static class HudManagerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(HudManager.Update))]
    public static void UpdatePostfix(HudManager __instance)
    {
        if (LobbyBehaviour.Instance) return;
        if (!__instance.IsIntroDisplayed) AbilityManager.CustomAbilityButtons.ForEach(ability => ability.HudUpdateHandler(__instance));
        CustomProtectionManager.UpdateProtections();

        var local = PlayerControl.LocalPlayer;
        if (!local) return;
        var role = local.GetModdedRole();
        if (role == null) return;
        local.GetModdedRole()?.HudUpdate(__instance);
        local.GetModifier()?.HudUpdate(__instance);
        local.GetWinConModifier()?.HudUpdate(__instance);
        
        __instance.KillButton.gameObject.SetActive(role.Config.HasKillButton && !MeetingHud.Instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(HudManager.Start))]
    public static void StartPostfix(HudManager __instance)
    {
        var buttons = __instance.transform.Find("Buttons");
        Transform bottomRight; 
        if (buttons)
            bottomRight = buttons.Find("BottomRight");
        else
            return;

        foreach (var ability in AbilityManager.CustomAbilityButtons)
        {
            ability.CreateButton(bottomRight);
            ClassicLogger.Log($"Creating {ability.Name} ability button");
        }
    }
}