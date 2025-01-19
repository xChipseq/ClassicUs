using System;
using System.Collections.Generic;
using System.Linq;
using ClassicUs.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;

namespace ClassicUs.Objects.Abilities;

public abstract class MeetingPlayerAbilityButton
{
    // Properties
    public abstract string Name { get; }
    public abstract bool InfiniteUses { get; }
    public abstract int StartUses { get; }
    public abstract int UsesPerMeeting { get; }
    public abstract Sprite Sprite { get; }
    public abstract Color HighlightColor { get; }

    // Systems
    public Dictionary<PlayerControl, PassiveButton> Buttons { get; private set; } = new();
    private bool FirstMeeting = true;
    public int UsesLeft { get; private set; }
    public int UsesLeftThisMeeting { get; private set; }

    internal void CreateButton(PlayerVoteArea parent)
    {
        var player = Helpers.PlayerById(parent.TargetPlayerId);
        if (!IsTargetValid(player)) //|| Buttons.ContainsKey(player))
            return;

        ClassicLogger.Log($"Creating {Name} meeting player ability for player {player.Data.PlayerName} ({parent.TargetPlayerId})");

        // Creating the button and setting it's propeties
        var button = GameObject.Instantiate(parent.ConfirmButton.GetComponent<PassiveButton>(), parent.transform);
        button.gameObject.SetActive(true);
        button.name = Name + "PlayerAbility";
        button.transform.localScale = new Vector2(0.4f, 0.4f);
        button.transform.position = new Vector2(-5.0708f, 3.7637f);
        button.transform.localPosition = new Vector2(-0.9f, -0f);
        button.transform.SetWorldZ(-15);

        Buttons.Add(player, button);
        
        // Sprite
        var renderer = button.GetComponent<SpriteRenderer>();
        renderer.sprite = Sprite;

        // Highlight
        var highlight = button.transform.Find("ControllerHighlight").GetComponent<SpriteRenderer>();
        highlight.color = HighlightColor;
        highlight.transform.SetWorldZ(-14);

        // Setting uses
        if (!InfiniteUses && FirstMeeting)
        {
            UsesLeft = StartUses;
            FirstMeeting = false;
        }
        UsesLeftThisMeeting = UsesPerMeeting;

        var passiveButton = button.GetComponent<PassiveButton>();
        passiveButton.OnClick.RemoveAllListeners();
        passiveButton.OnClick = new ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)(() => ClickHandler(player)));
    }

    internal void DeleteButton()
    {
        Buttons.Clear();
    }

    public abstract bool IsTargetValid(PlayerControl player);

    protected abstract void OnClick(PlayerControl target);

    public virtual bool CanUse()
    {
        return (InfiniteUses || UsesLeft > 0) && UsesLeftThisMeeting > 0;
    }

    public virtual void HudUpdate() { }
    public virtual void MeetingStart() { }

    public abstract bool Enabled();

    public void ChangeUses(int number)
    {
        UsesLeft += number;
    }
    public void SetUses(int number)
    {
        UsesLeft = number;
    }

    protected virtual void ClickHandler(PlayerControl target)
    {
        if (!CanUse())
            return;

        if (!InfiniteUses)
        {
            UsesLeft--;
            UsesLeftThisMeeting--;
        }

        OnClick(target);
    }

    internal virtual void HudUpdateHandler()
    {
        if (CanUse() && !PlayerControl.LocalPlayer.Data.IsDead && MeetingHud.Instance.SkipVoteButton.gameObject.active)
        {
            Buttons.Values.ToList().ForEach(b => b.gameObject.SetActive(true));
        }
        else
        {
            Buttons.Values.ToList().ForEach(b => b.gameObject.SetActive(false));
        }

        HudUpdate();
    }
}