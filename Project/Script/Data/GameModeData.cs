using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Game Mode")]
    public sealed class GameModeData : ScriptableObject
    {
        [SerializeField] private int minimumPlayerCount = 2;
        public int MinimumPlayerCount { get { return minimumPlayerCount; } }

        [SerializeField] private int debugIntermissionTime = 1;
        [SerializeField] private int intermissionTime = 15;
        public int IntermissionTime { get { return DebugMode.IsDebug ? debugIntermissionTime : intermissionTime; } }

        [SerializeField] private bool hasPreRound = false;
        public bool HasPreRound { get { return hasPreRound; } }

        [SerializeField] private int preStartTime = 30;
        public int PreStartTime { get { return preStartTime; } }

        [SerializeField] private bool hasTimeLimit = true;
        public bool HasTimeLimit { get { return hasTimeLimit; } }

        [SerializeField] private int roundTime = 60;
        public int RoundTime { get { return roundTime; } }

        [SerializeField] private int preEndTime = 5;
        public int PreEndTime { get { return preEndTime; } }

        [SerializeField] private int spectateDelayTime = 3;
        public int SpectateDelayTime { get { return spectateDelayTime; } }

        [SerializeField] private int winScore = 3;
        public int WinScore { get { return winScore; } }

        [SerializeField] private int loseScore = 0;
        public int LoseScore { get { return loseScore; } }

        [SerializeField] private int winBread = 300;
        public int WinBread { get { return winBread; } }

        [SerializeField] private int loseBread = 100;
        public int LoseBread { get { return loseBread; } }

        [SerializeField] private bool allowBread = false;
        public bool AllowBread { get { return allowBread; } }

        [SerializeField] private bool allowSuicide = true;
        public bool AllowSuicide { get { return allowSuicide; } }

        [SerializeField] private bool allowTeamKill = false;
        public bool AllowTeamKill { get { return allowTeamKill; } }

        [SerializeField] private bool allowTeamCollision = false;
        public bool AllowTeamCollision { get { return allowTeamCollision; } }

        [SerializeField] private bool allowFootprint = false;
        public bool AllowFootprint { get { return allowFootprint; } }

        [SerializeField] private bool allowGadgetDrop = true;
        public bool AllowGadgetDrop { get { return allowGadgetDrop; } }

        [SerializeField] private bool showTeamIdentity = true;
        public bool ShowTeamIdentity { get { return showTeamIdentity; } }

        [SerializeField] private bool hideSpectateMessage = true;
        public bool HideSpectateMessage { get { return hideSpectateMessage; } }

        [SerializeField] private bool globalVoiceChat = true;
        public bool GlobalVoiceChat { get { return globalVoiceChat; } }
    }
}