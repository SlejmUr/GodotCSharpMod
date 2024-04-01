namespace ModAPI
{
    public class Debugger
    {
        public static bool Enabled = false;

        public static void Print(string str)
        {
            if (Enabled)
                Console.WriteLine(str);
        }
    }
}
