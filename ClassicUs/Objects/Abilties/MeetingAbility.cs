using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;

namespace ClassicUs.Objects.Abilities;

public abstract class MeetingAbilityButton
{
    // Properties
    public abstract string Name { get; }
    public abstract Color Color { get; }
    public abstract bool InfiniteUses { get; }
    public abstract int StartUses { get; }
    public abstract int UsesPerMeeting { get; }

    // Systems
    public PlayerVoteArea Button { get; private set; }
    public TextMeshPro UsesText { get; private set; }
    private bool FirstMeeting = true;
    public int UsesLeft { get; private set; }
    public int UsesLeftThisMeeting { get; private set; }

    internal void CreateButton(Transform parent, MeetingHud meetingHud, int index)
    {
        if (Button)
            return;

        // Creating the button and setting it's propeties
        Button = GameObject.Instantiate(meetingHud.SkipVoteButton, parent);
        Button.name = Name + "Ability";
        Button.GetComponentInChildren<TextTranslatorTMP>().Destroy();
        Button.GetComponentInChildren<TextMeshPro>().text = Name.ToUpperInvariant();
        Button.transform.localPosition += new Vector3(1.2f * index, 0);
        Button.transform.Find("Buttons").gameObject.Destroy();

        // Color
        var renderer = Button.GetComponent<SpriteRenderer>();
        renderer.color = Color;

        // Creating the uses text
        var UsesSprite = GameObject.Instantiate(HudManager.Instance.AbilityButton.usesRemainingSprite, Button.transform);
        UsesSprite.name = "Uses";
        UsesSprite.transform.localScale = new Vector2(0.5f, 0.5f);
        UsesSprite.transform.position += new Vector3(0.9f, -0.25f);
        UsesText = UsesSprite.transform.Find("Text_TMP").GetComponent<TextMeshPro>();

        // Setting uses
        if (InfiniteUses)
        {
            UsesSprite.gameObject.SetActive(false);
        }
        else
        {
            UsesSprite.gameObject.SetActive(true);
            if (FirstMeeting)
            {
                UsesLeft = StartUses;
                FirstMeeting = false;
            }
        }
        UsesLeftThisMeeting = UsesPerMeeting;

        var passiveButton = Button.GetComponent<PassiveButton>();
        passiveButton.OnClick = new ButtonClickedEvent();
        passiveButton.OnClick.AddListener((UnityAction)ClickHandler);
    }

    internal void DeleteButton()
    {
        Button = null;
        UsesText = null;
    }

    protected abstract void OnClick();

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

    public void SetUse(bool canBeUsed)
    {
        if (!Button)
            return;
        
        var renderer = Button.GetComponent<SpriteRenderer>();
        renderer.color = canBeUsed ? Color * new Color(1, 1, 1) : Color * new Color(0.6f, 0.6f, 0.6f, 0.6f);
    }

    protected virtual void ClickHandler()
    {
        if (!CanUse())
            return;

        if (!InfiniteUses)
        {
            UsesLeft--;
            UsesLeftThisMeeting--;
            UsesText.text = UsesLeft.ToString();
        }

        MeetingHud.Instance.SkipVoteButton.transform.Find("Buttons").gameObject.SetActive(false);
        OnClick();
    }

    internal virtual void HudUpdateHandler()
    {
        if (CanUse() && !PlayerControl.LocalPlayer.Data.IsDead)
        {
            SetUse(true);
        }
        else
        {
            SetUse(false);
        }

        UsesText.text = UsesLeft.ToString();
        Button.gameObject.SetActive(MeetingHud.Instance.SkipVoteButton.gameObject.active);
        HudUpdate();
    }
}