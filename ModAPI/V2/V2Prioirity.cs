namespace ModAPI.V2;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class V2Priority : Attribute
{
    public int Priority { get; } = 0;
    public V2Priority(int priority)
    {
        Priority = priority;
    }

    public V2Priority()
    {

    }
}
