using UnityEngine;

namespace Scram
{
    public sealed class HeadlessDestroyer : MonoBehaviour
    {
        private void Awake()
        {
#if ISDEDICATED
                         Destroy(gameObject);
#endif
        }
    }
}