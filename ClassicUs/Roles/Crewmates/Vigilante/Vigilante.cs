using ClassicUs.Objects.Roles;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Crewmates.Vigilante;

public class VigilanteRole : ModdedRole
{
    public override string Name => "Vigilante";
    public override RoleTeam Team { get; protected set; } = RoleTeam.Crewmate;
    public override RoleAlignment Alignment => RoleAlignment.Killing;
    public override ColorGradient Color => ClassicPalette.Vigilante;
    public override string IntroText => "Take justice into your own hands";
    public override string TaskText => "Kill suspicious people.";
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