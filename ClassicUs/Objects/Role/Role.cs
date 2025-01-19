using System;
using System.Linq;
using ClassicUs.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Objects.Roles;

public abstract class ModdedRole
{
    public abstract string Name { get; }
    public abstract RoleTeam Team { get; protected set; }
    public abstract RoleAlignment Alignment { get; }
    public abstract ColorGradient Color { get; }
    public abstract string IntroText { get; }
    public abstract string TaskText { get; }
    public virtual bool Hidden { get; } = false;
    public virtual bool Generates { get; } = true;

    public abstract RoleConfig Config { get; }
    public virtual Type Options { get; }

    public ModdedRole(RoleTeam? overrideTeam = null)
    {
        if (overrideTeam.HasValue)
        {
            Team = overrideTeam.Value;
        }
    }

    public PlayerControl Player
    {
        get => Helpers.PlayerById(ModdedRoleManager.PlayerRoles.FirstOrDefault(v => v.Value == this).Key);
    }

    // Returns a string (role name by default) with a gradient applied based on role color
    public string GetColoredString(string text)
    {
        return Color.GetColoredString(text);
    }

    public string GetRoleText(PlayerControl player)
    {
        return $"<size=70%>{GetColoredString(Name)}</size>\n{player.Data.PlayerName}";
    }

    public virtual string GetPrefix(PlayerControl player)
    {
        return "";
    }

    public virtual bool DidWin(GameOverReason gameOverReason)
    {
        ClassicLogger.Log($"checking if {Player.Data.PlayerName} ({Name}) won");
        bool won = false;
        won = (Team == RoleTeam.Crewmate && GameManager.Instance.DidHumansWin(gameOverReason)) ||
                (Team == RoleTeam.Impostor && GameManager.Instance.DidImpostorsWin(gameOverReason));
        ClassicLogger.Log($"{Player.Data.PlayerName} ({Name}) {(won ? "won" : "lost")}");
        return won;
    }

    public virtual void CheckGameEnd()
    {
        if (Team == RoleTeam.Apocalypse)
        {
            if (!Player.Data.IsDead || !Player.Data.Disconnected)
            {
                var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(p => p.GetTeam() != RoleTeam.Apocalypse && !p.Data.IsDead && !p.Data.Disconnected).ToList();

                if (allPlayers.Count == 0) GameManager.Instance.RpcEndGame((GameOverReason)ModdedGameOverReasons.ApocalypseWin, false);
            }
        }
    }

    public virtual bool PreventsGameEnd()
    {
        return (Team == RoleTeam.Neutral && Alignment == RoleAlignment.Killing && PlayerControl.AllPlayerControls.ToArray().Where(p => p != Player && !p.Data.IsDead && !p.Data.Disconnected).ToList().Count > 0) ||
                Team == RoleTeam.Apocalypse && PlayerControl.AllPlayerControls.ToArray().Where(p => p.GetTeam() != RoleTeam.Apocalypse && !p.Data.IsDead && !p.Data.Disconnected).ToList().Count > 0; 
    }

    public virtual void HudUpdate(HudManager hudManager) { }
    public virtual void PlayerFixedUpdate(PlayerControl player) { }
    public virtual void OnDeath(DeathReason reason) { }
    public virtual void MeetingStart(MeetingHud meetingHud) { }
    public virtual void MeetingEnd() { }
}

public struct RoleConfig
{
    public bool CanVent;
    public bool CanMoveInVents;
    public bool CanSabotage;
    public bool HasKillButton;
    public bool HasImpostorVision;
    public bool CanDoTasks;
    public bool TasksCountForProgress;

    public RoleConfig(RoleTeam team)
    {
        CanVent = team == RoleTeam.Impostor;
        CanMoveInVents = CanVent;
        CanSabotage = team == RoleTeam.Impostor;
        HasKillButton = team == RoleTeam.Impostor;
        HasImpostorVision = team == RoleTeam.Impostor || team == RoleTeam.Apocalypse;
        CanDoTasks = team == RoleTeam.Crewmate;
        TasksCountForProgress = team == RoleTeam.Crewmate;
    }
}