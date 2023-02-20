using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TurretNetwork : GlobalEventListener
    {
        private Turret turret;

        private void Awake()
        {
            turret = GetComponent<Turret>();
        }

        public override void EntityDetached(BoltEntity entity)
        {
            turret.CheckShooter(entity);
        }
    }
}