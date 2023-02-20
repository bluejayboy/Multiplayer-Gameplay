using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public abstract class GadgetDraw : EntityBehaviour<IPlayerState>, IEnable
    {
        [SerializeField] private GadgetData defaultData;
        [SerializeField] private int bulletImpact;

        private CoroutineHandle handle;

        public virtual void Equip()
        {
            state.BulletImpact = bulletImpact;
            state.ActiveGadget.IsDrawn = false;
            handle = Timing.RunCoroutine(Draw());
        }

        public virtual void Dequip()
        {
            state.ActiveGadget.IsDrawn = false;
            state.AltMoveSpeed = 1;
            Timing.KillCoroutines(handle);
        }

        public override void Detached()
        {
            state.ActiveGadget.IsDrawn = false;
            Timing.KillCoroutines(handle);
        }

        private IEnumerator<float> Draw()
        {
            yield return Timing.WaitForSeconds(0.5f);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            state.ActiveGadget.IsDrawn = true;

            if (defaultData != null)
            {
                state.AltMoveSpeed = defaultData.MoveSpeedMultiplier;
            }
            else
            {
                state.AltMoveSpeed = 1;
            }
        }
    }
}