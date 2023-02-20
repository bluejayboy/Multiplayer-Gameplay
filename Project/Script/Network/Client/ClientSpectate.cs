using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour]
    public sealed class ClientSpectate : GlobalEventListener
    {
        public static ClientSpectate Instance { get; private set; }

        public Player TargetPlayer { get; private set; }
        public int TargetIndex { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            ClientPlayerAction.Instance.OnSpectate += DisableSpectate;
            ClientPlayerAction.Instance.OnSpawn += DisableSpectate;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void OnEvent(SpectateEvent evnt)
        {
            if (evnt.PlayerIndex <= -1)
            {
                ClientPlayerAction.Instance.OnSpectate.Invoke();
                Stage.Instance.StageViewer.SetActive(true);

                return;
            }

            var entity = BoltNetwork.FindEntity(evnt.NetworkID);

            if (entity == null)
            {
                BoltGlobalEvent.SendSpectateRequest(TargetIndex, false);

                return;
            }

            ClientPlayerAction.Instance.OnSpectate.Invoke();

            var state = entity.GetState<IPlayerState>();

            TargetIndex = evnt.PlayerIndex;
            TargetPlayer = entity.GetComponent<Player>();
            TargetPlayer.Spectate(true);
        }

        private void DisableSpectate()
        {
            PlayerHud.Instance.Detach();

            if (TargetPlayer == null)
            {
                return;
            }

            TargetPlayer.Spectate(false);
            TargetIndex = 0;
            TargetPlayer = null;
        }
    }
}