﻿using HermitLib;
using System;

namespace HermitClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleAssistance.WriteLine("Welcome to use Hermit!", ConsoleColor.Yellow);
            ConsoleAssistance.WriteLine("Generic instant-message software.");

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
        }
    }
}