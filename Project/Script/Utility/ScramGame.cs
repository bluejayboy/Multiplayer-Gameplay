using UdpKit;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    public static class ScramGame
    {
        public static void Enable(this GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public static void ResetAllPlayerTeams()
        {
            for (var i = 0; i < HostPlayerRegistry.Instance.PlayerConnections.Count; i++)
            {
                IPlayerInfoState playerInfoState = HostPlayerRegistry.Instance.PlayerConnections[i].PlayerInfo.state;

                playerInfoState.Team = "Default";
                playerInfoState.Color = new Color32(196, 196, 196, 255);
                playerInfoState.Creature = string.Empty;
                playerInfoState.Gadget1 = string.Empty;
                playerInfoState.Gadget2 = string.Empty;
                playerInfoState.Gadget3 = string.Empty;
                playerInfoState.Armor = string.Empty;
            }
        }

        public static PlayerConnection GetPlayerConnection(this BoltConnection boltConnection)
        {
            if (boltConnection != null)
            {
                return boltConnection.UserData as PlayerConnection;
            }
            else
            {
                return HostPlayerRegistry.Instance.ServerPlayerConnection;
            }
        }

        public static void SetPlayerConnection(this BoltConnection boltConnection, PlayerConnection playerConnection)
        {
            if (boltConnection != null)
            {
                boltConnection.UserData = playerConnection;
            }
            else
            {
                HostPlayerRegistry.Instance.ServerPlayerConnection = playerConnection;
            }
        }

        public static UdpEndPoint GetEndPoint(this ulong id)
        {
            return new UdpEndPoint(new UdpSteamID(id));
        }
    }
}