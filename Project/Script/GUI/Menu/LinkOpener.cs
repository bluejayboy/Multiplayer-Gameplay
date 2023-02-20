using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class LinkOpener : MonoBehaviour
    {
        [SerializeField] private string link = string.Empty;
        [SerializeField] private GameObject prompt = null;

        public void OpenLink()
        {
            Application.OpenURL(link);
            ClosePrompt();
        }

        public void ClosePrompt()
        {
            if (prompt != null)
            {
                prompt.SetActive(false);
            }
        }
    }
}