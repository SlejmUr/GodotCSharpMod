namespace ModAPI.V1.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class V1EventAttribute : Attribute
    {
        public int InterfaceTypeNumer { get; } = 0;
        public V1EventAttribute(int interfaceTypeNumer)
        {
            InterfaceTypeNumer = interfaceTypeNumer;
        }

        public V1EventAttribute() 
        { 
        
        }
    }
}
