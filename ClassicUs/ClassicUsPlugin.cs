using BepInEx;
using BepInEx.Unity.IL2CPP;
using ClassicUs.GameOptions;
using ClassicUs.Objects.Abilities;
using HarmonyLib;
using Reactor;
using Reactor.Networking.Attributes;
using Reactor.Utilities;

namespace ClassicUs;

[BepInPlugin(Id, "Classic Us", VersionString)]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
public partial class ClassicUsPlugin : BasePlugin
{
    public const string Id = "com.chipseq.classicus";
    public const string VersionString = "1.0.0";
    public const bool DevBuild = true;

    public static System.Version Version = System.Version.Parse(VersionString);
    public Harmony Harmony { get; } = new(Id);
    
    public override void Load()
    {
        OptionsManager.Initialize();
        AbilityManager.RegisterAbilityButtons();
        AbilityManager.RegisterMeetingAbilities();
        Harmony.PatchAll();

        ReactorCredits.Register("Classic Us", VersionString, true, ReactorCredits.AlwaysShow);
    }
}
