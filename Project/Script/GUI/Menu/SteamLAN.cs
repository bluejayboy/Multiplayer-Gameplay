using Photon.Bolt;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UdpKit;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class SteamLAN : GlobalEventListener
    {
        [SerializeField] private TMP_Dropdown map = null;
        [SerializeField] private string address = "127.0.0.1";
        [SerializeField] private ushort port = 1337;

        public static bool IsLan { get; private set; }

        public override void BoltStartDone()
        {
#if ISDEDICATED
            BoltNetwork.LoadScene(map.captionText.text);

            return;
#endif

            if (BoltNetwork.IsSinglePlayer)
            {
                BoltNetwork.LoadScene(map.captionText.text);

                return;
            }

            if (!IsLan)
            {
                return;
            }

            var connectToken = new ConnectToken
            {
                PenName = "Test",
                Face = ObscuredPrefs.GetString("Face"),
                HeadGarb = ObscuredPrefs.GetString("Head Garb"),
                FaceGarb = ObscuredPrefs.GetString("Face Garb"),
                NeckGarb = ObscuredPrefs.GetString("Neck Garb"),
                HeadColor = ObscuredPrefs.GetColor("Head Color"),
                TorsoColor = ObscuredPrefs.GetColor("Torso Color"),
                LeftHandColor = ObscuredPrefs.GetColor("Left Hand Color"),
                RightHandColor = ObscuredPrefs.GetColor("Right Hand Color"),
                LeftFootColor = ObscuredPrefs.GetColor("Left Foot Color"),
                RightFootColor = ObscuredPrefs.GetColor("Right Foot Color")
            };

            if (BoltNetwork.IsServer)
            {
                BoltNetwork.LoadScene(map.captionText.text, connectToken);
            }
            else if (BoltNetwork.IsClient)
            {
                BoltNetwork.Connect(port, connectToken);
            }
        }

        public void PlaySingleplayer()
        {
            if (!BoltNetwork.IsRunning)
            {
                BoltLauncher.StartSinglePlayer();
            }
        }

        public void JoinLan()
        {
            if (!BoltNetwork.IsRunning)
            {
                IsLan = true;
                BoltLauncher.StartClient();
            }
        }

        public void HostLan()
        {
            if (!BoltNetwork.IsRunning)
            {
#if !ISDEDICATED
                IsLan = true;
#endif

                BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Parse(address), port));
            }
        }
    }
}