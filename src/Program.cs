﻿using System;

namespace WOLtool
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0) // No args provided, get user input
            {
                Console.Write("Enter MAC Address: ");
                string mac = Console.ReadLine().Trim();
                return WOL.Send(mac);
            }
            else if (args.Length == 1) // arg1 = MAC Address
            {
                return WOL.Send(args[0]);
            }
            else // Handle unexpected startup
            {
                Console.WriteLine("Unexpected arguments.");
                return -1;
            }
        }
    }
}