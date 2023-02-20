using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    public sealed class LeaderboardMenu : MonoBehaviour
    {
        public static LeaderboardMenu Instance { get; private set; }

        [SerializeField] private GameObject menu = null;
        [SerializeField] private LeaderboardSnippet snippet = null;
        [SerializeField] private Transform content = null;
        [SerializeField] private Color32 attachColor = default(Color32);

        public Dictionary<PlayerInfo, LeaderboardSnippet> LeaderboardSnippets { get; set; }

        public bool IsViewingLeaderboard { get; private set; }

        private void Awake()
        {
            Instance = this;
            LeaderboardSnippets = new Dictionary<PlayerInfo, LeaderboardSnippet>(64);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void Attach(PlayerInfo playerInfo)
        {
            LeaderboardSnippets[playerInfo].Background.color = attachColor;
        }

        public void ShowLeaderboard(bool willShow)
        {
            IsViewingLeaderboard = willShow;

            menu.SetActive(willShow);

            ScramInput.LockAndHideCursor(!willShow);
        }

        public void AddPlayerInfoState(PlayerInfo playerInfo)
        {
            var snippet = Instantiate(this.snippet, content);

            LeaderboardSnippets.Add(playerInfo, snippet);
            //snippet.GetComponent<LeaderboardSnippet>().SteamID = ulong.Parse(playerInfo.state.SteamID);
        }

        public void RemovePlayerInfoState(PlayerInfo playerInfo)
        {
            if (!LeaderboardSnippets.ContainsKey(playerInfo))
            {
                return;
            }

            if (LeaderboardSnippets[playerInfo] != null && LeaderboardSnippets[playerInfo].gameObject != null)
            {
                Destroy(LeaderboardSnippets[playerInfo].gameObject);
            }

            LeaderboardSnippets.Remove(playerInfo);
        }
    }
}