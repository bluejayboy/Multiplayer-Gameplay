using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class LeaderboardSnippet : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI penName = null;
        public TextMeshProUGUI PenName { get { return penName; } }

        [SerializeField] private TextMeshProUGUI kills = null;
        public TextMeshProUGUI Kills { get { return kills; } }

        [SerializeField] private TextMeshProUGUI deaths = null;
        public TextMeshProUGUI Deaths { get { return deaths; } }

        [SerializeField] private TextMeshProUGUI score = null;
        public TextMeshProUGUI Score { get { return score; } }

        [SerializeField] private TextMeshProUGUI bread = null;
        public TextMeshProUGUI Bread { get { return bread; } }

        [SerializeField] private Image background = null;
        public Image Background { get { return background; } }
        [SerializeField] private TextMeshProUGUI pingText = null;
        public TextMeshProUGUI PingText { get { return pingText; } }
        public ulong SteamID;
    }
}