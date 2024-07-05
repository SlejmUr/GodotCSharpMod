namespace ModAPI
{
    public struct LoadSettings
    {
        public bool V0_SkipAllAction;
        public bool V1_EnableICustomModInterfaceParsing;
        public bool V1_EnableMainAsMod;
        public bool V2_EnableMainAsMod;
        public bool LoadWithoutAssemblyDefinedModAPI;
        public bool LoadAsMainCaller;
        public ModAPIEnum ModAPI;

        public LoadSettings()
        {
            this.V0_SkipAllAction = false;
            this.V1_EnableICustomModInterfaceParsing = false;
            this.V1_EnableMainAsMod = false;
            this.V2_EnableMainAsMod = false;
            this.LoadWithoutAssemblyDefinedModAPI = false;
            this.LoadAsMainCaller = false;
            this.ModAPI = ModAPIEnum.None;
        }
    }
}
