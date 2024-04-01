using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.csharp.ModAdds
{
    public class Functioner : IFunctioner
    {
        public int InterfaceNumber => 7777777;
        public int Number = 3;
        public int FuncInt()
        {
            return InterfaceNumber;
        }

        public void FuncVoid()
        {
            Console.WriteLine("Func Viod write");
        }

        public void FuncVoidParam(int param)
        {
            Console.WriteLine("FuncVoidParam! Param: " + param);
        }

        public int OnlyFunctionHere()
        {
            return InterfaceNumber;
        }
    }
}
