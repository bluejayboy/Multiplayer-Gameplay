using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Note : GlobalEventListener
    {
        [SerializeField] private GameObject note = null;

        public override void SceneLoadLocalDone(string map, IProtocolToken token)
        {
            Invoke("Open", 1.0f);
        }

        private void Open()
        {
            IngameShop.Instance.IsViewingShopMenu = true;

            ScramInput.LockAndHideCursor(false);
            note.SetActive(true);
        }

        public void Close()
        {
            IngameShop.Instance.IsViewingShopMenu = false;

            ScramInput.LockAndHideCursor(true);
            note.SetActive(false);
        }
    }
}