﻿using ModAPI.V1.Attributes;
using ModAPI.V1.Interfaces;
using System.Reflection;

namespace ModAPI.V1
{
    public class V1Manager
    {
        public static bool EnableICustomModInterfaceParsing = false;
        private static readonly Dictionary<Type, object> ModHandlers = new();
        private static readonly Dictionary<int, EventInterface> ModInterfaces = new();
        public static readonly Dictionary<Type, int> TypeToMods = new();
        public static void LoadFromMain(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface)
                    continue;

                foreach (var interfaces in type.GetInterfaces())
                {
                    if (interfaces != typeof(ICustomMod))
                        continue;
                    var obj = Activator.CreateInstance(type);
                    if (obj != null)
                    {
                        ICustomMod? modInterface = (ICustomMod)obj;
                        if (modInterface != null)
                        {
                            Debugger.Print($"{type.Name} registered with number: {modInterface.InterfaceNumber}");
                            ModInterfaces.Add(modInterface.InterfaceNumber, new EventInterface(modInterface));
                            TypeToMods.Add(type, modInterface.InterfaceNumber);
                        }
                    }
                }
            }
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
            V1IModManager.Register(assembly);
            foreach (var type in assembly.GetTypes())
            {
                RegisterEvents(type);
            }
        }

        private static void RegisterEvents(Type plugin)
        {
            foreach (MethodInfo methodInfo in plugin.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var eventAttrib = methodInfo.GetCustomAttribute<V1EventAttribute>();
                if (eventAttrib != null && eventAttrib.InterfaceTypeNumer != 0)
                {
                    Debugger.Print($"(V1) {methodInfo.Name} attached to number: {eventAttrib.InterfaceTypeNumer}");
                    object obj;
                    if (!ModHandlers.TryGetValue(plugin, out obj))
                    {
                        obj = Activator.CreateInstance(plugin);
                        ModHandlers.Add(plugin, obj);
                    }
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
            if (!EnableICustomModInterfaceParsing)
            {
                var numb = CheckTypeAndGetNumber(pararms[0]);
                //  This happens when there is multiple registered Events and we use ICustomModInterface.
                //  TLD: Do not use ICustomModInterface if you can.
                if (numb != eventAttribute.InterfaceTypeNumer)
                {
                    Debugger.Print("Event ID missmatch! "  + numb + " " + eventAttribute.InterfaceTypeNumer);
                    return;
                }
            }

            Debugger.Print(eventAttribute.InterfaceTypeNumer + " Register event");
            if (!ModInterfaces.TryGetValue(eventAttribute.InterfaceTypeNumer, out var @event))
            {
                Debugger.Print(string.Format("Event {0} is not registered in Manager method {1}!", eventAttribute.InterfaceTypeNumer, methodInfo.Name));
                return;
            }
            else
            {
                Debugger.Print(string.Format("Registered event {0} ({1}) in plugin {2}!", methodInfo.Name, eventAttribute.InterfaceTypeNumer, plugin.FullName));
                V1IModManager.RegisterMethod(plugin.Assembly.FullName, methodInfo, eventAttribute.InterfaceTypeNumer);
                @event.RegisterInvoker(plugin, eventHandler, methodInfo);
            }
        }

        private static bool CheckType(Type paramType)
        {
            foreach (var item in TypeToMods.Keys)
            {
                if (paramType.IsAssignableFrom(item))
                {
                    return true;
                }
            }
            return false;
        }

        private static int CheckTypeAndGetNumber(Type paramType)
        {
            foreach (var item in TypeToMods)
            {
                if (paramType.IsAssignableFrom(item.Key))
                {
                    return item.Value;
                }
            }
            return 0;
        }

        public static void ExecuteEvent(ICustomMod customModInterface)
        {
            if (!ModInterfaces.TryGetValue(customModInterface.InterfaceNumber, out var @event))
            {
                Debugger.Print(string.Format("Event {0} is not registered in Manager!", customModInterface.InterfaceNumber));
                return;
            }
            foreach (List<EventInvokeLocation> list in @event.Invokers.Values)
            {
                foreach (EventInvokeLocation eventInvokeLocation in list)
                {
                    eventInvokeLocation.Method.Invoke(eventInvokeLocation.Target, [customModInterface]);
                }
            }
        }
    }
}
