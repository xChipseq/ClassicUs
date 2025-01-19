using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Crewmates.Shrimpmate;

public class ShrimpmateRole : ModdedRole
{
    public override string Name => "Shrimpmate";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Crewmate;
    public override RoleAlignment Alignment => RoleAlignment.Power;
    public override ColorGradient Color => new ColorGradient(Helpers.HexToColor("#00ff87"), Helpers.HexToColor("#60efff"));
    public override string IntroText => "shrimp around";
    public override string TaskText => "shrimpmate always comes back";

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