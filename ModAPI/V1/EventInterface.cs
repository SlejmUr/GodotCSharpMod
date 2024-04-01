using ModAPI.V1.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ModAPI.V1
{
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

    internal class EventInvokeLocation
    {
#if NET8_0
        public required Type Mod;
        public required object Target;
        public required MethodInfo Method;
#elif NET6_0
        public Type? Mod;
        public object? Target;
        public MethodInfo? Method;
#endif
        public override string ToString()
        {
            return $"Name: {Mod?.Name} Target: {Target} Method: {Method?.Name}";
        }
    }
}
