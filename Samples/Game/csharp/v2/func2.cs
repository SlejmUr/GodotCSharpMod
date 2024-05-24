using ModAPI.V2;
using System;

namespace Game.csharp.v2
{
    public class func2 : BaseEvent
    {
        public int x;

        public int y;

        public void test()
        {
            Console.WriteLine("func2.test");
            Console.WriteLine(x); 
            Console.WriteLine(y);
            x = x + y;
        }
    }
}
