using System;

namespace ClassicUs.Objects.Abilities;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterCustomAbility : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class RegisterMeetingAbility : Attribute
{
}