using ClassicUs.Utilities;
using UnityEngine;
using static ClassicUs.Utilities.Extensions;

namespace ClassicUs;

public static class ClassicPalette
{
    public static readonly Color ApocalypseColor = Helpers.HexToColor("#ff1a60");
    public static readonly Color TransformedApocalypseColor = Helpers.HexToColor("#753649");

    public static readonly Color GlobalModifierColor = Helpers.HexToColor("#8ec8d1");
    public static readonly Color CrewModifierColor = Helpers.HexToColor("#98d984");
    public static readonly Color KillerModifierColor = Helpers.HexToColor("#824b46");

    // Roles
    public static readonly ColorGradient Vigilante = new ColorGradient(Helpers.HexToColor("#d4af53"));
    public static readonly ColorGradient Cleric = new ColorGradient(Helpers.HexToColor("#4cfc4f"));

    public static readonly ColorGradient Jester = new ColorGradient(new Color(1f, 0.75f, 0.8f));
    public static readonly ColorGradient SerialKiller = new ColorGradient(Helpers.HexToColor("#101dde"));
    public static readonly ColorGradient Pirate = new ColorGradient(Helpers.HexToColor("#fcba03"));
}