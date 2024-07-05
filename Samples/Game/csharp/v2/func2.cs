using ModAPI.V2;

namespace Game.csharp.v2
{
    public class func2 : BaseEvent
    {
        public int x;

        public int y;

        public void test()
        {
            x = x + y;
        }

        public override string ToString()
        {
            return $"func2: {x} {y}";
        }
    }
}
