using ClassicUs.Objects.Roles;
using HarmonyLib;
using UnityEngine;

namespace ClassicUs.Patches;

[HarmonyPatch(typeof(ShipStatus))]
public static class ShipStatusPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ShipStatus.CalculateLightRadius))]
    public static bool CalculateLightRadiusPrefix(ShipStatus __instance, ref NetworkedPlayerInfo player, ref float __result)
    {
        if (LobbyBehaviour.Instance) return true;
        if (GameManager.Instance.IsHideAndSeek()) return true;

        var pc = player.Object;
        var role = pc.GetModdedRole();
        if (role == null) return true;

        if (player == null || player.IsDead)
        {
            __result = __instance.MaxLightRadius;
            return false;
        }
        if (player.Role.IsImpostor || role.Config.HasImpostorVision)
        {
            __result = __instance.MaxLightRadius * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(AmongUs.GameOptions.FloatOptionNames.ImpostorLightMod);
            return false;
        }
        
        float num = 1f;
        if (!FungleShipStatus.Instance)
        {
            var switchSystem = __instance.Systems[SystemTypes.Electrical]?.TryCast<SwitchSystem>();
            num = switchSystem != null ? switchSystem.Value / 255f : 1;
        }

        __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, num) * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(AmongUs.GameOptions.FloatOptionNames.CrewLightMod);

        return false;
    }
}