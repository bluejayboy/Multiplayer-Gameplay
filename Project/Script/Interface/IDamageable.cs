using UnityEngine;

namespace Scram
{
    public interface IDamageable
    {
        void TakeDamage(int damage, Vector3 direction, string enemyName = "", PlayerConnection enemyConnection = null, bool isFall = false);
        void Die(Vector3 direction, string enemyName = "", PlayerConnection enemyConnection = null);
    }
}