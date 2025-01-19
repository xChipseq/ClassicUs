using ClassicUs.Objects.Roles;
using HarmonyLib;
using UnityEngine;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(Console))]
public static class ConsolePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Console.CanUse))]
    public static bool CanUsePrefix(Console __instance, [HarmonyArgument(0)] NetworkedPlayerInfo playerInfo, ref float __result)
    {
        var playerControl = playerInfo.Object;
        var role = playerControl.GetModdedRole();

        if (role == null) return true;

        if (!role.Config.CanDoTasks && !__instance.AllowImpostor)
        {
            __result = float.MaxValue;
            return false;
        }

        return true;
    }
}