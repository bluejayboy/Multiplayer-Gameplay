using Photon.Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Plank : EntityBehaviour<IPlankState>, IDamageable
    {
        [SerializeField] private int health = 100;
        [SerializeField] private PlankHammerGadgetData data = null;
        [SerializeField] private Renderer rend;
        [SerializeField] private BoxCollider hitbox = null;
        [SerializeField] private Transform plank = null;
        [SerializeField] private Transform startNail = null;
        [SerializeField] private Transform endNail = null;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public override void Attached()
        {
            var token = entity.AttachToken as PlankToken;

            if (token == null)
            {
                return;
            }

            hitbox.gameObject.layer = LayerMask.NameToLayer("Team A Barrier");
            rb.isKinematic = true;

            state.AddCallback("Health", ApplyHealth);

            plank.localScale = new Vector3(data.Height, data.Width, token.Length);
            startNail.SetPositionAndRotation(token.StartPosition, Quaternion.FromToRotation(Vector3.up, token.Normal));
            endNail.SetPositionAndRotation(token.EndPosition, Quaternion.FromToRotation(Vector3.up, token.Normal));

            if (!entity.IsOwner)
            {
                return;
            }

            state.Health = health;

            for (var i = 0; i < HostPlayerRegistry.Instance.PlayerConnections.Count; i++)
            {
                var player = HostPlayerRegistry.Instance.PlayerConnections[i].Player;

                if (player == null)
                {
                    continue;
                }

                var newEntity = BoltNetwork.FindEntity(token.NetworkID);

                if (!newEntity.IsAttached)
                {
                    continue;
                }

                if (player.state.Team == "Humans" && player.entity != newEntity)
                {
                    player.IgnoreCollider(hitbox, true);
                    BoltGlobalEvent.SendIgnoreCollider(true, player.entity.NetworkId, entity.NetworkId, player.entity.Controller);
                }
            }
        }

        public override void Detached()
        {
            if (!entity.IsOwner)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }
        }

        public void TakeDamage(int damage, Vector3 direction, string enemyName = "", PlayerConnection enemyConnection = null, bool isFall = false)
        {
            if (!entity.IsAttached)
            {
                return;
            }

            if (state.Health <= 0)
            {
                return;
            }

            state.Health -= damage;

            if (enemyConnection != null && enemyConnection != entity.Controller.GetPlayerConnection())
            {
                enemyConnection.PlayerInfo.state.Bread += damage;
                BoltGlobalEvent.SendBread(damage, enemyConnection.BoltConnection);
            }
        }

        private void ApplyHealth()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            Material plankMaterial = rend.material;

            if (state.Health <= 0)
            {
                hitbox.gameObject.layer = LayerMask.NameToLayer("Ignore Collision Ignore Raycast");
                rb.isKinematic = false;
                plankMaterial.color = data.DeadColor;

                if (entity.IsOwner)
                {
                    BoltPooler.Instance.Destroy(entity, 5);
                }
            }
            else
            {
                plankMaterial.color = data.AliveColor;
            }
        }

        public void Die(Vector3 direction, string enemyName = "", PlayerConnection enemyConnection = null)
        {
        }
    }
}