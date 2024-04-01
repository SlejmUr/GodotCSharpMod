using ModAPI.Enums;

namespace ModAPI.ModLoad
{
    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    public struct LoadSettings
    {
        public bool V1_EnableICustomModInterfaceParsing;
        public bool LoadWithoutAssemblyDefinedModAPI;
        public ModAPIEnum ModAPI;

        public LoadSettings()
        {
            this.V1_EnableICustomModInterfaceParsing = false;
            this.LoadWithoutAssemblyDefinedModAPI = false;
            this.ModAPI = ModAPIEnum.None;
        }
    }
}
