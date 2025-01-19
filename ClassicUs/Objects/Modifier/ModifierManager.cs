using System;
using System.Collections.Generic;
using ClassicUs.Modifiers.Flash;
using HarmonyLib;
using Reactor.Networking.Attributes;
using UnityEngine;

namespace ClassicUs.Objects.Modifiers;

public static class ModifierManager
{
    public static readonly Dictionary<PlayerControl, ModdedModifier> PlayerModifiers = new();
    public static readonly Dictionary<PlayerControl, WinConditionModifier> PlayerWinConModifiers = new();

    public static readonly Dictionary<ModdedModifiers, Type> ModifierTypes = new(){
        { ModdedModifiers.Flash, typeof(FlashModifier) },
    };


    [MethodRpc((uint)RpcCalls.SetModifier)]
    public static void RpcSetModifier(this PlayerControl player, int modifierType)
    {
        var type = ModifierTypes[(ModdedModifiers)modifierType];
        if (type == typeof(WinConditionModifier))
        {
            PlayerWinConModifiers[player] = (WinConditionModifier)Activator.CreateInstance(type);
        }
        else
        {
            PlayerModifiers[player] = (ModdedModifier)Activator.CreateInstance(type);
        }

    }
    public static void RpcSetModifier(this PlayerControl player, ModdedModifiers modifierType)
    {
        player.RpcSetModifier((int)modifierType);
    }

    [MethodRpc((uint)RpcCalls.ModifierSelection)]
    public static void RpcModifierSelection(this PlayerControl host)
    {
        PlayerModifiers.Clear();
        PlayerWinConModifiers.Clear();
    }

    public static ModdedModifier GetModifier(this PlayerControl player)
    {
        if (PlayerModifiers.ContainsKey(player))
            return PlayerModifiers[player];
        else
            return null;
    }
    public static T GetModifier<T>(this PlayerControl player) where T : ModdedModifier
    {
        if (PlayerModifiers.ContainsKey(player))
            return (T)PlayerModifiers[player];
        else
            return null;
    }

    public static WinConditionModifier GetWinConModifier(this PlayerControl player)
    {
        if (PlayerWinConModifiers.ContainsKey(player))
            return PlayerWinConModifiers[player];
        else
            return null;
    }
    public static T GetWinConModifier<T>(this PlayerControl player) where T : WinConditionModifier
    {
        if (PlayerWinConModifiers.ContainsKey(player))
            return (T)PlayerWinConModifiers[player];
        else
            return null;
    }

    public static void SelectModifiers()
    {
        RpcModifierSelection(PlayerControl.LocalPlayer);
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            
        }
    }
}