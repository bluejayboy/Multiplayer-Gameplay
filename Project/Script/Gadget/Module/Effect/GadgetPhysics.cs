using Photon.Bolt;
using Hitbox;
using System.Linq;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetPhysics : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private GameObject[] bulletImpacts;
        [SerializeField] private Transform playerRebound = null;

        private Player player = null;
        private Pooler pooler = null;
        private IDamageable myDamageable = null;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            player = player ?? GetComponentInParent<Player>();
            myDamageable = myDamageable ?? GetComponentInParent<IDamageable>();
        }

        private void Start()
        {
#if ISDEDICATED
                return;
#endif
            pooler = Pooler.Instance;
        }

        public void Raycast(Ray ray, int damage, float range, float force)
        {
            Initialize();

            using (TimePhysics.RewindFrames(BoltNetwork.ServerFrame - state.ServerFrame))
            {
                var hit = default(RaycastHit);

                if (TimePhysics.Raycast(ray, out hit, range, player.ImpactLayerMask))
                {
                    if (entity.HasControl && !hit.collider.CompareTag("Flesh"))
                    {
                        SpawnImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag);
                    }

                    if (hit.collider.CompareTag("Shield") || hit.collider.CompareTag("Block"))
                    {
                        if (entity.IsOwner)
                        {
                            BoltEntityEvent.SendImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag, entity, EntityTargets.EveryoneExceptController);
                        }

                        return;
                    }

                    var plank = hit.collider.GetComponentInParent<Plank>();

                    if (plank != null && entity.Controller != plank.entity.Controller && state.Team == "Humans")
                    {
                        if (entity.IsOwner)
                        {
                            BoltEntityEvent.SendImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag, entity, EntityTargets.EveryoneExceptController);
                        }

                        return;
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
                            finalDamage = hitbox.DealDynamicDamage(damage, ray.direction, state.PenName, entity.Controller.GetPlayerConnection());
                        }
                        else
                        {
                            damageable.TakeDamage(damage, ray.direction, state.PenName, entity.Controller.GetPlayerConnection());
                        }

                        var player = hit.collider.GetComponentInParent<Player>();

                        if (player == null || !player.state.IsInvincible)
                        {
                            state.ActiveGadget.Hit();
                            BoltEntityEvent.SendDamageText(finalDamage, hit.point, entity);
                        }
                    }
                    else if (hit.collider.attachedRigidbody != null)
                    {
                        hit.collider.attachedRigidbody.velocity = new Vector3(ray.direction.x * force, ray.direction.y * force, ray.direction.z * force);
                    }

                    if (hit.collider.CompareTag("Flesh"))
                    {
                        float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;
                        Quaternion rot = Quaternion.Euler(0, angle + 90, 0);

                        BoltEntityEvent.SendImpact(hit.point, rot, hit.collider.tag, entity, EntityTargets.Everyone);
                    }
                    else
                    {
                        BoltEntityEvent.SendImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag, entity, EntityTargets.EveryoneExceptController);
                    }
                }
            }
        }

        public void RaycastAll(Ray ray, int damage, float range, float force)
        {
            Initialize();

            using (TimePhysics.RewindFrames(BoltNetwork.ServerFrame - state.ServerFrame))
            {
                RaycastHit[] hits = TimePhysics.RaycastAll(ray, range, player.ImpactLayerMask).OrderBy(hit => hit.distance).ToArray();

                for (var i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.CompareTag("Shield") || hits[i].collider.CompareTag("Block"))
                    {
                        SpawnImpact(hits[i].point, Quaternion.FromToRotation(Vector3.up, hits[i].normal), hits[i].collider.tag);

                        if (entity.IsOwner)
                        {
                            BoltEntityEvent.SendImpact(hits[i].point, Quaternion.FromToRotation(Vector3.up, hits[i].normal), hits[i].collider.tag, entity, EntityTargets.EveryoneExceptController);
                        }

                        return;
                    }

                    var damageable = hits[i].collider.GetComponentInParent<IDamageable>();

                    if (damageable != null && damageable == myDamageable)
                    {
                        continue;
                    }

                    if (entity.HasControl && !hits[i].collider.CompareTag("Flesh"))
                    {
                        SpawnImpact(hits[i].point, Quaternion.FromToRotation(Vector3.up, hits[i].normal), hits[i].collider.tag);
                    }

                    if (!entity.IsOwner)
                    {
                        break;
                    }

                    if (damageable != null && damageable != myDamageable)
                    {
                        var hitbox = hits[i].collider.GetComponent<Hitbox>();
                        var finalDamage = damage;

                        if (hitbox != null)
                        {
                            finalDamage = hitbox.DealDynamicDamage(damage, ray.direction, state.PenName, entity.Controller.GetPlayerConnection());
                        }
                        else
                        {
                            damageable.TakeDamage(damage, playerRebound.forward, state.PenName, entity.Controller.GetPlayerConnection());
                        }

                        var player = hits[i].collider.GetComponentInParent<Player>();

                        if (player == null || !player.state.IsInvincible)
                        {
                            state.ActiveGadget.Hit();
                            BoltEntityEvent.SendDamageText(finalDamage, hits[i].point, entity);
                        }
                    }
                    else if (hits[i].collider.attachedRigidbody != null)
                    {
                        hits[i].collider.attachedRigidbody.velocity = new Vector3(ray.direction.x * force, ray.direction.y * force, ray.direction.z * force);
                    }

                    if (hits[i].collider.CompareTag("Flesh"))
                    {
                        float angle = Mathf.Atan2(hits[i].normal.x, hits[i].normal.z) * Mathf.Rad2Deg + 180;
                        Quaternion rot = Quaternion.Euler(0, angle + 90, 0);

                        BoltEntityEvent.SendImpact(hits[i].point, rot, hits[i].collider.tag, entity, EntityTargets.Everyone);
                    }
                    else
                    {
                        BoltEntityEvent.SendImpact(hits[i].point, Quaternion.FromToRotation(Vector3.up, hits[i].normal), hits[i].collider.tag, entity, EntityTargets.EveryoneExceptController);
                    }

                    break;
                }
            }
        }

        public bool RaycastWithNoDamage(Ray ray, int damage, float range, float force)
        {
            Initialize();

            using (TimePhysics.RewindFrames(BoltNetwork.ServerFrame - state.ServerFrame))
            {
                var hit = default(RaycastHit);

                if (TimePhysics.Raycast(ray, out hit, range, player.ImpactLayerMask))
                {
                    if (hit.collider.CompareTag("Shield") || hit.collider.CompareTag("Block"))
                    {
                        SpawnImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag);

                        if (entity.IsOwner)
                        {
                            BoltEntityEvent.SendImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag, entity, EntityTargets.EveryoneExceptController);
                        }

                        return true;
                    }

                    var plank = hit.collider.GetComponentInParent<Plank>();

                    if (plank != null && entity.Controller != plank.entity.Controller && state.Team == "Humans")
                    {
                        SpawnImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag);

                        if (entity.IsOwner)
                        {
                            BoltEntityEvent.SendImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag, entity, EntityTargets.EveryoneExceptController);
                        }

                        return true;
                    }

                    var damageable = hit.collider.GetComponentInParent<IDamageable>();

                    if (entity.HasControl && !hit.collider.CompareTag("Flesh"))
                    {
                        SpawnImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag);
                    }

                    if (!entity.IsOwner)
                    {
                        return true;
                    }

                    if (damageable != null && damageable != myDamageable)
                    {
                        var player = hit.collider.GetComponentInParent<Player>();

                        if (player == null || !player.state.IsInvincible)
                        {
                            BoltEntityEvent.SendDamageText(damage, hit.point, entity);
                        }
                    }
                    else if (hit.collider.attachedRigidbody != null)
                    {
                        hit.collider.attachedRigidbody.velocity = new Vector3(ray.direction.x * force, ray.direction.y * force, ray.direction.z * force);
                    }

                    if (hit.collider.CompareTag("Flesh"))
                    {
                        float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;
                        Quaternion rot = Quaternion.Euler(0, angle + 90, 0);

                        BoltEntityEvent.SendImpact(hit.point, rot, hit.collider.tag, entity, EntityTargets.Everyone);
                    }
                    else
                    {
                        BoltEntityEvent.SendImpact(hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal), hit.collider.tag, entity, EntityTargets.EveryoneExceptController);
                    }

                    return true;
                }
            }

            return false;
        }

        public bool RaycastAllWithNoDamage(Ray ray, int damage, float range, float force)
        {
            Initialize();

            using (TimePhysics.RewindFrames(BoltNetwork.ServerFrame - state.ServerFrame))
            {
                RaycastHit[] hits = TimePhysics.RaycastAll(ray, range, player.ImpactLayerMask).OrderBy(hit => hit.distance).ToArray();

                for (var i = 0; i < hits.Length; i++)
                {
                    var damageable = hits[i].collider.GetComponentInParent<IDamageable>();

                    if (damageable != null && damageable == myDamageable)
                    {
                        continue;
                    }

                    if (hits[i].collider.CompareTag("Shield") || hits[i].collider.CompareTag("Block"))
                    {
                        SpawnImpact(hits[i].point, Quaternion.FromToRotation(Vector3.up, hits[i].normal), hits[i].collider.tag);

                        if (entity.IsOwner)
                        {
                            BoltEntityEvent.SendImpact(hits[i].point, Quaternion.FromToRotation(Vector3.up, hits[i].normal), hits[i].collider.tag, entity, EntityTargets.EveryoneExceptController);
                        }

                        return true;
                    }

                    if (entity.HasControl && !hits[i].collider.CompareTag("Flesh"))
                    {
                        SpawnImpact(hits[i].point, Quaternion.FromToRotation(Vector3.up, hits[i].normal), hits[i].collider.tag);
                    }

                    if (!entity.IsOwner)
                    {
                        return true;
                    }

                    if (damageable != null && damageable != myDamageable)
                    {
                        var player = hits[i].collider.GetComponentInParent<Player>();

                        if (player == null || !player.state.IsInvincible)
                        {
                            BoltEntityEvent.SendDamageText(damage, hits[i].point, entity);
                        }
                    }
                    else if (hits[i].collider.attachedRigidbody != null)
                    {
                        hits[i].collider.attachedRigidbody.velocity = new Vector3(ray.direction.x * force, ray.direction.y * force, ray.direction.z * force);
                    }

                    if (hits[i].collider.CompareTag("Flesh"))
                    {
                        float angle = Mathf.Atan2(hits[i].normal.x, hits[i].normal.z) * Mathf.Rad2Deg + 180;
                        Quaternion rot = Quaternion.Euler(0, angle + 90, 0);

                        BoltEntityEvent.SendImpact(hits[i].point, rot, hits[i].collider.tag, entity, EntityTargets.Everyone);
                    }
                    else
                    {
                        BoltEntityEvent.SendImpact(hits[i].point, Quaternion.FromToRotation(Vector3.up, hits[i].normal), hits[i].collider.tag, entity, EntityTargets.EveryoneExceptController);
                    }

                    return true;
                }
            }

            return false;
        }

        public void OverlapSphere(int damage, float radius, float force)
        {
            Initialize();

            var ray = new Ray(playerRebound.position, playerRebound.forward);
            var hasHit = false;

            if (GameMode.Instance.Data.AllowTeamKill)
            {
                hasHit = RaycastAllWithNoDamage(ray, damage, radius, force);
            }
            else
            {
                hasHit = RaycastWithNoDamage(ray, damage, radius, force);
            }

            if (!entity.IsOwner)
            {
                return;
            }

            using (TimePhysics.RewindFrames(BoltNetwork.ServerFrame - state.ServerFrame))
            {
                var hits = new Collider[10];
                int count = TimePhysics.OverlapSphereNonAlloc(playerRebound.position, radius, hits, player.ImpactLayerMask);

                for (var i = 0; i < count; i++)
                {
                    if (hits[i].CompareTag("Shield") || hits[i].CompareTag("Block"))
                    {
                        SpawnImpact(hits[i].transform.position, Quaternion.FromToRotation(Vector3.up, hits[i].transform.position), hits[i].gameObject.tag);

                        if (entity.IsOwner)
                        {
                            BoltEntityEvent.SendImpact(hits[i].transform.position, Quaternion.FromToRotation(Vector3.up, hits[i].transform.position), hits[i].gameObject.tag, entity, EntityTargets.EveryoneExceptController);
                        }

                        return;
                    }
                }

                for (var i = 0; i < count; i++)
                {
                    var plank = hits[i].GetComponentInParent<Plank>();

                    if (plank != null && entity.Controller != plank.entity.Controller && state.Team == "Humans")
                    {
                        return;
                    }

                    var damageable = hits[i].GetComponentInParent<IDamageable>();

                    if (damageable == null || damageable == myDamageable)
                    {
                        continue;
                    }

                    var hit = default(RaycastHit);

                    if (Physics.Linecast(playerRebound.position, hits[i].ClosestPoint(playerRebound.position), out hit, this.player.WallLayerMask))
                    {
                        if (hit.collider != hits[i])
                        {
                            continue;
                        }
                    }

                    var hitbox = hits[i].GetComponent<Hitbox>();
                    var player = hits[i].GetComponentInParent<Player>();

                    if (player == null || !player.state.IsInvincible)
                    {
                        state.ActiveGadget.Hit();
                    }

                    if (hitbox != null)
                    {
                        hitbox.DealDamage(damage, playerRebound.forward, state.PenName, entity.Controller.GetPlayerConnection());

                        if (!hasHit)
                        {
                            state.Effect.MeleeHit();
                        }
                    }
                    else
                    {
                        damageable.TakeDamage(damage, playerRebound.forward, state.PenName, entity.Controller.GetPlayerConnection());
                    }

                    break;
                }
            }
        }

        public void SpawnImpact(Vector3 position, Quaternion rotation, string tag)
        {
#if ISDEDICATED
                return;
#endif

            pooler = Pooler.Instance;
            var bulletImpact = pooler.Pooliate(bulletImpacts[state.BulletImpact].name, position, rotation).GetComponent<BulletImpact>();
            bulletImpact.SpawnBulletImpact(tag);
        }
    }
}