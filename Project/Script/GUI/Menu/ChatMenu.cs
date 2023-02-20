using Photon.Bolt;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ChatMenu : MonoBehaviour
    {
        public static ChatMenu Instance { get; private set; }

        [SerializeField] private Transform content = null;
        [SerializeField] private TextMeshProUGUI messageSnippet = null;
        [SerializeField] private TMP_InputField chatInput = null;
        [SerializeField] private GameObject chatOutput = null;
        [SerializeField] private ScrollRect scrollRect = null;
        [SerializeField] private Image[] scrollbars = null;

        private bool isTeam = false;

        public bool IsChatting { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            if (chatInput == null || chatOutput == null)
            {
                yield break;
            }

            chatInput.richText = false;

            RefreshChat();
            Invoke("RefreshChat", 0.5f);

            DisableChatInput();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void RefreshChat()
        {
            chatOutput.SetActive(false);
            chatOutput.SetActive(true);
        }

        public void EnableChatInput(bool isTeam)
        {
            IsChatting = true;
            this.isTeam = isTeam;

            chatInput.gameObject.SetActive(true);
            chatOutput.SetActive(true);
            chatInput.ActivateInputField();
            chatInput.richText = false;

            for (var i = 0; i < scrollbars.Length; i++)
            {
                scrollbars[i].enabled = true;
            }

            ScramInput.LockAndHideCursor(!IsChatting);
        }

        public void DisableChatInput()
        {
            IsChatting = false;

            chatInput.text = string.Empty;
            chatInput.DeactivateInputField();
            chatInput.gameObject.SetActive(false);

            for (var i = 0; i < scrollbars.Length; i++)
            {
                scrollbars[i].enabled = false;
            }

            ScramInput.LockAndHideCursor(!IsChatting);
        }

        public void ToggleChatOutput()
        {
            chatOutput.SetActive(!chatOutput.activeSelf);
            DisableChatInput();
        }

        public void SendChatMessage()
        {
            if (string.IsNullOrEmpty(chatInput.text))
            {
                DisableChatInput();

                return;
            }

            BoltGlobalEvent.SendChat(chatInput.text, Color.white, isTeam);

            DisableChatInput();
        }

        public void SpawnChatMessage(string chatMessage, Color32 color)
        {
#if ISDEDICATED
            return;
#endif

            var message = Pooler.Instance.Pooliate(messageSnippet.name).GetComponent<TextMeshProUGUI>();
            message.transform.SetParent(content, false);

            message.text = chatMessage;
            message.color = color;

            scrollRect.velocity = new Vector2(0.0f, 1000.0f);
        }
    }
}