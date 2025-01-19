using ClassicUs.Objects.Modifiers;
using ClassicUs.Utilities;

namespace ClassicUs.Modifiers.Flash;

public class FlashModifier : ModdedModifier
{
    public override string Name => "Flash";
    public override ModifierTeam Team { get; } = ModifierTeam.Global;
    public override string TaskText => "Gotta go fast";

    public float StartSpeed = 0;

    public override void PlayerFixedUpdate(PlayerControl player)
    {
        if (StartSpeed == 0) StartSpeed = player.MyPhysics.Speed;
        player.MyPhysics.Speed = StartSpeed * 1.5f;
    }
}