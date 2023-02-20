using Photon.Bolt;
using TMPro;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ClientConnect : GlobalEventListener
    {
        public static ClientConnect Instance { get; private set; }

        [SerializeField] private MenuChanger menuChanger = null;
        [SerializeField] private TextMeshProUGUI refuseText = null;

        private void Awake()
        {
            Instance = this;

            ScramInput.LockAndHideCursor(false);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void Display()
        {
            if (ClientNetwork.Shutdown == ClientNetwork.ShutdownState.Refusal)
            {
                menuChanger.OpenMenu("Disconnect Menu");
                refuseText.text = ClientNetwork.RefuseReason;
            }
            else
            {
                menuChanger.OpenMenu("Disconnect Menu");
                refuseText.text = "Connection was lost.";
            }

            ClientNetwork.Shutdown = ClientNetwork.ShutdownState.None;
        }
    }
}