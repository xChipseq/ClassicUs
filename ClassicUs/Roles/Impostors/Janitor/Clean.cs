using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;

namespace ClassicUs.Roles.Impostors.Janitor;

[RegisterCustomAbility]
public class JanitorClean : CustomAbilityButton<DeadBody>
{
    public override string Name => "Clean";
    public override float Cooldown => 30;
    public override bool InfiniteUses => false;
    public override int StartUses => 0;
    public override Sprite Sprite => ResourceLoader.LoadSprite("ClassicUs.Assets.PlaceholderButton.png");
    public override Color OutlineColor => Palette.ImpostorRed;

    public override bool Enabled()
    {
        return Player.Is<JanitorRole>();
    }

    protected override void OnClick()
    {
        CustomRpc.RpcCleanBody(Player, Target.ParentId);
    }
}