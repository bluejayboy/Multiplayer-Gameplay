using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour]
    public sealed class GameInput : MonoBehaviour
    {
        private void Update()
        {
            ApplyInput();
        }

        private void ApplyInput()
        {
            Pause();

            if (PauseMenu.Instance != null && PauseMenu.Instance.IsPaused)
            {
                return;
            }

            if (LeaderboardMenu.Instance != null)
            {
                if (Input.GetKeyUp(InputCode.Scoreboard))
                {
                    LeaderboardMenu.Instance.ShowLeaderboard(false);
                }
            }

            Chat();

            if (ChatMenu.Instance != null && ChatMenu.Instance.IsChatting)
            {
                return;
            }

            if (LeaderboardMenu.Instance != null)
            {
                if (Input.GetKeyDown(InputCode.Scoreboard))
                {
                    LeaderboardMenu.Instance.ShowLeaderboard(true);
                }
            }

            RequestSpectate();

            if ((LeaderboardMenu.Instance != null && LeaderboardMenu.Instance.IsViewingLeaderboard) || (IngameShop.Instance != null && IngameShop.Instance.IsViewingShopMenu))
            {
                return;
            }

            LockCursor();
        }

        private void Pause()
        {
            if (PauseMenu.Instance == null || ChatMenu.Instance == null)
            {
                return;
            }

            if (Input.GetKeyDown(InputCode.Pause))
            {
                if (!ChatMenu.Instance.IsChatting)
                {
                    PauseMenu.Instance.Pause();
                }
                else
                {
                    ChatMenu.Instance.DisableChatInput();
                }
            }
        }

        private void Chat()
        {
            if (ChatMenu.Instance == null)
            {
                return;
            }

            if (!ChatMenu.Instance.IsChatting)
            {
                if (Input.GetKeyDown(InputCode.Chat))
                {
                    ChatMenu.Instance.EnableChatInput(false);
                }
                else if (Input.GetKeyDown(InputCode.TeamChat))
                {
                    ChatMenu.Instance.EnableChatInput(true);
                }

                if (Input.GetKeyDown(KeyCode.Home))
                {
                    GameHud.Instance.HideGUI();
                }
            }
            else if (ChatMenu.Instance.IsChatting && Input.GetKeyDown(InputCode.Confirm))
            {
                ChatMenu.Instance.SendChatMessage();
            }
        }

        private void LockCursor()
        {
            if (Input.anyKeyDown)
            {
                ScramInput.LockAndHideCursor(true);
            }
        }

        private void RequestSpectate()
        {
            if (ClientSpectate.Instance != null && ClientSpectate.Instance.TargetPlayer != null && Input.GetKeyDown(InputCode.Jump))
            {
                BoltGlobalEvent.SendSpectateRequest(ClientSpectate.Instance.TargetIndex, false);
            }
        }
    }
}