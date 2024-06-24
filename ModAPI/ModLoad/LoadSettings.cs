namespace ModAPI
{
    public struct LoadSettings
    {
        public bool V0_SkipAllAction;
        public bool V1_EnableICustomModInterfaceParsing;
        public bool V2_EnableModAsEvent;
        public bool LoadWithoutAssemblyDefinedModAPI;
        public bool LoadAsMainCaller;
        public ModAPIEnum ModAPI;

        public LoadSettings()
        {
            this.V0_SkipAllAction = false;
            this.V1_EnableICustomModInterfaceParsing = false;
            this.V2_EnableModAsEvent = false;
            this.LoadWithoutAssemblyDefinedModAPI = false;
            this.LoadAsMainCaller = false;
            this.ModAPI = ModAPIEnum.None;
        }
    }
}
