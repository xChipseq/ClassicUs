using System;
using System.Linq;
using ClassicUs.Objects.Roles;

namespace ClassicUs.Objects.Roles;

public static class ModdedRoleSingleton<T> where T : ModdedRole
{
    private static ModdedRole _instance;

    #pragma warning disable CA1000
    public static ModdedRole Instance =>
        _instance ??= ModdedRoleManager.RoleTypes.Values.OfType<T>().SingleOrDefault();
    #pragma warning restore CA1000
}