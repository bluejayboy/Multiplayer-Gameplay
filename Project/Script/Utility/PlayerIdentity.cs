using UnityEngine;
using UnityEngine.UI;
using Photon.Bolt;
using TMPro;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerIdentity : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private TextMeshProUGUI penNameText = null;
        [SerializeField] private Image healthBar = null;
        [SerializeField] private Image briefHealthBar = null;

        public override void Attached()
        {
            state.AddCallback("PenName", ApplyPenName);
            state.AddCallback("Color", ApplyColor);
            state.AddCallback("ActiveHealth", ApplyHealthBar);
            state.AddCallback("BriefHealth", ApplyBriefHealthBar);
        }

        public void ApplyPenName()
        {
            if (entity.IsAttached)
            {
                penNameText.text = state.PenName;
            }
        }

        public void ApplyColor()
        {
            if (entity.IsAttached)
            {
                penNameText.color = state.Color;
            }
        }

        public void ApplyHealthBar()
        {
            if (entity.IsAttached)
            {
                healthBar.fillAmount = (float)state.ActiveHealth / state.MaxHealth;
            }
        }

        public void ApplyBriefHealthBar()
        {
            if (entity.IsAttached)
            {
                if (state.BriefHealth > 0)
                {
                    briefHealthBar.fillAmount = (float)(state.BriefHealth + state.ActiveHealth) / state.MaxHealth;
                }
                else
                {
                    briefHealthBar.fillAmount = 0.0f;
                }
            }
        }
    }
}