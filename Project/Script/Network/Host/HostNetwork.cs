using Photon.Bolt;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using Photon.Bolt.Matchmaking;
using UdpKit.Platform.Photon;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostNetwork : GlobalEventListener
    {
        public static HostNetwork Instance { get; private set; }

        public Dictionary<ulong, UdpEndPoint> ConnectBuffers { get; private set; }
        public Dictionary<UdpEndPoint, ulong> DisconnectBuffers { get; private set; }
        private const string serverPrefix = "<color=#CCCCCC>[<color=red>Server<color=#CCCCCC>] ";
        private List<ulong> spleens = new List<ulong>();
        private List<ulong> mods = new List<ulong>();

        private int maxPlayers;
        private List<UdpIPv4Address> addresses = new List<UdpIPv4Address>(64);
        private string serverVersion;

        private void Awake()
        {
            Instance = this;
            ConnectBuffers = new Dictionary<ulong, UdpEndPoint>(64);
            DisconnectBuffers = new Dictionary<UdpEndPoint, ulong>(64);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void SessionCreatedOrUpdated(UdpSession session)
        {
            var photonSession = session as PhotonSession;

            maxPlayers = Mathf.Clamp((int)photonSession.Properties["MaxPlayers"], 2, 50);
        }

        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            if (SteamLAN.IsLan)
            {
                BoltNetwork.Accept(endpoint);
                return;
            }

            var connectToken = token as ConnectToken;

            if (connectToken.Version == serverVersion && HostPlayerRegistry.Instance.PlayerConnections.Count < maxPlayers && !addresses.Contains(endpoint.Address))
            {
                addresses.Add(endpoint.Address);
                BoltNetwork.Accept(endpoint);
            }
            else if (connectToken.Version != serverVersion)
            {
                var refuseToken = new RefuseToken
                {
                    RefuseReason = "Version is out of date. Update or reinstall your game."
                };

                BoltNetwork.Refuse(endpoint, refuseToken);
            }
            else if (HostPlayerRegistry.Instance.PlayerConnections.Count >= maxPlayers)
            {
                var refuseToken = new RefuseToken
                {
                    RefuseReason = "Server is full."
                };

                BoltNetwork.Refuse(endpoint, refuseToken);
            }
            else
            {
                BoltNetwork.Refuse(endpoint);
            }
        }

        public override void Disconnected(BoltConnection connection)
        {
            if (connection.GetPlayerConnection() != null)
            {
                HostPlayerRegistry.Instance.DestroyPlayerConnection(connection.GetPlayerConnection());
            }

            if (DisconnectBuffers.ContainsKey(connection.RemoteEndPoint))
            {
                DisconnectBuffers.Remove(connection.RemoteEndPoint);
            }

            addresses.Remove(connection.RemoteEndPoint.Address);
        }

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            if (BoltNetwork.IsSinglePlayer)
            {
                var playerToken = new PlayerToken
                {
                    Creature = "Human",
                    Gadget1 = "Sword",
                    Gadget2 = "Knife",
                    Gadget3 = "Grenade",
                    TeamID = "Humans",
                    TeamLayer = "Team A",
                    TeamColor = Color.blue
                };

                BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Creature, playerToken, new Vector3(0.0f, 5.0f, 0.0f), Quaternion.identity);

                playerEntity.TakeControl(playerToken);
                Stage.Instance.Singleplayer();
                BoltPooler.Instance.SpawnWeapons();
            }

            HostPlayerRegistry.Instance.InstantiatePlayerConnection(null, token, null);
            if (scene != "Elim.Custom")
            {
                JoinMatch(HostPlayerRegistry.Instance.ServerPlayerConnection);
            }

            if (token is ConnectToken connectToken)
            {
                serverVersion = connectToken.Version;
            }
        }

        public override void SceneLoadRemoteDone(BoltConnection connection, IProtocolToken token)
        {
            HostPlayerRegistry.Instance.InstantiatePlayerConnection(connection);

            if (connection.GetPlayerConnection() == null)
            {
                return;
            }

            JoinMatch(connection.GetPlayerConnection());

            if (DisconnectBuffers.ContainsKey(connection.RemoteEndPoint))
            {
                DisconnectBuffers.Remove(connection.RemoteEndPoint);
            }
        }

        private void JoinMatch(PlayerConnection connection)
        {
            if (Game.Instance.MatchStatus == Game.MatchState.PreStart || Game.Instance.MatchStatus == Game.MatchState.MidRound || Game.Instance.MatchStatus == Game.MatchState.PreEnd)
            {
                GameMode.Instance.ConnectedDuringPlay(connection);
            }

            if (Lounge.Instance != null)
            {
                if (Game.Instance.MatchStatus == Game.MatchState.Intermission)
                {
                    Lounge.Instance.ConnectedDuringIntermission(connection);
                }
                else if (Game.Instance.MatchStatus == Game.MatchState.WaitingForPlayers)
                {
                    Lounge.Instance.ConnectedDuringWaitingForPlayers();
                }
            }
        }
    }
}