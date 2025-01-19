using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Neutral.Jester;

public class JesterRole : ModdedRole
{
    public override string Name => "Jester";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Neutral;
    public override RoleAlignment Alignment => RoleAlignment.Evil;
    public override ColorGradient Color => ClassicPalette.Jester;
    public override string IntroText => "Fool everyone and make them vote you";
    public override string TaskText => "Get voted out.";

    public override RoleConfig Config => new RoleConfig()
    {
        CanVent = true,
        CanMoveInVents = false,
        CanSabotage = false,
        HasKillButton = false,
        HasImpostorVision = false,
        CanDoTasks = false,
        TasksCountForProgress = false,
    };

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)ModdedGameOverReasons.JesterWin; 
    }

    public override void OnDeath(DeathReason reason)
    {
        if (reason == DeathReason.Exile)
        {
            GameManager.Instance.RpcEndGame((GameOverReason)ModdedGameOverReasons.JesterWin, false);
        }
    }
}