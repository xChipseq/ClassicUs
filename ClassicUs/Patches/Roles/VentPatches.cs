using ClassicUs.Objects.Roles;
using HarmonyLib;
using UnityEngine;

namespace ClassicUs.Patches.Roles;

public static class VentPatches
{
    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    private static class Vent_CanUse
    {
        private static bool Prefix(Vent __instance, ref float __result, ref NetworkedPlayerInfo pc, [HarmonyArgument(1)] ref bool returnedCanUse)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            float num = float.MaxValue;
            PlayerControl @object = pc.Object;
            var role = @object.GetModdedRole();
            if (role == null) return true;

            bool canUse = role.Config.CanVent && @object.CanMove && !@object.inVent && !@object.MustCleanVent(__instance.Id) && !@object.Data.IsDead;
            if (Vent.currentVent == __instance) canUse = true;
            if (canUse)
            {
                Vector3 center = @object.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance(center, position);
                if (num > __instance.UsableDistance || PhysicsHelpers.AnythingBetween(@object.Collider, center, position, Constants.ShipOnlyMask, false))
                {
                    num = float.MaxValue;
                    canUse = false;
                }
            }
            returnedCanUse = canUse;
            __result = num;

            return false;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    private static class Vent_SetButtons
    {
        private static bool Prefix(Vent __instance)
        {
            if (GameManager.Instance.IsHideAndSeek()) return true;

            var role = PlayerControl.LocalPlayer.GetModdedRole();
            if (role == null) return true;
            if (role.Config.CanMoveInVents) return true;
            return false;
        }
    }
}