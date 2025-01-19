using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Neutral.Pirate;

public class PirateRole : ModdedRole
{
    public override string Name => "Pirate";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Neutral;
    public override RoleAlignment Alignment => RoleAlignment.Chaos;
    public override ColorGradient Color => ClassicPalette.Pirate;
    public override string IntroText => "Prowl for plunder amongst those who hold riches";
    public override string TaskText => "Loot 3 of your targets to win.";

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
        return false; //gameOverReason == (GameOverReason)ModdedGameOverReasons.JesterWin; 
    }

    public override void OnDeath(DeathReason reason)
    {
        if (reason == DeathReason.Exile)
        {
            GameManager.Instance.RpcEndGame((GameOverReason)ModdedGameOverReasons.JesterWin, false);
        }
    }
}