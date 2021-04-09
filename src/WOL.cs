using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace WOLtool
{
    internal static class WOL
    {
        public static int Send(string macParam)
        {
            try
            {
                using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) // Create socket
                {
                    sock.EnableBroadcast = true; // Enable broadcast, required for macOS compatibility
                    byte[] magicPacket = BuildMagicPacket(macParam); // Get magic packet byte array based on MAC Address
                    sock.SendTo(magicPacket, new IPEndPoint(IPAddress.Broadcast, 7)); // Transmit Magic Packet on Port 7
                    sock.SendTo(magicPacket, new IPEndPoint(IPAddress.Broadcast, 9)); // Transmit Magic Packet on Port 9
                    sock.Close(); // Close socket
                }
                Console.WriteLine($"({macParam}) Success!");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"({macParam}) Failed!\n{ex}");
                return -1;
            }
        }
        private static byte[] BuildMagicPacket(string macParam) // MacAddress in any standard HEX format
        {
            try
            {
                macParam = Regex.Replace(macParam, "[. : -]", ""); // Remove chars . - : from string (common in mac address format)
                byte[] macBytes = new byte[6];
                for (int i = 0; i < 6; i++)
                {
                    macBytes[i] = Convert.ToByte(macParam.Substring(i * 2, 2), 16);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        for (int i = 0; i < 6; i++)  //First 6 times 0xff
                        {
                            bw.Write((byte)0xff);
                        }
                        for (int i = 0; i < 16; i++) // then 16 times MacAddress
                        {
                            bw.Write(macBytes);
                        }
                    }
                    return ms.ToArray(); // return 102 bytes magic packet
                }
            }
            catch
            {
                Console.WriteLine($"({macParam}) Error building magic packet. Please verify MAC Address is entered correctly.");
                throw;
            }
        }
    }
}
