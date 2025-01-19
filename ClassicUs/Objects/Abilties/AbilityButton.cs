using ClassicUs.Utilities;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;

namespace ClassicUs.Objects.Abilities;

public abstract class CustomAbilityButton
{
    // Ability properties
    public abstract string Name { get; }
    public abstract float Cooldown { get; }
    public abstract bool InfiniteUses { get; }
    public abstract int StartUses { get; }
    public abstract Sprite Sprite { get; }

    // Systems
    public ActionButton Button { get; private set; }
    public float CooldownTimer { get; private set; }
    public int UsesLeft { get; private set; }

    public PlayerControl Player { get => PlayerControl.LocalPlayer; }

    internal void CreateButton(Transform parent)
    {
        if (Button)
            return;

        // Creating the button and setting it's propeties
        Button = GameObject.Instantiate(HudManager.Instance.AbilityButton, parent);
        Button.name = Name + "Ability";
        Button.OverrideText(Name.ToUpperInvariant());
        Button.graphic.sprite = Sprite;

        // Setting uses
        if (InfiniteUses)
        {
            Button.SetInfiniteUses();
        }
        else
        {
            Button.SetUsesRemaining(StartUses);
        }

        // Systems
        CooldownTimer = Cooldown / 2;
        UsesLeft = StartUses;

        var passiveButton = Button.GetComponent<PassiveButton>();
        passiveButton.OnClick = new ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)ClickHandler);
    }

    protected abstract void OnClick();

    public virtual bool CanUse()
    {
        return CooldownTimer <= 0 && (InfiniteUses || UsesLeft > 0) && !Player.Data.IsDead;
    }

    public virtual void HudUpdate(HudManager hudManager) { }
    public virtual void PlayerFixedUpdate(PlayerControl player) {  }
    public virtual void MeetingEnd()
    {
        CooldownTimer = Cooldown;
    }

    public abstract bool Enabled();

    public void ResetCooldown(float? cooldown = null)
    {
        if (cooldown.HasValue)
        {
            CooldownTimer = cooldown.Value;
        }
        else
        {
            CooldownTimer = Cooldown;
        }
    }

    public void ChangeUses(int number)
    {
        UsesLeft += number;
    }
    public void SetUses(int number)
    {
        UsesLeft = number;
    }

    protected virtual void ClickHandler()
    {
        if (!CanUse())
            return;

        if (!InfiniteUses)
        {
            UsesLeft--;
            Button.SetUsesRemaining(UsesLeft);
        }

        OnClick();
        CooldownTimer = Cooldown;
    }

    internal virtual void HudUpdateHandler(HudManager __instance)
    {
        if (LobbyBehaviour.Instance) return;
        if (!Button || !Enabled()) return;
        Button?.gameObject.SetActive(Enabled());

        if (CooldownTimer >= 0)
        {
            CooldownTimer -= Time.deltaTime;
            if (Player.Data.IsDead)
                CooldownTimer = 0;
        }

        if (CanUse() && !PlayerControl.LocalPlayer.Data.IsDead)
        {
            Button?.SetEnabled();
        }
        else
        {
            Button?.SetDisabled();
        }
        Button?.SetCoolDown(CooldownTimer, Cooldown);
        Button?.gameObject.SetActive(!MeetingHud.Instance);

        HudUpdate(__instance);
    }
}

public abstract class CustomAbilityButton<T> : CustomAbilityButton where T : MonoBehaviour
{
    public T? Target { get; protected set; }

    public abstract Color OutlineColor { get; }
    public virtual float Distance => PlayerControl.LocalPlayer.Data.Role.GetAbilityDistance();

    public virtual T? GetTarget()
    {
        if (typeof(T) == typeof(PlayerControl))
        {
            return PlayerControl.LocalPlayer.GetClosestPlayer(true, Distance, false) as T;
        }
        else if (typeof(T) == typeof(DeadBody))
        {
            return PlayerControl.LocalPlayer.GetClosestDeadBody(Distance) as T;
        }

        return null;
    }

    public virtual void SetOutline(bool active)
    {
        if (!Target)
            return;

        if (Target is PlayerControl)
        {
            (Target as PlayerControl).cosmetics.SetOutline(active, new Il2CppSystem.Nullable<Color>(OutlineColor));
        }
        else if (Target is DeadBody)
        {
            (Target as DeadBody).bodyRenderers[0].material.SetFloat("_Outline", active ? 1 : 0);
            (Target as DeadBody).bodyRenderers[0].material.SetColor("_OutlineColor", OutlineColor);
        }
    }

    public override bool CanUse()
    {
        var newTarget = GetTarget();
        if (newTarget != Target)
        {
            SetOutline(false);
        }

        Target = newTarget;
        if (!Player.Data.IsDead && CooldownTimer <= 0)
        {
            SetOutline(true);
        }
        return base.CanUse() && Target;
    }
}