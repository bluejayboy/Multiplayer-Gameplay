using Photon.Bolt;
using MEC;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scram
{
    [DisallowMultipleComponent]
    public class KnifeProjectile : EntityBehaviour<IPickupState>
    {
        [SerializeField] private int impactDamage = 300;

        protected Rigidbody newRigidbody;

        private string enemyName;
        private PlayerConnection thrower;
        private bool canDamage = false;

        private void Awake()
        {
            newRigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (BoltNetwork.IsSinglePlayer)
            {
                return;
            }

            if (!entity.IsAttached || !entity.IsOwner || !canDamage)
            {
                return;
            }

            var player = other.GetComponent<Player>();

            if (player == null || player.entity == null || player.state == null || !player.entity.IsAttached)
            {
                canDamage = false;

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
            canDamage = false;
        }

        public virtual void AddForce(Vector3 force, string enemyName, PlayerConnection enemyConnection)
        {
            canDamage = true;

            this.enemyName = enemyName;
            this.thrower = enemyConnection;

            newRigidbody.AddForce(force);
            newRigidbody.AddTorque(Vector3.one * Random.Range(200, 600));
        }
    }
}