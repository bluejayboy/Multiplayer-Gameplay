using Photon.Bolt;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public abstract class GameMode : GlobalEventListener
    {
        public static GameMode Instance { get; private set; }

        [SerializeField] protected GameModeData data = null;
        public GameModeData Data { get { return data; } }

        public virtual void ConnectedDuringPlay(PlayerConnection connection) { }
        public virtual void SetNewPlayerTeam(PlayerConnection connection) { }
        public virtual void ApplyAllPlayerTeams() { }

        protected virtual void SpawnPlayerByTeam(PlayerConnection connection) { }
        protected virtual void OnPlayerDeath(PlayerConnection connection, Vector3 position, float yaw) { }
        protected virtual void ApplyWinner() { }

        protected virtual void Awake()
        {
            Instance = this;
        }

        protected virtual void Start()
        {

        }

        public virtual void ClearDebris()
        {

        }

        public virtual void OnDisconnect(PlayerConnection connection)
        {
        }

        public void CheckScore(ulong steamID, int leaderboardID)
        {
            //StartCoroutine(ItemController.GetLeaderboardScore(steamID, leaderboardID, UserGetBack));
        }

        private void UnloadGarbage()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }

        public virtual void PreStartRound()
        {
            HostPlayerAction.Instance.OnDeath = OnPlayerDeath;
            Game.Instance.MatchStatus = Game.MatchState.PreStart;
            GameHud.Instance.state.Status = "Preparing for Round";

            if (HostGameAction.Instance.OnPreRoundStart != null)
            {
                HostGameAction.Instance.OnPreRoundStart.Invoke();
            }

            HostGameAction.Instance.OnTimerEnd = StartRound;
            GameTimer.Instance.StartTimer(data.PreStartTime);
        }

        public virtual void StartRound()
        {
            HostPlayerAction.Instance.OnDeath = OnPlayerDeath;
            Game.Instance.MatchStatus = Game.MatchState.MidRound;
            GameHud.Instance.state.Status = string.Empty;

            if (HostGameAction.Instance.OnRoundStart != null)
            {
                HostGameAction.Instance.OnRoundStart.Invoke();
            }

            if (data.HasTimeLimit)
            {
                HostGameAction.Instance.OnTimerEnd = PreEndRound;
                GameTimer.Instance.StartTimer(data.RoundTime);
            }
        }

        protected virtual void PreEndRound()
        {
            if (BoltNetwork.IsClient || (Game.Instance.MatchStatus != Game.MatchState.PreStart && Game.Instance.MatchStatus != Game.MatchState.MidRound))
            {
                return;
            }

            HostGameAction.Instance.OnTimerEnd = null;
            Game.Instance.MatchStatus = Game.MatchState.PreEnd;
            GameHud.Instance.state.Status = "Game Over";
            GameHud.Instance.state.Timer = -1;

            if (HostGameAction.Instance.OnRoundPreEnd != null)
            {
                HostGameAction.Instance.OnRoundPreEnd.Invoke();
            }

            ApplyWinner();
            MEC.Timing.RunCoroutine(EndRound());
        }

        protected virtual IEnumerator<float> EndRound()
        {
            double currentTime = TimeController.GetTime();

            yield return MEC.Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= data.PreEndTime)
                {
                    return true;
                }

                return false;
            });

            if (this == null)
            {
                yield break;
            }

            HostPlayerAction.Instance.OnDeath = null;

            if (HostGameAction.Instance.OnRoundEnd != null)
            {
                HostGameAction.Instance.OnRoundEnd.Invoke();
            }

            HostPlayerRegistry.Instance.DestroyAllPlayers();
            BoltPooler.Instance.Refresh();
            ClearDebris();
            UnloadGarbage();

            Lounge.Instance.StartIntermission();
            BoltGlobalEvent.SendObjectiveEvent(string.Empty, string.Empty, default(Color32));
        }

        protected virtual void SendObjective(string text, string sound, Team team)
        {
            for (var i = 0; i < team.Players.Count; i++)
            {
                BoltGlobalEvent.SendObjectiveEvent(text, sound, team.Data.Color, team.Players[i].BoltConnection);
            }
        }

        protected virtual void SpawnPlayerAndSendObjective(string text, string sound, Team team)
        {
            for (var i = 0; i < team.Players.Count; i++)
            {
                SpawnPlayerByTeam(team.Players[i]);

                BoltGlobalEvent.SendObjectiveEvent(text, sound, team.Data.Color, team.Players[i].BoltConnection);
            }
        }

        [Serializable]
        public sealed class Team
        {
            [SerializeField] private TeamData data = null;
            public TeamData Data { get { return data; } }

            [SerializeField] private PlayerSpawner playerSpawner = null;
            public PlayerSpawner PlayerSpawner { get { return playerSpawner; } }

            public List<PlayerConnection> Players { get; private set; }

            private Team()
            {
                Players = new List<PlayerConnection>(64);
            }
        }
    }
}