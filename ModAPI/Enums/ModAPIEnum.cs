namespace ModAPI;

public enum ModAPIEnum
{
    None,
    V0, //  This is using abstract class for Load, "Unload" and things that V1 and V2 registers, calls.
    V1, //  This is using Interfaces and Attributes to create and call an event.
    V2, //  This is using Event system and you can set priority with it too.
    All,
}
