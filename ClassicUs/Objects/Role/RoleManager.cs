using System;
using System.Collections.Generic;
using System.Linq;
using ClassicUs.Objects.Modifiers;
using ClassicUs.Roles;
using ClassicUs.Utilities;
using HarmonyLib;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

using ClassicUs.Roles.Apocalypse.Baker;
using ClassicUs.Roles.Crewmates.Cleric;
using ClassicUs.Roles.Crewmates.Shrimpmate;
using ClassicUs.Roles.Crewmates.Vigilante;
using ClassicUs.Roles.Impostors.Janitor;
using ClassicUs.Roles.Neutral.Jester;
using ClassicUs.Roles.Neutral.Pirate;
using ClassicUs.Roles.Neutral.SerialKiller;
using ClassicUs.GameOptions;

namespace ClassicUs.Objects.Roles;

[HarmonyPatch(typeof(RoleManager))]
public static class ModdedRoleManager
{
    public static Dictionary<byte, ModdedRole> PlayerRoles = new();
    public static readonly Dictionary<ModdedRoles, Type> RoleTypes = new()
    {
        // Vanilla Roles
        { ModdedRoles.Crewmate, typeof(ModdedCrewmate) },
        { ModdedRoles.Impostor, typeof(ModdedImpostor) },

        // Crewmates
        { ModdedRoles.Shrimpmate, typeof(ShrimpmateRole) }, // S H R I M P M A T E
        { ModdedRoles.Vigilante, typeof(VigilanteRole) },
        { ModdedRoles.Cleric, typeof(ClericRole) },

        // Neutral
        { ModdedRoles.SerialKiller, typeof(SerialKillerRole) },
        { ModdedRoles.Jester, typeof(JesterRole) },
        { ModdedRoles.Pirate, typeof(PirateRole) },

        // Impostors
        { ModdedRoles.Janitor, typeof(JanitorRole) },

        // Apocalypse
        { ModdedRoles.Baker, typeof(BakerRole) },

        { ModdedRoles.Famine, typeof(FamineRole) },
    };

    public static readonly Dictionary<ModdedRoles, RoleListEntry> RoleListEntries = new()
    {
        { ModdedRoles.Any, new RoleListEntry("Any", Color.white)},
        { ModdedRoles.RandomCrewmate, new RoleListEntry("<color=#2919ff>Random</color> Crewmate", Palette.CrewmateBlue, RoleTeam.Crewmate)},
        { ModdedRoles.RandomImpostor, new RoleListEntry("<color=#2919ff>Random</color> Impostor", Palette.ImpostorRed, RoleTeam.Impostor)},
        { ModdedRoles.RandomNeutral, new RoleListEntry("<color=#2919ff>Random</color> Neutral", Color.grey, RoleTeam.Neutral)},
        { ModdedRoles.RandomApocalypse, new RoleListEntry("<color=#2919ff>Random</color> Apocalypse", ClassicPalette.ApocalypseColor, RoleTeam.Apocalypse)},

        { ModdedRoles.CrewInvestigative, new RoleListEntry("Crew <color=#2919ff>Investigative</color>", Palette.CrewmateBlue, RoleTeam.Crewmate, RoleAlignment.Investigative)},
        { ModdedRoles.CrewProtective, new RoleListEntry("Crew <color=#2919ff>Protective</color>", Palette.CrewmateBlue, RoleTeam.Crewmate, RoleAlignment.Protective)},
        { ModdedRoles.CrewKilling, new RoleListEntry("Crew <color=#2919ff>Killing</color>", Palette.CrewmateBlue, RoleTeam.Crewmate, RoleAlignment.Killing)},
        { ModdedRoles.CrewSupport, new RoleListEntry("Crew <color=#2919ff>Support</color>", Palette.CrewmateBlue, RoleTeam.Crewmate, RoleAlignment.Support)},
        { ModdedRoles.CrewPower, new RoleListEntry("Crew <color=#2919ff>Power</color>", Palette.CrewmateBlue, RoleTeam.Crewmate, RoleAlignment.Power)},

        { ModdedRoles.NeutralEvil, new RoleListEntry("Neutral <color=#2919ff>Evil</color>", Color.gray, RoleTeam.Neutral, RoleAlignment.Evil)},
        { ModdedRoles.NeutralKilling, new RoleListEntry("Neutral <color=#2919ff>Killing</color>", Color.gray, RoleTeam.Neutral, RoleAlignment.Killing)},
        { ModdedRoles.NeutralChaos, new RoleListEntry("Neutral <color=#2919ff>Chaos</color>", Color.gray, RoleTeam.Neutral, RoleAlignment.Chaos)},
        { ModdedRoles.NeutralPariah, new RoleListEntry("Neutral <color=#2919ff>Pariah</color>", Color.gray, RoleTeam.Neutral, RoleAlignment.Pariah)},

        { ModdedRoles.ImpostorDeception, new RoleListEntry("Impostor <color=#2919ff>Deception</color>", Palette.ImpostorRed, RoleTeam.Impostor, RoleAlignment.Deception)},
        { ModdedRoles.ImpostorKilling, new RoleListEntry("Impostor <color=#2919ff>Killing</color>", Palette.ImpostorRed, RoleTeam.Impostor, RoleAlignment.Killing)},
        { ModdedRoles.ImpostorSupport, new RoleListEntry("Impostor <color=#2919ff>Support</color>", Palette.ImpostorRed, RoleTeam.Impostor, RoleAlignment.Support)},
        { ModdedRoles.ImpostorPower, new RoleListEntry("Impostor <color=#2919ff>Power</color>", Palette.ImpostorRed, RoleTeam.Impostor, RoleAlignment.Power)},
    };


