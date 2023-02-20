using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class JoinMenu : MonoBehaviour
    {
        [SerializeField] private GameObject main = null;
        [SerializeField] private GameObject page = null;
        [SerializeField] private GameObject joinButton = null;
        [SerializeField] private GameObject hostButton = null;
        [SerializeField] private GameObject errorText = null;
        [SerializeField] private GameObject dedicatedRefreshButton = null;
        [SerializeField] private GameObject lobbyRefreshButton = null;
        public NewNetwork network;

        private void OnEnable()
        {
            //ActivateMain();
            network.OpenJoin();
        }

        public void ActivateDedicated()
        {
            main.SetActive(false);
            page.SetActive(true);
            joinButton.SetActive(false);
            hostButton.SetActive(false);
            //errorText.SetActive(false);
            dedicatedRefreshButton.SetActive(true);
            lobbyRefreshButton.SetActive(false);
        }

        public void ActivateP2P()
        {
            main.SetActive(false);
            page.SetActive(true);
            joinButton.SetActive(true);
            hostButton.SetActive(true);
            //errorText.SetActive(true);
            dedicatedRefreshButton.SetActive(false);
            lobbyRefreshButton.SetActive(true);
        }

        public void ActivateMain()
        {
            main.SetActive(true);
            page.SetActive(false);
            joinButton.SetActive(false);
            hostButton.SetActive(false);
            //errorText.SetActive(false);
            dedicatedRefreshButton.SetActive(false);
            lobbyRefreshButton.SetActive(false);
        }
    }
}