using ClassicUs.Objects.Roles;
using HarmonyLib;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(EndGameManager))]
public static class EndGameManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(EndGameManager.SetEverythingUp))]
    public static void SetEverythingUpPostfix(EndGameManager __instance)
    {
        if (GameManager.Instance.IsHideAndSeek()) return;

        GameObject winnersText = Object.Instantiate(__instance.WinText.gameObject);
        winnersText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.5f, __instance.WinText.transform.position.z);
        winnersText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        TMPro.TMP_Text textRenderer = winnersText.GetComponent<TMPro.TMP_Text>();

        string winnerString;
        ColorGradient winnerColor;

        switch (EndGameResult.CachedGameOverReason)
        {
            case GameOverReason.HumansByVote:
            case GameOverReason.ImpostorDisconnect:
                winnerString = "Crewmate Win";
                winnerColor = new ColorGradient(Palette.CrewmateBlue);
                break;
            case GameOverReason.HumansByTask:
                winnerString = "Crewmate Task Win";
                winnerColor = new ColorGradient(Palette.CrewmateBlue);
                break;

            case GameOverReason.ImpostorByVote:
            case GameOverReason.ImpostorByKill:
            case GameOverReason.ImpostorBySabotage:
            case GameOverReason.HumansDisconnect:
                winnerString = "Impostor Win";
                winnerColor = new ColorGradient(Palette.ImpostorRed);
                break;

            case (GameOverReason)ModdedGameOverReasons.ApocalypseWin:
                winnerString = "Apocalypse Win";
                winnerColor = new ColorGradient(ClassicPalette.ApocalypseColor);
                break;

            case (GameOverReason)ModdedGameOverReasons.JesterWin:
                winnerString = "Jester Win";
                winnerColor = ClassicPalette.Jester;
                break;
            case (GameOverReason)ModdedGameOverReasons.SerialKillerWin:
                winnerString = "Serial Killer Win";
                winnerColor = ClassicPalette.SerialKiller;
                break;

            default:
                winnerString = "Nobody Win";
                winnerColor = new ColorGradient(Palette.DisabledGrey);
                break;
        }

        textRenderer.text = winnerColor.GetColoredString(winnerString);

        __instance.BackgroundBar.material.SetColor("_Color", winnerColor.GetColor());
    }
}