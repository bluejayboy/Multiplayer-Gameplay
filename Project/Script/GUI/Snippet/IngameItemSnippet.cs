using Photon.Bolt;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class IngameItemSnippet : MonoBehaviour
    {
        [SerializeField] private AudioClip purchase = null;
        [SerializeField] private TextMeshProUGUI displayText = null;
        [SerializeField] private Image displayImage = null;
        [SerializeField] private TextMeshProUGUI price = null;

        private IPlayerInfoState activePlayerInfoState = null;
        private Button button = null;

        private string id = null;
        private int breadPrice = 0;

        public void Attach(IPlayerInfoState playerInfoState)
        {
            button = button ?? GetComponent<Button>();

            activePlayerInfoState = playerInfoState;
            activePlayerInfoState.AddCallback("Bread", () => button.interactable = (activePlayerInfoState.Bread >= breadPrice && activePlayerInfoState.Creature != id && activePlayerInfoState.Gadget1 != id && activePlayerInfoState.Gadget2 != id && activePlayerInfoState.Gadget3 != id && activePlayerInfoState.Armor != id));
        }

        public void SetInfo(string id, string displayText, Sprite displayImage, int price)
        {
            this.id = id;
            this.displayText.text = displayText;
            this.displayImage.sprite = displayImage;
            this.price.text = "$" + price;

            breadPrice = price;
        }

        public void Purchase()
        {
            BoltGlobalEvent.SendGameItem(id);
            AudioPlayer.Instance.PlayAudioClip(purchase);
        }
    }
}