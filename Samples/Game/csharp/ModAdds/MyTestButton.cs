using System;

namespace Game.csharp.ModAdds
{
    public class MyTestButton : IMyTestButton
    {
        public Guid MyGuid { get; set; }

        public bool Test = false;

        public int InterfaceNumber => 666;
        public MyTestButton(Guid guid)
        {
            MyGuid = guid;
        }
    
        public MyTestButton()
        {

        }
    }
}
