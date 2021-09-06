using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace WOLtool
{
    public class WakeOnLan : IDisposable
    {
        private bool _disposed = false;
        private readonly Socket _sock;
        private static readonly IPEndPoint[] _endpoints = new IPEndPoint[]
        {
            new IPEndPoint(IPAddress.Broadcast, 7), // Common WOL Port
            new IPEndPoint(IPAddress.Broadcast, 9) // Common WOL Port
        };
        public WakeOnLan()
        {
            _sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true // Enable broadcast, required for macOS compatibility
            };
        }

        public void Send(string macAddress)
        {
            try
            {
                byte[] magicPacket = BuildMagicPacket(macAddress); // Get magic packet byte array based on MAC Address
                foreach (var ep in _endpoints) // Broadcast to *all* WOL Endpoints
                {
                    _sock.SendTo(magicPacket, ep); // Broadcast magic packet
                }
            }
            catch (Exception ex)
            {
                throw new WakeOnLanException("ERROR broadcasting WakeOnLan Magic Packet.", ex);
            }
        }

        private static byte[] BuildMagicPacket(string macAddress)
        {
            macAddress = Regex.Replace(macAddress, "[. : -]", "");
            if (macAddress.Length != 12) throw new ArgumentException("Invalid MAC Address Length! Must be 12 hexadecimal characters.");
            byte[] macBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                macBytes[i] = Convert.ToByte(macAddress.Substring(i * 2, 2), 16);
            }
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                for (int i = 0; i < 6; i++)
                {
                    bw.Write((byte)0xff);
                }
                for (int i = 0; i < 16; i++)
                {
                    bw.Write(macBytes);
                }
                return ms.ToArray();
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

    public class WakeOnLanException : Exception
    {
        public WakeOnLanException()
        {
        }

        public WakeOnLanException(string message)
            : base(message)
        {
        }

        public WakeOnLanException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
