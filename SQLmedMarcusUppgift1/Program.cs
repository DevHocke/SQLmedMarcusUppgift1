using System;

namespace SQLmedMarcusUppgift1
{
    internal class Program
    {
        private static void Main()
        {
            ObamaTree obama = new ObamaTree();
            obama.SetUp();
            obama.StartMenu();
            Console.ReadLine();
        }
    }
}