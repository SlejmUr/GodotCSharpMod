using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace ModAPI.ModLoad
{
    //  Source Code used: https://github.com/godotengine/godot/blob/master/modules/mono/glue/GodotSharp/GodotPlugins/Main.cs
    //  Thanks Godot for this!
    public sealed class ModLoadContextWrapper
    {
        private ModLoadContext? _ModLoadContext;
        private readonly WeakReference _weakReference;
        public ModLoadContextWrapper(ModLoadContext ModLoadContext, WeakReference weakReference)
        {
            this._ModLoadContext = ModLoadContext;
            this._weakReference = weakReference;
        }

        public string AssemblyLoadedPath
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                ModLoadContext? ModLoadContext = this._ModLoadContext;
                if (ModLoadContext == null)
                {
                    return string.Empty;
                }
                return ModLoadContext.AssemblyLoadedPath;
            }
        }

        public bool IsCollectible
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                ModLoadContext? ModLoadContext = this._ModLoadContext;
                return ModLoadContext == null || ModLoadContext.IsCollectible;
            }
        }

        public bool IsAlive
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                return this._weakReference.IsAlive;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ValueTuple<Assembly, ModLoadContextWrapper> CreateAndLoadFromAssemblyName(AssemblyName assemblyName, string pluginPath, ICollection<string> sharedAssemblies, AssemblyLoadContext mainLoadContext, bool isCollectible)
        {
            ModLoadContext ModLoadContext = new ModLoadContext(pluginPath, sharedAssemblies, mainLoadContext, isCollectible);
            WeakReference reference = new WeakReference(ModLoadContext, true);
            ModLoadContextWrapper wrapper = new ModLoadContextWrapper(ModLoadContext, reference);
            return new ValueTuple<Assembly, ModLoadContextWrapper>(ModLoadContext.LoadFromAssemblyName(assemblyName), wrapper);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Unload()
        {
            ModLoadContext? ModLoadContext = this._ModLoadContext;
            if (ModLoadContext != null)
            {
                ModLoadContext.Unload();
            }
            this._ModLoadContext = null;
        }
    }
}
