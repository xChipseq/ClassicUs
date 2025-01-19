using System.Linq;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Neutral.SerialKiller;

public class SerialKillerRole : ModdedRole
{
    public override string Name => "Serial Killer";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Neutral;
    public override RoleAlignment Alignment => RoleAlignment.Killing;
    public override ColorGradient Color => ClassicPalette.SerialKiller;
    public override string IntroText => "Use your bloodlust to kill";
    public override string TaskText => "Kill and be the last one standing.";

    public int KillsToBloodlust = 2;

    public override RoleConfig Config => new RoleConfig()
    {
        CanVent = false,
        CanMoveInVents = false,
        CanSabotage = false,
        HasKillButton = false,
        HasImpostorVision = true,
        CanDoTasks = false,
        TasksCountForProgress = false,
    };

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)ModdedGameOverReasons.SerialKillerWin;
    }

    public override void CheckGameEnd()
    {
        if (!Player.Data.IsDead && !Player.Data.Disconnected)
        {
            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(p => !p.AmOwner && !p.Data.IsDead && !p.Data.Disconnected).ToList();

            if (allPlayers.Count == 0) GameManager.Instance.RpcEndGame((GameOverReason)ModdedGameOverReasons.SerialKillerWin, false);
        }
    }
}