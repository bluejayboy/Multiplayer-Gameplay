using Photon.Bolt;
using UdpKit;
using UnityEngine;
using Photon.Bolt.Utils;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RagdollToken : IProtocolToken
    {
        public bool IsController { get; set; }

        public string Creature { get; set; }
        public string HeadGarb { get; set; }
        public string FaceGarb { get; set; }
        public string NeckGarb { get; set; }
        public string HipGarb { get; set; }
        public string Face { get; set; }

        public Color32 HeadColor { get; set; }
        public Color32 TorsoColor { get; set; }
        public Color32 LeftHandColor { get; set; }
        public Color32 RightHandColor { get; set; }
        public Color32 LeftFootColor { get; set; }
        public Color32 RightFootColor { get; set; }

        public float VoicePitch { get; set; }

        public void Read(UdpPacket packet)
        {
            IsController = packet.ReadBool();
            Creature = packet.ReadString();
            Face = packet.ReadString();
            HeadGarb = packet.ReadString();
            FaceGarb = packet.ReadString();
            NeckGarb = packet.ReadString();
            HipGarb = packet.ReadString();
            HeadColor = packet.ReadColor32RGB();
            TorsoColor = packet.ReadColor32RGB();
            LeftHandColor = packet.ReadColor32RGB();
            RightHandColor = packet.ReadColor32RGB();
            LeftFootColor = packet.ReadColor32RGB();
            RightFootColor = packet.ReadColor32RGB();
            VoicePitch = packet.ReadFloat();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteBool(IsController);
            packet.WriteString(Creature);
            packet.WriteString(Face);
            packet.WriteString(HeadGarb);
            packet.WriteString(FaceGarb);
            packet.WriteString(NeckGarb);
            packet.WriteString(HipGarb);
            packet.WriteColor32RGB(HeadColor);
            packet.WriteColor32RGB(TorsoColor);
            packet.WriteColor32RGB(LeftHandColor);
            packet.WriteColor32RGB(RightHandColor);
            packet.WriteColor32RGB(LeftFootColor);
            packet.WriteColor32RGB(RightFootColor);
            packet.WriteFloat(VoicePitch);
        }
    }
}