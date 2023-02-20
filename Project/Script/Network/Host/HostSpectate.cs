using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class HostSpectate : GlobalEventListener
    {
        public static HostSpectate Instance { get; private set; }

        [SerializeField] private GameModeData gameModeData = null;

        private List<Player> alivePlayers = new List<Player>();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void OnEvent(SpectateRequestEvent evnt)
        {
            CheckForAlivePlayers();

            var newPlayerIndex = ++evnt.PlayerIndex;
            var finalPlayerIndex = (newPlayerIndex >= 0 && newPlayerIndex <= alivePlayers.Count - 1) ? newPlayerIndex : 0;

            if (evnt.HasDelay)
            {
                SpectateWithDelay(evnt.RaisedBy.GetPlayerConnection(), finalPlayerIndex);
            }
            else
            {
                Spectate(evnt.RaisedBy.GetPlayerConnection(), finalPlayerIndex);
            }
        }

        public void Spectate(PlayerConnection spectator, int index = 0)
        {
            if (Game.Instance.MatchStatus != Game.MatchState.PreStart && Game.Instance.MatchStatus != Game.MatchState.MidRound && Game.Instance.MatchStatus != Game.MatchState.PreEnd)
            {
                return;
            }

            CheckForAlivePlayers();

            if (HostPlayerRegistry.Instance.PlayerConnections.Count > 0)
            {
                var finalPlayerIndex = (index >= 0 && index <= alivePlayers.Count - 1) ? index : 0;
                var targetPlayer = alivePlayers[finalPlayerIndex];

                BoltGlobalEvent.SendSpectate(index, targetPlayer.entity.NetworkId, spectator.BoltConnection);
                BoltGlobalEvent.SendCue("Spectating: " + targetPlayer.state.PenName, Color.white, spectator.BoltConnection);
            }
            else
            {
                BoltGlobalEvent.SendSpectate(-1, default(NetworkId), spectator.BoltConnection);
                BoltGlobalEvent.SendCue("Spectating: The World", Color.white, spectator.BoltConnection);
            }
        }

        private void CheckForAlivePlayers()
        {
            alivePlayers.Clear();

            var playerConnections = HostPlayerRegistry.Instance.PlayerConnections;

            for (var i = 0; i < playerConnections.Count; i++)
            {
                if (playerConnections[i].Player != null)
                {
                    alivePlayers.Add(playerConnections[i].Player);
                }
            }
        }

        public IEnumerator<float> SpectateWithDelay(PlayerConnection connection, int playerIndex = 0)
        {
            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= gameModeData.SpectateDelayTime)
                {
                    return true;
                }

                return false;
            });

            if (this == null || connection == null)
            {
                yield break;
            }

            Spectate(connection, playerIndex);
        }
    }
}