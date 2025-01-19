using System;
using System.Collections.Generic;
using System.Linq;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Apocalypse.Baker;

public class FamineRole : ModdedRole
{
    public override string Name => "Famine";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Apocalypse;
    public override RoleAlignment Alignment => RoleAlignment.None;
    public override ColorGradient Color => new ColorGradient(ClassicPalette.TransformedApocalypseColor);
    public override string IntroText => "";
    public override string TaskText => "Starve people to death.";
    public override bool Generates => false;
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

    public override Type Options => typeof(BakerOptions);

    public Dictionary<PlayerControl, int> Breads = new();

    public void Starve(PlayerControl target)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.GetTeam() == RoleTeam.Apocalypse) continue;

            Breads[player] -= player == target ? 3 : 1;
            if (Breads[player] <= 0)
            {
                Player.RpcCustomMurder(player, true, false, teleportMurderer: false);
            }
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)ModdedGameOverReasons.ApocalypseWin;
    }
}