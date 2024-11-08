namespace ModAPI;

public enum ModAPIEnum
{
    /// <summary>
    /// This is disabling mods
    /// </summary>
    None,
    /// <summary>
    /// This is using abstract class for Load, "Unload" and things that V1 and V2 registers, calls.
    /// </summary>
    V0,
    /// <summary>
    /// This is using Interfaces and Attributes to create and call an event.
    /// </summary>
    V1,
    /// <summary>
    /// This is using Event system and you can set priority with it too.
    /// </summary>
    V2,
    /// <summary>
    /// This is using all V0-V3
    /// </summary>
    All,
}
