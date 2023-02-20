using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class IngameShop : MonoBehaviour
    {
        public static IngameShop Instance { get; private set; }

        [SerializeField] private IngameItemSnippet gameItemSnippet = null;
        [SerializeField] private TextMeshProUGUI bread = null;
        [SerializeField] private GameObject shopMenu = null;
        [SerializeField] private GameObject[] teamMenus = null;
        [SerializeField] private ItemMenu[] itemMenus = null;
        [SerializeField] private GameObject[] sections = null;

        private IPlayerInfoState activePlayerInfoState = null;
        private List<IngameItemSnippet> gameItemSnippets = new List<IngameItemSnippet>(100);

        public bool IsViewingShopMenu { get;  set; }
        public Dictionary<string, IngameItemData> GameItems { get; private set; }

        private void Awake()
        {
            Instance = this;
            GameItems = new Dictionary<string, IngameItemData>();

            for (var i = 0; i < itemMenus.Length; i++)
            {
                for (var j = 0; j < itemMenus[i].IngameShop.GameItems.Length; j++)
                {
                    var data = itemMenus[i].IngameShop.GameItems[j];
                    var snippet = Instantiate(gameItemSnippet, itemMenus[i].Content);

                    snippet.SetInfo(data.ID, data.DisplayText, data.DisplayImage, data.Price);

                    gameItemSnippets.Add(snippet);
                    GameItems.Add(data.ID, data);
                }
            }
        }

        private void Start()
        {
            if (ClientPlayerAction.Instance != null)
            {
                ClientPlayerAction.Instance.OnSpawn += CloseDasMenu;
                ClientPlayerAction.Instance.OnSpectate += CloseDasMenu;
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            var playerEntity = other.GetComponent<BoltEntity>();

            if (playerEntity == null || !playerEntity.IsAttached || !playerEntity.StateIs<IPlayerState>() || !playerEntity.HasControl)
            {
                return;
            }

            OpenMenu(activePlayerInfoState.Team);
        }

        private void OnTriggerExit(Collider other)
        {
            var playerEntity = other.GetComponent<BoltEntity>();

            if (playerEntity == null || !playerEntity.IsAttached || !playerEntity.StateIs<IPlayerState>() || !playerEntity.HasControl)
            {
                return;
            }

            CloseMenu();
        }

        public void Attach(IPlayerInfoState state)
        {
            activePlayerInfoState = state;
            activePlayerInfoState.AddCallback("Bread", () => bread.text = "$" + state.Bread);

            for (var i = 0; i < gameItemSnippets.Count; i++)
            {
                gameItemSnippets[i].Attach(activePlayerInfoState);
            }
        }

        public void OpenSection(string section)
        {
            for (int i = 0; i < sections.Length; i++)
            {
                if (sections[i].name == section)
                {
                    sections[i].SetActive(true);
                }
                else
                {
                    sections[i].SetActive(false);
                }
            }
        }

        private void OpenMenu(string team)
        {
            for (var i = 0; i < teamMenus.Length; i++)
            {
                if (teamMenus[i].name == team)
                {
                    IsViewingShopMenu = true;

                    ScramInput.LockAndHideCursor(false);
                    shopMenu.SetActive(true);
                    teamMenus[i].SetActive(true);
                }
                else
                {
                    teamMenus[i].SetActive(false);
                }
            }

            OpenSection("Weapon");
        }

        public void CloseDasMenu()
        {
            CloseMenu();
            Invoke("CloseMenu", 0.5f);
        }

        public void CloseMenu()
        {
            IsViewingShopMenu = false;

            ScramInput.LockAndHideCursor(true);
            shopMenu.SetActive(false);
        }

        [Serializable]
        private struct ItemMenu
        {
            [SerializeField] private Transform content;
            public Transform Content { get { return content; } }

            [SerializeField] private IngameShopData ingameShop;
            public IngameShopData IngameShop { get { return ingameShop; } }
        }
    }
}