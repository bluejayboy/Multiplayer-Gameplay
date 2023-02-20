using Photon.Bolt;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scram
{
    [DisallowMultipleComponent]
    public class Projectile : EntityBehaviour<IProjectileState>
    {
        [SerializeField] private bool enableWallbang;
        [SerializeField] private bool enableExplodeOnPlayer;
        [SerializeField] private GameObject projectile;
        [SerializeField] private GameObject explosion;
        [SerializeField] private int selfDamage = 30;
        [SerializeField] private int impactDamage = 50;
        [SerializeField] private int explosionDamage = 50;
        [SerializeField] private float radius = 5.0f;
        [SerializeField] private float impactForce = 20.0f;
        [SerializeField] private AudioClip explosionSound = null;
        [SerializeField] private LayerMask playerLayerMask = default(LayerMask);
        [SerializeField] private LayerMask wallLayerMask = default(LayerMask);

        protected Rigidbody newRigidbody;
        protected CoroutineHandle handle;

        private AudioSource audioSource;
        private string enemyName;
        private PlayerConnection thrower;
        private ExplosionShake shake;

        private bool hasDoneDamage;

        private void Awake()
        {
            newRigidbody = GetComponent<Rigidbody>();
            audioSource = GetComponent<AudioSource>();
            shake = GetComponent<ExplosionShake>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (BoltNetwork.IsSinglePlayer)
            {
                return;
            }

            if (!enableExplodeOnPlayer || !entity.IsAttached || !entity.IsOwner)
            {
                return;
            }

            var player = other.GetComponent<Player>();

            if (player == null || player.entity == null || player.state == null || !player.entity.IsAttached)
            {
                return;
            }

            if (player == thrower.Player || player.state.Team == thrower.Player.state.Team)
            {
                return;
            }

            if (thrower.Player != null)
            {
                thrower.Player.state.ActiveGadget.Hit();
            }

            player.TakeDamage(impactDamage, transform.forward, enemyName, thrower);
            ExplodeDamage();
            state.HasExploded = true;

            Timing.KillCoroutines(handle);
        }

        public override void Attached()
        {
            projectile.SetActive(true);
            explosion.SetActive(false);

            state.SetTransforms(state.Transform, transform);
            state.AddCallback("HasExploded", ExplodeEffect);

            if (entity.IsOwner)
            {
                state.HasExploded = false;
                hasDoneDamage = false;
                newRigidbody.isKinematic = false;
            }
        }

        public override void Detached()
        {
            if (!entity.IsOwner)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }

            state.RemoveAllCallbacks();
        }

        public virtual void AddForce(Vector3 force, string enemyName, PlayerConnection enemyConnection)
        {
            this.enemyName = enemyName;
            this.thrower = enemyConnection;

            newRigidbody.AddForce(force);
            newRigidbody.AddTorque(Vector3.one * Random.Range(200, 600));

            BoltPooler.Instance.Destroy(entity, 5);
        }

        private void ExplodeEffect()
        {
            if (state.HasExploded)
            {
                projectile.SetActive(false);
                explosion.SetActive(true);
                audioSource.PlayAudioClip(explosionSound);
            }
            else
            {
                projectile.SetActive(true);
                explosion.SetActive(false);
            }
        }

        protected void ExplodeDamage()
        {
            if (hasDoneDamage)
            {
                return;
            }

            hasDoneDamage = true;
            newRigidbody.isKinematic = true;

            if (BoltNetwork.IsSinglePlayer)
            {
                return;
            }

            var hits = new Collider[20];
            var count = Physics.OverlapSphereNonAlloc(transform.position, radius, hits, playerLayerMask);

            for (var i = 0; i < count; i++)
            {
                var hit = default(RaycastHit);

                if (hits[i] == null)
                {
                    return;
                }

                var player = hits[i].GetComponent<Player>();

                if (player == null)
                {
                    continue;
                }

                if (Physics.Linecast(transform.position, hits[i].transform.position, out hit, wallLayerMask))
                {
                    if (enableWallbang)
                    {
                        if (hit.collider != hits[i] && player != null && thrower.Player != null && player == thrower.Player)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (hit.collider != hits[i])
                        {
                            continue;
                        }
                    }
                }

                var damageable = hits[i].GetComponent<IDamageable>();
                var frigidbody = hits[i].GetComponent<Frigidbody>();
                var direction = hits[i].transform.position - transform.position;

                if (thrower != null && player != null && thrower.Player != null && player == thrower.Player)
                {
                    player.TakeDamage(selfDamage, transform.forward, "a grenade", thrower);
                }
                else if (player != null && player.state.Team == "Humans")
                {
                    continue;
                }
                else
                {
                    if (thrower != null && thrower.Player != null)
                    {
                        thrower.Player.state.ActiveGadget.Hit();
                    }

                    if (damageable != null)
                    {
                        damageable.TakeDamage(explosionDamage, transform.forward, enemyName, thrower);
                    }
                }

                if (frigidbody != null)
                {
                    frigidbody.AddDirectForce(new Vector3(impactForce * direction.x, impactForce * direction.y, impactForce * direction.z));
                }
            }

            if (shake != null)
            {
                shake.Shake();
            }
        }
    }
}