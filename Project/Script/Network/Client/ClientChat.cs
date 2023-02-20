using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour]
    public sealed class ClientChat : GlobalEventListener
    {
        public override void OnEvent(MessageEvent evnt)
        {
            if (ChatMenu.Instance != null)
            {
                ChatMenu.Instance.SpawnChatMessage(evnt.Text, evnt.Color);
            }
        }
    }
}