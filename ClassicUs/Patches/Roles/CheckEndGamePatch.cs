using System;
using ClassicUs.Objects.Roles;
using HarmonyLib;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
[HarmonyPriority(Priority.Last)]
public static class CheckEndGamePatch
{
    public static bool Prefix(LogicGameFlowNormal __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return false;
        if (ShipStatus.Instance.Systems != null)
        {
            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
            {
                var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (lifeSuppSystemType.Countdown < 0f) return true;
            }

            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
            {
                var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                if (reactorSystemType.Countdown < 0f) return true;
            }

            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
            {
                var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                if (reactorSystemType.Countdown < 0f) return true;
            }
        }

        GameManager.Instance.CheckEndGameViaTasks();

        int playersAlive = 0;
        int impostorsAlive = 0;
        int playersPreventingGameEnd = 0;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.Data.IsDead && !player.Data.Disconnected)
            {
                playersAlive++;
                var role = player.GetModdedRole();
                if (role == null) continue;
                if (role.Team == RoleTeam.Impostor) impostorsAlive++;
                if (role.PreventsGameEnd()) playersPreventingGameEnd++;
            }
        }

        if (impostorsAlive > 0 && playersAlive - impostorsAlive == 0)
        {
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorByKill, false);
        }

        if (playersPreventingGameEnd == 0)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsDead && !player.Data.Disconnected)
                {
                    player.GetModdedRole()?.CheckGameEnd();
                }
            }
        }

        return impostorsAlive == 0 && playersPreventingGameEnd == 0;
    }
}