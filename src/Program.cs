using System;

namespace WOLtool
{
    class Program
    {
        private static readonly WOL _wol = new WOL();
        static int Main(string[] args)
        {
            if (args.Length == 0) // No args provided, get console input
            {
                while (true) // Continue prompting user until no more input is received
                {
                    Console.Write("Enter MAC Address: ");
                    string mac = Console.ReadLine().Trim();
                    if (mac == String.Empty) break; // User is done, exit
                    _wol.Send(mac);
                }
                _wol.Close();
                return 0;
            }
            else // Args provided
            {
                int numFailed = 0;
                foreach (string arg in args) // iterate all arguments
                {
                    if (_wol.Send(arg) == -1) numFailed--;
                }
                _wol.Close();
                return numFailed;
            }
        }
    }
}