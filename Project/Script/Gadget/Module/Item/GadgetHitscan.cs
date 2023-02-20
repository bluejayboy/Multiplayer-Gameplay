using Photon.Bolt;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    public sealed class GadgetHitscan : GadgetDraw, IPrimaryDown, IHitscan, IEnable
    {
        [SerializeField] private Transform playerRebound = null;
        [SerializeField] private ThirdPersonGadgetController thirdPersonGadget = null;
        [SerializeField] private GadgetVisualEffect thirdPersonGadgetVisual = null;
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private Frigidbody frigidbody = null;
        [SerializeField] private HitscanGadgetData data = null;
        [SerializeField] private GadgetKick gadgetKick = null;
        [SerializeField] private GadgetVisualEffect gadgetVisual = null;
        [SerializeField] private GadgetPhysics gadgetPhysics = null;
        private List<int> clientQueue = new List<int>();

        private float timer = 0.0f;
        private float activeSpread = 0.0f;

        public GadgetData Data { get { return data; } }

        public override void Equip()
        {
            base.Equip();

            if (entity.IsAttached && entity.HasControl && PlayerHud.Instance.Crosshair != null)
            {
                PlayerHud.Instance.Crosshair.enabled = data.EnableCrosshair;
            }
        }

        public override void Dequip()
        {
            base.Dequip();

            if (entity.IsAttached && entity.HasControl && PlayerHud.Instance.Crosshair != null)
            {
                PlayerHud.Instance.Crosshair.enabled = true;
            }
        }

        private void FixedUpdate()
        {
            ApplySpread(BoltNetwork.FrameDeltaTime);
        }

        private void Update()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            if (!entity.IsOwner)
            {
                for (int i = 0; i < clientQueue.Count; i++)
                {
                    if (clientQueue[i] <= (state.OwnedGadgets[data.Slot].ServerToken as AmmoPredictToken).frame)
                    {
                        clientQueue.RemoveAt(i);
                        i--;
                    }
                }

                if (state.OwnedGadgets[data.Slot].ServerToken != null)
                {
                    state.OwnedGadgets[data.Slot].PredictedAmmo = (state.OwnedGadgets[data.Slot].ServerToken as AmmoPredictToken).ammo - clientQueue.Count;
                }
            }
        }

        public void PrimaryDown()
        {
            if (!state.ActiveGadget.IsDrawn)
            {
                return;
            }

            if (state.ActiveGadget.IsSlapping || state.ActiveGadget.IsSafe)
            {
                return;
            }

            if (!state.unlimitedAmmo && !data.HasInfiniteAmmo && state.OwnedGadgets[data.Slot].ActiveAmmo <= 0 && entity.IsOwner || !state.unlimitedAmmo && !data.HasInfiniteAmmo && state.OwnedGadgets[data.Slot].PredictedAmmo <= 0 && !entity.IsOwner)
            {
                state.ActiveGadget.DryFire();

                return;
            }

            if (timer > BoltNetwork.ServerFrame)
            {
                return;
            }

            timer = data.PrimaryFireRate + BoltNetwork.ServerFrame;
            state.ActiveGadget.PrimaryDown();

            Fire();

            if (!entity.IsOwner && !data.HasInfiniteAmmo && !state.unlimitedAmmo)
            {
                clientQueue.Add(BoltNetwork.ServerFrame);
            }

            if (!entity.IsOwner || data.HasInfiniteAmmo || state.unlimitedAmmo)
            {
                return;
            }

            state.OwnedGadgets[data.Slot].ActiveAmmo--;
            state.OwnedGadgets[data.Slot].ServerToken = null;
            state.OwnedGadgets[data.Slot].ServerToken = new AmmoPredictToken(BoltNetwork.ServerFrame, state.OwnedGadgets[data.Slot].ActiveAmmo);

            if (state.OwnedGadgets[data.Slot].ActiveAmmo <= 0)
            {
                state.ActiveGadget.EmptyMag();
            }
        }

        public void PlayPrimaryDown()
        {
            if (entity.HasControl)
            {
                gadgetVisual.FlashMuzzle();
                gadgetVisual.SpawnBulletShell();
            }
            else
            {
                thirdPersonGadgetVisual.FlashMuzzle();
                thirdPersonGadgetVisual.SpawnBulletShell();
            }

            thirdPersonGadget.Recoil();
            audioSource.PlayAudioClip(data.PrimaryFire);
        }

        public void ApplyDryFireEffect()
        {
            audioSource.PlayAudioClip(data.DryFire);
        }

        public void PlayEmptyMagEffect()
        {
            if (data != null && data.HasInfiniteAmmo)
            {
                return;
            }

            if (entity.HasControl && state.OwnedGadgets[data.Slot].PredictedAmmo > 0)
            {
                return;
            }

            if (entity.HasControl)
            {
                gadgetVisual.DropMag();
            }
            else
            {
                thirdPersonGadgetVisual.DropMag();
            }

            audioSource.PlayAudioClip(data.MagUnload);
        }

        private void Fire()
        {
            var fragments = 0;
            var ray = default(Ray);

            for (var i = 0; i < data.Fragments; i++)
            {
                fragments++;
                ray = GetRay(fragments);

                DealImpact(ray);

                activeSpread += data.SpreadAddend;
                activeSpread = Mathf.Clamp(activeSpread, 0.0f, state.ActiveGadget.IsAiming ? data.ScopeSpread : data.Spread);
            }

            var impactForce = data.ImpactForce * fragments;

            gadgetKick.KickGadget();
            frigidbody.AddForce(new Vector3(-ray.direction.x * impactForce, -ray.direction.y * impactForce, -ray.direction.z * impactForce));
        }

        private void DealImpact(Ray ray)
        {
            if (entity.HasControl)
            {
                gadgetVisual.SpawnTracer(ray.direction);
            }

            if (entity.IsOwner)
            {
                BoltEntityEvent.SendTracer(ray.direction, entity);
            }

            if (GameMode.Instance.Data.AllowTeamKill)
            {
                gadgetPhysics.RaycastAll(ray, data.Damage, data.Range, data.ImpactForce);
            }
            else
            {
                gadgetPhysics.Raycast(ray, data.Damage, data.Range, data.ImpactForce);
            }
        }

        private void ApplySpread(float time)
        {
            activeSpread -= data.SpreadSubtrahend * time;
            activeSpread = Mathf.Clamp(activeSpread, 0.0f, state.ActiveGadget.IsAiming ? data.ScopeSpread : data.Spread);
        }

        private Ray GetRay(int fragment)
        {
            Random.InitState(state.ServerFrame * fragment);

            var origin = playerRebound.position;
            var direction = new Vector3(playerRebound.forward.x + Random.Range(-activeSpread, activeSpread), playerRebound.forward.y + Random.Range(-activeSpread, activeSpread), playerRebound.forward.z);

            /*var recoilXOffset = Quaternion.AngleAxis(Random.Range(-activeSpread * 1000, activeSpread * 1000), playerRebound.right);
            var recoilYOffset = Quaternion.AngleAxis(Random.Range(-activeSpread * 1000, activeSpread * 1000), playerRebound.up);

            return new Ray(origin, recoilYOffset * (recoilXOffset * playerRebound.forward));*/

            return new Ray(origin, direction);
        }
    }
}