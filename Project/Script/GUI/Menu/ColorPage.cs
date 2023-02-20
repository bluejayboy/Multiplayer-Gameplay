using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ColorPage : MonoBehaviour
    {
        public static ColorPage Instance { get; private set; }

        [SerializeField] private Customization customization = null;
        [SerializeField] private TextMeshProUGUI bodyPart = null;
        [SerializeField] private string[] bodyParts = null;
        [SerializeField] private Transform parent = null;
        [SerializeField] private Button colorSnippet = null;
        [SerializeField] private Color32[] defaultColors = null;
        [SerializeField] private Color32[] premiumColors = null;

        private int activeIndex = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            for (int i = 0; i < defaultColors.Length; i++)
            {
                var button = Instantiate(colorSnippet, parent);
                var color = button.colors;

                color.normalColor = defaultColors[i];
                button.colors = color;
            }

            for (int i = 0; i < premiumColors.Length; i++)
            {
                var button = Instantiate(colorSnippet, parent);
                var color = button.colors;

                color.normalColor = premiumColors[i];
                button.colors = color;
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void SelectColor(Color32 color)
        {
            ObscuredPrefs.SetColor(bodyPart.text + " Color", color);
            customization.ApplyCustomization();
        }

        private void SelectBodyPart()
        {
            bodyPart.text = bodyParts[activeIndex];
        }

        public void SelectLeft()
        {
            if (activeIndex <= 0)
            {
                activeIndex = bodyParts.Length - 1;
            }
            else
            {
                activeIndex--;
            }

            SelectBodyPart();
        }

        public void SelectRight()
        {
            if (activeIndex >= bodyParts.Length - 1)
            {
                activeIndex = 0;
            }
            else
            {
                activeIndex++;
            }

            SelectBodyPart();
        }
    }
}