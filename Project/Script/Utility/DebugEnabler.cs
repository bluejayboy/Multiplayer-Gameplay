using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class DebugEnabler : MonoBehaviour
    {
        [SerializeField] private GameObject[] debugTools = null;

        private void Start()
        {
            if (!DebugMode.IsDebug)
            {
                return;
            }

            for (var i = 0; i < debugTools.Length; i++)
            {
                debugTools[i].SetActive(true);
            }
        }
    }
}