    [MethodRpc((uint)RpcCalls.SetRole)]
    public static void RpcSetModdedRole(this PlayerControl player, int roleType, int overrideTeam = 0)
    {
        player.SetModdedRole(roleType, overrideTeam);
    }
    public static void RpcSetModdedRole(this PlayerControl player, ModdedRoles roleType, RoleTeam? overrideTeam = null)
    {
        player.RpcSetModdedRole((int)roleType, overrideTeam.HasValue ? (int)overrideTeam : 0);
    }

    public static void SetModdedRole(this PlayerControl player, int roleType, int overrideTeam = 0)
    {
        if (PlayerRoles.ContainsKey(player.Data.PlayerId))
        {
            PlayerRoles.Remove(player.Data.PlayerId);
        }

        if (overrideTeam == 0)
        {
            PlayerRoles.Add(player.Data.PlayerId, (ModdedRole)Activator.CreateInstance(RoleTypes[(ModdedRoles)roleType]));
        }
        else
        {
            PlayerRoles.Add(player.Data.PlayerId, (ModdedRole)Activator.CreateInstance(RoleTypes[(ModdedRoles)roleType], [(RoleTeam)overrideTeam]));
        }
    }

    public static ModdedRole GetModdedRole(this PlayerControl player)
    {
        if (PlayerRoles.ContainsKey(player.Data.PlayerId))
        {
            return PlayerRoles[player.Data.PlayerId];
        }
        else
        {
            // i got tired of seeing null errors so i made it return crewmate when player has no role
            // what could possibly go wrong?
            return (ModdedRole)Activator.CreateInstance(typeof(ModdedCrewmate));
        }
    }
    public static T GetModdedRole<T>(this PlayerControl player) where T : ModdedRole
    {
        if (PlayerRoles.ContainsKey(player.Data.PlayerId))
            return (T)PlayerRoles[player.Data.PlayerId];
        else
            return null;
    }

    public static RoleTeam GetTeam(this PlayerControl player)
    {
        var role = player.GetModdedRole();
        if (role != null)
            return role.Team;
        else
            return RoleTeam.Crewmate;
    }

    public static RoleTeam GetTeam(ModdedRoles role)
    {
        if (!RoleTypes.ContainsKey(role))
        {
            var entry = RoleListEntries[role];
            if (role == ModdedRoles.Any)
            {
                return RoleTeam.Crewmate;
            }
            return entry.RoleTeam.Value;
        }

        var tempRole = (ModdedRole)Activator.CreateInstance(RoleTypes[role]);
        return tempRole.Team;
    }

