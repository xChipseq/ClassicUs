using System;
using System.Collections.Generic;
using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ClassicUs.Utilities;

/// <summary>
///     Custom protections system
/// </summary>
public static class CustomProtectionManager
{
    public enum ProtectionType { Timed, UntilMeeting, Infinite }

    /// <summary>
    ///     Custom protection class     
    /// </summary>
    public class CustomProtection
    {
        /// <summary>
        /// Person who gave the protection
        /// </summary>
        public PlayerControl Source { get; private set; }

        /// <summary>
        /// Type of protection
        /// </summary>
        public ProtectionType Type { get; private set; }

        /// <summary>
        /// Should the player be invincible - ignoreProtection in custom murder won't work
        /// </summary>
        public bool Invincible { get; set; } = false;

        /// <summary>
        /// Should the player see murder attempt on them
        /// </summary>
        public bool ShowMurderAttempt { get; set; } = true;

        /// <summary>
        /// Color of the animation's shield
        /// </summary>
        public Color ProtectionColor { get; private set; }

        /// <summary>
        /// Killer's kill cooldown after murder attempt
        /// </summary>
        public float KillCooldownReset { get; set; }

        /// <summary>
        /// Should the protection be removed on murder attempt
        /// </summary>
        public bool RemoveOnMurderAttempt { get; set; } = false;

        public float Timer = 0;

        public CustomProtection(PlayerControl source, ProtectionType type, Color protectionColor, float duration = 0, float kcReset = -1, bool invincible = false, bool showMurderAttempt = false, bool removeOnMurderAttempt = false)
        {
            Source = source;
            Type = type;
            ProtectionColor = protectionColor;
            Timer = duration;

            if (kcReset == -1)
            {
                KillCooldownReset = GameOptionsManager.Instance.currentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
            }
        }
    }


    public static readonly Dictionary<byte, CustomProtection> Protections = new();

    [MethodRpc((uint)RpcCalls.CustomProtection)]
    public static void RpcCustomProtectPlayer(this PlayerControl source, PlayerControl target, int type, string protectionColor, float duration = 0, float kcReset = -1, bool invincible = false, bool showMurderAttempt = false, bool removeOnMurderAttempt = false)
    {
        ColorUtility.TryParseHtmlString(protectionColor, out Color color);
        var protection = new CustomProtection(source, (ProtectionType)type, color, duration, kcReset, invincible, showMurderAttempt, removeOnMurderAttempt);

        Protections.Add(target.Data.PlayerId, protection);
    }
    public static void RpcCustomProtectPlayer(this PlayerControl source, PlayerControl target, ProtectionType type, Color protectionColor, float duration = 0, float kcReset = -1, bool invincible = false, bool showMurderAttempt = false, bool removeOnMurderAttempt = false)
    {
        var colorString = ColorUtility.ToHtmlStringRGB(protectionColor);

        RpcCustomProtectPlayer(source, target, (int)type, colorString, duration, kcReset);
    }

    [MethodRpc((uint)RpcCalls.RemoveCustomProtection)]
    public static void RpcRemoveCustomProtection(this PlayerControl player)
    {
        if (Protections.ContainsKey(player.Data.PlayerId))
        {
            Protections.Remove(player.Data.PlayerId);
        }
        else
        {
            ClassicLogger.Log($"{player.Data.PlayerName} is not protected, can't remove protection from them", ClassicLogger.LogFlags.Warn);
        }
    }

    public static bool IsCustomProtected(this PlayerControl player)
    {
        if (Protections.ContainsKey(player.Data.PlayerId))
        {
            return true;
        }
        return false;
    }

    public static CustomProtection GetCustomProtection(this PlayerControl player)
    {
        if (Protections.ContainsKey(player.Data.PlayerId))
        {
            return Protections[player.Data.PlayerId];
        }
        return null;
    }

    public static void ShowCustomFailedMurder(this PlayerControl player)
    {
        RoleEffectAnimation roleEffectAnimation = UnityEngine.Object.Instantiate<RoleEffectAnimation>(DestroyableSingleton<RoleManager>.Instance.protectAnim, player.gameObject.transform);
        roleEffectAnimation.SetMaskLayerBasedOnWhoShouldSee(true);
        roleEffectAnimation.Play(player, null, player.cosmetics.FlipX, RoleEffectAnimation.SoundType.Global, 0f, true, 0f);
    }

    public static void UpdateProtections(bool isMeeting = false)
    {
        foreach (var protection in Protections)
        {
            if (protection.Value.Timer >= 0 && protection.Value.Type == ProtectionType.Timed)
            {
                protection.Value.Timer -= Time.deltaTime;
            }

            if (protection.Value.Type == ProtectionType.UntilMeeting && isMeeting)
            {
                Protections.Remove(protection.Key);
                continue;   
            }
            if (protection.Value.Type == ProtectionType.Timed && protection.Value.Timer <= 0)
            {
                Protections.Remove(protection.Key);
                continue;
            }
        }
    }
}

[HarmonyPatch(typeof(RoleEffectAnimation), nameof(RoleEffectAnimation.Play))]
public static class RoleEffectAnimationColorPatch
{
    public static void Postfix(RoleEffectAnimation __instance, ref PlayerControl parent)
    {
        if (__instance.effectType == RoleEffectAnimation.EffectType.ProtectLoop)
        {
            var protectData = parent.GetCustomProtection();
            if (protectData != null)
            {
                __instance.Renderer.color = protectData.ProtectionColor;
            }
        }
    }
}