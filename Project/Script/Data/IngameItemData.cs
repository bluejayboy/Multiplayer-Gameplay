using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Ingame Item")]
    public sealed class IngameItemData : ScriptableObject
    {
        public enum TypeState { Creature, Gadget1, Gadget2, Gadget3, Armor }

        [SerializeField] private TypeState type = TypeState.Gadget1;
        public TypeState Type { get { return type; } }

        [SerializeField] private string id = string.Empty;
        public string ID { get { return id; } }

        [SerializeField] private string displayText = string.Empty;
        public string DisplayText { get { return displayText; } }

        [SerializeField] private Sprite displayImage = null;
        public Sprite DisplayImage { get { return displayImage; } }

        [SerializeField] private int price = 0;
        public int Price { get { return DebugMode.IsDebug ? 1 : price; ; } }

        [SerializeField] private string team = string.Empty;
        public string Team { get { return team; } }

        [SerializeField] private string description = string.Empty;
        public string Description { get { return description; } }
    }
}