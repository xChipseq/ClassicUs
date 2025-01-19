using ClassicUs.Objects.Abilities;
using ClassicUs.Utilities;
using HarmonyLib;
using Reactor.Utilities.Extensions;

namespace ClassicUs.Patches.Meeting;

[HarmonyPatch(typeof(PlayerVoteArea))]
public static class PlayerVoteAreaPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerVoteArea.SetCosmetics))]
    public static void SetCosmeticsPostfix(PlayerVoteArea __instance)
    {
        // foreach (var ability in AbilityManager.MeetingPlayerAbilityButtons)
        // {
        //     if (ability.Enabled() && !PlayerControl.LocalPlayer.Data.IsDead)
        //     {
        //         ability.CreateButton(__instance);
        //     }

        //     ability.MeetingStart();
        // }
    }
}