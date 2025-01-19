using ClassicUs.Utilities;
using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ClassicUs;

public static class MenuRevamp
{
    [HarmonyPatch(typeof(AccountTab), nameof(AccountTab.Awake))]
    public static class AccountTab_Awake
    {
        public static void Postfix(AccountTab __instance)
        {
            // hides the account tab thing at the top of the screen, nobody uses that
            __instance.transform.Find("GameHeader").gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class MainMenuManager_Start
    {
        public static void Prefix(MainMenuManager __instance)
        {
            var f = __instance.mainMenuUI.transform.FindChild("AspectScaler").FindChild("FullScreen");
            f.GetComponent<SpriteRenderer>().color = Color.black;
            f.gameObject.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Awake))]
    public static class MainMenuManager_Awake
    {
        public static void Postfix(MainMenuManager __instance)
        {
            RevampMenu(__instance);
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
    public static class MainMenuManager_LateUpdate
    {
        public static void Postfix(MainMenuManager __instance)
        {
            TweakReactorVersionShower();
            if (MenuLoaded)
            {
                try
                {
                    var leftPanel = GameObject.Find("LeftPanel");
                    GameObject.Find("FullScreen").gameObject.SetActive(false);
                    leftPanel.GetComponent<AspectScaledAsset>().originalDistanceFromEdge = 2.5f;

                    var logo = GameObject.Find("LOGO-AU");
                    logo.transform.localScale = new Vector2(1, 1);
                }
                catch { }
                return;
            }
            RevampMenu(__instance);
        }
    }

    public static bool MenuLoaded = false;

    public static void RevampMenu(MainMenuManager __instance)
    {
        // FIX: the left panel is not centered correctly and i have no idea how to fix it, fuck unity honestly
        var aspectScaler = __instance.mainMenuUI.transform.FindChild("AspectScaler");
        var leftPanel = aspectScaler.Find("LeftPanel").gameObject;
        var leftPanelButtons = leftPanel.transform.Find("Main Buttons");
        var rightPanel = aspectScaler.Find("RightPanel").gameObject;

        // Removes the background
        aspectScaler.FindChild("BackgroundTexture").gameObject.SetActive(false);
        // Moves the logo
        leftPanel.transform.FindChild("Sizer").SetLocalY(3.5f);
        leftPanel.transform.FindChild("Sizer").localScale = new Vector2(1.4f, 1.4f);
        var logo = GameObject.Find("LOGO-AU");
        var sprite = ResourceLoader.LoadSprite("ClassicUs.Assets.Logo.png", 0.6f);
        logo.GetComponent<SpriteRenderer>().sprite = sprite;
        // Centers the left panel
        leftPanel.GetComponent<AspectScaledAsset>().originalDistanceFromEdge = 2.5f;
        // Removes the left panel background
        leftPanel.GetComponent<SpriteRenderer>().color = Color.clear;
        // Removes the divider
        leftPanelButtons.transform.FindChild("Divider")?.gameObject.SetActive(false);

        // Removes the right panel's frame
        rightPanel.GetComponent<SpriteRenderer>().color = Color.clear;
        // Removes shine from right panel
        rightPanel.transform.FindChild("WindowShine").gameObject.SetActive(false);
        // Masked black screen changes
        var maskBlack = rightPanel.transform.FindChild("MaskedBlackScreen").gameObject;
        maskBlack.GetComponent<SpriteRenderer>().color = Color.clear;
        maskBlack.transform.SetWorldZ(-20);
        // Some tint changes
        var tint = __instance.screenTint;
        tint.transform.SetWorldZ(-19);
        tint.transform.localScale = new Vector2(15, 15);
        // Gamemode buttons
        var gmButtons = __instance.gameModeButtons;
        gmButtons.GetComponent<AspectPosition>().anchorPoint = new Vector2(0.4759f, 0.487f);
        gmButtons.transform.FindChild("Divider").gameObject.SetActive(false);
        gmButtons.transform.FindChild("Header").gameObject.SetActive(false);
        // Changing my account button into stats button
        var accountButton = __instance.myAccountButton;
        accountButton.OnClick.RemoveAllListeners();
        accountButton.OnClick.AddListener((System.Action)(() =>
                {
                    var stats = __instance.transform.FindChild("StatsPopup").gameObject;
                    stats.SetActive(!stats.active);
                }));
        var accountText = accountButton.transform.FindChild("FontPlacer").FindChild("Text_TMP").gameObject;
        accountText.GetComponent<TextTranslatorTMP>().Destroy();
        accountText.GetComponent<TextMeshPro>().text = "Statistics";
        // Credits
        __instance.creditsScreen.transform.position -= new Vector3(1, 0);


        MenuLoaded = true;
        Logger<ClassicUsPlugin>.Warning("everything is good, disabling fullscreen");
    }

    public static void TweakReactorVersionShower()
    {
        // Changes the reactor version shower position
        var version = GameObject.Find("ReactorVersion");
        var aspectPosition = version.AddComponent<AspectPosition>();
        var distanceFromEdge = new Vector3(10.13f, 2.75f, -1);
        aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftTop;
        aspectPosition.DistanceFromEdge = distanceFromEdge;
        aspectPosition.AdjustPosition();
    }
}