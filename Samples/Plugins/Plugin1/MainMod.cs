using ModAPI.V0;
using System.Reflection;
using Newtonsoft.Json;

namespace Plugin1
{
    class jsonobj
    {
        public string xx;
    }

    internal class MainMod : V0Mod
    {

        public override void V1Register(MethodInfo methodInfo, int InterfaceNumber)
        {
            Console.WriteLine("MainMod Checked methodInfo: " + methodInfo.Name + " Int umb:" + InterfaceNumber);
        }

        public override void Load()
        {
            Console.WriteLine("START!!!!");
            // this only here because we want to include Json as used reference!
            jsonobj jsonobj = new()
            { 
                xx = "yeet"
            };
            JsonConvert.SerializeObject(jsonobj);

        }

        public override void Unload()
        {
            Console.WriteLine("STOP!!!!");
        }
    }
}
