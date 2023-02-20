using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Health Pack Gadget")]
    public sealed class PeanutBagGadgetData : GadgetData
    {
        [SerializeField] private float healTime = 3.0f;
        public float HealTime { get { return healTime; } }

        [SerializeField] private Pose healPose = default(Pose);
        public Pose HealPose { get { return healPose; } }
    }
}