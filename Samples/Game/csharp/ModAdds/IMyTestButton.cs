using ModAPI.V1.Interfaces;
using System;

namespace Game.csharp.ModAdds
{
    public interface IMyTestButton : ICustomMod
    {
        public Guid MyGuid { get; set; }
    }
}
