using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Armor : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private int healthAdd = 50;
        [SerializeField] private float moveSpeedMultiplier = 0.8f;

        private void OnEnable()
        {
            if (!entity.IsAttached || !entity.IsOwner)
            {
                return;
            }

            state.MaxHealth += healthAdd;
            state.ActiveHealth = state.MaxHealth;
            state.MainMoveSpeed = moveSpeedMultiplier;
        }
    }
}