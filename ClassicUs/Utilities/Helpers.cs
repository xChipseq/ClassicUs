using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClassicUs.Utilities;

public static class Helpers
{
    public static PlayerControl? PlayerById(byte id)
    {
        return PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(player => player.Data.PlayerId == id);
    }

    public static Color HexToColor(string hexString)
    {
        ColorUtility.TryParseHtmlString(hexString, out Color color);
        return color;
    }

    public static string ColorToHex(Color color)
    {
        return Mathf.RoundToInt(color.r * 255).ToString("X2") +
               Mathf.RoundToInt(color.g * 255).ToString("X2") +
               Mathf.RoundToInt(color.b * 255).ToString("X2") +
               Mathf.RoundToInt(color.a * 255).ToString("X2");
    }

    public static ContactFilter2D CreateFilter(int layerMask)
    {
        return ContactFilter2D.CreateLegacyFilter(layerMask, float.MinValue, float.MaxValue);
    }

    public static List<PlayerControl> GetClosestPlayers(PlayerControl source, float distance = 2f, bool ignoreColliders = false, bool ignoreSource = true)
    {
        if (!ShipStatus.Instance)
        {
            return [];
        }

        var myPos = source.GetTruePosition();
        var players = GetClosestPlayers(myPos, distance, ignoreColliders);

        return ignoreSource ? players.Where(plr => plr.PlayerId != source.PlayerId).ToList() : players;
    }

    public static List<PlayerControl> GetClosestPlayers(Vector2 source, float distance = 2f, bool ignoreColliders = true)
    {
        if (!ShipStatus.Instance)
        {
            return [];
        }

        List<PlayerControl> outputList = [];
        outputList.Clear();
        var allPlayers = GameData.Instance.AllPlayers.ToArray().Select(x => x.Object);

        outputList.AddRange(
            from playerControl in allPlayers
            where playerControl && playerControl.Collider.enabled
            let vector = playerControl.GetTruePosition() - source
            let magnitude = vector.magnitude
            where magnitude <= distance && (ignoreColliders || !PhysicsHelpers.AnyNonTriggersBetween(
                source,
                vector.normalized,
                magnitude,
                Constants.ShipAndObjectsMask))
            select playerControl);

        outputList.Sort(
            delegate(PlayerControl a, PlayerControl b)
            {
                var magnitude2 = (a.GetTruePosition() - source).magnitude;
                var magnitude3 = (b.GetTruePosition() - source).magnitude;
                if (magnitude2 > magnitude3)
                {
                    return 1;
                }

                if (magnitude2 < magnitude3)
                {
                    return -1;
                }

                return 0;
            });
        return outputList;
    }

    public static List<DeadBody> GetNearestDeadBodies(Vector2 source, float radius, ContactFilter2D filter)
    {
        var results = new Il2CppSystem.Collections.Generic.List<Collider2D>();
        Physics2D.OverlapCircle(source, radius, filter, results);
        return results.ToArray()
            .Where(collider2D => collider2D.CompareTag("DeadBody"))
            .Select(collider2D => collider2D.GetComponent<DeadBody>()).ToList();
    }
}
