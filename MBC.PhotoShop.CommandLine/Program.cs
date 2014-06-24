/**MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.13, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System;
using MBC.Adobe.PhotoShop.Connection;

namespace MBC.PhotoShop.CommandLine
{
    class Program
    {
        static void Main(string[] args)
        {
            if (2 != args.Length)
            {
                Console.Error.WriteLine(
                    "Syntax is " + 
                        "\"PhotoshopCommandLine ServerName Password\" " +
                        "please try again.");
                return;
            }

            IOHandler cmdHandler = null;

            try
            {
                cmdHandler =
                    IOHandler.CreateNew(
                        args[1], 
                        args[0], 
                        Console.Error);
                cmdHandler.ShowByteDisplayInAsRunLog = true;
            }
            catch (Exception)
            {
                Console.Error.WriteLine(
                    "cannot access to PhotoShop. \n" +
                        "please try again.");
                return;
            }

            while (true)
            {
                Console.Error.WriteLine(
                    "\nReady for input, " +
                        "type in JavaScript or type ? for commands: ");

                var userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "?":
                        ShowCommands();
                        break;
                    case "q":
                        cmdHandler.Dispose();
                        return;
                    case "s":
                        if (null == cmdHandler.AsRunLogger)
                            cmdHandler.AsRunLogger = Console.Error;
                        else
                            cmdHandler.AsRunLogger = null;
                        break;
                    case "m":
                        cmdHandler.ShowByteDisplayInAsRunLog =
                            !cmdHandler.ShowByteDisplayInAsRunLog;
                        break;
                    default:
                        cmdHandler.ProcessJavaScript(userInput);

                        break;
                }
            }
        }

        static void ShowCommands() 
        {
            Console.Error.WriteLine("q for quit");
            Console.Error.WriteLine("s for messages toggle");
            Console.Error.WriteLine("m for byte display toggle");
            Console.Error.WriteLine("? for this list");
        }
    }
}
