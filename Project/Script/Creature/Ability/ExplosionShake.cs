using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ExplosionShake : MonoBehaviour
    {
        [SerializeField] private float explosionRadius = 20.0f;
        [SerializeField] private LayerMask playerLayerMask = default(LayerMask);

        private Transform cachedTransform = null;

        private void Awake()
        {
            cachedTransform = transform;
        }

        public void Shake()
        {
            Collider[] hits = Physics.OverlapSphere(cachedTransform.position, explosionRadius, playerLayerMask);

            for (var i = 0; i < hits.Length; i++)
            {
                var player = hits[i].GetComponent<Player>();

                if (player != null)
                {
                    player.state.Effect.ShakeScreen();
                }
            }
        }
    }
}