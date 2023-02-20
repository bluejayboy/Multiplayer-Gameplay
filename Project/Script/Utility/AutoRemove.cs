using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Scram
{
    [DisallowMultipleComponent]
    public abstract class AutoRemove : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.2f;
        [SerializeField] protected bool removeParent = false;

        protected CoroutineHandle handle = default(CoroutineHandle);

        protected abstract void Remove();

        private void OnEnable()
        {
            handle = Timing.RunCoroutine(StartRemove());
        }

        private void OnDisable()
        {
            Timing.KillCoroutines(handle);

            gameObject.SetActive(false);
        }

        private IEnumerator<float> StartRemove()
        {
            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= lifetime)
                {
                    return true;
                }

                return false;
            });

            if (this == null)
            {
                yield break;
            }

            Remove();
        }
    }
}