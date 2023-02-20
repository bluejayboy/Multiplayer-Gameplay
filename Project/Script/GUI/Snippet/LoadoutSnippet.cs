using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class LoadoutSnippet : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;

        public void ToggleEquip(Color32 color)
        {
            image.color = color;
        }

        public void ApplyName(string text)
        {
            this.text.text = text;
        }
    }
}