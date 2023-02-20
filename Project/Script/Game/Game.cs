using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; }

        public enum MatchState { WaitingForPlayers, Intermission, PreStart, MidRound, PreEnd }
        public MatchState MatchStatus { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}