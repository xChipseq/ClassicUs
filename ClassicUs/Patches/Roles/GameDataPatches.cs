using ClassicUs.Objects.Roles;
using HarmonyLib;

namespace ClassicUs.Patches.Roles;

[HarmonyPatch(typeof(GameData))]
public static class GameDataPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameData.RecomputeTaskCounts))]
    public static bool RecomputeTaskCountsPrefix(GameData __instance)
    {
        if (GameManager.Instance == null || LobbyBehaviour.Instance)
        {
            return false;
        }
        __instance.TotalTasks = 0;
        __instance.CompletedTasks = 0;

        foreach (var data in __instance.AllPlayers)
        {
            var role = data.Object.GetModdedRole();
            bool flag = true;
            if (role != null) flag = role.Config.TasksCountForProgress;
            if (!data.Disconnected && data.Tasks != null && data.Object && (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) && !data.Role.IsImpostor && flag)
            {
                foreach (var task in data.Tasks)
                {
                    __instance.TotalTasks++;
                    if (task.Complete) __instance.CompletedTasks++;
                }
            }
        }
        return false;
    }
}