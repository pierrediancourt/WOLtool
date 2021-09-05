using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace WOLtool
{
    public class WOL : IDisposable
    {
        private bool _disposed = false;
        private readonly Socket _sock;
        private static readonly IPEndPoint[] _endpoints = new IPEndPoint[]
        {
            new IPEndPoint(IPAddress.Broadcast, 7), // Common WOL Port
            new IPEndPoint(IPAddress.Broadcast, 9) // Common WOL Port
        };
        public WOL()
        {
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true // Enable broadcast, required for macOS compatibility
            };
        }

        public int Send(string macParam)
        {
            try
            {
                byte[] magicPacket = BuildMagicPacket(macParam); // Get magic packet byte array based on MAC Address
                foreach (var ep in _endpoints) // Send to all WOL Endpoints
                {
                    _sock.SendTo(magicPacket, ep); // Transmit magic packet
                }
                Console.WriteLine($"{macParam} [OK]");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{macParam} [FAIL] {ex}");
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
            catch (Exception ex)
            {
                throw new Exception($"Error building magic packet. Please verify MAC Address is entered correctly: {ex}");
            }
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                _sock?.Dispose();
            }

            _disposed = true;
        }
    }
}
