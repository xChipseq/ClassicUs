using System.Linq;
using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Roles;
using HarmonyLib;

namespace ClassicUs.Roles.Impostors.Janitor;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
public static class JanitorKillPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.Is<JanitorRole>())
        {
            var cleanAbility = AbilityManager.CustomAbilityButtons.First(p => p is JanitorClean);

            cleanAbility.ChangeUses(1);
            cleanAbility.ResetCooldown(__instance.killTimer);
        }
    }
}