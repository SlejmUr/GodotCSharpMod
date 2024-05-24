using Godot;
using System;
using ModAPI;
using ModAPI.V1;
using ModAPI.ModLoad;
using Game.csharp.ModAdds;
using ModAPI.V2;
using Game.csharp.v2;
namespace Game;

public partial class Menu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Debugger.Enabled = true;
        V1Manager.EnableICustomModInterfaceParsing = true;
        MainLoader.Init();
        MainLoader.AddSharedAssembly(typeof(Menu).Assembly);
        MainLoader.SetMainModAssembly(typeof(Menu).Assembly);
        MainLoader.LoadPlugins();
        //V2EventManager.SubscribeEvent<func2>(v2runner.func2_run);
        //V2EventManager.SubscribeEvent<test2>(v2runner.test_run);
        /*
		PluginAPI.Test.Main.Init();
		bool Loaded = PluginAPI.Test.Main.LoadProjectAssembly("G:\\GODOT\\PluginMaker\\Game\\EXPORT\\Mods\\Plugin1.dll", out string asmPath, out var assembly);
        Console.WriteLine(Loaded);
        Console.WriteLine(asmPath);
        Console.WriteLine(PluginAPI.Test.Main._projectLoadContext + " "+ PluginAPI.Test.Main._projectLoadContext.AssemblyLoadedPath);
        if (assembly != null)
        {
            foreach (var item in assembly.GetTypes())
            {
                if (item.IsValidEntrypoint())
                {
                    Console.WriteLine(item.Name);
                    var entry = item.GetValidEntrypoint();
                    var obj = Activator.CreateInstance(item);
                    entry.Invoke(obj, null);
                }
                if (item.IsAssignableTo(typeof(ITestInterface)))
                {
                    Console.WriteLine(item.Name);
                    var obj = (ITestInterface)Activator.CreateInstance(item);
                    obj.InitTest();
                }
            }
        }

        var x = PluginAPI.Test.Main.GetAndLoadPlugin("G:\\GODOT\\PluginMaker\\Game\\EXPORT\\Mods\\Plugin2.dll");
        Console.WriteLine(x.Item1);
        Console.WriteLine(x.Item2 + " " + x.Item2.AssemblyLoadedPath);
        Console.WriteLine(PluginAPI.Test.Main._projectLoadContext + " " + PluginAPI.Test.Main._projectLoadContext.AssemblyLoadedPath);
        if (x.Item1 != null)
        {
            foreach (var item in x.Item1.GetTypes())
            {
                if (item.IsValidEntrypoint())
                {
                    Console.WriteLine(item.Name);
                    var entry = item.GetValidEntrypoint();
                    var obj = Activator.CreateInstance(item);
                    entry.Invoke(obj, null);
                }
                if (item.IsAssignableTo(typeof(ITestInterface)))
                {
                    Console.WriteLine(item.Name);
                    var obj = (ITestInterface)Activator.CreateInstance(item);
                    obj.InitTest();
                }
            }
        }
        PluginAPI.Test.Main.UnloadPlugin(ref x.Item2);
        */

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

    }

    public override void _ExitTree()
    {
        MainLoader.DeInit();
    }

    public void TestPressed()
	{
        var test = new MyTestButton(Guid.NewGuid());
        V1Manager.ExecuteEvent(test);
        GD.Print("Bool:" + test.Test);
        GD.Print("TEST");

        func2 func2 = new()
        { 
            x = 66,
            y = 99
        };
        V2EventManager.TriggerEvent(func2);
        GD.Print(func2.y);
        GD.Print(func2.x);


    }
    public void Test2Pressed()
    {
        var func = new Functioner();
        V1Manager.ExecuteEvent(func);
        func.FuncVoidParam(111);
        GD.Print("Number (we set as 3 inside)"+func.Number);
        GD.Print("TEST 2!");

        test2 test2 = new()
        {
            yeet = "yeeeeeet",
            func = new()
            { 
                x = 66,
                y = 99,
            }
        };
        V2EventManager.TriggerEvent(test2);
        GD.Print(test2.yeet);
        GD.Print(test2.func.y);
        GD.Print(test2.func.x);
        V2EventManager.TriggerEvent(test2.func);
        GD.Print(test2.func.y);
        GD.Print(test2.func.x);
    }
}
