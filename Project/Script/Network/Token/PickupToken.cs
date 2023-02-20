using Photon.Bolt;
using UdpKit;
using UnityEngine;
using Photon.Bolt.Utils;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PickupToken : IProtocolToken
    {
        public Vector3 Force { get; set; }
        public int Ammo { get; set; }
        public BoltEntity DropEntity { get; set; }
        public Color32 Color { get; set; }

        public void Read(UdpPacket packet)
        {
            Force = packet.ReadVector3();
            Ammo = packet.ReadInt();
            DropEntity = packet.ReadBoltEntity();
            Color = packet.ReadColor32RGB();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteVector3(Force);
            packet.WriteInt(Ammo);
            packet.WriteBoltEntity(DropEntity.IsAttached ? DropEntity : null);
            packet.WriteColor32RGB(Color);
        }
    }
}