using ClassicUs.GameOptions;
using ClassicUs.Utilities;
using HarmonyLib;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(KillButton))]
public static class KillButtonPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(KillButton.DoClick))]
    public static bool DoClickPrefix(KillButton __instance)
    {
        if (GameManager.Instance.IsHideAndSeek() && OptionsManager.HNSBetterKillButton && __instance.currentTarget == null && !__instance.isCoolingDown && !PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.inVent)
        {
            PlayerControl.LocalPlayer.SetKillTimer(5f);
            return false;
        }

        if (__instance.isActiveAndEnabled && __instance.currentTarget && !__instance.isCoolingDown && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove)
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(__instance.currentTarget);
            __instance.SetTarget(null);
        }
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(KillButton.SetTarget))]
    public static void SetTargetPostfix(KillButton __instance)
    {
        if (GameManager.Instance.IsHideAndSeek() && OptionsManager.HNSBetterKillButton)
        {
            __instance.SetEnabled();
        }
    }
}