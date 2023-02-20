using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public class GadgetGuitar : EntityBehaviour<IPlayerState>
    {
        public GuitarGadgetData data;

        private void Update()
        {
            if (!entity.HasControl)
            {
                return;
            }    

            for (int i = 0; i < data.KeyCodes.Length; i++)
            {
                if (Input.GetKey(data.KeyCodes[i]) && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    SendEvent(i);
                }
            }
        }

        private void SendEvent(int index)
        {
            var evnt = GuitarEvent.Create(entity, EntityTargets.Everyone);

            evnt.Note = index;
            evnt.Send();
        }
    }
}