using ModAPI.V1;
using ModAPI.V2;
using System.Reflection;

namespace ModAPI.V0;

public abstract class V0Mod
{
    public virtual void Load()
    {

    }

    public virtual void V1Register(MethodInfo methodInfo, int InterfaceNumber)
    {

    }

    public virtual void V1Call(EventInvokeLocation eventInvokeLocation, ICustomMod argument)
    {

    }

    public virtual void V2Register(V2Priority? priority, Type eventType, MethodInfo methodInfo)
    {

    }

    public virtual void V2Call(MethodInfo methodInfo, BaseEvent @event)
    {

    }

    public virtual void V2Unregister(Type eventType, MethodInfo methodInfo)
    {

    }

    public virtual void Unload() 
    { 
    
    }
}
