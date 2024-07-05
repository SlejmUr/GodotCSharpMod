using Game.csharp.ModAdds;
using Godot;
using ModAPI.V1;
using System;

namespace Game.csharp.tryplugininside
{
    internal class MyTestButtonFire
    {
        //Acceptable
        [V1Event(666)]
        public void Test(MyTestButton myTestButton)
        {
            GD.Print(myTestButton.MyGuid);
        }

        //Acceptable
        [V1Event(666)]
        public void TestwithInterface(IMyTestButton myTestButton)
        {
            GD.Print(myTestButton.MyGuid);
        }

        //NOT OK
        [V1Event(666)]
        public void TestwithBasicInterface(ICustomMod customModInterface)
        {
            if (customModInterface is IMyTestButton button && button != null)
            {
                GD.Print(button.MyGuid);

            }

        }
    }
}
