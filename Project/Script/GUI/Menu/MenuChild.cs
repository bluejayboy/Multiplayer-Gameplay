using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class MenuChild : MonoBehaviour
    {
        [SerializeField] private MenuParent menu = null;

        private void OnEnable()
        {
            menu.Activate();
        }
    }
}