using Game.csharp.ModAdds;
using ModAPI.V1;

namespace Plugin1
{
    internal class MyFunc
    {
        [V1Event(7777777)]
        public void Functin(Functioner functioner)
        {
            functioner.FuncVoidParam(33);
            Console.WriteLine(functioner.GetNumber());
            Console.WriteLine(functioner.Number);
            functioner.Number = 666;
            Console.WriteLine(functioner.Number);
            Console.WriteLine("Should have manipulated the number");
        }

        [V1Event(7777777)]
        public void Functin(IFunctioner functioner)
        {
            functioner.FuncVoidParam(33);
            Console.WriteLine(functioner.FuncInt());
        }
    }
}
