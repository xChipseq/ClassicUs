using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using Cpp2IL.Core.Extensions;
using HarmonyLib;
using Newtonsoft.Json.Converters;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ClassicUs.GameOptions;

public static class OptionsManager
{
    public static readonly List<string> ModRoleListPaths = new()
    {
        "ClassicUs.Assets.RoleLists.Vanilla.json",
        "ClassicUs.Assets.RoleLists.AllAny.json",
    };
    public static readonly List<RoleList> ModRoleLists = new();
    public static RoleList CurrentRoleList;

    // H&S
    public static bool HNSBetterKillButton = false;
    public static bool HNSMoveOrDie = false;

    public static void Initialize()
    {

        string modFolderPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "ClassicUs");
        string userRoleListsPath = Path.Combine(modFolderPath, "Role Lists");
        string userGameOptionsPath = Path.Combine(modFolderPath, "Game Options");

        // Create the mod folders if it doesn't already exist
        if (!Directory.Exists(modFolderPath))
        {
            Directory.CreateDirectory(modFolderPath);
        }
        if (!Directory.Exists(userRoleListsPath))
        {
            Directory.CreateDirectory(userRoleListsPath);
        }
        if (!Directory.Exists(userGameOptionsPath))
        {
            Directory.CreateDirectory(userGameOptionsPath);
        }

        LoadModRoleLists();
        CurrentRoleList = ModRoleLists[0];
    }

    public static void LoadModRoleLists()
    {
        List<RoleList> roleLists = new();
        foreach (string path in ModRoleListPaths)
        {
            string jsonString = ResourceLoader.LoadJson(path);
            RoleList roleList = JsonSerializer.Deserialize<RoleList>(jsonString);

            foreach (string roleString in roleList.roles)
            {
                bool success = Enum.TryParse<ModdedRoles>(roleString, true, out ModdedRoles result);
                if (success)
                {
                    roleList.Roles.Add(result);
                }
            }
            foreach (string impostorString in roleList.impostors)
            {
                bool success = Enum.TryParse<ModdedRoles>(impostorString, true, out ModdedRoles result);
                if (success)
                {
                    roleList.Impostors.Add(result);
                }
            }

            // Color tags
            if (roleList.color.ToLower() == "$crewmate")
            {
                roleList.color = "#" + Palette.CrewmateBlue.ToHtmlStringRGBA();
            }
            else if (roleList.color.ToLower() == "$impostor")
            {
                roleList.color = "#" + Palette.ImpostorRed.ToHtmlStringRGBA();
            }

            roleLists.Add(roleList);
            ClassicLogger.Log($"Loaded {roleList.name} role list");
        }

        ModRoleLists.Clear();
        ModRoleLists.AddRange(roleLists);
    }

    [MethodRpc((uint)RpcCalls.ModRoleListChange, LocalHandling = RpcLocalHandling.None)]
    public static void RpcModRoleListChange(this PlayerControl player, int roleList)
    {
        CurrentRoleList = ModRoleLists[roleList];
        ClassicLogger.Log($"(RPC) Role list change: {CurrentRoleList.name} (mod)");
    }
    public static void RpcModRoleListChange(this PlayerControl player, RoleList roleList)
    {
        RpcModRoleListChange(player, ModRoleLists.IndexOf(roleList));
    }

    public static List<ModdedRoles> GetRolesFromCurrentRoleList(RoleTeam team = RoleTeam.Crewmate)
    {
        List<ModdedRoles> list = new();
        List<ModdedRoles> selectedRoles = new();

        int index = 1;
        int impostors = 0;
        int players = PlayerControl.AllPlayerControls.Count;
        if (team != RoleTeam.Impostor)
        {
            foreach (var role in CurrentRoleList.Roles)
            {
                if (!ModdedRoleManager.RoleListEntries.ContainsKey(role))
                {
                    if (ModdedRoleManager.RoleTypes.ContainsKey(role))
                    {
                        var tempRole = (ModdedRole)Activator.CreateInstance(ModdedRoleManager.RoleTypes[role]);

                        if (tempRole.Team == team)
                        {
                            list.Add(role);
                            selectedRoles.Add(role);
                            index++;
                            if (index > players) break;
                            continue;
                        }
                    }
                }
                else
                {
                    var entry = ModdedRoleManager.RoleListEntries[role];

                    if (role == ModdedRoles.Any)
                    {
                        var random = team switch { RoleTeam.Crewmate => ModdedRoles.RandomCrewmate, RoleTeam.Neutral => ModdedRoles.RandomNeutral, RoleTeam.Apocalypse => ModdedRoles.RandomApocalypse };
                        list.Add(random);
                    }
                    else if (entry.RoleTeam == team)
                    {
                        list.Add(role);
                    }

                    index++;
                    if (index > players) break;
                    continue;
                }
            }
        }
        else
        {
            foreach (var role in CurrentRoleList.Impostors)
            {
                if (impostors >= GameOptionsManager.Instance.currentGameOptions.GetAdjustedNumImpostors(players)) break;
                if (role == ModdedRoles.Any)
                {
                    list.Add(ModdedRoles.RandomImpostor);
                }
                else
                {
                    list.Add(role);
                }
                index++;
                impostors++;
                if (index > players) break;
            }
        }

        ClassicLogger.Log($"Roles that need to be categorized: ({team})");
        var stringRoles = new List<string>();
        foreach (var r in list) stringRoles.Add(r.ToString());
        ClassicLogger.Log(JsonSerializer.Serialize(list));

        var list2 = new List<ModdedRoles>(list);
        list.Clear();
        foreach (var role in list2)
        {
            if (ModdedRoleManager.RoleListEntries.ContainsKey(role))
            {
                var entry = ModdedRoleManager.RoleListEntries[role];
                var possibleRoles = ModdedRoleManager.GetAllRolesForTeam(entry.RoleTeam.Value, entry.RoleAlignment);
                ModdedRoles random = possibleRoles.Random(); 
                while (selectedRoles.Contains(random) && !CurrentRoleList.allowDuplicates) random = possibleRoles.Random();
                list.Add(random);
            }
            else
            {
                list.Add(role);
            }
        }

        ClassicLogger.Log($"Roles that will be handed: ({team})");
        stringRoles.Clear();
        foreach (var r in list) stringRoles.Add(r.ToString());
        ClassicLogger.Log(JsonSerializer.Serialize(stringRoles));
        return list;
    }
}

public class RoleList
{
    public string name { get; set; }
    public string color { get; set; } = "#ffffff";
    public string version { get; set; }

    public bool allowDuplicates { get; set; }

    public List<ModdedRoles> Roles { get; } = new();
    public string[] roles { get; set; }

    public List<ModdedRoles> Impostors { get; } = new();
    public string[] impostors { get; set; }
}

public struct RoleListEntry
{
    public string Name { get; }
    public Color Color { get; }

    public RoleAlignment? RoleAlignment = null;
    public RoleTeam? RoleTeam = null;

    public RoleListEntry(string name, Color color, RoleTeam? team = null, RoleAlignment? alignment = null)
    {
        Name = name;
        Color = color;

        if (team.HasValue) RoleTeam = team.Value;
        if (alignment.HasValue) RoleAlignment = alignment.Value;
    }
}

public abstract class OptionsBase;
public abstract class RoleOptions : OptionsBase;