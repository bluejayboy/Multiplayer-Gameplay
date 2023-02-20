using Photon.Bolt;
using UdpKit;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RoomToken : IProtocolToken
    {
        public string MapName { get; set; }
        public int MaxPlayerCount  { get; set; }

        public void Read(UdpPacket packet)
        {
            MapName = packet.ReadString();
            MaxPlayerCount = packet.ReadInt();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(MapName);
            packet.WriteInt(MaxPlayerCount);
        }
    }
}