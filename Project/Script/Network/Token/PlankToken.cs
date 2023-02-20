using Photon.Bolt;
using UdpKit;
using UnityEngine;
using Photon.Bolt.Utils;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlankToken : IProtocolToken
    {
        public float Length { get; set; }
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public Vector3 Normal { get; set; }
        public NetworkId NetworkID { get; set; }

        public void Read(UdpPacket packet)
        {
            Length = packet.ReadFloat();
            StartPosition = packet.ReadVector3();
            EndPosition = packet.ReadVector3();
            Normal = packet.ReadVector3();
            NetworkID = packet.ReadNetworkId();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteFloat(Length);
            packet.WriteVector3(StartPosition);
            packet.WriteVector3(EndPosition);
            packet.WriteVector3(Normal);
            packet.WriteNetworkId(NetworkID);
        }
    }
}