using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RescueRadio : EntityBehaviour<IRescueRadioState>, IHoldInteractable
    {
        [SerializeField] private string display = string.Empty;

        [SerializeField] private int bread = 100;
        [SerializeField] private int score = 2;

        [SerializeField] private TeamData teamData = null;
        [SerializeField] private AudioClip startIntereact = null;
        [SerializeField] private AudioClip endInteract = null;

        [SerializeField] private float holdSeconds = 10.0f;
        public float HoldSeconds { get { return holdSeconds; } }

        [SerializeField] private EvacuationGameMode evac = null;
        [SerializeField] private GameObject sparkle = null;

        private Transform cachedTransform = null;
        private AudioSource audioSource = null;
        private float holdTimer = 0.0f;

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
            cachedTransform = transform;
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (HostGameAction.Instance != null)
            {
                HostGameAction.Instance.OnRoundEnd += Uninteract;
                HostGameAction.Instance.OnRoundEnd += ResetRadio;
            }
        }

        public override void Attached()
        {
            state.AddCallback("Position", ApplyPosition);
            state.AddCallback("Rotation", ApplyRotation);
            state.AddCallback("HasInteracted", OnInteract);
            state.OnStartInteract += () => audioSource.PlayAudioClip(startIntereact);
            state.OnEndInteract += () => audioSource.PlayAudioClip(endInteract);
        }

        private void ApplyPosition()
        {
            cachedTransform.localPosition = state.Position;
        }

        private void ApplyRotation()
        {
            cachedTransform.localRotation = state.Rotation;
        }

        public void Interact(Player player)
        {
            if (Game.Instance.MatchStatus != Game.MatchState.MidRound && Game.Instance.MatchStatus != Game.MatchState.PreEnd)
            {
                Uninteract();
                ResetRadio();

                return;
            }

            if (!entity.IsAttached || !player.entity.IsAttached || state.HasInteracted || player.state.Team != teamData.ID)
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

            if (ActivePlayer.entity.IsAttached)
            {
                ActivePlayer.state.InteractTimer = holdTimer / holdSeconds;
            }

            if (holdTimer >= holdSeconds)
            {
                state.HasInteracted = true;
                state.EndInteract();

                if (ActivePlayer.entity.IsAttached && ActivePlayer.entity.Controller.GetPlayerConnection() != null)
                {
                    var playerInfoState = ActivePlayer.entity.Controller.GetPlayerConnection().PlayerInfo.state;

                    playerInfoState.Score += score;
                    playerInfoState.Bread += bread;

                    BoltGlobalEvent.SendBread(bread, ActivePlayer.entity.Controller);
                }

                evac.StartArrival();
                Uninteract();
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

        private void ResetRadio()
        {
            state.IsInteracting = false;
            state.HasInteracted = false;
            ActivePlayer = null;
            holdTimer = 0.0f;
        }

        private void OnInteract()
        {
            sparkle.SetActive(!state.HasInteracted);
        }
    }
}