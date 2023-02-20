using Photon.Bolt;
using UdpKit;
using UnityEngine;
using Photon.Bolt.Utils;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TurretToken : IProtocolToken
    {
        public NetworkId NetworkID { get; set; }

        public void Read(UdpPacket packet)
        {
            NetworkID = packet.ReadNetworkId();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteNetworkId(NetworkID);
        }
    }
}