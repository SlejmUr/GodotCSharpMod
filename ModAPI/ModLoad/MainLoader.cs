using Godot.Bridge;
using ModAPI.V0;
using ModAPI.V1;
using ModAPI.V2;
using Serilog;
using System.Reflection;
using System.Runtime.Loader;

namespace ModAPI;

public class MainLoader
{
    public static readonly AssemblyLoadContext MainLoadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly()) ?? AssemblyLoadContext.Default;
    public static string ModsDirName = "Mods";
    public static List<Assembly> Mods { get; internal set; } = [];
    static List<AssemblyName> LoadedNames = [];
    internal static LoadSettings settings = new();
    internal static Assembly? CallingAsm;

    /// <summary>
    /// Initializing the loader with special settings
    /// </summary>
    /// <param name="_settings">The Loader settings</param>
    public static void Init(LoadSettings _settings)
    {
        settings = _settings;
        CallingAsm = Assembly.GetCallingAssembly();
        Init();
    }

    /// <summary>
    /// Initialize the Assembly.
    /// </summary>
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
        V1Manager.Unload();
        V2Manager.Unload();      
        Mods.Clear();
        settings.ModAPI = ModAPIEnum.None;
    }

    public static bool SetMainModAssembly(Assembly assembly)
    {
        var attrib = assembly.GetCustomAttribute<ModAPITypeAttribute>();
        if (attrib == null && !settings.LoadWithoutAssemblyDefinedModAPI)
            return false;

        if (attrib != null && attrib.APIType != ModAPIEnum.None)
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

    public static void LoadDependencies(string dependecyDir = "Dependencies", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        string dep = dependecyDir;
        if (!dependecyDir.Contains(Path.DirectorySeparatorChar))
            dep = Path.Combine(Directory.GetCurrentDirectory(), ModsDirName, dependecyDir);
        if (!Directory.Exists(dep))
            Directory.CreateDirectory(dep);
        foreach (var dll in Directory.GetFiles(dep, "*.dll", searchOption))
        {
            var assembly = MainLoadContext.LoadFromAssemblyPath(dll);
            Log.Debug("{AssemblyName} Loaded as Dependecy!", assembly.GetName());
        }
        LoadedNames = MainLoadContext.Assemblies.Select(x=>x.GetName()).ToList();
        Log.Information("Loaded all dependency!");
    }

    private static Assembly? MainLoadContext_Resolving(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        var asm = context.Assemblies.Where(x=>x.GetName().FullName == assemblyName.FullName).FirstOrDefault();
        if (asm != null)
            return asm;

        Log.Warning("ERROR! You are missing a Dependency! Name: {AssemblyName}", assemblyName);
        File.WriteAllText("Context_ASM.txt", string.Join("\n", context.Assemblies.Select(x=>x.GetName().FullName)));
        return null;
    }

    private static void MainLoadContext_Unloading(AssemblyLoadContext obj)
    {
        Log.Verbose("MainLoadContext_Unloading");
        if (obj.IsCollectible)
            obj.Unload();
    }

    public static bool LoadModsInModsDir(SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), ModsDirName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        List<bool> rets = [];
        foreach (var dll in Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly))
        {
            rets.Add(LoadMod(dll));
        }
        return !rets.Any(x => x == false);
    }

    public static bool LoadModInCustomDirectory(string directoryName, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        string dir = directoryName;
        if (!directoryName.Contains(Path.DirectorySeparatorChar))
            dir = Path.Combine(Directory.GetCurrentDirectory(), ModsDirName, directoryName);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        List<bool> rets = [];
        foreach (var dll in Directory.GetFiles(dir, "*.dll", searchOption))
        {
            rets.Add(LoadMod(dll));
        }
        return !rets.Any(x=> x == false);
    }

    public static bool LoadMod(string ModPath)
    {
        try
        {
            Log.Verbose("Loading plugin: {ModPath}", ModPath);
            // Loading Assembly
            var asm = MainLoadContext.LoadFromAssemblyPath(ModPath);
            var loaded_nv = LoadedNames.Select(x=>x.Name + " v" + x.Version?.ToString());
            var ref_nv = asm.GetReferencedAssemblies().Select(x => x.Name + " v" + x.Version?.ToString());
            var req = ref_nv.Where(x=> !loaded_nv.Contains(x)).ToList();
            if (req.Count > 0)
            {
                Log.Warning("!!! Plugin Requires Assemblies: \n{ListOfRequiredAssemblies}\n Plugin will not load! Fix issues!", string.Join("\n", req));
                File.WriteAllText("Context_ASM.txt", string.Join("\n", MainLoadContext.Assemblies.Select(x => x.GetName().FullName)));
                File.WriteAllText("loaded_nv.txt", string.Join("\n", loaded_nv));
                File.WriteAllText("ref_nv.txt", string.Join("\n", ref_nv));
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
            Log.Error("Could not load mod {Mod}! {Exception}", ModPath, ex);
            return false;
        }
    }
}
