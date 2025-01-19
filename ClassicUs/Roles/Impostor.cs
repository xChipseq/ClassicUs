using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles;

public class ModdedImpostor : ModdedRole
{
    public override string Name => "Impostor";
    public override RoleTeam Team { get; protected set;} = RoleTeam.Impostor;
    public override RoleAlignment Alignment => RoleAlignment.None;
    public override ColorGradient Color => new ColorGradient(Palette.ImpostorRed);
    public override string IntroText => "Sabotage and kill";
    public override string TaskText => "Sabotage and kill everyone";
    public override bool Hidden => true;
    public override bool Generates => false;

    public override RoleConfig Config => new RoleConfig()
    {
        CanVent = true,
        CanMoveInVents = true,
        CanSabotage = true,
        HasKillButton = true,
        HasImpostorVision = true,
        CanDoTasks = false,
        TasksCountForProgress = false,
    };
}