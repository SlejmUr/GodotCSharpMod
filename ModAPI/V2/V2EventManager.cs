using System.Reflection;
using System.Linq.Expressions;

namespace ModAPI.V2
{
    public class V2EventManager
    {
        private static Dictionary<Type, HashSet<Delegate>> _events = [];
        private static List<Type> BaseEventDeclaredTypes = [];

        public static void LoadFromMain(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(BaseEvent))
                    continue;
                Debugger.Print($"V2 Event type added: {type}");
                BaseEventDeclaredTypes.Add(type);
            }
            List<MethodInfo> noPriority_Methods = new();
            List<MethodInfo> HighPriority_Methods = new();
            List<MethodInfo> LowPriority_Methods = new();
            foreach (var type in assembly.GetTypes())
            {
                var methods = type.GetMethods().Where(x => x.GetParameters().Length == 1 && BaseEventDeclaredTypes.Contains(x.GetParameters()[0].ParameterType));
                if (!methods.Any())
                    continue;
                noPriority_Methods.AddRange(methods.Where(x=>x.GetCustomAttribute<V2Priority>() == null));
                HighPriority_Methods.AddRange(methods.Where(x => x.GetCustomAttribute<V2Priority>() != null && x.GetCustomAttribute<V2Priority>()!.Priority > 0));
                LowPriority_Methods.AddRange(methods.Where(x => x.GetCustomAttribute<V2Priority>() != null && x.GetCustomAttribute<V2Priority>()!.Priority < 0));
            }

            HighPriority_Methods = HighPriority_Methods.OrderByDescending(x => x.GetCustomAttribute<V2Priority>()!.Priority).ToList();
            foreach (var item in HighPriority_Methods)
            {
                Type param = item.GetParameters()[0].ParameterType;
                var @delegate = Delegate.CreateDelegate(Expression.GetActionType(param), item);
                SubscribeEvent(param, @delegate);
                Debugger.Print($"V2 Event type {param} has a calling function: {item} (Priority: {item.GetCustomAttribute<V2Priority>()!.Priority})");
            }
            foreach (var item in noPriority_Methods)
            {
                Type param = item.GetParameters()[0].ParameterType;
                var @delegate = Delegate.CreateDelegate(Expression.GetActionType(param), item);
                SubscribeEvent(param, @delegate);
                Debugger.Print($"V2 Event type {param} has a calling function: {item} (No Priority)");
            }
            LowPriority_Methods = LowPriority_Methods.OrderByDescending(x => x.GetCustomAttribute<V2Priority>()!.Priority).ToList();
            foreach (var item in LowPriority_Methods)
            {
                Type param = item.GetParameters()[0].ParameterType;
                var @delegate = Delegate.CreateDelegate(Expression.GetActionType(param), item);
                SubscribeEvent(param, @delegate);
                Debugger.Print($"V2 Event type {param} has a calling function: {item} (Priority: {item.GetCustomAttribute<V2Priority>()!.Priority})");
            }
        }

        public static void Unload()
        {
            BaseEventDeclaredTypes.Clear();
            _events.Clear();
        }


        public static void TriggerEvent<TEvent>(TEvent e) where TEvent : BaseEvent
        {
            if (_events.TryGetValue(typeof(TEvent), out var delegates))
            {
                foreach (var @delegate in delegates)
                {
                    ((Action<TEvent>)@delegate)(e);
                }
            }
        }

        public static void SubscribeEvent<T>(Action<T> @delegate) where T : BaseEvent
        {
            HashSet<Delegate> delegates;
            if (!_events.TryGetValue(typeof(T), out delegates))
            {
                _events[typeof(T)] = delegates = [];
            }

            delegates.Add(@delegate);
        }

        public static void SubscribeEvent(Type baseEventType , Delegate @delegate)
        {
            HashSet<Delegate> delegates;
            if (!_events.TryGetValue(baseEventType, out delegates))
            {
                _events[baseEventType] = delegates = [];
            }

            delegates.Add(@delegate);
        }

        public static void UnsubscribeEvent<T>(Action<T> @delegate) where T : BaseEvent
        {
            if (_events.TryGetValue(typeof(T), out var delegates))
            {
                delegates.Remove(@delegate);
            }
        }
    }
}
