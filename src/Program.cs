using System;

namespace WOLtool
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) // No args provided, get console input
            {
                Console.Write("Enter MAC Address: ");
                return WOL.Send(Console.ReadLine().Trim());
            }
            else // Args provided
            {
                return WOL.Send(args[0]); // First arg should be MAC Address
            }
        }
    }
}