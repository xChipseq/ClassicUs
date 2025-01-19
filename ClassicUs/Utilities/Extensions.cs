using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ClassicUs.Utilities;

public static class Extensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (var i = list.Count - 1; i > 0; --i)
        {
            var j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static void RemoveComponent<T>(this GameObject gameObject) where T : Component
    {
        gameObject.GetComponent<T>()?.Destroy();
    }

    public static PlayerControl? GetClosestPlayer(this PlayerControl playerControl, bool includeImpostors, float distance, bool ignoreColliders = false, Predicate<PlayerControl> isValid = null)
    {
        var filteredPlayers = Helpers.GetClosestPlayers(playerControl, distance, ignoreColliders)
            .Where(
                playerInfo => !playerInfo.Data.Disconnected &&
                              playerInfo.PlayerId != playerControl.PlayerId &&
                              !playerInfo.Data.IsDead &&
                              (includeImpostors || !playerInfo.Data.Role.IsImpostor))
            .ToList();

        return isValid != null ? filteredPlayers.Find(isValid) : filteredPlayers.FirstOrDefault();
    }

    public static DeadBody? GetClosestDeadBody(this PlayerControl playerControl, float radius)
    {
        return Helpers
            .GetNearestDeadBodies(playerControl.GetTruePosition(), radius, Helpers.CreateFilter(Constants.NotShipMask))
            .Find(component => component && !component.Reported);
    }

    public struct ColorGradient
    {
        public Color Start;
        public Color End;

        public ColorGradient(Color a, Color b)
        {
            Start = a;
            End = b;
        }

        public ColorGradient(Color color)
        {
            Start = color;
            End = color;
        }

        public Color GetColor()
        {
            return Start;
        }

        public string GetColoredString(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            string result = "";
            for (int i = 0; i < text.Length; i++)
            {
                var color = UnityEngine.Color.Lerp(Start, End, (float)i / (float)(text.Length - 1));
                result += $"<color=#{color.ToHtmlStringRGBA()}>{text[i]}</color>";
            }

            return result;
        }
    }
}