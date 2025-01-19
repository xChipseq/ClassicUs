using ClassicUs.Objects.Roles;
using HarmonyLib;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(SabotageButton))]
public static class SabotagePatches
{
    [HarmonyPatch(nameof(SabotageButton.DoClick))]
    [HarmonyPrefix]
    public static bool DoClickPrefix(SabotageButton __instance)
    {
        var player = PlayerControl.LocalPlayer;
        var role = player.GetModdedRole();
        if (role == null) return true;

        if (!role.Config.CanSabotage || PlayerControl.LocalPlayer.inVent || !GameManager.Instance.SabotagesEnabled())
        {
            return false;
        }

        HudManager.Instance.ToggleMapVisible(
            new MapOptions
            {
                Mode = MapOptions.Modes.Sabotage,
            });
        
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SabotageButton.Refresh))]
    public static bool RefreshPrefix(SabotageButton __instance)
    {
        var player = PlayerControl.LocalPlayer;
        if (GameManager.Instance == null || player == null)
        {
            __instance.ToggleVisible(false);
            __instance.SetDisabled();
            return false;
        }

        var role = player.GetModdedRole(); 

        if (role == null)
        {
            return true;
        }

        if (player.inVent || !GameManager.Instance.SabotagesEnabled() || player.petting)
        {
            __instance.ToggleVisible(role.Config.CanSabotage && GameManager.Instance.SabotagesEnabled());
            __instance.SetDisabled();
            return false;
        }
        __instance.SetEnabled();
        return false;
    }
}