using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostSuicide : GlobalEventListener
    {
        public override void OnEvent(SuicideEvent evnt)
        {
            if (!GameMode.Instance.Data.AllowSuicide)
            {
                return;
            }

            if (evnt.RaisedBy.GetPlayerConnection() == null)
            {
                return;
            }

            var player = evnt.RaisedBy.GetPlayerConnection().Player;

            if (player != null)
            {
                player.Die(Vector3.up);
            }
        }
    }
}