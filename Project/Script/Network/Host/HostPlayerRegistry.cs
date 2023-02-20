using Photon.Bolt;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostPlayerRegistry : MonoBehaviour
    {
        public static HostPlayerRegistry Instance { get; private set; }

        public PlayerConnection ServerPlayerConnection { get; set; }
        public List<PlayerConnection> PlayerConnections { get; private set; }

        private void Awake()
        {
            Instance = this;
            PlayerConnections = new List<PlayerConnection>(64);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void InstantiatePlayerConnection(BoltConnection connection, IProtocolToken serverConnectToken = null, IProtocolToken serverAcceptToken = null)
        {
            var connectToken = (connection != null) ? connection.ConnectToken as ConnectToken : serverConnectToken as ConnectToken;

            if (connectToken == null)
            {
                return;
            }

            var playerInfoInstance = BoltPooler.Instance.Instantiate(BoltPrefabs.Player_Info, null, Vector3.zero, Quaternion.identity, false);

            if (connection != null)
            {
                playerInfoInstance.AssignControl(connection);
            }
            else
            {
                playerInfoInstance.TakeControl();
            }

            var playerConnection = new PlayerConnection
            {
                BoltConnection = connection,
                PlayerInfo = playerInfoInstance.GetComponent<PlayerInfo>()
            };

            connection.SetPlayerConnection(playerConnection);
            PlayerConnections.Add(playerConnection);

            var playerInfo = playerConnection.PlayerInfo;
            var name = connectToken.PenName;

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name) || name.Length < 1 || name.Length > 20 || name.Contains("nig") || name.Contains("Nig") || name.Contains("NIG"))
            {
                name = "Monkey";
            }

            if (!Regex.IsMatch(name, "^[a-zA-Z0-9. -_?]*$"))
            {
                name = "Monkey";
            }

            playerInfo.state.PenName = name;
            playerInfo.state.Face = connectToken.Face;
            playerInfo.state.Garb.Head = connectToken.HeadGarb;
            playerInfo.state.Garb.Face = connectToken.FaceGarb;
            playerInfo.state.Garb.Neck = connectToken.NeckGarb;
            playerInfo.state.SkinColor.Head = connectToken.HeadColor;
            playerInfo.state.SkinColor.Torso = connectToken.TorsoColor;
            playerInfo.state.SkinColor.LeftHand = connectToken.LeftHandColor;
            playerInfo.state.SkinColor.RightHand = connectToken.RightHandColor;
            playerInfo.state.SkinColor.LeftFoot = connectToken.LeftFootColor;
            playerInfo.state.SkinColor.RightFoot = connectToken.RightFootColor;
            playerInfo.state.VoicePitch = 1.5f;

            GameHud.Instance.state.PlayerCount = PlayerConnections.Count;
            BoltGlobalEvent.SendMessage(playerInfo.state.PenName + " joined!", Color.green);
        }

        public void DestroyPlayerConnection(PlayerConnection connection)
        {
            var playerInfo = connection.PlayerInfo;
            var penName = playerInfo.state.PenName;

            DestroyPlayer(connection);
            BoltPooler.Instance.Destroy(playerInfo.entity);
            connection.PlayerInfo = null;

            PlayerConnections.Remove(connection);
            connection.BoltConnection.SetPlayerConnection(null);
            connection.BoltConnection = null;

            GameHud.Instance.state.PlayerCount = PlayerConnections.Count;
            GameMode.Instance.OnDisconnect(connection);
            BoltGlobalEvent.SendMessage(penName + " left!", Color.red);
        }

        public Player InstantiatePlayer(IProtocolToken token, Vector3 position, Quaternion rotation, PlayerConnection connection)
        {
            if (connection == null)
            {
                return null;
            }

            var entityInstance = BoltPooler.Instance.Instantiate(BoltPrefabs.Creature, token, position, rotation, false);
            var player = entityInstance.GetComponent<Player>();

            connection.Player = player;

            return player;
        }

        public void DestroyPlayer(PlayerConnection connection)
        {
            if (connection == null || connection.Player == null)
            {
                return;
            }

            /*if (HostPlayerAction.Instance.TurretDeath != null)
            {
                HostPlayerAction.Instance.TurretDeath.Invoke(connection.Player.entity);
            }*/

            BoltPooler.Instance.Destroy(connection.Player.entity);
            connection.Player = null;
        }

        public void DestroyAllPlayers()
        {
            for (var i = 0; i < PlayerConnections.Count; i++)
            {
                DestroyPlayer(PlayerConnections[i]);
            }
        }
    }
}