using System.Linq;
using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Modifiers;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using HarmonyLib;
using Reactor.Utilities.Extensions;

namespace ClassicUs.Patches.Meeting;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHudPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Awake))]
    public static void AwakePostfix(MeetingHud __instance)
    {
        int buttonIndex = 1;
        foreach (var ability in AbilityManager.MeetingAbilityButtons)
        {
            if (ability.Enabled() && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                ability.CreateButton(__instance.ButtonParent, __instance, buttonIndex);
                ClassicLogger.Log($"Creating {ability.Name} meeting ability");
            }

            buttonIndex++;
            ability.MeetingStart();
        }

        var local = PlayerControl.LocalPlayer;
        local.GetModdedRole()?.MeetingStart(__instance);
        local.GetModifier()?.MeetingStart(__instance);
        local.GetWinConModifier()?.MeetingStart(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Update))]
    public static void UpdatePostfix(MeetingHud __instance)
    {
        AbilityManager.MeetingAbilityButtons.ForEach(button => button.HudUpdateHandler());
        AbilityManager.MeetingPlayerAbilityButtons.ForEach(button => button.HudUpdateHandler());

        CustomProtectionManager.UpdateProtections(true);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.OnDestroy))]
    public static void OnDestroyPostfix(MeetingHud __instance)
    {
        AbilityManager.CustomAbilityButtons.ForEach(button => button.MeetingEnd());
        AbilityManager.MeetingAbilityButtons.ForEach(button => button.DeleteButton());
        AbilityManager.MeetingPlayerAbilityButtons.ForEach(button => button.DeleteButton());

        var local = PlayerControl.LocalPlayer;
        local.GetModdedRole()?.MeetingEnd();
        local.GetModifier()?.MeetingEnd();
        local.GetWinConModifier()?.MeetingEnd();
    }
}