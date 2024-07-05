using ModAPI.V1;
using System;

namespace Game.csharp.ModAdds
{
    public interface IMyTestButton : ICustomMod
    {
        public Guid MyGuid { get; set; }
    }
}
