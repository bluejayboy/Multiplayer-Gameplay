using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class JihadiAbility : Ability
    {
        [SerializeField] private int explosionDamage = 50;
        [SerializeField] private float explosionRadius = 10.0f;
        [SerializeField] private float impactForce = 20.0f;
        [SerializeField] private LayerMask playerLayerMask = default(LayerMask);
        [SerializeField] private LayerMask wallLayerMask = default(LayerMask);

        private Transform cachedTransform = null;
        private ExplosionShake shake;
        private Player player;

        protected override void Awake()
        {
            base.Awake();

            cachedTransform = transform;
            shake = GetComponent<ExplosionShake>();
            player = GetComponentInParent<Player>();
        }

        public override void Attached()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            base.Attached();

            if (entity.IsOwner)
            {
                state.AddCallback("OwnedGadgets[].ActiveAmmo", CheckEmpty);
            }
        }

        public override void ApplyAbility()
        {
            var hits = new Collider[20];
            var count = Physics.OverlapSphereNonAlloc(cachedTransform.position, explosionRadius, hits, playerLayerMask);

            for (var i = 0; i < count; i++)
            {
                var hit = default(RaycastHit);

                if (Physics.Linecast(cachedTransform.position, hits[i].transform.position, out hit, wallLayerMask))
                {
                    if (hit.collider != hits[i])
                    {
                        continue;
                    }
                }

                var damageable = hits[i].GetComponent<IDamageable>();
                /*var plank = hits[i].GetComponentInParent<Plank>();

                if (plank != null)
                {
                    damageable = plank.GetComponent<IDamageable>();
                }*/

                var frigidbody = hits[i].GetComponent<Frigidbody>();
                var direction = hits[i].transform.position - cachedTransform.position;

                damageable.TakeDamage(explosionDamage, cachedTransform.forward, state.PenName, entity.Controller.GetPlayerConnection());
                frigidbody.AddDirectForce(new Vector3(impactForce * direction.x, impactForce * direction.y, impactForce * direction.z));
            }

            BoltGlobalEvent.SendSuicideExplosion(transform.position, transform.rotation);
            shake.Shake();
            player.Die(Vector3.up, "a suicide explosion");
        }

        private void CheckEmpty()
        {
            if (state.OwnedGadgets[0].ActiveAmmo <= 0)
            {
                ApplyAbility();
            }
        }
    }
}