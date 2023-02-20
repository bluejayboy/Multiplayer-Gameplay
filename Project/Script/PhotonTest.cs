using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using Scram;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UdpKit;
using UnityEngine;

public class PhotonTest : GlobalEventListener
{
    public void Host()
    { 
        BoltLauncher.StartServer();
    }

    public void Join()
    {
        BoltLauncher.StartClient();
    }

    public override void BoltStartDone()
    {
        var connectToken = new ConnectToken
        {
            PenName = "Test",
            HeadColor = ObscuredPrefs.GetColor("Head Color"),
            TorsoColor = ObscuredPrefs.GetColor("Torso Color"),
            LeftHandColor = ObscuredPrefs.GetColor("Left Hand Color"),
            RightHandColor = ObscuredPrefs.GetColor("Right Hand Color"),
            LeftFootColor = ObscuredPrefs.GetColor("Left Foot Color"),
            RightFootColor = ObscuredPrefs.GetColor("Right Foot Color")
        };

        if (BoltNetwork.IsServer)
        {
            BoltMatchmaking.CreateSession("Test", null, "Evacuation.Puketown", connectToken);
        }
        else if (BoltNetwork.IsClient)
        {
            BoltMatchmaking.JoinRandomSession(connectToken);
        }
    }
}