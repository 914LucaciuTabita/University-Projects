using Lab4.Implementations;
using System;
using System.Collections.Generic;

namespace Lab4
{
    internal class Program
    {
        private static readonly List<string> Urls = new List<string>()
        {
            "www.google.com",
            "www.emag.ro/",
            "www.serbanology.com/",
            "www.dspcluj.ro/HTML/CORONAVIRUS/incidenta.html"
        };

        static void Main(string[] args)
        {
            Console.WriteLine("1. Callback");
            Console.WriteLine("2. Task");
            Console.WriteLine("3. Async / Await");

            bool ok = true;
            while (ok)
            {
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        new CallbackSolution(Urls);
                        break;
                    case "2":
                        new TaskSolution(Urls);
                        break;
                    case "3":
                        new AsyncAwaitSolution(Urls);
                        break;
                    case "exit":
                        ok = false;
                        break;
                    default:
                        Console.WriteLine("Invalid input!");
                        break;
                }
            }
        }
    }
}
