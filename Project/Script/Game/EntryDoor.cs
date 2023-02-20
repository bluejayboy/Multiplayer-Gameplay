using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class EntryDoor : EntityBehaviour<IGameState>
    {
        [SerializeField] private GameObject door = null;
        [SerializeField] private GameObject effect = null;

        private ExplosionShake explosionShake = null;

        private void Awake()
        {
            explosionShake = GetComponent<ExplosionShake>();
        }

        public override void Attached()
        {
            state.AddCallback("HasStartedRound", ToggleDoor);

            if (entity.IsOwner)
            {
                HostGameAction.Instance.OnRoundStart += () => state.HasStartedRound = true;
                HostGameAction.Instance.OnRoundEnd += () => state.HasStartedRound = false;
            }
        }

        private void ToggleDoor()
        {
            door.SetActive(!state.HasStartedRound);
            effect.SetActive(state.HasStartedRound);

            if (state.HasStartedRound)
            {
                explosionShake.Shake();
            }
        }
    }
}