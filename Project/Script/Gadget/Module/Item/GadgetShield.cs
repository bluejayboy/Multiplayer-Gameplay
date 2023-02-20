using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetShield : GadgetDraw, IEnable
    {
        [SerializeField] private GadgetData data = null;

        public GadgetData Data { get { return data; } }

        public override void Equip()
        {
            base.Equip();

            state.DisableArmLook = true;
        }

        public override void Dequip()
        {
            base.Dequip();

            state.DisableArmLook = false;
        }

        public void PrimaryDown()
        {

        }

        public void PlayPrimaryDown()
        {

        }
    }
}