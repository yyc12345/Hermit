using System;
using HermitLib;

namespace HermitServer {
    class Program {
        static void Main(string[] args) {

            ConsoleAssistance.WriteLine("Welcome to use Hermit!", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("Generic instant-message software.");

            //initialize
            //todo:init
            ConsoleAssistance.WriteLine("[Main] Initializing server config...");
            General.serverConfig = new ServerConfig(Information.WorkPath.Enter("config.json").Path());
            ConsoleAssistance.WriteLine("[Main] Initializing server database...");
            General.serverDatabase = new ServerDatabase();
            ConsoleAssistance.WriteLine("[Main] Initializing server socket...");
            General.serverSocket = new ServerSocket();
            ConsoleAssistance.WriteLine("[Main] Start listening...");
            General.serverSocket.StartListen();

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
