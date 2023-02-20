using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RocketProjectile : Projectile
    {
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (!entity.IsAttached || !entity.IsOwner || other.GetComponent<Player>() != null)
            {
                return;
            }

            ExplodeDamage();
            state.HasExploded = true;
        }
    }
}