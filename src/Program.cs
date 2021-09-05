using System;

namespace WOLtool
{
    class Program
    {
        static int Main(string[] args)
        {
            using var wol = new WOL();
            if (args.Length == 0) // No args provided, get console input
            {
                while (true) // Continue prompting user until no more input is received
                {
                    Console.Write("Enter MAC Address: ");
                    string mac = Console.ReadLine().Trim();
                    if (mac == String.Empty) break; // User is done, exit
                    wol.Send(mac);
                }
                return 0;
            }
            else // Args provided
            {
                int numFailed = 0;
                foreach (string arg in args) // iterate all arguments
                {
                    if (wol.Send(arg) == -1) numFailed--;
                }
                return numFailed;
            }
        }
    }
}