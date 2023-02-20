using Photon.Bolt;
using UdpKit;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class LicenseToken : IProtocolToken
    {
        public bool HasCommunismLicense { get; set; }
        public bool HasScralloweenLicense { get; set; }
        public bool HasSchristmasLicense { get; set; }
        public bool HasScracticalLicense { get; set; }

        public void Read(UdpPacket packet)
        {
            HasCommunismLicense = packet.ReadBool();
            HasScralloweenLicense = packet.ReadBool();
            HasSchristmasLicense = packet.ReadBool();
            HasScracticalLicense = packet.ReadBool();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteBool(HasCommunismLicense);
            packet.WriteBool(HasScralloweenLicense);
            packet.WriteBool(HasSchristmasLicense);
            packet.WriteBool(HasScracticalLicense);
        }
    }
}