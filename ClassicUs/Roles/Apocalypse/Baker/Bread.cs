using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;

namespace ClassicUs.Roles.Apocalypse.Baker;

[RegisterCustomAbility]
public class BakerBread : CustomAbilityButton<PlayerControl>
{
    public override string Name => "Bread";
    public override float Cooldown => 15;
    public override bool InfiniteUses => true;
    public override int StartUses => 0;
    public override Sprite Sprite => ResourceLoader.LoadSprite("ClassicUs.Assets.Bread.png");
    public override Color OutlineColor => ClassicPalette.ApocalypseColor;

    public override bool Enabled()
    {
        return Player.Is<BakerRole>();
    }

    protected override void OnClick()
    {
        var baker = Player.GetModdedRole<BakerRole>();
        baker.PeopleWithBread.Add(Target);
        baker.CheckForFamine();
    }
}