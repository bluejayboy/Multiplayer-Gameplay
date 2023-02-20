using Photon.Bolt;
using UdpKit;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TagToken : IProtocolToken
    {
        public string Tag { get; set; }

        public void Read(UdpPacket packet)
        {
            Tag = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(Tag);
        }
    }
}