using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GameHud : EntityBehaviour<IGameState>
    {
        public static GameHud Instance { get; private set; }

        [SerializeField] private GameObject gui = null;
        [SerializeField] private AudioClip beep = null;
        [SerializeField] private TextMeshProUGUI timer = null;
        [SerializeField] private TextMeshProUGUI status = null;
        [SerializeField] private TextMeshProUGUI objective = null;
        [SerializeField] private TextMeshProUGUI bread = null;
        [SerializeField] private TextMeshProUGUI cue = null;
        [SerializeField] private TextMeshProUGUI playerCount = null;

        private CoroutineHandle objectiveHandle = default(CoroutineHandle);
        private CoroutineHandle breadHandle = default(CoroutineHandle);

        private int activeBread = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void Attached()
        {
#if ISDEDICATED
                return;
#endif

            state.AddCallback("Timer", ApplyTimer);
            state.AddCallback("Status", ApplyStatus);
            state.AddCallback("PlayerCount", ApplyPlayerCount);
        }

        public void HideGUI()
        {
            gui.SetActive(!gui.activeSelf);
        }

        private void ApplyPlayerCount()
        {
            playerCount.text = "Players: " + state.PlayerCount;
        }

        private void ApplyTimer()
        {
#if ISDEDICATED
                return;
#endif

            if (state.Timer > 0)
            {
                timer.text = (state.Timer / 60).ToString("00:") + (state.Timer % 60).ToString("00");
            }
            else
            {
                timer.text = string.Empty;
            }

            if (state.Timer >= 0 && state.Timer <= 10)
            {
                timer.color = Color.red;

                if (AudioPlayer.Instance != null)
                {
                    AudioPlayer.Instance.PlayAudioClip(beep);
                }
            }
            else
            {
                timer.color = Color.white;
            }
        }

        private void ApplyStatus()
        {
#if ISDEDICATED
                return;
#endif

            status.text = state.Status;
        }

        public void SetBread(int amount)
        {
#if ISDEDICATED
                return;
#endif

            activeBread += amount;
            bread.text = "+ $" + activeBread;
            bread.CrossFadeAlpha(1.0f, 0.0f, true);
            bread.CrossFadeAlpha(0.0f, 3.0f, false);

            Timing.KillCoroutines(breadHandle);
            breadHandle = Timing.RunCoroutine(FadeBread());
        }

        public void SetCue(string text, Color32 color)
        {
#if ISDEDICATED
                return;
#endif

            cue.text = text;
            cue.color = color;
        }

        public void SetObjective(string text, string audioClip, Color32 color)
        {
#if ISDEDICATED
                return;
#endif

            Timing.KillCoroutines(objectiveHandle);
            objectiveHandle = Timing.RunCoroutine(SetNewObjective(text, audioClip, color).CancelWith(gameObject));
        }

        private IEnumerator<float> FadeBread()
        {
            yield return Timing.WaitForSeconds(3.0f);

            if (this == null)
            {
                yield break;
            }

            activeBread = 0;
        }

        private IEnumerator<float> SetNewObjective(string text, string audioClip, Color32 color)
        {
#if ISDEDICATED
                yield break;
#endif

            objective.text = text;

            if (string.IsNullOrEmpty(objective.text))
            {
                yield break;
            }

            AudioPlayer.Instance.PlayAudioClip(audioClip);

            yield return Timing.WaitForSeconds(10.0f);

            if (this == null)
            {
                yield break;
            }

            objective.text = string.Empty;
        }
    }
}