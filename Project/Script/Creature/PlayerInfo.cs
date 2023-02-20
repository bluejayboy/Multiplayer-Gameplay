using Photon.Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerInfo : EntityBehaviour<IPlayerInfoState>
    {
        public static PlayerInfo Instance { get; private set; }

        public Player MyPlayer { get; set; }

        private TextMeshProUGUI pingText;

        void Start(){
            InvokeRepeating("RefreshPing", 0, 3);
        }

        void RefreshPing(){
            if(BoltNetwork.IsServer && entity.IsAttached){
                if(entity.HasControl){
                    state.Ping = 0;
                }else{
                    state.Ping = Mathf.RoundToInt(entity.Controller.PingNetwork * 1000);
                }
            }
        }

        public override void Attached()
        {
            if (LeaderboardMenu.Instance == null)
            {
                return;
            }

            LeaderboardMenu.Instance.AddPlayerInfoState(this);

            var leaderboardSnippet = LeaderboardMenu.Instance.LeaderboardSnippets[this];
            pingText = leaderboardSnippet.PingText;

            if (GameMode.Instance.Data.AllowBread)
            {
                state.AddCallback("Bread", () => leaderboardSnippet.Bread.text = "$" + state.Bread);
            }
            else
            {
                leaderboardSnippet.Bread.text = string.Empty;
            }

            state.AddCallback("PenName", () => leaderboardSnippet.PenName.text = state.PenName);
            state.AddCallback("Color", () => leaderboardSnippet.PenName.color = state.Color);
            state.AddCallback("Kills", () => leaderboardSnippet.Kills.text = state.Kills.ToString());
            state.AddCallback("Deaths", () => leaderboardSnippet.Deaths.text = state.Deaths.ToString());
            state.AddCallback("Score", () => leaderboardSnippet.Score.text = state.Score.ToString());
            state.AddCallback("SteamID", () => leaderboardSnippet.SteamID = ulong.Parse(state.SteamID));
            state.AddCallback("Ping", SetPing);

            if (entity.IsOwner)
            {
                state.Color = new Color32(196, 196, 196, 255);
            }
        }

        public override void Detached()
        {
            if (!entity.IsOwner)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }

            state.PenName = string.Empty;
            state.Team = string.Empty;
            state.Creature = string.Empty;
            state.Gadget1 = string.Empty;
            state.Gadget2 = string.Empty;
            state.Gadget3 = string.Empty;
            state.Armor = string.Empty;
            state.Bread = 0;
            state.Kills = 0;
            state.Deaths = 0;
            state.Score = 0;
            state.Garb.Head = string.Empty;
            state.Garb.Face = string.Empty;
            state.Garb.Chest = string.Empty;
            state.Garb.Ears = string.Empty;
            state.Garb.Neck = string.Empty;
            state.Garb.Hip = string.Empty;
            state.License.HasCommunismLicense = false;
            state.License.HasScralloweenLicense = false;
            state.License.HasSchristmasLicense = false;
            state.License.HasScracticalLicense = false;
            state.Face = string.Empty;
            state.pitch = 1;

            state.RemoveAllCallbacks();

            if (LeaderboardMenu.Instance != null)
            {
                LeaderboardMenu.Instance.RemovePlayerInfoState(this);
            }
        }

        public override void ControlGained()
        {
            Instance = this;

            if (LeaderboardMenu.Instance != null)
            {
                LeaderboardMenu.Instance.Attach(this);
            }

            if (IngameShop.Instance != null)
            {
                IngameShop.Instance.Attach(state);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void SetPing()
        {
            if (pingText == null)
            {
                return;
            }

            if (state.Ping < 100)
            {
                pingText.text = "<color=#B9FF47>" + state.Ping + "</color>";
            }
            else if (state.Ping < 200)
            {
                pingText.text = "<color=#FFE648>" + state.Ping + "</color>";
            }
            else
            {
                pingText.text = "<color=#FF5A48>" + state.Ping + "</color>";
            }
        }
    }
}