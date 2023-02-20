using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour]
    public sealed class TokenRegistry : GlobalEventListener
    {
        public override void BoltStartBegin()
        {
            BoltNetwork.RegisterTokenClass<RoomToken>();
            BoltNetwork.RegisterTokenClass<ConnectToken>();
            BoltNetwork.RegisterTokenClass<RefuseToken>();
            BoltNetwork.RegisterTokenClass<LicenseToken>();
            BoltNetwork.RegisterTokenClass<PlayerToken>();
            BoltNetwork.RegisterTokenClass<TeamToken>();
            BoltNetwork.RegisterTokenClass<PickupToken>();
            BoltNetwork.RegisterTokenClass<TagToken>();
            BoltNetwork.RegisterTokenClass<PlankToken>();
            BoltNetwork.RegisterTokenClass<AmmoPredictToken>();
            BoltNetwork.RegisterTokenClass<TurretToken>();
            BoltNetwork.RegisterTokenClass<RagdollToken>();
        }
    }
}