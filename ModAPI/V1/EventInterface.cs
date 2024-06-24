using System.Reflection;

namespace ModAPI.V1;

internal class EventInterface
{
    public EventInterface(ICustomMod args)
    {
        EventArg = args;
        EventArgType = EventArg.GetType();
    }

    public void RegisterInvoker(Type mod, object handle, MethodInfo method)
    {
        if (!Invokers.ContainsKey(mod))
        {
            Invokers.Add(mod, new List<EventInvokeLocation>());
        }
        Invokers[mod].Add(new EventInvokeLocation
        {
            Mod = mod,
            Target = handle,
            Method = method
        });
    }

    public readonly ICustomMod EventArg;
    public readonly Type EventArgType;
    public readonly Dictionary<Type, List<EventInvokeLocation>> Invokers = new Dictionary<Type, List<EventInvokeLocation>>();
}

public class EventInvokeLocation
{
    public required Type Mod;
    public required object Target;
    public required MethodInfo Method;
    public override string ToString()
    {
        return $"Name: {Mod?.Name} Target: {Target} Method: {Method?.Name}";
    }
}
