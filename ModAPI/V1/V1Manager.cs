using ModAPI.V0;
using Serilog;
using System.Reflection;

namespace ModAPI.V1;

public class V1Manager
{
    private static readonly Dictionary<Type, object> ModHandlers = [];
    private static readonly Dictionary<int, EventInterface> ModInterfaces = [];
    public static readonly Dictionary<Type, int> TypeToMods = [];
    public static void LoadFromMain(Assembly assembly)
    {
        //  we should add smth here, so double ICustomMod can also be used.
        foreach (var type in assembly.GetTypes().Where(x => !x.IsInterface && x.GetInterfaces().Contains(typeof(ICustomMod))))
        {
            foreach (var interfaces in type.GetInterfaces())
            {
                if (interfaces != typeof(ICustomMod))
                    continue;
                ICustomMod? modInterface = (ICustomMod?)Activator.CreateInstance(type);
                if (modInterface != null)
                {
                    Log.Verbose("{TypeName} registered with {InterfaceNumber}", type.Name, modInterface.InterfaceNumber);
                    ModInterfaces.Add(modInterface.InterfaceNumber, new EventInterface(modInterface));
                    TypeToMods.Add(type, modInterface.InterfaceNumber);
                }
            }
        }
        if (MainLoader.settings.V1_EnableMainAsMod)
            RegisterEvents(assembly);
    }

    /// <summary>
    /// If the assembly not registered you can do events here. (Not quite recommended)
    /// </summary>
    /// <param name="plugin"></param>
    public static void RegisterEvents(object plugin)
    {
        RegisterEvents(plugin.GetType());
    }

    public static void RegisterEvents(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            RegisterEvents(type);
        }
    }

    private static void RegisterEvents(Type plugin)
    {
        // only static functions
        foreach (MethodInfo methodInfo in plugin.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var eventAttrib = methodInfo.GetCustomAttribute<V1EventAttribute>();
            if (eventAttrib != null && eventAttrib.InterfaceTypeNumer != 0)
            {
                Log.Verbose("(V1) {MethodName} attached to {InterfaceNumber}", methodInfo.Name, eventAttrib.InterfaceTypeNumer);
                object? obj;
                if (!ModHandlers.TryGetValue(plugin, out obj))
                {
                    obj = Activator.CreateInstance(plugin);
                    if (obj != null)
                        ModHandlers.Add(plugin, obj);
                }
                if (obj != null)
                    RegisterEventMethod(plugin, obj, methodInfo, eventAttrib);
            }
        }
    }

    private static void RegisterEventMethod(Type plugin, object eventHandler, MethodInfo methodInfo, V1EventAttribute eventAttribute)
    {
        var pararms = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
        if (pararms.Length != 1)
            return;
        if (!CheckType(pararms[0]))
            return;
        if (!MainLoader.settings.V1_EnableICustomModInterfaceParsing)
        {
            var numb = CheckTypeAndGetNumber(pararms[0]);
            //  This happens when there is multiple registered Events and we use ICustomModInterface.
            //  Do not use ICustomModInterface if you can.
            if (numb != eventAttribute.InterfaceTypeNumer)
            {
                Log.Verbose("Event ID missmatch! {Id} {InterfaceNumber}", numb, eventAttribute.InterfaceTypeNumer);
                return;
            }
        }

        Log.Verbose(eventAttribute.InterfaceTypeNumer + " Register event");
        if (!ModInterfaces.TryGetValue(eventAttribute.InterfaceTypeNumer, out var @event))
        {
            Log.Warning("Event {InterfaceNumber} is not registered in Manager method {Method}!", eventAttribute.InterfaceTypeNumer, methodInfo.Name);
            return;
        }
        else
        {
            Log.Verbose("Registered event {MethodName} ({InterfaceNumber}) in plugin {PluginName}!", methodInfo.Name, eventAttribute.InterfaceTypeNumer, plugin.FullName);
            V0Manager.V1Register(methodInfo, eventAttribute.InterfaceTypeNumer);
            @event.RegisterInvoker(plugin, eventHandler, methodInfo);
        }
    }

    private static bool CheckType(Type paramType)
    {
        foreach (var item in TypeToMods.Keys)
            if (paramType.IsAssignableFrom(item))
                return true;
        return false;
    }

    private static int CheckTypeAndGetNumber(Type paramType)
    {
        foreach (var item in TypeToMods)
            if (paramType.IsAssignableFrom(item.Key))
                return item.Value;
        return 0;
    }

    public static void ExecuteEvent(ICustomMod customModInterface)
    {
        if (!ModInterfaces.TryGetValue(customModInterface.InterfaceNumber, out var @event))
        {
            Log.Warning("Event {InterfaceNumber} is not registered in Manager!", customModInterface.InterfaceNumber);
            return;
        }
        foreach (List<EventInvokeLocation> list in @event.Invokers.Values)
        {
            foreach (EventInvokeLocation eventInvokeLocation in list)
            {
                eventInvokeLocation.Method?.Invoke(eventInvokeLocation.Target, [customModInterface]);
                V0Manager.V1Call(eventInvokeLocation, customModInterface);
            }
        }
    }

    public static void Unload()
    {
        ModInterfaces.Clear(); 
        ModHandlers.Clear();
        TypeToMods.Clear();
    }
}
