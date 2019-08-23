using System;
using Savify.Core;

namespace Savify.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                System.Console.Clear();
                System.Console.WriteLine("- Savify -\n" +
                "\n" +
                "Menu:\n" +
                "1 - Enter Search Query\n" +
                "2 - Settings\n" +
                "\n" +
                "0 - Exit\n");
                int input = Convert.ToInt32(System.Console.ReadLine());

                switch (input)
                {
                    case 1:
                        System.Console.Clear();
                        System.Console.WriteLine("Enter link or search phrase: ");
                        string search = System.Console.ReadLine();

                        Song song = new Song(search);
                        song.Download();
                        break;
                    case 2:
                        break;
                    case 0:
                        running = false;
                        break;
                    default:
                        System.Console.Clear();
                        System.Console.WriteLine("Please enter a valid option! [RETURN]");
                        System.Console.ReadLine();
                        break;
                }
            }
        }
    }
}
