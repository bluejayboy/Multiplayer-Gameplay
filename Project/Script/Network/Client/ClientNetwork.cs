using Photon.Bolt;
using UdpKit;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour]
    public sealed class ClientNetwork : GlobalEventListener
    {
        public enum ShutdownState { None, Refusal}

        public static ShutdownState Shutdown { get; set; }

        public static string RefuseReason { get; set; }

        public override void ConnectRefused(UdpEndPoint endpoint, IProtocolToken token)
        {
            var refuseToken = token as RefuseToken;

            if (refuseToken != null)
            {
                Shutdown = ShutdownState.Refusal;
                RefuseReason = refuseToken.RefuseReason;
            }

            if (ClientConnect.Instance != null)
            {
                ClientConnect.Instance.Display();
            }
        }
    }
}