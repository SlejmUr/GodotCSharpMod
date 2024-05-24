using ModAPI.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
