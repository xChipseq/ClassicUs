using ClassicUs.Objects.Abilities;
using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.CustomProtectionManager;

namespace ClassicUs.Roles.Crewmates.Cleric;

[RegisterCustomAbility]
public class ClericBarrier : CustomAbilityButton<PlayerControl>
{
    public override string Name => "Barrier";
    public override float Cooldown => 10;
    public override bool InfiniteUses => false;
    public override int StartUses => 1;
    public override Sprite Sprite => ResourceLoader.LoadSprite("ClassicUs.Assets.PlaceholderButton.png");
    public override Color OutlineColor => ClassicPalette.Cleric.GetColor();

    public override bool Enabled()
    {
        return Player.Is<ClericRole>();
    }

    protected override void OnClick()
    {
        Player.RpcCustomProtectPlayer(Target, ProtectionType.UntilMeeting, ClassicPalette.Cleric.GetColor(), showMurderAttempt: true, removeOnMurderAttempt: true);
        Player.GetModdedRole<ClericRole>().CurrentProtection = Target;
    }

    public override void MeetingEnd()
    {
        base.MeetingEnd();
        SetUses(1);
    }

    public override void HudUpdate(HudManager hudManager)
    {
        var currentProtection = Player.GetModdedRole<ClericRole>().CurrentProtection;
        Button.usesRemainingSprite.gameObject.SetActive(false);

        currentProtection.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(Helpers.HexToColor("#4cfc4f")));
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player == currentProtection || player == Target) continue;
            player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(Helpers.HexToColor("#4cfc4f")));
        }
    }
}