using Godot;
using System;
using ModAPI;
using ModAPI.V1;
using Game.csharp.ModAdds;
using ModAPI.V2;
using Game.csharp.v2;
using System.Reflection;
using Game.csharp;
namespace Game;

public partial class Menu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GameLog.CreateNew();
        Debugger.ParseLogger(GameLog.logger);
        // this is a mod loading settings!
        LoadSettings loadSettings = new()
        { 
            // If we set this true we dont need to use "SetMainModAssembly!"
            LoadAsMainCaller = true,
            //  This only exist to able to load our Main assembly if you dont want to create AssemblyInfo file, use it with ModAP!
            LoadWithoutAssemblyDefinedModAPI = false,
            // Used with LoadWithoutAssemblyDefinedModAPI, to able to load specified ModAPI's.
            ModAPI = ModAPIEnum.All,
            // Used in V0, it skips all actions to call V0 Mods. (Faster if noone uses V0)
            V0_SkipAllAction = false,
            // Enables ICustomMod Interface as an argument. Check TestwithBasicInterface for check how it would work.
            V1_EnableICustomModInterfaceParsing = true,
            // TODO
            V1_EnableMainAsMod = true,
            // Enables to MainAssembly to be the sender and receiver.
            // You can call your own functions and stuff inside this assembly.
            // We use here since we have "tryplugininside".
            V2_EnableMainAsMod = true
        };
        // we loading the mainloader here
        MainLoader.Init(loadSettings);

        //  We load all required dependencies that not in our directory.
        MainLoader.LoadDependencies();

        // we load already existing mod from the Samples directory. (Already compiled and put into Mods directory)
        bool IsSuccess = MainLoader.LoadModInMods("Plugin1.dll");

        if (!IsSuccess)
        {
            GD.Print("Loading our mod not succeded! Mod couldnt load due to errors. You might have a DLL that not exists in Dependencies dir (Or mod doesnt exists)");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

    }

    public override void _ExitTree()
    {
        // Not quite neccessary but we want to clean our things.
        MainLoader.DeInit();
        Debugger.ClearLogger();
        GameLog.Close();
    }

    public void TestPressed()
	{
        // v1 test!
        var test = new MyTestButton(Guid.NewGuid());
        GD.Print("Bool:" + test.Test);
        V1Manager.ExecuteEvent(test);
        GD.Print("Bool:" + test.Test);
        var func = new Functioner();
        GD.Print("Number (we set as 3 inside)" + func.Number);
        func.FuncVoid();
        GD.Print("GetNumber" + func.GetNumber());
        func.FuncVoidParam(111);
        GD.Print("GetNumber" + func.GetNumber());
        V1Manager.ExecuteEvent(func);
        GD.Print("Number (we set as 3 inside)" + func.Number);
        func.FuncVoid();
        GD.Print("GetNumber" + func.GetNumber());
        func.FuncVoidParam(111);
        GD.Print("GetNumber" + func.GetNumber());
    }

    public void Test2Pressed()
    {
        test2 test2 = new()
        {
            yeet = "yeeeeeet",
            func = new()
            { 
                x = 66,
                y = 99,
            }
        };
        GD.Print(test2.ToString());
        V2Manager.TriggerEvent(test2);
        GD.Print(test2.ToString());
        V2Manager.TriggerEvent(test2.func);
        GD.Print(test2.ToString());
    }
}
