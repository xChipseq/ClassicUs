using ClassicUs.Objects.Roles;
using ClassicUs.Utilities;
using HarmonyLib;
using UnityEngine;

namespace ClassicUs.Patches.Lobby;

[HarmonyPatch(typeof(LobbyBehaviour))]
public static class LobbyBehaviourPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LobbyBehaviour.Start))]
    public static void StartPostfix()
    {
        ModdedRoleManager.PlayerRoles.Clear();
        CustomProtectionManager.Protections.Clear();

        var laptop = GameObject.Find("SmallBox").transform.Find("Panel");
        laptop.GetComponent<SpriteRenderer>().sprite = ResourceLoader.LoadSprite("ClassicUs.Assets.Laptop.png", 0.5f);
        laptop.localScale = new Vector3(0.4f, 0.4f);
    }
}