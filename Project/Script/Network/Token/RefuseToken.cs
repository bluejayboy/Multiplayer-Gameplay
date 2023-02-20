using Photon.Bolt;
using UdpKit;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RefuseToken : IProtocolToken
    {
        public string RefuseReason { get; set; }

        public void Read(UdpPacket packet)
        {
            RefuseReason = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(RefuseReason);
        }
    }
}