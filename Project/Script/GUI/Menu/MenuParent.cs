using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public abstract class MenuParent : MonoBehaviour
    {
        public abstract void Activate();
    }
}