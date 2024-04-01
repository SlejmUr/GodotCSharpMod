using ModAPI.V1.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plugin1
{
    internal class MainMod : IMod
    {
        public void MethodRegistered(MethodInfo methodInfo, int InterfaceNumber)
        {
            Console.WriteLine("MainMod Checked methodInfo: " + methodInfo.Name + " Int umb:" + InterfaceNumber);
        }

        public void Start()
        {
            Console.WriteLine("START!!!!");
        }

        public void Stop()
        {
            Console.WriteLine("STOP!!!!");
        }
    }
}
