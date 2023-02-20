using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Grenade Gadget")]
    public sealed class GrenadeGadgetData : GadgetData
    {
        [SerializeField] private Pose chargePose = default(Pose);
        public Pose ChargePose { get { return chargePose; } }

        [SerializeField] private Pose throwPose = default(Pose);
        public Pose ThrowPose { get { return throwPose; } }
    }
}