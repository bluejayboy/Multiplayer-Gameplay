using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour]
    public sealed class ClientHud : GlobalEventListener
    {
        public override void OnEvent(ObjectiveEvent evnt)
        {
            if (GameHud.Instance != null)
            {
                GameHud.Instance.SetObjective(evnt.Text, evnt.AudioClip, evnt.Color);
            }
        }

        public override void OnEvent(BreadEvent evnt)
        {
            if (!GameMode.Instance.Data.AllowBread)
            {
                return;
            }

            if (GameHud.Instance != null)
            {
                GameHud.Instance.SetBread(evnt.Bread);
            }
        }

        public override void OnEvent(CueEvent evnt)
        {
            if (GameHud.Instance != null)
            {
                GameHud.Instance.SetCue(evnt.Text, evnt.Color);
            }
        }
    }
}