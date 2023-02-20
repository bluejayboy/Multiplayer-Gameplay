using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class MenuChanger : MonoBehaviour
    {
        public static MenuChanger Instance { get; private set; }

        [SerializeField] private GameObject[] menus = null;

        private void Awake()
        {
            Instance = this;

            if (string.IsNullOrEmpty(ObscuredPrefs.GetString("PenName")))
            {
                OpenMenu("Setup Menu");
            }
            else
            {
                OpenMenu("Home Menu");
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void OpenMenu(string menuName)
        {
#if ISDEDICATED
                return;
#endif

            for (var i = 0; i < menus.Length; i++)
            {
                if (menus[i].name == menuName)
                {
                    menus[i].SetActive(true);
                }
                else
                {
                    menus[i].SetActive(false);
                }
            }
        }

        public void CloseAllMenus()
        {
#if ISDEDICATED
                return;
#endif

            for (var i = 0; i < menus.Length; i++)
            {
                menus[i].SetActive(false);
            }
        }
    }
}