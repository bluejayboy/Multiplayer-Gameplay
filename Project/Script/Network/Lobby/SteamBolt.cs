using Photon.Bolt;
using CodeStage.AntiCheat.ObscuredTypes;
using UdpKit;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class SteamBolt : GlobalEventListener
    {
        private const ushort roomPort = 1337;

        public static bool IsListen { get; private set; }
        public static bool IsDedicated { get; private set; }

        private string map;
        private string ipPort;

        public void Join(string name, string map, int playerLimit)
        {
            IsDedicated = false;
            IsListen = true;
            this.map = map;

          //  BoltLauncher.SetUdpPlatform(new SteamPlatform());
                BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, roomPort));
                BoltLauncher.StartClient();
        }

        public void Join(string ipPort, bool isHost)
        {
            IsListen = false;
            IsDedicated = true;

            if (isHost)
            {
                return;
            }

            this.ipPort = ipPort;

      //      BoltLauncher.SetUdpPlatform(new DotNetPlatform());
            BoltLauncher.StartClient();
        }

        public override void BoltStartDone()
        {
            if (!IsListen && !IsDedicated)
            {
                return;
            }

         //   if (BoltNetwork.isServer)
            {
                if (IsListen)
                {
                    BoltNetwork.LoadScene(map, GetToken());
                }
            }
           // else if (BoltNetwork.isClient)
            {
                if (IsDedicated)
                {
                  //  BoltNetwork.Connect(UdpEndPoint.Parse(ipPort), GetToken());
                }
                else if (IsListen)
                {
                    uint ip;
                    ushort port;
                }
            }
        }

        private ConnectToken GetToken()
        {
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

            return connectToken;
        }
    }
}