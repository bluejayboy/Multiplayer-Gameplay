using MEC;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Animancer;

namespace Scram
{
    [DisallowMultipleComponent]
    public  class GadgetMelee : GadgetDraw, IPrimaryDown
    {
        public AnimancerComponent fpAnim;
        public AnimancerComponent tpAnim;
        public AnimationClip attack;

        [SerializeField] private float speed = 1;
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private MeleeGadgetData data = null;
        [SerializeField] private GadgetPhysics gadgetPhysics = null;

        private float timer = 0.0f;

        public GadgetData Data { get { return data; } }

        public void PrimaryDown()
        {
            if (!state.ActiveGadget.IsDrawn)
            {
                return;
            }

            if (timer > BoltNetwork.ServerFrame)
            {
                return;
            }

            timer = data.PrimaryFireRate + BoltNetwork.ServerFrame;
            state.ActiveGadget.PrimaryDown();

            Timing.RunCoroutine(DealImpact());
        }

        public void PlayPrimaryDown()
        {
            AnimancerState animState;

            if (entity.HasControl)
            {
                animState = fpAnim.Play(attack);
            }
            else
            {
                animState = tpAnim.Play(attack);
            }

            animState.Speed = speed;
            animState.Events.OnEnd = OnActionEnd;
            audioSource.PlayAudioClip(data.PrimaryFire);
        }

        private void OnActionEnd()
        {
            fpAnim.Stop();
            tpAnim.Stop();
        }

        private IEnumerator<float> DealImpact()
        {
            yield return Timing.WaitForSeconds(data.ImpactDelay);

            if (this == null || !entity.IsAttached || !gameObject.activeSelf)
            {
                yield break;
            }

            //var knife = transform.GetComponentInChildren<BzKovSoft.ObjectSlicer.Samples.BzKnife>();
            //knife.BeginNewSlice();
            gadgetPhysics.OverlapSphere(data.Damage, data.ImpactRadius, data.ImpactForce);
        }
    }
}