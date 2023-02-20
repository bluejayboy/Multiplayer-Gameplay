using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Ingame Shop")]
    public sealed class IngameShopData : ScriptableObject
    {
        [SerializeField] private IngameItemData[] gameItems;
        public IngameItemData[] GameItems { get { return gameItems; } }
    }
}