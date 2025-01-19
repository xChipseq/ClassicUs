using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;

namespace ClassicUs.Roles.Neutral.SerialKiller;

[RegisterCustomAbility]
public class SerialKillerStab : CustomAbilityButton<PlayerControl>
{
    public override string Name => "Stab";
    public override float Cooldown => 30;
    public override bool InfiniteUses => true;
    public override int StartUses => 0;
    public override Sprite Sprite => ResourceLoader.LoadSprite("ClassicUs.Assets.PlaceholderButton.png");
    public override Color OutlineColor => ClassicPalette.SerialKiller.GetColor();

    public override bool Enabled()
    {
        return Player.Is<SerialKillerRole>();
    }

    protected override void OnClick()
    {
        Player.RpcCustomMurder(Target, false, false);

        var sk = Player.GetModdedRole<SerialKillerRole>();
        sk.KillsToBloodlust--;
        if (sk.KillsToBloodlust == 0)
        {
            ResetCooldown(0);
            return;
        }
        else if (sk.KillsToBloodlust == 1)
        {
            Button.OverrideText("Bloodlust");
            Button.OverrideColor(new Color(1, 0.8f, 0.8f));
        }
        else
        {
            Button.OverrideText("Kill");
            Button.OverrideColor(new Color(1, 1, 1));
        }

        ResetCooldown(GameOptionsManager.Instance.currentGameOptions.GetFloat(AmongUs.GameOptions.FloatOptionNames.KillCooldown));
    }
}