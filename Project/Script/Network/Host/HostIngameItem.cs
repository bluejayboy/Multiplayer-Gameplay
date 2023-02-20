using Photon.Bolt;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostIngameItem : GlobalEventListener
    {
        public enum TypeState { Creature, Gadget1, Gadget2, Gadget3, Armor }

        public override void OnEvent(IngameItemEvent evnt)
        {
            if (!IngameShop.Instance.GameItems.ContainsKey(evnt.ID))
            {
                return;
            }

            IPlayerInfoState playerInfoState = evnt.RaisedBy.GetPlayerConnection().PlayerInfo.state;
            int price = IngameShop.Instance.GameItems[evnt.ID].Price;

            if (playerInfoState.Bread < price)
            {
                return;
            }

            if (playerInfoState.Creature == evnt.ID || playerInfoState.Gadget1 == evnt.ID || playerInfoState.Gadget2 == evnt.ID || playerInfoState.Gadget3 == evnt.ID || playerInfoState.Armor == evnt.ID)
            {
                return;
            }

            Dictionary<string, IngameItemData> gameItems = IngameShop.Instance.GameItems;

            if (!gameItems.ContainsKey(evnt.ID) || playerInfoState.Team != gameItems[evnt.ID].Team)
            {
                return;
            }

            playerInfoState.Bread -= price;

            switch ((int)gameItems[evnt.ID].Type)
            {
                case (int)TypeState.Creature:
                {
                    playerInfoState.Creature = evnt.ID;

                    break;
                }
                case (int)TypeState.Gadget1:
                {
                    playerInfoState.Gadget1 = evnt.ID;

                    break;
                }
                case (int)TypeState.Gadget2:
                {
                    playerInfoState.Gadget2 = evnt.ID;

                    break;
                }
                case (int)TypeState.Gadget3:
                {
                    playerInfoState.Gadget3 = evnt.ID;

                    break;
                }
                case (int)TypeState.Armor:
                {
                    playerInfoState.Armor = evnt.ID;

                    break;
                }
            }
        }
    }
}