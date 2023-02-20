using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Melee Gadget")]
    public class MeleeGadgetData : GadgetData
    {
        [SerializeField] private float impactDelay = 0.5f;
        public float ImpactDelay { get { return impactDelay; } }
    }
}