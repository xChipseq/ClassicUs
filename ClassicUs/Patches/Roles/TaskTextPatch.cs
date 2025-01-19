using ClassicUs.Objects.Modifiers;
using ClassicUs.Objects.Roles;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__141), nameof(PlayerControl._CoSetTasks_d__141.MoveNext))]
public static class TaskTextPatch
{
    public static void Postfix(PlayerControl._CoSetTasks_d__141 __instance)
    {
        if (__instance == null) return;
        var player = __instance.__4__this;
        var role = player.GetModdedRole();
        var modifier = player.GetModifier();
        var winConModifier = player.GetWinConModifier();

        if (role == null) return;
        if (role.Player != PlayerControl.LocalPlayer) return;
        var task = new GameObject("RoleInfoTask").AddComponent<ImportantTextTask>();
        task.transform.SetParent(player.transform, false);

        if (!role.Hidden)
        {
            task.Text = $"<size=130%>{role.GetColoredString($"Role: {role.Name}")}</size>\n";
            task.Text += $"<size=125%>{role.GetColoredString(role.TaskText)}</size>\n";
        }
        
        if (modifier != null)
        {
            Color color = modifier.Team switch
            {
                ModifierTeam.Global => ClassicPalette.GlobalModifierColor,
                ModifierTeam.Crew => ClassicPalette.CrewModifierColor,
                ModifierTeam.Killers => ClassicPalette.KillerModifierColor,
                _ => Color.white,
            };
            task.Text += $"<size=110%><color=#{color.ToHtmlStringRGBA()}>Modifier: {modifier.Name}</color></size>\n";
            task.Text += $"<size=105%><color=#{color.ToHtmlStringRGBA()}>{modifier.TaskText}</color></size>\n";
        }

        if (winConModifier != null)
        {
            task.Text += $"<size=110%><color=#{winConModifier.Color.ToHtmlStringRGBA()}>Modifier: {modifier.Name}</color></size>\n";
            task.Text += $"<size=105%><color=#{winConModifier.Color.ToHtmlStringRGBA()}>{modifier.TaskText}</color></size>\n";
        }

        if (!role.Config.TasksCountForProgress && !player.Data.Role.IsImpostor)
        {
            task.Text += role.GetColoredString("\nFake Tasks:");
        }

        player.myTasks.Insert(0, task);
    }
}