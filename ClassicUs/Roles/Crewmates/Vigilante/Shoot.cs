using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;

namespace ClassicUs.Roles.Crewmates.Vigilante;

[RegisterCustomAbility]
public class VigilanteShoot : CustomAbilityButton<PlayerControl>
{
    public override string Name => "Shoot";
    public override float Cooldown => 30;
    public override bool InfiniteUses => false;
    public override int StartUses => 3;
    public override Sprite Sprite => ResourceLoader.LoadSprite("ClassicUs.Assets.PlaceholderButton.png");
    public override Color OutlineColor => ClassicPalette.Vigilante.GetColor();

    public override bool Enabled()
    {
        return Player.Is<VigilanteRole>();
    }

    protected override void OnClick()
    {
        if (Target.GetTeam() == RoleTeam.Crewmate)
        {
            SetUses(0);
            Player.RpcCustomMurder(PlayerControl.LocalPlayer, true, false, teleportMurderer: false, showKillAnim: false);
        }
        else
        {
            Player.RpcCustomMurder(Target, false, false);
        }
        ResetCooldown(GameOptionsManager.Instance.currentGameOptions.GetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown));
    }
}