using UnityEngine;

namespace ClassicUs.Objects.Modifiers;

public abstract class ModdedModifier
{
    public abstract string Name { get; }
    public abstract ModifierTeam Team { get; }
    public abstract string TaskText { get; }

    public virtual void HudUpdate(HudManager hudManager) { }
    public virtual void PlayerFixedUpdate(PlayerControl player) { }
    public virtual void MeetingStart(MeetingHud meetingHud) { }
    public virtual void MeetingEnd() { }
}

public abstract class WinConditionModifier : ModdedModifier
{
    public abstract Color Color { get; }
    public abstract bool DidWin(GameOverReason gameOverReason);
}