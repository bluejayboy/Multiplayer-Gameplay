using Photon.Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }

        [SerializeField] private GameObject pauseMenu = null;
        [SerializeField] private GameObject buttonsMenu = null;
        [SerializeField] private GameObject optionsMenu = null;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void Pause()
        {
            IsPaused = !IsPaused;

            pauseMenu.SetActive(IsPaused);
            buttonsMenu.SetActive(true);
            optionsMenu.SetActive(false);

            ScramInput.LockAndHideCursor(!IsPaused);
        }

        public void Resume()
        {
            IsPaused = false;

            pauseMenu.SetActive(false);
            buttonsMenu.SetActive(true);
            optionsMenu.SetActive(false);

            ScramInput.LockAndHideCursor(true);
        }

        public void CommitSuicide()
        {
            BoltGlobalEvent.SendSuicide();

            Resume();
        }

        public void Leave()
        {
            if (BoltNetwork.IsRunning)
            {
                BoltNetwork.Shutdown();
                SceneManager.LoadScene("Home");
            }
        }
    }
}