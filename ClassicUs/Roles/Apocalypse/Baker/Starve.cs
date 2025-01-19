using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;

namespace ClassicUs.Roles.Apocalypse.Baker;

[RegisterCustomAbility]
public class FamineStarve : CustomAbilityButton<PlayerControl>
{
    public override string Name => "Starve";
    public override float Cooldown => 20;
    public override bool InfiniteUses => true;
    public override int StartUses => 0;
    public override Sprite Sprite => ResourceLoader.LoadSprite("ClassicUs.Assets.Bread.png");
    public override Color OutlineColor => ClassicPalette.TransformedApocalypseColor;

    public override bool Enabled()
    {
        return Player.Is<FamineRole>();
    }

    protected override void OnClick()
    {
        var famine = Player.GetModdedRole<FamineRole>();
        famine.Starve(Target);
    }
}