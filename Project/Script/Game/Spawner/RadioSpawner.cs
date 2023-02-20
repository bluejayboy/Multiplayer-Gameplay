using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RadioSpawner : EntityBehaviour<IRescueRadioState>
    {
        public Transform[] spawns = null;

        public override void Attached()
        {
            if (HostGameAction.Instance != null)
            {
                if (GameMode.Instance.Data.HasPreRound)
                {
                    HostGameAction.Instance.OnPreRoundStart += SpawnRadio;
                }
                else
                {
                    HostGameAction.Instance.OnRoundStart += SpawnRadio;
                }
            }
        }

        private void SpawnRadio()
        {
            var spawn = spawns[Random.Range(0, spawns.Length)];

            state.Position = spawn.position;
            state.Rotation = spawn.rotation;
        }
    }
}