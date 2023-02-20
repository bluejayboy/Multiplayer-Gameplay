using UnityEngine;

namespace Scram
{
    public static class ScramInput
    {
        public static bool IsPausedOrChatting
        {
            get
            {
                if ((PauseMenu.Instance != null && PauseMenu.Instance.IsPaused) || (PauseMenu.Instance != null && ChatMenu.Instance.IsChatting))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool CursorIsLocked
        {
            get
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static void LockAndHideCursor(bool willLock)
        {
            if (willLock && (Stage.Instance == null || (Stage.Instance != null && !Stage.Instance.IsLoaded) || (PauseMenu.Instance != null && PauseMenu.Instance.IsPaused) || (ChatMenu.Instance != null && ChatMenu.Instance.IsChatting) || (LeaderboardMenu.Instance != null && LeaderboardMenu.Instance.IsViewingLeaderboard) || (IngameShop.Instance != null && IngameShop.Instance.IsViewingShopMenu)))
            {
                return;
            }

            Cursor.lockState = (willLock) ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !willLock;
        }
    }
}