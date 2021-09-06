using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace WOLtool
{
    public class WakeOnLan : IDisposable
    {
        private readonly UdpClient _client;
        private static readonly IPEndPoint[] _endpoints = new IPEndPoint[]
        {
            new IPEndPoint(IPAddress.Broadcast, 7), // echo
            new IPEndPoint(IPAddress.Broadcast, 9) // discard
        };

        public WakeOnLan()
        {
            _client = new UdpClient(AddressFamily.InterNetwork)
            {
                EnableBroadcast = true // Enable broadcast, required for macOS compatibility
            };
        }

        public void Send(string macAddress)
        {
            try
            {
                var macParse = PhysicalAddress.Parse(macAddress); // Parse string value
                byte[] magicPacket = BuildMagicPacket(macParse); // Get magic packet byte array based on MAC Address
                foreach (var ep in _endpoints) // Broadcast to *all* WOL Endpoints
                {
                    _client.Send(magicPacket, magicPacket.Length, ep); // Broadcast magic packet
                }
            }
            catch (Exception ex)
            {
                throw new WakeOnLanException("ERROR broadcasting WakeOnLan Magic Packet.", ex);
            }
        }
        public void Send(PhysicalAddress macAddress)
        {
            try
            {
                byte[] magicPacket = BuildMagicPacket(macAddress); // Get magic packet byte array based on MAC Address
                foreach (var ep in _endpoints) // Broadcast to *all* WOL Endpoints
                {
                    _client.Send(magicPacket, magicPacket.Length, ep); // Broadcast magic packet
                }
            }
            catch (Exception ex)
            {
                throw new WakeOnLanException("ERROR broadcasting WakeOnLan Magic Packet.", ex);
            }
        }

        public async Task SendAsync(string macAddress)
        {
            try
            {
                var macParse = PhysicalAddress.Parse(macAddress); // Parse string value
                byte[] magicPacket = await Task.Run(() => BuildMagicPacket(macParse)).ConfigureAwait(false); // Get magic packet byte array based on MAC Address
                foreach (var ep in _endpoints) // Broadcast to *all* WOL Endpoints
                {
                    await _client.SendAsync(magicPacket, magicPacket.Length, ep).ConfigureAwait(false); // Broadcast magic packet
                }
            }
            catch (Exception ex)
            {
                throw new WakeOnLanException("ERROR broadcasting WakeOnLan Magic Packet.", ex);
            }
        }
        public async Task SendAsync(PhysicalAddress macAddress)
        {
            try
            {
                byte[] magicPacket = await Task.Run(() => BuildMagicPacket(macAddress)).ConfigureAwait(false); // Get magic packet byte array based on MAC Address
                foreach (var ep in _endpoints) // Broadcast to *all* WOL Endpoints
                {
                    await _client.SendAsync(magicPacket, magicPacket.Length, ep).ConfigureAwait(false); // Broadcast magic packet
                }
            }
            catch (Exception ex)
            {
                throw new WakeOnLanException("ERROR broadcasting WakeOnLan Magic Packet.", ex);
            }
        }

        private static byte[] BuildMagicPacket(PhysicalAddress macAddress)
        {
            byte[] macBytes = macAddress.GetAddressBytes(); // Convert MAC Address to array of bytes
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                for (int i = 0; i < 6; i++) // 6 times 0xFF
                {
                    bw.Write((byte)0xff);
                }
                for (int i = 0; i < 16; i++) // 16 times MAC Address
                {
                    bw.Write(macBytes);
                }
                return ms.ToArray(); // 102 Byte Magic Packet
            }
        }

        // Public implementation of Dispose pattern callable by consumers.
        private bool _disposed = false;
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
                _client?.Dispose();
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
