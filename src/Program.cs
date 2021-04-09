using System;

namespace WOLtool
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) // No args provided, get console input
            {
                while (true) // Continue prompting user until no more input is received
                {
                    Console.Write("Enter MAC Address: ");
                    string mac = Console.ReadLine().Trim();
                    if (mac == String.Empty) break; // User is done, exit
                    WOL.Send(mac);
                }
                return 0;
            }
            else // Args provided
            {
                int numFailed = 0;
                foreach (string arg in args) // iterate all arguments
                {
                    if (WOL.Send(arg) == -1) numFailed--;
                }
                return numFailed;
            }
        }
    }
}