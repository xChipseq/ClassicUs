using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClassicUs.Utilities;

namespace ClassicUs.Objects.Abilities;

public static class AbilityManager
{
    public static List<CustomAbilityButton> CustomAbilityButtons = new();
    public static List<MeetingAbilityButton> MeetingAbilityButtons = new();
    public static List<MeetingPlayerAbilityButton> MeetingPlayerAbilityButtons = new();

    public static void RegisterAllAbilities()
    {
        RegisterAbilityButtons();
        RegisterMeetingAbilities();
    }

    public static void RegisterAbilityButtons()
    {
        ClassicLogger.Log($"Registering ability buttons...");
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.GetCustomAttribute<RegisterCustomAbility>() != null);
        
        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is not CustomAbilityButton button)
            {
                ClassicLogger.Log($"Error while registering '{type}' - type is not a CustomAbilityButton");
                return;
            }
            CustomAbilityButtons.Add(button);
            ClassicLogger.Log($"Registered '{button.Name}' ability");
        }
    }

    public static void RegisterMeetingAbilities()
    {
        ClassicLogger.Log($"Registering meeting abilities...");
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.GetCustomAttribute<RegisterMeetingAbility>() != null);
        
        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is MeetingAbilityButton button)
            {
                MeetingAbilityButtons.Add(button);
                ClassicLogger.Log($"Registered '{button.Name}' meeting ability");
            }
            else if (Activator.CreateInstance(type) is MeetingPlayerAbilityButton playerButton)
            {
                MeetingPlayerAbilityButtons.Add(playerButton);
                ClassicLogger.Log($"Registered '{playerButton.Name}' player meeting ability");
            }
        }
    }
}