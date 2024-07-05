using Godot.Bridge;
using ModAPI.V0;
using ModAPI.V1;
using ModAPI.V2;
using System.Reflection;
using System.Runtime.Loader;

namespace ModAPI
{
    public class MainLoader
    {
        static readonly AssemblyLoadContext MainLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;
        public static string ModsDirName = "Mods";
        public static List<Assembly> Mods = new();
        static List<AssemblyName> LoadedNames = new();
        internal static LoadSettings settings = new();
        internal static Assembly? CallingAsm;

        public static void Init(LoadSettings _settings)
        {
            settings = _settings;
            CallingAsm = Assembly.GetCallingAssembly();
            Init();
        }

        public static void Init()
        {
            MainLoadContext.Unloading += MainLoadContext_Unloading;
            MainLoadContext.Resolving += MainLoadContext_Resolving;
            if (settings.LoadAsMainCaller && CallingAsm != null)
            {
                SetMainModAssembly(CallingAsm);
            }
        }

        public static void DeInit()
        {
            MainLoadContext.Unloading -= MainLoadContext_Unloading;
            MainLoadContext.Resolving -= MainLoadContext_Resolving;
            V0Manager.Unload();
            V2Manager.Unload();      
            Mods.Clear();
            settings.ModAPI = ModAPIEnum.None;
        }

        public static bool SetMainModAssembly(Assembly assembly)
        {
            var attrib = assembly.GetCustomAttribute<ModAPITypeAttribute>();
            if (attrib == null && !settings.LoadWithoutAssemblyDefinedModAPI)
                return false;

            if (settings.LoadWithoutAssemblyDefinedModAPI)
            {
                Debugger.logger?.Verbose($"Loading Main ModAPIType failed. Settings said we should use: {settings.ModAPI}");
                switch (settings.ModAPI)
                {
                    case ModAPIEnum.V1:
                        V1Manager.LoadFromMain(assembly);
                        break;
                    case ModAPIEnum.V2:
                        V2Manager.LoadFromMain(assembly);
                        break;
                    case ModAPIEnum.All:
                        V1Manager.LoadFromMain(assembly);
                        V2Manager.LoadFromMain(assembly);
                        break;
                    default:
                        break;
                }
                return true;
            }
            if (attrib != null && attrib.APIType != ModAPIEnum.None)
            {
                settings.ModAPI = attrib.APIType;
                switch (settings.ModAPI)
                {
                    case ModAPIEnum.V1:
                        V1Manager.LoadFromMain(assembly);
                        break;
                    case ModAPIEnum.V2:
                        V2Manager.LoadFromMain(assembly);
                        break;
                    case ModAPIEnum.All:
                        V1Manager.LoadFromMain(assembly);
                        V2Manager.LoadFromMain(assembly);
                        break;
                    default:
                        break;
                }
                return true;
            }
            return false;
        }

        public static void LoadDependencies()
        {
            var dep = Path.Combine(Directory.GetCurrentDirectory(), ModsDirName, "Dependencies");
            if (!Directory.Exists(dep))
                Directory.CreateDirectory(dep);
            foreach (var dll in Directory.GetFiles(dep, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var assembly = MainLoadContext.LoadFromAssemblyPath(dll);
                var assemblyName = assembly.GetName();
                Debugger.logger?.Debug(assemblyName + " Loaded as Dependecy");
            }
            LoadedNames = MainLoadContext.Assemblies.Select(x=>x.GetName()).ToList();
            Debugger.logger?.Information("Loaded all dependency!");
        }

        private static Assembly? MainLoadContext_Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            var asm = context.Assemblies.Where(x=>x.GetName().FullName == assemblyName.FullName).FirstOrDefault();
            if (asm != null)
                return asm;

            Debugger.logger?.Warning($"ERROR! You are missing a Dependency! Name: {assemblyName}");

            return null;
        }

        private static void MainLoadContext_Unloading(AssemblyLoadContext obj)
        {
            Debugger.logger?.Verbose("MainLoadContext_Unloading");
            if (obj.IsCollectible)
                obj.Unload();
        }

        public static bool LoadModInMods(string modName)
        {
            var mods = Path.Combine(Directory.GetCurrentDirectory(), ModsDirName);
            if (File.Exists(Path.Combine(mods, modName)))
            {
                return LoadMod(Path.Combine(mods, modName));
            }
            return false;
        }

        public static bool LoadMod(string ModPath)
        {
            try
            {
                Debugger.logger?.Verbose("Loading plugin: " + ModPath);
                // Loading Assembly
                var asm = MainLoadContext.LoadFromAssemblyPath(ModPath);
                var loaded_nv = LoadedNames.Select(x=>x.Name + " v" + x.Version?.ToString());
                var ref_nv = asm.GetReferencedAssemblies().Select(x => x.Name + " v" + x.Version?.ToString());
                var req = ref_nv.Where(x=> !loaded_nv.Contains(x)).ToList();
                if (req.Count > 0)
                {
                    Debugger.logger?.Warning($"!!! Plugin Requires Assemblies: \n{string.Join("\n", req)}\n Plugin will not load! Fix issues!");
                    return false;
                }
                Mods.Add(asm);
                switch (settings.ModAPI)
                {
                    case ModAPIEnum.V0:
                        V0Manager.Register(asm);
                        break;
                    case ModAPIEnum.V1:
                        V1Manager.RegisterEvents(asm);
                        break;
                    case ModAPIEnum.V2:
                        V2Manager.LoadMod(asm);
                        break;
                    case ModAPIEnum.All:
                        V0Manager.Register(asm);
                        V1Manager.RegisterEvents(asm);
                        V2Manager.LoadMod(asm);
                        break;
                    default:
                        break;
                }
                //  For godot.
                ScriptManagerBridge.LookupScriptsInAssembly(asm);
                return true;
            }
            catch (Exception ex)
            {
                Debugger.logger?.Error("Could not load mod {Mod}! {Exception}", ModPath, ex);
                return false;
            }
        }
    }
}
