using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Hitbox : MonoBehaviour
    {
        public enum HitboxTypeState { Head, Torso, Limb }

        [SerializeField] private HitboxTypeState hitboxType = HitboxTypeState.Limb;

        private IDamageable damageable = null;

        private void Awake()
        {
            damageable = GetComponentInParent<IDamageable>();
        }

        public void DealDamage(int damage, Vector3 direction, string enemyName, PlayerConnection enemyConnection)
        {
            damageable.TakeDamage(damage, direction, enemyName, enemyConnection);
        }

        public int DealDynamicDamage(int damage, Vector3 direction, string enemyName, PlayerConnection enemyConnection)
        {
            damage = GetDamage(damage);

            damageable.TakeDamage(damage, direction, enemyName, enemyConnection);

            return damage;
        }

        private int GetDamage(int damage)
        {
            switch (hitboxType)
            {
                case HitboxTypeState.Head:
                    {
                        return damage * 2;
                    }
                case HitboxTypeState.Torso:
                    {
                        return damage;
                    }
                default:
                    {
                        return damage / 2;
                    }
            }
        }
    }
}