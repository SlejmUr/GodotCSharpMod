using System;

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
            Console.WriteLine("Func Void");
            Number++;
        }

        public void FuncVoidParam(int param)
        {
            Console.WriteLine("FuncVoidParam! Param: " + param);
            Number += param;
        }

        public int GetNumber()
        {
            return Number;
        }
    }
}
