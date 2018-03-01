using System;
using HermitLib;

namespace HermitServer {
    class Program {
        static void Main(string[] args) {

            ConsoleAssistance.WriteLine("Welcome to use Hermit!", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("Generic instant-message software.");

            //initialize
            //todo:init


            //circle
            while (true) {
                var result = Console.ReadKey(true);
                string command = "";
                if (result.Key == ConsoleKey.Tab) {
                    ConsoleAssistance.WriteLine("Hermit>", ConsoleColor.Yellow);
                    command = Console.ReadLine();
                    //todo:process command
                }
            }

        }
    }
}
