﻿using ModAPI.V1.Interfaces;
using System.Reflection;

namespace ModAPI.V1
{
    internal class V1IModManager
    {
        public static Dictionary<string,IMod> Mods = new();

        public static void Register(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IMod).IsAssignableFrom(type))
                {
                    IMod mod = (IMod)Activator.CreateInstance(type);
                    Mods.Add(assembly.FullName, mod);
                    mod.Start();
                }
            }
        }

        public static void RegisterMethod(string type, MethodInfo methodInfo, int InterfaceNumber)
        {
            if (Mods.TryGetValue(type, out IMod mod))
            {
                mod.MethodRegistered(methodInfo, InterfaceNumber);
            }
        }

        public static void Unload() 
        {
            foreach (var item in Mods)
            {
                item.Value.Stop();
            }
            Mods.Clear();
        }


    }
}
