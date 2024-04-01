using System.Reflection;

namespace ModAPI.V1.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMod
    {
        /// <summary>
        /// Executes when first got the interface.
        /// </summary>
        public void Start();

        /// <summary>
        /// Executes when the Method got registered. (Ensure it working)
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="InterfaceNumber"></param>
        public void MethodRegistered(MethodInfo methodInfo, int InterfaceNumber);

        /// <summary>
        /// Executes when we stop/unload this plugin.
        /// </summary>
        public void Stop();
    }
}
