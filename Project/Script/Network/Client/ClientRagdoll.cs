using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ClientRagdoll : GlobalEventListener
    {
        [SerializeField] private PlayerRagdoll ragdoll;
        [SerializeField] private bool isDissolve = false;

        public override void OnEvent(RagdollEvent evnt)
        {
#if ISDEDICATED
                return;
#endif

            var ragdoll = Instantiate(this.ragdoll, evnt.Position, evnt.Rotation);
            ragdoll.Initialize(evnt.Direction, evnt.Token, evnt.IsDissolve);
        }
    }
}