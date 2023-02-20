using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Stage : MonoBehaviour
    {
        public static Stage Instance { get; private set; }

        [SerializeField] private GameObject stageViewer = null;
        public GameObject StageViewer { get { return stageViewer; } }

        [SerializeField] private GameObject loadingMenu = null;
        [SerializeField] private GameObject migrateMenu = null;

        public bool IsLoaded { get; private set; }

        private void Awake()
        {
            Instance = this;
            AudioListener.volume = ObscuredPrefs.GetFloat("Main Volume");
        }

        private void Start()
        {
            if (ClientPlayerAction.Instance == null)
            {
                return;
            }

            ClientPlayerAction.Instance.OnSpawn += () => stageViewer.SetActive(false);
            ClientPlayerAction.Instance.OnSpectate += () => stageViewer.SetActive(false);

            ClientPlayerAction.Instance.OnSpawn += DisableLoading;
            ClientPlayerAction.Instance.OnSpectate += DisableLoading;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void Migrate()
        {
            StageViewer.SetActive(true);
            loadingMenu.SetActive(false);
            migrateMenu.SetActive(true);
        }

        public void Singleplayer()
        {
            IsLoaded = true;

            stageViewer.SetActive(false);
            loadingMenu.SetActive(false);
            migrateMenu.SetActive(false);

            ScramInput.LockAndHideCursor(true);
        }

        private void DisableLoading()
        {
            IsLoaded = true;

            stageViewer.SetActive(false);
            loadingMenu.SetActive(false);
            migrateMenu.SetActive(false);

            ScramInput.LockAndHideCursor(true);

            ClientPlayerAction.Instance.OnSpawn -= DisableLoading;
            ClientPlayerAction.Instance.OnSpectate -= DisableLoading;
        }
    }
}