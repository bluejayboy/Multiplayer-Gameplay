using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerEntityEvent : EntityEventListener<IPlayerState>
    {
        public GuitarGadgetData data;
        public AudioSource guitarAudioSource;

        [SerializeField] private AudioClip[] farts;
        private AudioSource ass;

        private PlayerLoadout playerLoadout = null;
        private PlayerRebound playerRebound = null;
        private GadgetPhysics gadgetPhysics = null;

        private void Awake()
        {
            ass = GetComponent<AudioSource>();
            playerLoadout = GetComponent<PlayerLoadout>();
            playerRebound = GetComponentInChildren<PlayerRebound>(true);
            gadgetPhysics = GetComponentInChildren<GadgetPhysics>(true);
        }

        public override void OnEvent(TracerEvent evnt)
        {
#if ISDEDICATED
                return;
#endif

            if (playerLoadout.ActiveGadget != null && !entity.HasControl)
            {
                playerLoadout.ActiveGadget.ThirdPerson.GetComponent<GadgetVisualEffect>().SpawnTracer(evnt.Direction);
            }
        }

        public override void OnEvent(ImpactEvent evnt)
        {
#if ISDEDICATED
                return;
#endif

            gadgetPhysics.SpawnImpact(evnt.Position, evnt.Rotation, evnt.Tag);
        }

        public override void OnEvent(DamageTextEvent evnt)
        {
#if ISDEDICATED
                return;
#endif

            FloatingText.instance.InitializeScriptableText(0, evnt.Position, evnt.Damage.ToString());
        }

        public override void OnEvent(DamageReceiveEvent evnt)
        {
            evnt.Damage = Mathf.Clamp(evnt.Damage, 1, 50);

            if (playerRebound != null)
            {
                playerRebound.KickRebound(evnt.Damage / 3.0f, evnt.Damage / 3.0f, evnt.Damage / 3.0f, 0.0f);
            }

            if (entity.HasControl)
            {
                PlayerHud.Instance.DisplayDamage();
            }
        }

        public override void OnEvent(Fart evnt)
        {
#if ISDEDICATED
                return;
#endif

            ass.PlayAudioClip(farts[Random.Range(0, farts.Length)]);
        }

        public override void OnEvent(GuitarEvent evnt)
        {
            guitarAudioSource.PlayAudioClip(data.Notes[evnt.Note]);
        }
    }
}