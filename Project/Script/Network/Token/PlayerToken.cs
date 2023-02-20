using Photon.Bolt;
using UdpKit;
using UnityEngine;
using Photon.Bolt.Utils;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerToken : IProtocolToken
    {
        public string PenName { get; set; }

        public string Creature { get; set; }
        public string Gadget1 { get; set; }
        public string Gadget2 { get; set; }
        public string Gadget3 { get; set; }
        public string Armor { get; set; }

        public string TeamID { get; set; }
        public string TeamLayer { get; set; }
        public Color32 TeamColor { get; set; }

        public float Yaw { get; set; }

        public string Face { get; set; }
        public string HeadGarb { get; set; }
        public string FaceGarb { get; set; }
        public string NeckGarb { get; set; }

        public Color32 HeadColor { get; set; }
        public Color32 TorsoColor { get; set; }
        public Color32 LeftHandColor { get; set; }
        public Color32 RightHandColor { get; set; }
        public Color32 LeftFootColor { get; set; }
        public Color32 RightFootColor { get; set; }

        public bool HasCommunismLicense { get; set; }
        public bool HasScralloweenLicense { get; set; }
        public bool HasSchristmasLicense { get; set; }
        public bool HasScracticalLicense { get; set; }

        public float VoicePitch { get; set; }

        public void Read(UdpPacket packet)
        {
            PenName = packet.ReadString();
            Creature = packet.ReadString();
            Gadget1 = packet.ReadString();
            Gadget2= packet.ReadString();
            Gadget3 = packet.ReadString();
            Armor = packet.ReadString();
            TeamID = packet.ReadString();
            TeamLayer = packet.ReadString();
            TeamColor = packet.ReadColorRGB();
            Yaw = packet.ReadFloat();
            Face = packet.ReadString();
            HeadGarb = packet.ReadString();
            FaceGarb = packet.ReadString();
            NeckGarb = packet.ReadString();
            HeadColor = packet.ReadColor32RGB();
            TorsoColor = packet.ReadColor32RGB();
            LeftHandColor = packet.ReadColor32RGB();
            RightHandColor = packet.ReadColor32RGB();
            LeftFootColor = packet.ReadColor32RGB();
            RightFootColor = packet.ReadColor32RGB();
            HasCommunismLicense = packet.ReadBool();
            HasScralloweenLicense = packet.ReadBool();
            HasSchristmasLicense = packet.ReadBool();
            HasScracticalLicense = packet.ReadBool();
            VoicePitch = packet.ReadFloat();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(PenName);
            packet.WriteString(Creature);
            packet.WriteString(Gadget1);
            packet.WriteString(Gadget2);
            packet.WriteString(Gadget3);
            packet.WriteString(Armor);
            packet.WriteString(TeamID);
            packet.WriteString(TeamLayer);
            packet.WriteColorRGB(TeamColor);
            packet.WriteFloat(Yaw);
            packet.WriteString(Face);
            packet.WriteString(HeadGarb);
            packet.WriteString(FaceGarb);
            packet.WriteString(NeckGarb);
            packet.WriteColor32RGB(HeadColor);
            packet.WriteColor32RGB(TorsoColor);
            packet.WriteColor32RGB(LeftHandColor);
            packet.WriteColor32RGB(RightHandColor);
            packet.WriteColor32RGB(LeftFootColor);
            packet.WriteColor32RGB(RightFootColor);
            packet.WriteBool(HasCommunismLicense);
            packet.WriteBool(HasScralloweenLicense);
            packet.WriteBool(HasSchristmasLicense);
            packet.WriteBool(HasScracticalLicense);
            packet.WriteFloat(VoicePitch);
        }
    }
}