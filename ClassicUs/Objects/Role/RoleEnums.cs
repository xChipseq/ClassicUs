using System.Text.Json.Serialization;

namespace ClassicUs.Objects.Roles;

public enum RoleTeam
{
    Crewmate = 1,
    Neutral = 2,
    Impostor = 3,
    Apocalypse = 4,
}

public enum RoleAlignment
{
    None,
    Investigative,
    Protective,
    Killing,
    Support,
    Power,
    Evil,
    Chaos,
    Pariah,
    Deception,
}

public enum ModdedRoles
{
    Crewmate = 0,
    Impostor = 1,

    #region Role Lists
    Any = 2,
    RandomCrewmate = 3,
    RandomImpostor = 4,
    RandomNeutral = 5,

    CrewInvestigative = 6,
    CrewProtective = 7,
    CrewKilling = 8, 
    CrewSupport = 9,
    CrewPower = 10,

    NeutralEvil = 11,
    NeutralKilling = 12,
    NeutralChaos = 13,
    NeutralPariah = 14,

    RandomApocalypse = 15,

    ImpostorDeception = 16,
    ImpostorKilling = 17,
    ImpostorSupport = 18,
    ImpostorPower = 19,
    #endregion

    // Crew
    Shrimpmate = 20,
    Vigilante = 21,
    Cleric = 22,
    
    // Neutral
    SerialKiller = 200,
    Jester = 201,
    Pirate = 202,

    // Impostors
    Janitor = 400,

    // Apocalypse
    Baker = 600,
    Famine = 601,
}

public enum ModdedGameOverReasons
{
    ApocalypseWin = 10,
    SerialKillerWin = 11,
    JesterWin = 12,
}