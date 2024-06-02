using Godot.Bridge;
using ModAPI.Attributes;
using ModAPI.V1;
using ModAPI.V2;
using System.Reflection;
using System.Runtime.Loader;

namespace ModAPI.ModLoad
{
    public class MainLoader
    {
        //  Source Code used: https://github.com/godotengine/godot/blob/master/modules/mono/glue/GodotSharp/GodotPlugins/Main.cs
        //  Thanks Godot for this!

        public static string ModsDirName = "Mods";
        static readonly List<AssemblyName> SharedAssemblies = new List<AssemblyName>();
        static readonly Assembly CoreApiAssembly = typeof(MainLoader).Assembly;
        static readonly AssemblyLoadContext MainLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;
        static Assembly? ModMainAssembly = null;
        public static List<(Assembly assembly, ModLoadContextWrapper context)> Mods = new();
        static Enums.ModAPIEnum ModAPI = Enums.ModAPIEnum.None;
        public static void Init()
        {
            SharedAssemblies.Add(CoreApiAssembly.GetName());
            SharedAssemblies.Add(typeof(Godot.GodotObject).Assembly.GetName());
            var dep = Path.Combine(Directory.GetCurrentDirectory(), ModsDirName, "Dependencies");
            if (!Directory.Exists(dep))
                Directory.CreateDirectory(dep);
            foreach (var dll in Directory.GetFiles(dep, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var assembly = MainLoadContext.LoadFromAssemblyPath(dll);
                var assemblyName = assembly.GetName();
                Debugger.Print(assemblyName + " Loaded as Dependecy");
                SharedAssemblies.Add(assemblyName);
            }
        }

        public static void AddSharedAssembly(Assembly assembly)
        {
            SharedAssemblies.Add(assembly.GetName());
        }

        public static void AddSharedAssembly(string filename)
        {
            var assembly = MainLoadContext.LoadFromAssemblyPath(filename);
            var assemblyName = assembly.GetName();
            Debugger.Print(assemblyName + " Loaded as Dependecy");
            SharedAssemblies.Add(assemblyName);
        }

        public static bool SetMainModAssembly(Assembly assembly)
        {
            var attrib = assembly.GetCustomAttribute<ModAPITypeAttribute>();
            if (attrib == null)
                return false;

            if (attrib.APIType != Enums.ModAPIEnum.None)
            {
                ModMainAssembly = assembly;
                SharedAssemblies.Add(assembly.GetName());
                ModAPI = attrib.APIType;
                switch (ModAPI)
                {
                    case Enums.ModAPIEnum.V1:
                        V1Manager.LoadFromMain(ModMainAssembly);
                        break;
                    case Enums.ModAPIEnum.V2:
                        V2EventManager.LoadFromMain(ModMainAssembly);
                        break;
                    case Enums.ModAPIEnum.All:
                        V1Manager.LoadFromMain(ModMainAssembly);
                        V2EventManager.LoadFromMain(ModMainAssembly);
                        break;
                    default:
                        break;
                }
                return true;
            }
            return false;
        }

        public static void DeInit()
        {
            V2EventManager.Unload();
            V1IModManager.Unload();
            Mods.Clear();
            SharedAssemblies.Clear();
            ModMainAssembly = null;
            ModAPI = Enums.ModAPIEnum.None;
        }


        public static void LoadPlugins()
        {
            var mods = Path.Combine(Directory.GetCurrentDirectory(), ModsDirName);
            foreach (var mod in Directory.GetFiles(mods, "*.dll", SearchOption.TopDirectoryOnly))
            {
                string assemblyName = Path.GetFileNameWithoutExtension(mod);
                List<string> sharedAssemblies = new List<string>();
                foreach (AssemblyName assemblyName2 in SharedAssemblies)
                {
                    string? sharedAssemblyName = assemblyName2.Name;
                    if (sharedAssemblyName != null)
                    {
                        sharedAssemblies.Add(sharedAssemblyName);
                    }
                }
                var tuple = ModLoadContextWrapper.CreateAndLoadFromAssemblyName(new AssemblyName(assemblyName), mod, sharedAssemblies, MainLoadContext, false);
                Mods.Add(tuple);
                var assembly = tuple.Item1;
                switch (ModAPI)
                {
                    case Enums.ModAPIEnum.V1:
                        V1Manager.RegisterEvents(assembly);
                        break;
                    case Enums.ModAPIEnum.V2:
                        V2EventManager.LoadPlugin(assembly);
                        break;
                    case Enums.ModAPIEnum.All:
                        V1Manager.RegisterEvents(assembly);
                        V2EventManager.LoadPlugin(assembly);
                        break;
                    default:
                        break;
                }
                //  For godot.
               ScriptManagerBridge.LookupScriptsInAssembly(assembly);
            }
        }

        public static void LoadPlugin(string DllName)
        {
            Debugger.Print("Loading plugin: " + DllName);
            string assemblyName = Path.GetFileNameWithoutExtension(DllName);
            List<string> sharedAssemblies = new List<string>();
            foreach (AssemblyName assemblyName2 in SharedAssemblies)
            {
                string? sharedAssemblyName = assemblyName2.Name;
                if (sharedAssemblyName != null)
                {
                    sharedAssemblies.Add(sharedAssemblyName);
                }
            }
            var tuple = ModLoadContextWrapper.CreateAndLoadFromAssemblyName(new AssemblyName(assemblyName), Path.GetDirectoryName(DllName), sharedAssemblies, MainLoadContext, false);
            Mods.Add(tuple);
            var assembly = tuple.Item1;
            switch (ModAPI)
            {
                case Enums.ModAPIEnum.V1:
                    V1Manager.RegisterEvents(assembly);
                    break;
                case Enums.ModAPIEnum.V2:
                    V2EventManager.LoadPlugin(assembly);
                    break;
                case Enums.ModAPIEnum.All:
                    V1Manager.RegisterEvents(assembly);
                    V2EventManager.LoadPlugin(assembly);
                    break;
                default:
                    break;
            }
            //  For godot.
            ScriptManagerBridge.LookupScriptsInAssembly(assembly);
        }


        [Obsolete("Recommend using LoadPlugins instead if you load more than 1 mod/assembly")]
        public static ValueTuple<Assembly, ModLoadContextWrapper> GetAndLoadPlugin(string assemblyPath)
        {
            string assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
            List<string> sharedAssemblies = new List<string>();
            foreach (AssemblyName assemblyName2 in MainLoader.SharedAssemblies)
            {
                string? sharedAssemblyName = assemblyName2.Name;
                if (sharedAssemblyName != null)
                {
                    sharedAssemblies.Add(sharedAssemblyName);
                }
            }
            return ModLoadContextWrapper.CreateAndLoadFromAssemblyName(new AssemblyName(assemblyName), assemblyPath, sharedAssemblies, MainLoader.MainLoadContext, false);
        }

        //No idea how to use, nothing use this
        public static bool UnloadPlugin(ref ModLoadContextWrapper? ModLoadContext)
        {
            bool result;
            try
            {
                if (ModLoadContext == null)
                {
                    result = true;
                }
                else if (!ModLoadContext.IsCollectible)
                {
                    Console.Error.WriteLine("Cannot unload a non-collectible assembly load context.");
                    result = false;
                }
                else
                {
                    Console.WriteLine("Unloading assembly load context...");
                    ModLoadContext.Unload();
                    int startTimeMs = Environment.TickCount;
                    bool takingTooLong = false;
                    while (ModLoadContext.IsAlive)
                    {
                        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                        GC.WaitForPendingFinalizers();
                        if (!ModLoadContext.IsAlive)
                        {
                            break;
                        }
                        int elapsedTimeMs = Environment.TickCount - startTimeMs;
                        if (!takingTooLong && elapsedTimeMs >= 200)
                        {
                            takingTooLong = true;
                            Console.Error.WriteLine("Assembly unloading is taking longer than expected...");
                        }
                        else if (elapsedTimeMs >= 1000)
                        {
                            Console.Error.WriteLine("Failed to unload assemblies. Possible causes: Strong GC handles, running threads, etc.");
                            return false;
                        }
                    }
                    Console.WriteLine("Assembly load context unloaded successfully.");
                    ModLoadContext = null;
                    result = true;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                result = false;
            }
            return result;
        }
    }
}
