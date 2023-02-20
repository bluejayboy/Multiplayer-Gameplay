using UnityEngine;

namespace Photon.Bolt
{
    public static class BoltEntityEvent
    {
        public static void SendTracer(Vector3 direction, BoltEntity entity)
        {
            if (!entity.IsAttached)
            {
                return;
            }

            var evnt = TracerEvent.Create(entity, EntityTargets.EveryoneExceptController);

            evnt.Direction = direction;
            evnt.Send();
        }

        public static void SendImpact(Vector3 position, Quaternion rotation, string tag, BoltEntity entity, EntityTargets target)
        {
            if (!entity.IsAttached)
            {
                return;
            }

            var evnt = ImpactEvent.Create(entity, target);

            evnt.Position = position;
            evnt.Rotation = rotation;
            evnt.Tag = tag;
            evnt.Send();
        }

        public static void SendDamageText(int damage, Vector3 position, BoltEntity entity)
        {
            if (!entity.IsAttached)
            {
                return;
            }

            var evnt = DamageTextEvent.Create(entity, EntityTargets.OnlyController);

            evnt.Damage = damage;
            evnt.Position = position;
            evnt.Send();
        }

        public static void SendDamageReceive(int damage, BoltEntity entity)
        {
            if (!entity.IsAttached)
            {
                return;
            }

            var evnt = DamageReceiveEvent.Create(entity, EntityTargets.OnlyControllerAndOwner);

            evnt.Damage = damage;
            evnt.Send();
        }
    }
}