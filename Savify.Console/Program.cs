using System;
using System.IO;
using Savify.Core;

namespace Savify.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.Title = "Savify";
            Core.Settings.Default.OutputPath = Environment.CurrentDirectory + @"\";           

            bool inMenu = true;

            while (inMenu)
            {
                System.Console.Clear();
                System.Console.WriteLine("- Savify -\n" +
                "\n" +
                "Menu:\n" +
                "1 - Enter Search Query\n" +
                "2 - Settings\n" +
                "\n" +
                "0 - Exit\n");
                string input = System.Console.ReadLine();

                switch (input)
                {
                    case "1":
                        System.Console.Clear();
                        Search();
                        break;
                    case "2":
                        System.Console.Clear();
                        Settings();
                        break;
                    case "0":
                        inMenu = false;
                        break;
                    default:
                        System.Console.Clear();
                        System.Console.WriteLine("Please enter a valid option! [RETURN]");
                        System.Console.ReadLine();
                        break;
                }
            }
        }

        private static void Settings()
        {
            bool inSettings = true;

            while (inSettings)
            {
                System.Console.Clear();
                System.Console.WriteLine("- Savify -\n" +
                    "\n" +
                    "Settings:\n" +
                    "1 - Download Path\n" +
                    "2 - Download Format\n" +
                    "3 - Download Quality\n" +
                    "4 - Toggle Restrict Filenames\n" +
                    "5 - Search Provider\n" +
                    "\n" +
                    "0 - Back\n");
                string input = System.Console.ReadLine();

                switch (input)
                {
                    case "1":
                        System.Console.Clear();
                        SetPath();
                        break;
                    case "2":
                        System.Console.Clear();
                        SetFormat();
                        break;
                    case "3":
                        System.Console.Clear();
                        SetQuality();
                        break;
                    case "4":
                        System.Console.Clear();
                        ToggleRestrictFilenames();
                        break;
                    case "5":
                        System.Console.Clear();
                        SetSearch();
                        break;
                    case "0":
                        inSettings = false;
                        break;
                    default:
                        System.Console.Clear();
                        System.Console.WriteLine("Please enter a valid option! [RETURN]");
                        System.Console.ReadLine();
                        break;
                }
            }
        }

        private static void SetSearch()
        {
            bool inSearch = true;

            while (inSearch)
            {
                System.Console.Clear();
                System.Console.WriteLine("- Savify -\n" +
                    "\n" +
                    "Search Provider:\n" +
                    "1 - Search Provider A\n" +
                    "2 - Search Provider B\n" +
                    "\n" +
                    "Current: " + (Core.Settings.Default.Search == "ytsearch" ? "Search Provider A" : "Search Provider B") +
                    "\n\n" +
                    "0 - Back\n");
                string input = System.Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Core.Settings.Default.Search = "ytsearch";
                        inSearch = false;
                        break;
                    case "2":
                        Core.Settings.Default.Search = "scsearch";
                        inSearch = false;
                        break;
                    case "0":
                        inSearch = false;
                        break;
                    default:
                        System.Console.Clear();
                        System.Console.WriteLine("Please enter a valid option! [RETURN]");
                        System.Console.ReadLine();
                        break;
                }
            }

        }

        private static void ToggleRestrictFilenames()
        {
            System.Console.Clear();
            Core.Settings.Default.RestrictFilenames = !Core.Settings.Default.RestrictFilenames;
            System.Console.WriteLine("Restricting filenames is now turned " + (Core.Settings.Default.RestrictFilenames ? "on" : "off") + ". [RETURN]");
            System.Console.ReadLine();
        }

        private static void SetQuality()
        {
            bool inQuality = true;

            while (inQuality)
            {
                System.Console.Clear();
                System.Console.WriteLine("- Savify -\n" +
                    "\n" +
                    "Quality:\n" +
                    "Enter one of the following: 32, 96, 128, 192, 256, 320\n" +
                    "\n" +
                    "Current: " + Core.Settings.Default.Quality + "bps\n" +
                    "\n" +
                    "0 - Back\n");
                string input = System.Console.ReadLine();

                string[] qualities = new string[] { "32", "96", "128", "192", "256", "320" };

                if (Array.IndexOf(qualities, input) > -1)
                {
                    Core.Settings.Default.Quality = input + "K";
                    inQuality = false;
                }
                else if (input == "0")
                {
                    inQuality = false;
                }
                else
                {
                    System.Console.Clear();
                    System.Console.WriteLine("Please enter a valid option! [RETURN]");
                    System.Console.ReadLine();
                }
            }           
        }

        private static void SetFormat()
        {
            bool inFormat = true;

            while (inFormat)
            {
                System.Console.Clear();
                System.Console.WriteLine("- Savify -\n" +
                    "\n" +
                    "Format:\n" +
                    "Enter one of the following: mp3, aac, flac, m4a, opus, vorbis, wav\n" +
                    "\n" +
                    "Current: " + Enumerator.GetValueFromDescription<Format>(Core.Settings.Default.Format) +
                    "\n\n" +
                    "0 - Back\n");
                string input = System.Console.ReadLine().ToLower();

                string[] qualities = new string[] { "mp3", "aac", "flac", "m4a", "opus", "vorbis", "wav" };

                if (Array.IndexOf(qualities, input) > -1)
                {
                    Core.Settings.Default.Format = "." + input;
                    inFormat = false;
                }
                else if (input == "0")
                {
                    inFormat = false;
                }
                else
                {
                    System.Console.Clear();
                    System.Console.WriteLine("Please enter a valid option! [RETURN]");
                    System.Console.ReadLine();
                }
            }
        }

        private static void SetPath()
        {
            bool inPath = true;

            while (inPath)
            {
                System.Console.Clear();
                System.Console.WriteLine("- Savify -\n" +
                    "\n" +
                    "Download Path:\n" +
                    "Enter a full valid path\n" +
                    "\n" +
                    "Current: " + Core.Settings.Default.OutputPath +
                    "\n\n" +
                    "0 - Back\n");
                string input = System.Console.ReadLine();

                if (Directory.Exists(input))
                {
                    Core.Settings.Default.OutputPath = (input[input.Length-1] == '\\' ? input : input + @"\");
                    inPath = false;
                }
                else if (input == "0")
                {
                    inPath = false;
                }
                else
                {
                    System.Console.Clear();
                    System.Console.WriteLine("Please enter a valid path! {0} does not exist. [RETURN]", input);
                    System.Console.ReadLine();
                }
            }
        }

        static void Search()
        {
            System.Console.WriteLine("- Savify -\n" +
                "\n" +
                "Download Song(s):\n" +
                "Enter link or search phrase: \n" +
                "\n" +
                "Enter 0 to go back\n");
            string search = System.Console.ReadLine();

            if (search != "0")
            {
                Song song = new Song(search);
                song.Download();
                System.Console.ReadLine();
            }           
        }
    }
}
