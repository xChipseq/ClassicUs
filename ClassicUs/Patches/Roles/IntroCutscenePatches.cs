using System.Linq;
using AmongUs.GameOptions;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using HarmonyLib;
using UnityEngine;

namespace ClassicUs.Patches.Roles;

public static class IntroCutscenePatches
{
    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
    private static class ShowRolePatch
    {
        public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
        {
            var role = PlayerControl.LocalPlayer.GetModdedRole();
            __instance.__4__this.RoleText.text = role.GetColoredString(role.Name);
            __instance.__4__this.RoleBlurbText.text = role.GetColoredString(role.IntroText);

            __instance.__4__this.RoleBlurbText.transform.localScale = new Vector3(0.8f, 0.8f);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    private static class BeginCrewmatePatch
    {
        public static bool Prefix(IntroCutscene __instance)
        {
            var team = PlayerControl.LocalPlayer.GetTeam();
            if (team == RoleTeam.Neutral)
            {
                __instance.BackgroundBar.material.SetColor("_Color", Color.gray);
                __instance.TeamTitle.text = "NEUTRAL";
                __instance.TeamTitle.color = Color.gray;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = RoleManager.Instance.AllRoles.FirstOrDefault(role => role.Role == RoleTypes.Shapeshifter).IntroSound;
            }
            else if (team == RoleTeam.Apocalypse)
            {
                __instance.BackgroundBar.material.SetColor("_Color", ClassicPalette.ApocalypseColor);
                __instance.TeamTitle.text = "APOCALYPSE";
                __instance.TeamTitle.color = ClassicPalette.ApocalypseColor;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = RoleManager.Instance.AllRoles.FirstOrDefault(role => role.Role == RoleTypes.Phantom).IntroSound;
            }
            else
            {
                return true;
            }

            var barTransform = __instance.BackgroundBar.transform;
            var position = barTransform.position;
            position.y -= 0.25f;
            barTransform.position = position;

            __instance.ourCrewmate = __instance.CreatePlayer(
                0,
                Mathf.CeilToInt(7.5f),
                PlayerControl.LocalPlayer.Data,
                false);
            return false;
        }
    }
}