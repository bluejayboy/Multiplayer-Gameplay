using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class AutoDestroy : AutoRemove
    {
        protected override void Remove()
        {
            if (removeParent)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}