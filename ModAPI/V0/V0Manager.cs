using ModAPI.V1;
using System.Reflection;
using ModAPI.V2;

namespace ModAPI.V0;

public class V0Manager
{
    public static List<V0Mod> Mods = new();

    public static void Register(Assembly assembly)
    {
        if (MainLoader.settings.V0_SkipAllAction)
            return;
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(V0Mod).IsAssignableFrom(type))
            {
                V0Mod? mod = (V0Mod?)Activator.CreateInstance(type);
                if (mod != null)
                {
                    Mods.Add(mod);
                    mod.Load();
                }
            }
        }
    }

    public static void V1Register(MethodInfo methodInfo, int InterfaceNumber)
    {
        if (MainLoader.settings.V0_SkipAllAction)
            return;
        Mods.ForEach(m => m.V1Register(methodInfo, InterfaceNumber));
    }

    public static void V1Call(EventInvokeLocation eventInvokeLocation, ICustomMod argument)
    {
        if (MainLoader.settings.V0_SkipAllAction)
            return;
        Mods.ForEach(m => m.V1Call(eventInvokeLocation, argument));
    }

    public static void V2Register(V2Priority? priority, Type eventType, MethodInfo methodInfo)
    {
        if (MainLoader.settings.V0_SkipAllAction)
            return;
        Mods.ForEach(m => m.V2Register(priority, eventType, methodInfo));
    }

    public static void V2Unregister(Type eventType, MethodInfo methodInfo)
    {
        if (MainLoader.settings.V0_SkipAllAction)
            return;
        Mods.ForEach(m => m.V2Unregister(eventType, methodInfo));
    }

    public static void V2Call(MethodInfo methodInfo, BaseEvent @event)
    {
        if (MainLoader.settings.V0_SkipAllAction)
            return;
        Mods.ForEach(m => m.V2Call(methodInfo, @event));
    }

    public static void Unload()
    {
        if (MainLoader.settings.V0_SkipAllAction)
            return;
        Mods.ForEach(m => m.Unload());
        Mods.Clear();
    }
}
