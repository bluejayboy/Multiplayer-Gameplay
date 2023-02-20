using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ExterminationGameMode : GameMode
    {
        [SerializeField] private TeamData defaultTeamData = null;
        [SerializeField] private Team civilianTeam = null;
        [SerializeField] private Team exterminatorTeam = null;

        private List<PlayerConnection> leftOverPlayers = new List<PlayerConnection>(64);

        public override void OnDisconnect(PlayerConnection connection)
        {
            if (BoltNetwork.IsServer)
            {
                RemovePlayerFromTeam(connection);
            }
        }

        public override void ConnectedDuringPlay(PlayerConnection connection)
        {
            connection.PlayerInfo.state.Team = "Default";

            HostSpectate.Instance.Spectate(connection);
        }

        public override void PreStartRound()
        {
            base.PreStartRound();

            civilianTeam.PlayerSpawner.ResetSpawns();
            exterminatorTeam.PlayerSpawner.ResetSpawns();

            SpawnPlayerAndSendObjective("The government is coming! Quick, hide your peanuts!", "Alert", civilianTeam);
            SpawnPlayerAndSendObjective("The civilians are exposed to radiation. Prepare to breach.", "Alert", exterminatorTeam);
        }

        public override void StartRound()
        {
            base.StartRound();

            SendObjective("They're here! Keep your small butts hidden as long as you can!", "Alert", civilianTeam);
            SendObjective("They are all exposed! Destroy them!", "Alert", exterminatorTeam);
        }

        protected override void ApplyWinner()
        {
            var winnerTeam = string.Empty;

            for (int i = 0; i < HostPlayerRegistry.Instance.PlayerConnections.Count; i++)
            {
                PlayerConnection playerConnection = HostPlayerRegistry.Instance.PlayerConnections[i];
                IPlayerInfoState playerInfoState = playerConnection.PlayerInfo.state;

                if (playerInfoState.Team == winnerTeam)
                {
                    playerInfoState.Score += data.WinScore;
                }
                else
                {
                    playerInfoState.Score += data.LoseScore;
                }
            }

            if (civilianTeam.Players.Count > 0)
            {
                BoltGlobalEvent.SendObjectiveEvent("The civilians have successfully seduced the government!", "Game Complete", civilianTeam.Data.Color);

                winnerTeam = civilianTeam.Data.ID;
            }
            else
            {
                BoltGlobalEvent.SendObjectiveEvent("The civilians were too big to hide properly!", "Game Over", exterminatorTeam.Data.Color);

                winnerTeam = exterminatorTeam.Data.ID;
            }
        }

        public override void SetNewPlayerTeam(PlayerConnection connection)
        {
            int playerCount = HostPlayerRegistry.Instance.PlayerConnections.Count;
            int maxPlayerCount = Mathf.CeilToInt(exterminatorTeam.Data.Chance * playerCount);

            SetPlayerTeam(connection, maxPlayerCount);
        }

        public override void ApplyAllPlayerTeams()
        {
            civilianTeam.Players.Clear();
            exterminatorTeam.Players.Clear();

            List<PlayerConnection> players = HostPlayerRegistry.Instance.PlayerConnections;

            leftOverPlayers.AddRange(players);

            for (var i = 0; i < players.Count; i++)
            {
                var randomIndex = Random.Range(0, leftOverPlayers.Count);

                SetPlayerTeam(leftOverPlayers[randomIndex], Mathf.CeilToInt(exterminatorTeam.Data.Chance * players.Count));

                leftOverPlayers.Remove(leftOverPlayers[randomIndex]);
            }

            leftOverPlayers.Clear();
        }

        protected override void OnPlayerDeath(PlayerConnection connection, Vector3 position, float yaw)
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            RemovePlayerFromTeam(connection);

            Timing.RunCoroutine(HostSpectate.Instance.SpectateWithDelay(connection), "Respawn");
        }

        private void SetPlayerTeam(PlayerConnection connection, int maxPlayers)
        {
            IPlayerInfoState playerInfoState = connection.PlayerInfo.state;

            if (exterminatorTeam.Players.Count < maxPlayers)
            {
                exterminatorTeam.Players.Add(connection);

                playerInfoState.Color = exterminatorTeam.Data.Color;
                playerInfoState.Team = exterminatorTeam.Data.ID;

                BoltGlobalEvent.SendCue(exterminatorTeam.Data.Cue, exterminatorTeam.Data.Color, connection.BoltConnection);
            }
            else
            {
                civilianTeam.Players.Add(connection);

                playerInfoState.Color = civilianTeam.Data.Color;
                playerInfoState.Team = civilianTeam.Data.ID;

                BoltGlobalEvent.SendCue(civilianTeam.Data.Cue, civilianTeam.Data.Color, connection.BoltConnection);
            }
        }

        private void RemovePlayerFromTeam(PlayerConnection connection)
        {
            if (civilianTeam.Players.Contains(connection))
            {
                civilianTeam.Players.Remove(connection);
            }
            else if (exterminatorTeam.Players.Contains(connection))
            {
                exterminatorTeam.Players.Remove(connection);
            }

            if (connection != null && connection.PlayerInfo != null)
            {
                connection.PlayerInfo.state.Color = defaultTeamData.Color;
                connection.PlayerInfo.state.Team = defaultTeamData.ID;
            }

            if (civilianTeam.Players.Count <= 0 || exterminatorTeam.Players.Count <= 0)
            {
                if (Game.Instance.MatchStatus == Game.MatchState.PreStart || Game.Instance.MatchStatus == Game.MatchState.MidRound || Game.Instance.MatchStatus == Game.MatchState.PreEnd)
                {
                    PreEndRound();
                }
                else if (Game.Instance.MatchStatus == Game.MatchState.Intermission && HostPlayerRegistry.Instance.PlayerConnections.Count >= data.MinimumPlayerCount)
                {
                    ApplyAllPlayerTeams();
                }
            }
        }

        protected override void SpawnPlayerByTeam(PlayerConnection connection)
        {
            if (connection.PlayerInfo.state.Team == civilianTeam.Data.ID)
            {
                civilianTeam.PlayerSpawner.Spawn(connection);
            }
            else
            {
                exterminatorTeam.PlayerSpawner.Spawn(connection);
            }
        }
    }
}