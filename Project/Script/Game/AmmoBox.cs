using Photon.Bolt;
using MEC;
using System;
using TMPro;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class AmmoBox : EntityBehaviour<IAmmoBoxState>, IHoldInteractable
    {
        [SerializeField] private string display = string.Empty;

        [SerializeField] public bool disableTeam = false;
        [SerializeField] private TeamData teamData = null;
        [SerializeField] private AudioClip startIntereact = null;
        [SerializeField] private AudioClip endInteract = null;
        [SerializeField] private TextMeshPro timer = null;
        [SerializeField] private float holdSeconds = 2.0f;
        [SerializeField] private int refreshSeconds = 10;
        [SerializeField] private GameObject effect = null;

        public float HoldSeconds { get { return holdSeconds; } }

        private AudioSource audioSource = null;
        private float holdTimer = 0.0f;
        private CoroutineHandle refreshHandle = default(CoroutineHandle);

        public Player ActivePlayer { get; set; }

        public string Display
        {
            get
            {
                if (!state.IsInteracting && !state.HasInteracted)
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
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (HostGameAction.Instance != null)
            {
                HostGameAction.Instance.OnRoundEnd += ResetAmmoBox;
            }
        }

        public override void Attached()
        {
            state.AddCallback("Timer", ApplyTimer);
            state.AddCallback("position", NewPosition);
            state.AddCallback("rotation", NewRotation);
            state.AddCallback("IsInteracting", ToggleEffect);
            state.OnStartInteract += () => audioSource.PlayAudioClip(startIntereact);
            state.OnEndInteract += () => audioSource.PlayAudioClip(endInteract);

            if(BoltNetwork.IsServer){
                state.position = transform.position;
                state.rotation = transform.rotation;
            }
        }

        private void ToggleEffect()
        {
            effect.SetActive(!state.IsInteracting);
        }

        public void Interact(Player player)
        {
            if (!entity.IsAttached || !player.entity.IsAttached || state.HasInteracted || (!disableTeam && player.state.Team != teamData.ID))
            {
                return;
            }

            if (player.state.ActiveGadget.Slot < 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(player.state.OwnedGadgets[player.state.ActiveGadget.Slot].ID) || player.state.OwnedGadgets[player.state.ActiveGadget.Slot].ActiveAmmo >= player.state.OwnedGadgets[player.state.ActiveGadget.Slot].MaxAmmo)
            {
                return;
            }

            state.IsInteracting = true;

            if (ActivePlayer != null && !ActivePlayer.entity.IsAttached)
            {
                Uninteract();
            }

            if (ActivePlayer == null)
            {
                state.StartInteract();
            }

            holdTimer += BoltNetwork.FrameDeltaTime;
            ActivePlayer = player;
            ActivePlayer.state.InteractTimer = holdTimer / holdSeconds;

            if (holdTimer >= holdSeconds)
            {
                state.HasInteracted = true;
                state.EndInteract();

                Uninteract();

                player.state.OwnedGadgets[player.state.ActiveGadget.Slot].ActiveAmmo = player.state.OwnedGadgets[player.state.ActiveGadget.Slot].MaxAmmo;
                StartTimer(1);
            }
        }

        public void Uninteract()
        {
            if (ActivePlayer != null && ActivePlayer.entity.IsAttached)
            {
                ActivePlayer.state.InteractTimer = 0.0f;
            }

            state.IsInteracting = false;
            ActivePlayer = null;
            holdTimer = 0.0f;
        }

        private void ResetAmmoBox()
        {
            state.IsInteracting = false;
            state.HasInteracted = false;
            state.Timer = 0;
            ActivePlayer = null;
            holdTimer = 0.0f;

            Timing.KillCoroutines(refreshHandle);
        }

        private void NewPosition()
        {
            transform.position = state.position;
        }

        private void NewRotation(){
            transform.rotation = state.rotation;
        }

        private void ApplyTimer()
        {
            if (state.Timer == 0)
            {
                return;
            }

            timer.text = (state.Timer > 1) ? state.Timer.ToString("00") : string.Empty;
        }

        private int fullTime;
        private double dateTime = 0;

        private void Update()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            if (state.Timer <= 0.0f)
            {
                return;
            }

            if (state.Timer > 0.0f)
            {
                state.Timer = fullTime - GetTime(dateTime);
            }

            if (state.Timer <= 0.0f)
            {
                state.HasInteracted = false;
            }
        }

        public void StartTimer(int time)
        {
            state.Timer = time;
            fullTime = time;
            dateTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private int GetTime(double dateTime)
        {
            return Mathf.RoundToInt((float)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - dateTime));
        }
    }
}