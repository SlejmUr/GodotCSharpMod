using Game.csharp.ModAdds;
using ModAPI.V1.Attributes;
using ModAPI.V1.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.csharp.tryplugininside
{
    internal class MyTestButtonFire
    {
        //Acceptable
        [V1Event(666)]
        public void Test(MyTestButton myTestButton)
        {
            Console.WriteLine(myTestButton.MyGuid);
        }

        //Acceptable
        [V1Event(666)]
        public void TestwithInterface(IMyTestButton myTestButton)
        {
            Console.WriteLine(myTestButton.MyGuid);
        }

        //NOT OK
        [V1Event(666)]
        public void TestwithBasicInterface(ICustomMod customModInterface)
        {
            if (customModInterface is IMyTestButton button && button != null)
            {
                Console.WriteLine(button.MyGuid);

            }

        }
    }
}
