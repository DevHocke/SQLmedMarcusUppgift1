using System;

namespace SQLmedMarcusUppgift1
{
    class Program
    {
        static void Main()
        {
            ObamaTree obama = new ObamaTree();
            obama.SetUp();
            obama.MainMenu();
            Console.ReadLine();
        }
    }
}
