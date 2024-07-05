using ModAPI.V2;
using System;

namespace Game.csharp.v2
{
    public class test2 : BaseEvent
    {
        public string yeet;

        public func2 func;

        public void test()
        {
            Console.WriteLine("test2.test");
            func.test();
            func.y++;
            Console.WriteLine(this.ToString());
        }

        public override string ToString()
        {
            return $"test 2: {yeet} | {func.ToString()}";
        }
    }
}
