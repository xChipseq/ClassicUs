using System.Collections;
using System.Linq;
using ClassicUs.Objects.Roles;
using Reactor.Utilities.Extensions;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Roles.Impostors.Janitor;

public class JanitorRole : ModdedRole
{
    public override string Name => "Janitor";
    public override RoleTeam Team { get; protected set; } = RoleTeam.Impostor;
    public override RoleAlignment Alignment => RoleAlignment.Support;
    public override ColorGradient Color => new ColorGradient(Palette.ImpostorRed);
    public override string IntroText => "Get rid of evidence";
    public override string TaskText => "Clean dead bodies.";
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

    public static IEnumerator CleanCoroutine(DeadBody body)
    {
        var renderers = body.bodyRenderers.ToList();
        for (int i = 100; i <= 0; i--)
        {
            renderers.ForEach(renderer => renderer.color -= new Color(0, 0, 0, 0.01f));
            yield return new WaitForSeconds(0.01f);
        }

        body.gameObject.Destroy();
    }
}