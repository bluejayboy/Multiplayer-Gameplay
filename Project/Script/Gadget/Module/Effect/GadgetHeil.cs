using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetHeil : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private GameObject heil = null;
        [SerializeField] private GameObject gadget = null;
        [SerializeField] private Animation animator = null;

        public override void Attached()
        {
            //animator.Play("Idle");
            heil.SetActive(false);
            gadget.SetActive(true);
        }

        public override void Detached()
        {
            //animator.Play("Idle");
            heil.SetActive(false);
            gadget.SetActive(true);
        }

        public IEnumerator<float> Heil()
        {
            if (entity.IsControllerOrOwner)
            {
                state.ActiveGadget.IsSafe = true;
            }

            //gadget.SetActive(false);
            //heil.SetActive(true);
            //animator.Play("Heil");

            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= 2.0f)
                {
                    return true;
                }

                return false;
            });

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            //animator.Play("Idle");

            double currentTime2 = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime2) >= 0.5)
                {
                    return true;
                }

                return false;
            });

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            //heil.SetActive(false);
            //gadget.SetActive(true);

            if (entity.IsControllerOrOwner)
            {
                state.ActiveGadget.IsSafe = false;
            }
        }
    }
}