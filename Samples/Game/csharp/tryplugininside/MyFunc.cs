using Game.csharp.ModAdds;
using ModAPI.V1.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.csharp.tryplugininside
{
    internal class MyFunc
    {
        [V1Event(7777777)]
        public void Functin(Functioner functioner)
        {
            functioner.FuncVoidParam(33);
            Console.WriteLine(functioner.OnlyFunctionHere());
        }

        [V1Event(7777777)]
        public void Functin(IFunctioner functioner)
        {
            functioner.FuncVoidParam(33);
            Console.WriteLine(functioner.FuncInt());
        }
    }
}
