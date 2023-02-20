using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GrenadeProjectile : Projectile
    {
        [SerializeField] private float fuseTime = 3.0f;

        public override void AddForce(Vector3 force, string enemyName, PlayerConnection enemyConnection)
        {
            base.AddForce(force, enemyName, enemyConnection);

            Timing.KillCoroutines(handle);
            handle = Timing.RunCoroutine(Fuse(fuseTime));
        }

        private IEnumerator<float> Fuse(float fuseTime)
        {
            yield return Timing.WaitForSeconds(fuseTime);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            ExplodeDamage();
            state.HasExploded = true;
        }
    }
}