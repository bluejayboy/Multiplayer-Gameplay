using MEC;
using System;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class GameTimer : MonoBehaviour
    {
        public static GameTimer Instance { get; private set; }

        private CoroutineHandle timerHandle = default(CoroutineHandle);

        private int fullTime;
        private double dateTime = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update()
        {
            if (GameHud.Instance == null || !GameHud.Instance.entity.IsAttached)
            {
                return;
            }

            var state = GameHud.Instance.state;

            if (state.Timer <= 0.0f)
            {
                return;
            }

            if (GameHud.Instance != null && state.Timer >= 0.0f)
            {
                state.Timer = fullTime - GetTime(dateTime);
            }

            if (HostGameAction.Instance.OnTimerEnd != null && state.Timer <= 0.0f)
            {
                HostGameAction.Instance.OnTimerEnd.Invoke();
            }
        }

        public void StartTimer(int time)
        {
            GameHud.Instance.state.Timer = time;
            fullTime = time;
            dateTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private int GetTime(double dateTime)
        {
            return Mathf.RoundToInt((float)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - dateTime));
        }
    }
}