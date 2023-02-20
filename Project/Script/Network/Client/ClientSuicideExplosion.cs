using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ClientSuicideExplosion : GlobalEventListener
    {
        [SerializeField] private GameObject explosion;

        public override void OnEvent(SuicideExplosionEvent evnt)
        {
#if ISDEDICATED
                return;
#endif

            Instantiate(explosion, evnt.Position, evnt.Rotation);
        }
    }
}