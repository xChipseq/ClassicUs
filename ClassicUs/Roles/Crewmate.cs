using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles;

public class ModdedCrewmate : ModdedRole
{
    public override string Name => "Crewmate";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Crewmate;
    public override RoleAlignment Alignment => RoleAlignment.None;
    public override ColorGradient Color => new ColorGradient(Palette.CrewmateBlue);
    public override string IntroText => "Do your tasks";
    public override string TaskText => string.Empty;
    public override bool Hidden => true;
    public override bool Generates => false;

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
}