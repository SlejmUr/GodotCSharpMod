using Game.csharp.ModAdds;
using ModAPI.V1;

namespace Plugin1
{
    internal class MyTestButtonFire
    {
        [V1Event(666)]
        public void Test(MyTestButton myTestButton)
        {
            Console.WriteLine("Test" + myTestButton.MyGuid);
            Console.WriteLine(myTestButton.Test + " before manip");
            myTestButton.Test = true;
            Console.WriteLine(myTestButton.Test + " after manip");
        }

        [V1Event(666)]
        public void TestwithInterface(IMyTestButton myTestButton)
        {
            Console.WriteLine("TestwithInterface " + myTestButton.MyGuid);
        }

        [V1Event(666)]
        public void TestwithBasicInterface(ICustomMod customModInterface)
        {
            if (customModInterface is IMyTestButton button && button != null)
            {
                Console.WriteLine("TestwithBasicInterface "+ button.MyGuid);

            }
        }
    }
}
