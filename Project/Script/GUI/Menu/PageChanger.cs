using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PageChanger : MonoBehaviour
    {
        [SerializeField] private GameObject[] pages = null;

        private void OnEnable()
        {
            CloseAllPages();
            pages[0].SetActive(true);
        }

        public void OpenPage(string pageName)
        {
            for (var i = 0; i < pages.Length; i++)
            {
                if (pages[i].name == pageName)
                {
                    pages[i].SetActive(true);
                }
                else
                {
                    pages[i].SetActive(false);
                }
            }
        }

        public void CloseAllPages()
        {
            for (var i = 0; i < pages.Length; i++)
            {
                pages[i].SetActive(false);
            }
        }
    }
}