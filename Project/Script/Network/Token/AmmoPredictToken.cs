using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

public class AmmoPredictToken : IProtocolToken {

	public int frame;
	public int ammo;

	public AmmoPredictToken(int frame, int ammo){
		this.frame = frame;
		this.ammo = ammo;
	}
	public AmmoPredictToken(){}

    public void Write(UdpKit.UdpPacket packet)
    {
        packet.WriteInt(frame);
		packet.WriteInt(ammo);
    }

    public void Read(UdpKit.UdpPacket packet)
    {
        frame = packet.ReadInt();
		ammo = packet.ReadInt();
    }
}