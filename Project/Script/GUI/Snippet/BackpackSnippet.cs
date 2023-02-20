using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class BackpackSnippet : MonoBehaviour
    {
        public enum BodyAreaState { Head, Face, Neck, Chest, Hip, RealFace, Voice }

        public BodyAreaState BodyArea { get; set; }
        public string ID { get; set; }
        public float VoicePitch { get; set; }

        [SerializeField] private TextMeshProUGUI display;
        public RawImage icon;
        public TextMeshProUGUI Display { get { return display; } }

        public void SelectStuff()
        {
            BackpackPage.Instance.SelectGarb(this);
        }
    }
}