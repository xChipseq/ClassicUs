using System;
using System.Collections.Generic;
using ClassicUs.GameOptions;
using ClassicUs.Objects.Roles;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Apocalypse.Baker;

public class BakerRole : ModdedRole
{
    public override string Name => "Baker";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Apocalypse;
    public override RoleAlignment Alignment => RoleAlignment.None;
    public override ColorGradient Color => new ColorGradient(ClassicPalette.ApocalypseColor);
    public override string IntroText => "Let the famine begin";
    public override string TaskText => "Hand bread to 4 people to transfrom into Famine.";
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

    public List<PlayerControl> PeopleWithBread = new();

    public override string GetPrefix(PlayerControl player)
    {
        return PeopleWithBread.Contains(player) ? " <color=#f2d541>(^)</color>" : "";
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return gameOverReason == (GameOverReason)ModdedGameOverReasons.ApocalypseWin;
    }

    public void CheckForFamine()
    {
        // Removing dead people
        foreach (var player in PeopleWithBread) { if (player.Data.IsDead || player.Data.Disconnected) PeopleWithBread.Remove(player); }
        
        if (PeopleWithBread.Count >= 4)
        {
            Player.RpcApocalypseTransform((int)ModdedRoles.Famine, [ PeopleWithBread ]);
        }
    }
}

public class BakerOptions : RoleOptions
{
    public float BreadCooldown = 25;
    public float StarveCooldown = 30;
    public int BreadsToTransform = 4;
}