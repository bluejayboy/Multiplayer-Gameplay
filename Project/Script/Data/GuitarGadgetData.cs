using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Guitar Gadget")]
    public class GuitarGadgetData : GadgetData
    {
        public KeyCode[] KeyCodes;
        public AudioClip[] Notes;
    }
}