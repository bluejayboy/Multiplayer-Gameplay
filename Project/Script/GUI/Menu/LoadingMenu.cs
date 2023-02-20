using System.Collections.Generic;
using UnityEngine;
using MEC;
using TMPro;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class LoadingMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI loading = null;
        [SerializeField] private float loopRate = 0.5f;
        [SerializeField] private string[] displays = null;

        private void OnEnable()
        {
#if ISDEDICATED
                return;
#endif

            Timing.RunCoroutine(Loop());
        }

        public IEnumerator<float> Loop()
        {
            var index = 0;

            while (true)
            {
                if (index < displays.Length - 1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }

                loading.text = displays[index];

                yield return Timing.WaitForSeconds(loopRate);

                if (this == null)
                {
                    yield break;
                }
            }
        }
    }
}