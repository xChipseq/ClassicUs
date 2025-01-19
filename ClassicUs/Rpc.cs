using System.Collections.Generic;
using System.Linq;
using ClassicUs.Objects.Roles;
using ClassicUs.Roles.Apocalypse.Baker;
using ClassicUs.Roles.Impostors.Janitor;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace ClassicUs;

internal enum RpcCalls
{
    SetRole = 0,
    RoleSelection = 1,
    SetModifier = 2,
    ModifierSelection = 3,

    ModRoleListChange = 5,
    UserRoleListChange = 6,

    CustomMurder = 10,
    CustomProtection = 11,
    RemoveCustomProtection = 12,

    ApocalypseTransformation = 20,

    JanitorCleanBody = 30,
}

internal static class CustomRpc
{
    [MethodRpc((uint)RpcCalls.ApocalypseTransformation)]
    public static void RpcApocalypseTransform(this PlayerControl player, int role, object[] additionalData)
    {
        player.SetModdedRole(role);
        if (player.AmOwner)
        {
            switch ((ModdedRoles)role)
            {
                case ModdedRoles.Famine:
                    var famine = player.GetModdedRole<FamineRole>();
                    var oldList = (List<PlayerControl>)additionalData[0];
                    foreach (var p in PlayerControl.AllPlayerControls)
                    {
                        if (oldList.Contains(p))
                        {
                            famine.Breads.Add(p, 3);
                        }
                        else
                        {
                            famine.Breads.Add(p, 1);
                        }
                    }
                    break;
            }
        }

        Coroutines.Start(ClassicEffects.FlashCoroutine(ClassicPalette.TransformedApocalypseColor, 1.5f, 0));
    }

    [MethodRpc((uint)RpcCalls.JanitorCleanBody)]
    public static void RpcCleanBody(this PlayerControl player, byte bodyId)
    {
        var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == bodyId);

        if (body)
        {
            Coroutines.Start(JanitorRole.CleanCoroutine(body));
        }
    }
}