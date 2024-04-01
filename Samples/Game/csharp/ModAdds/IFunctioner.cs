using ModAPI.V1.Interfaces;

namespace Game.csharp.ModAdds
{
    public interface IFunctioner : ICustomMod
    {
        public int FuncInt();
        public void FuncVoid();
        public void FuncVoidParam(int param);
    }
}
