using HermitLib;
using System;

namespace HermitClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleAssistance.WriteLine("Welcome to use Hermit!", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("Light, free and secure instant-messaging(IM) software.");

            //initialize
            //todo:init

            //circle
            while (true) {
                var result = Console.ReadKey(true);
                string command = "";
                if (result.Key == ConsoleKey.Tab) {
                    ConsoleAssistance.WriteLine("Hermit-client>", ConsoleColor.Yellow);
                    command = Console.ReadLine();
                    //todo:process command
                }
            }

            //todo:exit


            ConsoleAssistance.WriteLine("You will go back to reality which is filled with dangers. Mind yourself! Good luck for you!", ConsoleColor.Yellow);
        }
    }
}
