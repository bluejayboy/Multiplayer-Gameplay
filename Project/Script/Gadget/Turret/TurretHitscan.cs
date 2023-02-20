using Photon.Bolt;
using Hitbox;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TurretHitscan : EntityBehaviour<ITurretState>
    {
        [SerializeField] private int damage = 20;
        [SerializeField] private float range = 500.0f;
        [SerializeField] private float impactForce = 10.0f;
        [SerializeField] private float spread = 0.1f;
        [SerializeField] private float spreadAddend = 0.1f;
        [SerializeField] private float spreadSubtrahend = 0.1f;
        [SerializeField] private float fireRate = 5.0f;
        [SerializeField] private Transform shootPoint;

        private Turret turret;
        private TurretVisual visual;
        private TurretRecoil recoil;
        private TurretRebound rebound;
        private IDamageable myDamageable = null;
        private float timer = 0.0f;
        private float activeSpread = 0.0f;

        private void Awake()
        {
            turret = GetComponent<Turret>();
            visual = GetComponent<TurretVisual>();
            recoil = GetComponentInChildren<TurretRecoil>();
            rebound = GetComponentInChildren<TurretRebound>();
        }

        private void FixedUpdate()
        {
            ApplySpread(BoltNetwork.FrameDeltaTime);
        }

        public void Fire(int serverFrame)
        {
            if (timer > BoltNetwork.ServerFrame)
            {
                return;
            }

            timer = fireRate + BoltNetwork.ServerFrame;
            state.Fire();

            Random.InitState(serverFrame);

            var origin = shootPoint.position;
            var direction = new Vector3(rebound.transform.forward.x + Random.Range(-activeSpread, activeSpread), rebound.transform.forward.y + Random.Range(-activeSpread, activeSpread), rebound.transform.forward.z);
            var ray = new Ray(origin, direction);

            DealImpact(ray, serverFrame);

            activeSpread += spreadAddend;
            activeSpread = Mathf.Clamp(activeSpread, 0.0f, spread);

            if (entity.HasControl)
            {
                recoil.KickRecoil(serverFrame);
            }

            rebound.KickRebound(serverFrame);
        }

        private void DealImpact(Ray ray, int serverFrame)
        {
            if (entity.HasControl)
            {
                visual.SpawnTracer(ray.direction);
            }

            if (entity.IsOwner)
            {
                BoltEntityEvent.SendTracer(ray.direction, entity);
            }

            Raycast(ray, damage, range, impactForce, serverFrame);
        }

        private void ApplySpread(float time)
        {
            activeSpread -= spreadSubtrahend * time;
            activeSpread = Mathf.Clamp(activeSpread, 0.0f, spread);
        }

        public void Raycast(Ray ray, int damage, float range, float force, int serverFrame)
        {
            myDamageable = myDamageable ?? turret.Shooter.GetComponentInParent<IDamageable>();

            using (TimePhysics.RewindFrames(BoltNetwork.ServerFrame - serverFrame))
            {
                var hit = default(RaycastHit);

                if (TimePhysics.Raycast(ray, out hit, range, turret.Shooter.ImpactLayerMask))
                {
                    if (entity.HasControl && !hit.collider.CompareTag("Flesh"))
                    {
                        visual.SpawnImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag);
                    }

                    if (!entity.IsOwner)
                    {
                        return;
                    }

                    var damageable = hit.collider.GetComponentInParent<IDamageable>();

                    if (damageable != null && damageable != myDamageable)
                    {
                        var hitbox = hit.collider.GetComponent<Hitbox>();
                        var finalDamage = damage;

                        if (hitbox != null)
                        {
                            finalDamage = hitbox.DealDynamicDamage(damage, ray.direction, turret.Shooter.state.PenName, entity.Controller.GetPlayerConnection());
                        }
                        else
                        {
                            damageable.TakeDamage(damage, rebound.transform.forward, turret.Shooter.state.PenName, entity.Controller.GetPlayerConnection());
                        }

                        var player = hit.collider.GetComponentInParent<Player>();

                        if (player == null || !player.state.IsInvincible)
                        {
                            turret.Shooter.state.ActiveGadget.Hit();
                            BoltEntityEvent.SendDamageText(finalDamage, hit.point, turret.Shooter.entity);
                        }
                    }
                    else if (hit.collider.attachedRigidbody != null)
                    {
                        hit.collider.attachedRigidbody.velocity = new Vector3(ray.direction.x * force, ray.direction.y * force, ray.direction.z * force);
                    }

                    if (hit.collider.CompareTag("Flesh"))
                    {
                        BoltEntityEvent.SendImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag, entity, EntityTargets.Everyone);
                    }
                    else
                    {
                        BoltEntityEvent.SendImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag, entity, EntityTargets.EveryoneExceptController);
                    }
                }
            }
        }
    }
}