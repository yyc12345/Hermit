using System;
using HermitLib;

namespace HermitServer {
    class Program {
        static void Main(string[] args) {

            ConsoleAssistance.WriteLine("Welcome to use Hermit!", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("Light, free and secure instant-messaging(IM) software.");

            //initialize
            //todo:init
            ConsoleAssistance.WriteLine("[Main] Initialize server config...");
            General.serverConfig = new ServerConfig(Information.WorkPath.Enter("config.json").Path());
            ConsoleAssistance.WriteLine("[Main] Initialize server database...");
            General.serverDatabase = new ServerDatabase();
            ConsoleAssistance.WriteLine("[Main] Initialize server socket...");
            General.serverSocket = new ServerSocket();
            ConsoleAssistance.WriteLine("[Main] Start listening...");
            General.serverSocket.StartListen();

            //circle
            while (true) {
                var result = Console.ReadKey(true);
                string command = "";
                if (result.Key == ConsoleKey.Tab) {
                    ConsoleAssistance.WriteLine("Hermit-server>", ConsoleColor.Yellow);
                    command = Console.ReadLine();
                    //todo:process command
                }
            }

            //close server
            ConsoleAssistance.WriteLine("[Main] Start closing server...");
            //todo:finish closing
            ConsoleAssistance.WriteLine("[Main] Close all clients...");
            General.serverSocket.Close();
            ConsoleAssistance.WriteLine("[Main] Stop listening...");
            General.serverSocket.StopListen();
            ConsoleAssistance.WriteLine("[Main] Store server database...");
            General.serverDatabase.Close();
            ConsoleAssistance.WriteLine("[Main] Save server config...");
            General.serverConfig.Save();


            ConsoleAssistance.WriteLine("Thanks for using Hermit.", ConsoleColor.Yellow);
        }
    }
}
