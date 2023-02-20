using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class BackpackPage : MonoBehaviour
    {
        public static BackpackPage Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        [SerializeField] private Customization customization = null;

        public void SelectGarb(BackpackSnippet backpackSnippet)
        {
            var bodyArea = backpackSnippet.BodyArea.ToString() + " Garb";

            if (backpackSnippet.BodyArea.ToString() == "RealFace")
            {
                bodyArea = "Face";
            }

            if (backpackSnippet.BodyArea.ToString() == "Voice")
            {
                if (ObscuredPrefs.GetFloat("Voice Pitch") != backpackSnippet.VoicePitch)
                {
                    ObscuredPrefs.SetFloat("Voice Pitch", backpackSnippet.VoicePitch);
                }
                else
                {
                    ObscuredPrefs.SetFloat("Voice Pitch", 1.5f);
                }
            }

            if (ObscuredPrefs.GetString(bodyArea) == backpackSnippet.ID)
            {
                ObscuredPrefs.SetString(bodyArea, string.Empty);
            }
            else
            {
                ObscuredPrefs.SetString(bodyArea, backpackSnippet.ID);
            }

            customization.ApplyCustomization();
        }
    }
}