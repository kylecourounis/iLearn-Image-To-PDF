namespace ImageToPDF
{
    using System;
    using System.Reflection;
    using System.Threading;

    using ImageToPDF.Core;
    using ImageToPDF.Core.Consoles;
    using ImageToPDF.Helpers;

    internal class Program
    {
        private const int Width  = 120;
        private const int Height = 32;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        private static void Main()
        {
            Natives.SetupConsole();

            Console.Title = $"{Assembly.GetExecutingAssembly().GetName().Name} Utility - v{Assembly.GetExecutingAssembly().GetName().Version}";

            Console.SetOut(new Prefixed());

            Console.SetWindowSize(Program.Width, Program.Height);
            Console.SetBufferSize(Program.Width, Program.Height);

            Console.ForegroundColor = ConsoleColor.Blue;

            Console.Write(@"
 _ _                             _____                             _____      __________________ 
(_) |                           |_   _|                           |_   _|     | ___ \  _  \  ___|
 _| |     ___  __ _ _ __ _ __     | | _ __ ___   __ _  __ _  ___    | | ___   | |_/ / | | | |_   
| | |    / _ \/ _` | '__| '_ \    | || '_ ` _ \ / _` |/ _` |/ _ \   | |/ _ \  |  __/| | | |  _|  
| | |___|  __/ (_| | |  | | | |  _| || | | | | | (_| | (_| |  __/   | | (_) | | |   | |/ /| |    
|_\_____/\___|\__,_|_|  |_| |_|  \___/_| |_| |_|\__,_|\__, |\___|   \_/\___/  \_|   |___/ \_|    
                                                       __/ |                                     
                                                      |___/                                      
           " + Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Starting...");

            Loader.Initialize();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
