using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    public sealed class GadgetCannon : GadgetDraw, IPrimaryDown, IHitscan
    {
        [SerializeField] private float force = 1000;
        [SerializeField] private BoltEntity projectile = null;
        [SerializeField] private Transform spawn = null;
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
            if (entity.IsOwner)
            {
                BoltEntity projectile = BoltPooler.Instance.Instantiate(this.projectile.PrefabId, null, spawn.position, spawn.rotation, true);

                projectile.GetComponent<Projectile>().AddForce(spawn.forward * force, state.PenName, entity.Controller.GetPlayerConnection());
            }

            gadgetKick.KickGadget();
            frigidbody.AddForce(new Vector3(-spawn.forward.x * data.ImpactForce, -spawn.forward.y * data.ImpactForce, -spawn.forward.z * data.ImpactForce));
        }
    }
}