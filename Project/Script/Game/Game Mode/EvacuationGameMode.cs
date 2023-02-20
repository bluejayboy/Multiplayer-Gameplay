using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class EvacuationGameMode : GameMode
    {
        [SerializeField] private RescueData rescueData = null;
        [SerializeField] private RescueVehicle rescueVehicle = null;
        [SerializeField] private Team humanTeam = null;
        [SerializeField] private Team mutantTeam = null;

        private List<PlayerConnection> leftOverPlayers = new List<PlayerConnection>(64);

        public List<PlayerConnection> EscapedHumans { get; private set; }
        public List<PlayerConnection> BornMutants { get; private set; }

        public int ArrivalTime { get; private set; }

        private BoltEntity nuke;

        protected override void Awake()
        {
            base.Awake();

            EscapedHumans = new List<PlayerConnection>(64);
            BornMutants = new List<PlayerConnection>(64);
        }

        public override void ClearDebris()
        {
            if (nuke != null)
            {
                BoltNetwork.Destroy(nuke);
            }
        }

        public override void ConnectedDuringPlay(PlayerConnection connection)
        {
            AddPlayerToTeam(connection, mutantTeam);
            mutantTeam.PlayerSpawner.Spawn(connection);
        }

        public override void OnDisconnect(PlayerConnection connection)
        {
            RemovePlayerFromTeam(connection);
        }

        public override void StartRound()
        {
            base.StartRound();

            GameHud.Instance.state.Status = "Find the Radio";

            humanTeam.PlayerSpawner.ResetSpawns();
            mutantTeam.PlayerSpawner.ResetSpawns();

            SpawnPlayerAndSendObjective("Find that small radio and call for evacuation!", "Alert", humanTeam);
            SpawnPlayerAndSendObjective("The peasants have been bad. Kill them.", "Alert", mutantTeam);
        }

        protected override void ApplyWinner()
        {
            List<PlayerConnection> winPlayers = null;

            if (EscapedHumans.Count > 0 || mutantTeam.Players.Count <= 0)
            {
                winPlayers = EscapedHumans;

                BoltGlobalEvent.SendObjectiveEvent("The peasants have escaped home to return to their kids!", "Game Complete", humanTeam.Data.Color);
            }
            else
            {
                winPlayers = BornMutants;

                BoltGlobalEvent.SendObjectiveEvent("The mutants have successfully eaten all peasants!", "Game Over", mutantTeam.Data.Color);
            }

            for (int i = 0; i < HostPlayerRegistry.Instance.PlayerConnections.Count; i++)
            {
                PlayerConnection playerConnection = HostPlayerRegistry.Instance.PlayerConnections[i];

                if (winPlayers.Contains(playerConnection))
                {
                    playerConnection.PlayerInfo.state.Score += data.WinScore;
                    playerConnection.PlayerInfo.state.Bread += data.WinBread;

                    BoltGlobalEvent.SendBread(data.WinBread, playerConnection.BoltConnection);
                }
                else
                {
                    playerConnection.PlayerInfo.state.Score += data.LoseScore;
                    playerConnection.PlayerInfo.state.Bread += data.LoseBread;

                    BoltGlobalEvent.SendBread(data.LoseBread, playerConnection.BoltConnection);
                }
            }

            EscapedHumans.Clear();
            BornMutants.Clear();
        }

        public override void SetNewPlayerTeam(PlayerConnection connection)
        {
            int playerCount = HostPlayerRegistry.Instance.PlayerConnections.Count;
            int maxPlayerCount = Mathf.CeilToInt(mutantTeam.Data.Chance * playerCount);

            SetPlayerTeam(connection, maxPlayerCount);
        }

        public override void ApplyAllPlayerTeams()
        {
            humanTeam.Players.Clear();
            mutantTeam.Players.Clear();
            EscapedHumans.Clear();
            BornMutants.Clear();

            List<PlayerConnection> players = HostPlayerRegistry.Instance.PlayerConnections;

            leftOverPlayers.AddRange(players);

            for (var i = 0; i < players.Count; i++)
            {
                var randomIndex = Random.Range(0, leftOverPlayers.Count);
                var randomPlayer = leftOverPlayers[randomIndex];

                SetPlayerTeam(randomPlayer, Mathf.CeilToInt(mutantTeam.Data.Chance * players.Count));

                leftOverPlayers.Remove(randomPlayer);
            }

            leftOverPlayers.Clear();
        }

        protected override void OnPlayerDeath(PlayerConnection connection, Vector3 position, float yaw)
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            if (humanTeam.Players.Contains(connection))
            {
                if (EscapedHumans.Contains(connection))
                {
                    EscapedHumans.Remove(connection);
                }

                RemovePlayerFromTeam(connection);
                AddPlayerToTeam(connection, mutantTeam);
                Timing.RunCoroutine(mutantTeam.PlayerSpawner.SpawnWithDelay(connection, position, yaw), "Respawn");
            }
            else
            {
                Timing.RunCoroutine(mutantTeam.PlayerSpawner.SpawnWithDelay(connection), "Respawn");
            }
        }

        private void SetPlayerTeam(PlayerConnection connection, int maxPlayerCount)
        {
            if (mutantTeam.Players.Count < maxPlayerCount)
            {
                BornMutants.Add(connection);
                AddPlayerToTeam(connection, mutantTeam);
                BoltGlobalEvent.SendCue(mutantTeam.Data.Cue, mutantTeam.Data.Color, connection.BoltConnection);
            }
            else
            {
                AddPlayerToTeam(connection, humanTeam);
                BoltGlobalEvent.SendCue(humanTeam.Data.Cue, humanTeam.Data.Color, connection.BoltConnection);
            }
        }

        private void AddPlayerToTeam(PlayerConnection connection, Team team)
        {
            team.Players.Add(connection);

            IPlayerInfoState playerInfoState = connection.PlayerInfo.state;

            playerInfoState.Team = team.Data.ID;
            playerInfoState.Color = team.Data.Color;
            playerInfoState.Creature = string.Empty;
            playerInfoState.Gadget1 = string.Empty;
            playerInfoState.Gadget2 = string.Empty;
            playerInfoState.Gadget3 = string.Empty;
            playerInfoState.Armor = string.Empty;
        }

        private void RemovePlayerFromTeam(PlayerConnection connection)
        {
            if (EscapedHumans.Contains(connection))
            {
                EscapedHumans.Remove(connection);
            }

            if (humanTeam.Players.Contains(connection))
            {
                humanTeam.Players.Remove(connection);
            }
            else if (mutantTeam.Players.Contains(connection))
            {
                mutantTeam.Players.Remove(connection);
            }

            if (humanTeam.Players.Count <= 0 || mutantTeam.Players.Count <= 0)
            {
                if (Game.Instance.MatchStatus == Game.MatchState.PreStart || Game.Instance.MatchStatus == Game.MatchState.MidRound)
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
            var playerInfoState = connection.PlayerInfo.state;

            if (playerInfoState.Team == humanTeam.Data.ID)
            {
                humanTeam.PlayerSpawner.Spawn(connection);
            }
            else
            {
                mutantTeam.PlayerSpawner.Spawn(connection);
            }

            playerInfoState.Creature = string.Empty;
            playerInfoState.Gadget1 = string.Empty;
            playerInfoState.Gadget2 = string.Empty;
            playerInfoState.Gadget3 = string.Empty;
            playerInfoState.Armor = string.Empty;
        }

        public void StartGas()
        {
            HostGameAction.Instance.OnTimerEnd = PreEndRound;
            GameHud.Instance.state.Status = "Turn off the gas";

            GameTimer.Instance.StartTimer(60);

            SendObjective("Turn that gas off!!", "Alert", humanTeam);
            SendObjective("Spank the peasants!", "Alert", mutantTeam);
        }

        public void StartGate()
        {
            HostGameAction.Instance.OnTimerEnd = PreEndRound;
            GameHud.Instance.state.Status = "Open the gates";

            GameTimer.Instance.StartTimer(60);

            SendObjective("Capture that control point to open the gates!", "Alert", humanTeam);
            SendObjective("They're breaching, charge at them!", "Alert", mutantTeam);
        }

        public void StartRadio()
        {
            HostGameAction.Instance.OnTimerEnd = PreEndRound;
            GameHud.Instance.state.Status = "Find the Radio";

            GameTimer.Instance.StartTimer(60);

            SendObjective("Find that small radio and call for evacuation!", "Alert", humanTeam);
            SendObjective("The peasants have been bad. Kill them.", "Alert", mutantTeam);
        }

        public void StartArrival()
        {
            HostGameAction.Instance.OnTimerEnd = StartStandby;
            GameHud.Instance.state.Status = "Wait for Rescue";

            ArrivalTime = rescueData.ArrivalTime + Mathf.Clamp(GameHud.Instance.state.Timer, 0, 120);
            GameTimer.Instance.StartTimer(ArrivalTime);
            rescueVehicle.state.Destination = 1;

            SendObjective("Hold your position, help is arriving!", "Alert", humanTeam);
            SendObjective("They have called for evacuation. Find them so you can ruin their lives.", "Alert", mutantTeam);
        }

        private void StartStandby()
        {
            HostGameAction.Instance.OnTimerEnd = StartDeparture;
            GameHud.Instance.state.Status = "Get to the Choppa";

            GameTimer.Instance.StartTimer(rescueData.StandbyTime);

            SendObjective("The helicopter has arrived, get your self in here.", "Alert", humanTeam);
            SendObjective("They are getting away. Hit them harder!", "Alert", mutantTeam);
        }

        private void StartDeparture()
        {
            Timing.RunCoroutine(Depart());
        }

        private IEnumerator<float> Depart()
        {
            HostGameAction.Instance.OnTimerEnd = null;
            GameHud.Instance.state.Status = "Departing from Zone";

            rescueVehicle.state.Destination = 2;

            nuke = BoltNetwork.Instantiate(BoltPrefabs.Nuke, new Vector3(0, 50, 0), Quaternion.identity);

            SendObjective("We are leaving now! Too bad for those who didn't make it. Haha!", "Alert", humanTeam);
            SendObjective("They are escaping.", "Alert", mutantTeam);

            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= rescueData.DepartureTime)
                {
                    return true;
                }

                return false;
            });

            if (this == null)
            {
                yield break;
            }

            PreEndRound();
        }
    }
}