using Photon.Bolt;
using UdpKit;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TeamToken : IProtocolToken
    {
        public string Team { get; set; }

        public void Read(UdpPacket packet)
        {
            Team = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(Team);
        }
    }
}