using UnityEngine;
using MEC;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class AutoDisable : AutoRemove
    {
        [SerializeField] private bool isStart;

        protected override void Remove()
        {
            if (isStart)
                return;

            if (removeParent)
            {
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}