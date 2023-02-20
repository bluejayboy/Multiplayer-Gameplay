using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public class ColorCheck : MonoBehaviour
    {
        public static ColorCheck Instance { get; private set; }

        [SerializeField] private List<Color32> defaultColors = null;
        public List<Color32> DefaultColors { get { return defaultColors; } }

        [SerializeField] private List<Color32> premiumColors = null;
        public List<Color32> PremiumColors { get { return premiumColors; } }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}