    public static bool Is<T>(this PlayerControl player) where T : ModdedRole
    {
        var role = player.GetModdedRole();
        if (role != null)
            return role is T;
        else
            return false;
    }

    public static List<ModdedRoles> GetAllRolesForTeam(RoleTeam team, RoleAlignment? alignment = null)
    {
        List<ModdedRoles> roles = new();
        foreach (var pair in RoleTypes)
        {
            var tempRole = (ModdedRole)Activator.CreateInstance(pair.Value);
            if (alignment.HasValue)
            {
                if (tempRole.Team == team && tempRole.Alignment == alignment.Value && tempRole.Generates) roles.Add(pair.Key);
            }
            else
            {
                if (tempRole.Team == team && tempRole.Generates) roles.Add(pair.Key);
            }
        }

        return roles;
    }

    public static List<ModdedRoles> GetAllRolesForAny()
    {
        var roles = new List<ModdedRoles>();
        roles.AddRange(GetAllRolesForTeam(RoleTeam.Crewmate));
        roles.AddRange(GetAllRolesForTeam(RoleTeam.Apocalypse));
        roles.AddRange(GetAllRolesForTeam(RoleTeam.Neutral));
        return roles;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(RoleManager.SelectRoles))]
    private static void SelectRolesPostfix()
    {
        if (AmongUsClient.Instance.AmHost)
        {
            ClassicLogger.Log("Handing roles...", ClassicLogger.LogFlags.Warn);

            // the plan
            // 1. get all roles from role list based on player count
            // 2. shuffle them and give them to players

            var players = PlayerControl.AllPlayerControls.ToArray().ToList();
            players.Shuffle();

            var impostorRoles = OptionsManager.GetRolesFromCurrentRoleList(RoleTeam.Impostor);
            var crewRoles = OptionsManager.GetRolesFromCurrentRoleList(RoleTeam.Crewmate);
            var neutralRoles = OptionsManager.GetRolesFromCurrentRoleList(RoleTeam.Neutral);
            var apocalypseRoles = OptionsManager.GetRolesFromCurrentRoleList(RoleTeam.Apocalypse);

            List<ModdedRoles> combined = [.. crewRoles, .. neutralRoles, .. apocalypseRoles];

            impostorRoles.Shuffle();
            crewRoles.Shuffle();
            neutralRoles.Shuffle();
            apocalypseRoles.Shuffle();

            foreach (var player in players)
            {
                ModdedRoles resultRole = ModdedRoles.Crewmate;
                if (player.Data.Role.IsImpostor)
                {
                    ClassicLogger.Log($"{player.Data.PlayerName} is an Impostor, handing impostor role", ClassicLogger.LogFlags.Warn);
                    resultRole = impostorRoles.Random();
                    impostorRoles.Remove(resultRole);
                }
                else
                {
                    ClassicLogger.Log($"{player.Data.PlayerName} is a Crewmate, handing crew or neutral role...", ClassicLogger.LogFlags.Warn);
                    resultRole = combined.Random();
                    combined.Remove(resultRole);
                }

                ClassicLogger.Log($"{player.Data.PlayerName} got: {resultRole}", ClassicLogger.LogFlags.Warn);
                player.RpcSetModdedRole((int)resultRole);
            }

            // When all roles are handed, select modifiers
            ModifierManager.SelectModifiers();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(RoleManager.SelectRoles))]
    private static void SelectRolesPrefix()
    {
        // Disabling all the vanilla roles
        GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.SetRoleRate(AmongUs.GameOptions.RoleTypes.Engineer, 0, 0);
        GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.SetRoleRate(AmongUs.GameOptions.RoleTypes.Scientist, 0, 0);
        GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.SetRoleRate(AmongUs.GameOptions.RoleTypes.GuardianAngel, 0, 0);
        GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.SetRoleRate(AmongUs.GameOptions.RoleTypes.Tracker, 0, 0);
        GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.SetRoleRate(AmongUs.GameOptions.RoleTypes.Noisemaker, 0, 0);
        GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.SetRoleRate(AmongUs.GameOptions.RoleTypes.Phantom, 0, 0);
        GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.SetRoleRate(AmongUs.GameOptions.RoleTypes.Shapeshifter, 0, 0);
    }
}