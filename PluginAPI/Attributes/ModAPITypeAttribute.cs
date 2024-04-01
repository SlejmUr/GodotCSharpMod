using ModAPI.Enums;

namespace ModAPI.Attributes
{
    /// <summary>
    /// Make sure we know what API type we use for this Assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class ModAPITypeAttribute : Attribute
    {
        public ModAPIEnum APIType = ModAPIEnum.None;
        public ModAPITypeAttribute(ModAPIEnum modAPI)
        {
            this.APIType = modAPI;
        }

        public ModAPITypeAttribute()
        {
        }
    }
}
