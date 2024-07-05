using Game.csharp.ModAdds;
using ModAPI.V1;
using System;

namespace Game.csharp.tryplugininside
{
    internal class MyFunc
    {
        [V1Event(7777777)]
        public void Functin(Functioner functioner)
        {
            Console.WriteLine(functioner.GetNumber());
            functioner.FuncVoidParam(33);
            Console.WriteLine(functioner.GetNumber());
        }

        [V1Event(7777777)]
        public void Functin(IFunctioner functioner)
        {
            Console.WriteLine(functioner.GetType().FullName);
            if (functioner is Functioner func)
            {
                Console.WriteLine(func.GetNumber());
            }
            functioner.FuncVoidParam(66);
            Console.WriteLine(functioner.FuncInt());
        }
    }
}
