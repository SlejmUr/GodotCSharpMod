﻿using System.Reflection;
using System.Linq.Expressions;
using ModAPI.V0;

namespace ModAPI.V2;

public class V2Manager
{
    private static Dictionary<Type, HashSet<Delegate>> _events = [];
    private static List<Type> BaseEventDeclaredTypes = [];

    public static void LoadFromMain(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(BaseEvent))))
        {
            Debugger.logger?.Verbose($"V2 Event type added: {type}");
            BaseEventDeclaredTypes.Add(type);
        }

        if (MainLoader.settings.V2_EnableMainAsMod)
            LoadMod(assembly);
    }

    public static void Unload()
    {
        BaseEventDeclaredTypes.Clear();
        _events.Clear();
    }

    public static void LoadMod(Assembly assembly)
    {
        Debugger.logger?.Verbose("Loading V2 Plugin: " + assembly.FullName);
        List<MethodInfo> noPriority_Methods = [];
        List<MethodInfo> HighPriority_Methods = [];
        List<MethodInfo> LowPriority_Methods = [];
        foreach (var type in assembly.GetTypes())
        {
            var methods = type.GetMethods().Where(x => x.GetParameters().Length == 1 && BaseEventDeclaredTypes.Contains(x.GetParameters()[0].ParameterType));
            if (!methods.Any())
                continue;
            noPriority_Methods.AddRange(methods.Where(x => x.GetCustomAttribute<V2Priority>() == null));
            HighPriority_Methods.AddRange(methods.Where(x => x.GetCustomAttribute<V2Priority>() != null && x.GetCustomAttribute<V2Priority>()!.Priority > 0));
            LowPriority_Methods.AddRange(methods.Where(x => x.GetCustomAttribute<V2Priority>() != null && x.GetCustomAttribute<V2Priority>()!.Priority < 0));
        }

        foreach (var item in HighPriority_Methods.OrderByDescending(x => x.GetCustomAttribute<V2Priority>()!.Priority).ToList())
        {
            Type param = item.GetParameters()[0].ParameterType;
            var @delegate = Delegate.CreateDelegate(Expression.GetActionType(param), item);
            SubscribeEvent(param, @delegate, item.GetCustomAttribute<V2Priority>());
            Debugger.logger?.Verbose($"V2 Event type {param} has a calling function: {item} (Priority: {item.GetCustomAttribute<V2Priority>()!.Priority})");
        }
        foreach (var item in noPriority_Methods)
        {
            Type param = item.GetParameters()[0].ParameterType;
            var @delegate = Delegate.CreateDelegate(Expression.GetActionType(param), item);
            SubscribeEvent(param, @delegate);
            Debugger.logger?.Verbose($"V2 Event type {param} has a calling function: {item} (No Priority)");
        }
        foreach (var item in LowPriority_Methods.OrderByDescending(x => x.GetCustomAttribute<V2Priority>()!.Priority).ToList())
        {
            Type param = item.GetParameters()[0].ParameterType;
            var @delegate = Delegate.CreateDelegate(Expression.GetActionType(param), item);
            SubscribeEvent(param, @delegate, item.GetCustomAttribute<V2Priority>());
            Debugger.logger?.Verbose($"V2 Event type {param} has a calling function: {item} (Priority: {item.GetCustomAttribute<V2Priority>()!.Priority})");
        }
    }

    public static void TriggerEvent<TEvent>(TEvent e) where TEvent : BaseEvent
    {
        if (_events.TryGetValue(typeof(TEvent), out var delegates))
        {
            foreach (var @delegate in delegates)
            {
                V0Manager.V2Call(@delegate.Method, e);
                ((Action<TEvent>)@delegate)(e);
            }
        }
    }

    public static void SubscribeEvent<T>(Action<T> @delegate) where T : BaseEvent
    {
        if (!_events.TryGetValue(typeof(T), out HashSet<Delegate>? delegates))
        {
            _events[typeof(T)] = delegates = [];
        }
        V0Manager.V2Register(null, typeof(T), @delegate.Method);
        delegates.Add(@delegate);
    }

    public static void SubscribeEvent(Type baseEventType , Delegate @delegate, V2Priority? priority = null)
    {
        if (!_events.TryGetValue(baseEventType, out HashSet<Delegate>? delegates))
        {
            _events[baseEventType] = delegates = [];
        }
        V0Manager.V2Register(priority, baseEventType, @delegate.Method);
        delegates.Add(@delegate);
    }

    public static void UnsubscribeEvent<T>(Action<T> @delegate) where T : BaseEvent
    {
        if (_events.TryGetValue(typeof(T), out var delegates))
        {
            V0Manager.V2Unregister(typeof(T), @delegate.Method);
            delegates.Remove(@delegate);
        }
    }
}
