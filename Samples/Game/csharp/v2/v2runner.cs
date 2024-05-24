using ModAPI.V2;
using System;

namespace Game.csharp.v2
{
    internal class v2runner
    {

        public static void func2_run(func2 func2)
        {
            Console.WriteLine("func2_run");
            func2.test();
        }

        public static void test_run(test2 test)
        {
            Console.WriteLine("test_run (No priority (0))");
            test.test();
        }

        [V2Priority(100)]
        public static void test_run_run(test2 test)
        {
            Console.WriteLine("test_run_run (priority 100)");
            test.func.y += 42;
        }

        [V2Priority(10)]
        public static void test_run_run10(test2 test)
        {
            Console.WriteLine("test_run_run (priority 10)");
            test.func.y += 42;
        }

        [V2Priority(1)]
        public static void test_run_run1(test2 test)
        {
            Console.WriteLine("test_run_run (priority 1)");
            test.func.y += 42;
        }

        [V2Priority(-10)]
        public static void last_runner10(test2 test)
        {
            Console.WriteLine("last runner! (priority -10)");
        }

        [V2Priority(-100)]
        public static void last_runner100(test2 test)
        {
            Console.WriteLine("last runner!");
        }
    }
}
