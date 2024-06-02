using System.Reflection;
using System.Runtime.Loader;

namespace ModAPI.ModLoad
{
    //  Source Code used: https://github.com/godotengine/godot/blob/master/modules/mono/glue/GodotSharp/GodotPlugins/PluginLoadContext.cs
    //  Thanks Godot for this!
    public class ModLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly ICollection<string> _sharedAssemblies;
        private readonly AssemblyLoadContext _mainLoadContext;
        public string AssemblyLoadedPath { get; private set; }

        public ModLoadContext(string pluginPath, ICollection<string> sharedAssemblies, AssemblyLoadContext mainLoadContext, bool isCollectible) : base(isCollectible)
        {
            this._resolver = new AssemblyDependencyResolver(pluginPath);
            this._sharedAssemblies = sharedAssemblies;
            this._mainLoadContext = mainLoadContext;
            this.AssemblyLoadedPath = pluginPath;
            if (string.IsNullOrEmpty(AppContext.BaseDirectory))
            {
                //Debugger.Print(pluginPath + " " + Path.GetDirectoryName(pluginPath));
                string? baseDirectory = pluginPath;
                if (baseDirectory != null)
                {
                    if (!Path.EndsInDirectorySeparator(baseDirectory))
                    {
                        baseDirectory += Path.DirectorySeparatorChar.ToString();
                    }
                    AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", baseDirectory);
                }
                else
                    Console.Error.WriteLine("Failed to set AppContext.BaseDirectory. Dynamic loading of libraries may fail.");
            }
        }
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            Debugger.Print($"MLC.Load: {assemblyName.FullName}");
            if (string.IsNullOrEmpty(assemblyName.Name))
            {
                return null;
            }
            if (this._sharedAssemblies.Contains(assemblyName.Name))
            {
                return this._mainLoadContext.LoadFromAssemblyName(assemblyName);
            }
            string context_file = Path.Combine(AppContext.BaseDirectory, assemblyName.FullName.Split(",")[0] + ".dll");
            if (File.Exists(context_file))
            {
                using (FileStream assemblyFile = File.Open(context_file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return base.LoadFromStream(assemblyFile);
                }
            }

            string possible_file = Path.Combine(this.AssemblyLoadedPath, assemblyName.FullName.Split(",")[0] + ".dll");
            if (File.Exists(possible_file))
            {
                using (FileStream assemblyFile = File.Open(possible_file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string pdbPath = Path.ChangeExtension(possible_file, ".pdb");
                    if (File.Exists(pdbPath))
                    {
                        using (FileStream pdbFile = File.Open(pdbPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            return base.LoadFromStream(assemblyFile, pdbFile);
                        }
                    };
                    return base.LoadFromStream(assemblyFile);
                }
            }
            return null;
        }
    }
}
