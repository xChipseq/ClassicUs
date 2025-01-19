using System;
using ClassicUs.Objects.Roles;
using ClassicUs.GameOptions;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Crewmates.Cleric;

public class ClericRole : ModdedRole
{
    public override string Name => "Cleric";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Crewmate;
    public override RoleAlignment Alignment => RoleAlignment.Protective;
    public override ColorGradient Color => ClassicPalette.Cleric;
    public override string IntroText => "Heal the crew";
    public override string TaskText => "Protect crew members from attacks.";
    public override RoleConfig Config => new RoleConfig()
    {
        CanVent = false,
        CanMoveInVents = false,
        CanSabotage = false,
        HasKillButton = false,
        HasImpostorVision = false,
        CanDoTasks = true,
        TasksCountForProgress = true,
    };

    public override Type Options => typeof(ClericOptions);

    public PlayerControl CurrentProtection = null;
}

public class ClericOptions : RoleOptions
{
    public float BarrierCooldown = 10;
}