using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using HarmonyLib;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(ExileController))]
public static class ExileControllerPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    public static void BeginPostfix(ExileController __instance)
    {
        if (__instance.initData.networkedPlayer)
        {
            var role = Helpers.PlayerById(__instance.initData.networkedPlayer.PlayerId).GetModdedRole();
            if (role != null)
            {
                __instance.completeString = $"{__instance.initData.networkedPlayer.PlayerName} was The {role.Name}";
            }
        }
    }
}