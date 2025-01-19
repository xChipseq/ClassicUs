using System.Collections;
using System.Linq;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ClassicUs.Utilities;
#pragma warning disable CS8632

public static class LORMurder
{
    [MethodRpc((uint)RpcCalls.CustomMurder)]
    public static void RpcCustomMurder(
        this PlayerControl source,
        PlayerControl target,
        bool ignoreProtection = false,
        bool resetKillTimer = true,
        bool createDeadBody = true,
        bool teleportMurderer = true,
        bool showKillAnim = true,
        bool playKillSound = true
    )
    {
        CustomMurderPlayer(source, target, ignoreProtection, resetKillTimer, createDeadBody, teleportMurderer, showKillAnim, playKillSound);
    }

    public static void CustomMurderPlayer(
        this PlayerControl source,
        PlayerControl target,
        bool ignoreProtection = false,
        bool resetKillTimer = true,
        bool createDeadBody = true,
        bool teleportMurderer = true,
        bool showKillAnim = true,
        bool playKillSound = true
    )
    {
        source.isKilling = false;
        ClassicLogger.Log(string.Format("{0} trying to murder {1}", source.PlayerId, target.PlayerId));
        NetworkedPlayerInfo data = target.Data;

        if (target.IsCustomProtected() && !ignoreProtection)
        {
            //target.protectedByGuardianThisRound = true;
            var protectionData = target.GetCustomProtection();
            if (source.AmOwner || (protectionData.ShowMurderAttempt && target.AmOwner))
            {
                target.ShowCustomFailedMurder();
            }
            if (source.AmOwner && resetKillTimer) source.SetKillTimer(protectionData.KillCooldownReset);
            if (protectionData.RemoveOnMurderAttempt) target.RpcRemoveCustomProtection();

            ClassicLogger.Log(string.Format("{0} failed to murder {1} due to custom protection by {2}", source.PlayerId, target.PlayerId, protectionData.Source));
            return;
        }

        DestroyableSingleton<DebugAnalytics>.Instance.Analytics.Kill(target.Data, source.Data);
        if (source.AmOwner)
        {
            StatsManager.Instance.IncrementStat(StringNames.StatsImpostorKills);
            if (source.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
            {
                StatsManager.Instance.IncrementStat(StringNames.StatsShapeshifterShiftedKills);
            }
            if (Constants.ShouldPlaySfx() && playKillSound)
            {
                SoundManager.Instance.PlaySound(source.KillSfx, false, 0.8f, null);
            }
            if (resetKillTimer)
            {
                float killCd = GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
                source.SetKillTimer(killCd);
            }
        }
        DestroyableSingleton<UnityTelemetry>.Instance.WriteMurder();
        target.gameObject.layer = LayerMask.NameToLayer("Ghost");
        if (target.AmOwner)
        {
            StatsManager.Instance.IncrementStat(StringNames.StatsTimesMurdered);
            if (Minigame.Instance)
            {
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }
            }
            if (showKillAnim) DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(source.Data, data);
            target.cosmetics.SetNameMask(false);
            target.RpcSetScanner(false);
        }
        DestroyableSingleton<AchievementManager>.Instance.OnMurder(source.AmOwner, target.AmOwner, source.CurrentOutfitType == PlayerOutfitType.Shapeshifted, source.shapeshiftTargetPlayerId, (int)target.PlayerId);
        source.MyPhysics.StartCoroutine(source.KillAnimations.Random()?.CoPerformCustomKill(source, target, createDeadBody, teleportMurderer));
        ClassicLogger.Log(string.Format("{0} succeeded in murdering {1}", source.PlayerId, target.PlayerId));
    }

    public static IEnumerator CoPerformCustomKill(
        this KillAnimation anim,
        PlayerControl source,
        PlayerControl target,
        bool createDeadBody = true,
        bool teleportMurderer = true)
    {
        var cam = Camera.main?.GetComponent<FollowerCamera>();
        var isParticipant = PlayerControl.LocalPlayer == source || PlayerControl.LocalPlayer == target;
        var sourcePhys = source.MyPhysics;

        target.GetPet().gameObject.Destroy();

        if (teleportMurderer)
        {
            KillAnimation.SetMovement(source, false);
        }

        KillAnimation.SetMovement(target, false);

        if (isParticipant)
        {
            PlayerControl.LocalPlayer.isKilling = true;
            source.isKilling = true;
        }

        DeadBody? deadBody = null;

        if (createDeadBody)
        {
            deadBody = UnityEngine.Object.Instantiate(GameManager.Instance.DeadBodyPrefab);
            deadBody.enabled = false;
            deadBody.ParentId = target.PlayerId;
            deadBody.bodyRenderers.ToList().ForEach(target.SetPlayerMaterialColors);

            target.SetPlayerMaterialColors(deadBody.bloodSplatter);
            var vector = target.transform.position + anim.BodyOffset;
            vector.z = vector.y / 1000f;
            deadBody.transform.position = vector;
        }

        if (isParticipant)
        {
            if (cam != null)
            {
                cam.Locked = true;
            }

            ConsoleJoystick.SetMode_Task();
            if (PlayerControl.LocalPlayer.AmOwner)
            {
                PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
            }
        }

        target.Die(DeathReason.Kill, true);
        yield return source.MyPhysics.Animations.CoPlayCustomAnimation(anim.BlurAnim);
        sourcePhys.Animations.PlayIdleAnimation();

        if (teleportMurderer)
        {
            source.NetTransform.SnapTo(target.transform.position);
            KillAnimation.SetMovement(source, true);
        }

        KillAnimation.SetMovement(target, true);

        if (deadBody != null)
        {
            deadBody.enabled = true;
        }

        if (!isParticipant)
        {
            yield break;
        }

        if (cam != null)
        {
            cam.Locked = false;
        }

        PlayerControl.LocalPlayer.isKilling = false;
        source.isKilling = false;
    }
}