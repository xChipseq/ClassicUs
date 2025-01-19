using System.Collections.Generic;
using System.Linq;
using ClassicUs.Objects.Roles;
using HarmonyLib;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
public static class GameEndPatch
{
    public static void Postfix()
    {
        EndGameResult.CachedWinners.Clear();
        var players = PlayerControl.AllPlayerControls.ToArray().Where(p => !p.Data.Disconnected && !p.Data.IsDead);
        foreach (var player in players)
        {
            var role = player.GetModdedRole();
            if (role == null) continue;
            
            if (role.DidWin(EndGameResult.CachedGameOverReason))
            {
                EndGameResult.CachedWinners.Add(new CachedPlayerData(player.Data));
            }
        }
        
    }
}