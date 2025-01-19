using ClassicUs.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
public static class TaskHintRemoval
{
    public static bool Prefix(TranslationController __instance, ref string __result, [HarmonyArgument(0)] StringNames id)
    {
        if (id == StringNames.RoleHint)
        {
            __result = "";
            return false;
        }
        return true;
    }
}