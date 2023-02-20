using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class HomeMenu : MonoBehaviour
    {
        [SerializeField] private GameObject prompt = null;

        private void Awake()
        {
            if (!ObscuredPrefs.GetBool("HasDiscordNew"))
            {
                ObscuredPrefs.SetBool("HasDiscordNew", true);

                prompt.SetActive(true);
            }
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}