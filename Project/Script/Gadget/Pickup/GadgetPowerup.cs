using Photon.Bolt;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetPowerup : EntityBehaviour<IInteractState>, IInteractable
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string display = string.Empty;
        [SerializeField] private HitscanGadgetData data = null;
        [SerializeField] private GameObject render = null;
        [SerializeField] private AudioClip grab = null;
        [SerializeField] private float rotateSpeed = 360.0f;
        [SerializeField] private float refreshSeconds = 20.0f;

        private Transform cachedTransform = null;
        private AudioSource audioSource = null;

        public string Display
        {
            get
            {
                if (entity.IsAttached && !state.HasInteracted)
                {
                    return display;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private void Awake()
        {
            cachedTransform = transform;
            audioSource = GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            Rotate(Time.fixedDeltaTime);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (!entity.IsAttached || !entity.IsOwner || state.HasInteracted)
            {
                return;
            }

            Player player = other.GetComponent<Player>();

            if (player == null || player.entity == null || player.state == null || !player.entity.IsAttached)
            {
                return;
            }

            if (!player.state.CanInteract || !string.IsNullOrEmpty(player.state.OwnedGadgets[data.Slot].ID))
            {
                return;
            }

            Interact(player);
        }

        public override void Attached()
        {
            state.AddCallback("HasInteracted", () => render.SetActive(!state.HasInteracted));
            state.OnStartInteract = () => audioSource.PlayOneShot(grab);
        }

        public void Interact(Player player)
        {
            if (!entity.IsAttached || !player.entity.IsAttached || state.HasInteracted)
            {
                return;
            }

            state.HasInteracted = true;
            state.StartInteract();

            player.entity.GetComponent<PlayerLoadout>().PickUpGadget(id, data.Slot, data.MaxAmmo, data.MaxAmmo, true);
            BoltGlobalEvent.SendPickup(id, data.Slot, data.MaxAmmo, data.MaxAmmo, player.entity, true);
            StartTimer((int)refreshSeconds);
        }

        private void Rotate(float time)
        {
            cachedTransform.Rotate(rotateSpeed * Vector3.up * time);
        }

        private IEnumerator<float> Refresh()
        {
            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= refreshSeconds)
                {
                    return true;
                }

                return false;
            });

            if (this == null)
            {
                yield break;
            }

            state.HasInteracted = false;
        }

        private int fullTime;
        private double dateTime = 0;
        private int timer;

        private void Update()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            if (timer <= 0.0f)
            {
                return;
            }

            if (timer >= 0.0f)
            {
                timer = fullTime - GetTime(dateTime);
            }

            if (timer <= 0.0f)
            {
                state.HasInteracted = false;
            }
        }

        public void StartTimer(int time)
        {
            timer = time;
            fullTime = time;
            dateTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private int GetTime(double dateTime)
        {
            return Mathf.RoundToInt((float)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - dateTime));
        }
    }
}