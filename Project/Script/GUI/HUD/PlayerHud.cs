using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerHud : MonoBehaviour
    {
        public static PlayerHud Instance { get; private set; }

        [SerializeField] private GameObject hud = null;
        [SerializeField] private GameObject ammo = null;
        [SerializeField] private GameObject ability = null;

        [SerializeField] private LoadoutSnippet[] loadout;
        [SerializeField] private Color32 equippedColor;
        [SerializeField] private Color32 dequippedColor;

        [SerializeField] private GameObject healTime = null;
        [SerializeField] private Image healTimeFill = null;
        [SerializeField] private Image crosshair = null;
        [SerializeField] private Image hitmarker = null;
        [SerializeField] private Image scope = null;
        [SerializeField] private Image blood = null;
        [SerializeField] private Image interactBar = null;
        [SerializeField] private Image activeHealthBar = null;
        [SerializeField] private Image briefHealthBar = null;
        [SerializeField] private Image ammoBar = null;
        [SerializeField] private Image abilityBar = null;

        [SerializeField] private TextMeshProUGUI healthText = null;
        [SerializeField] private TextMeshProUGUI ammoText = null;
        [SerializeField] private TextMeshProUGUI interactText = null;
        [SerializeField] private TextMeshProUGUI errorText = null;

        private BoltEntity activeEntity = null;
        private bool hasControl = false;
        private IPlayerState activeState = null;

        public GameObject Ability { get { return ability; } }
        public Image Crosshair { get { return crosshair; } }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        void Update(){
            if(activeEntity != null && activeEntity.IsAttached)
                hasControl = activeEntity.HasControl;

            if(activeEntity == null){
                ResetHealthBars();
                scope.enabled = false;
            }
        }

        public void Attach(BoltEntity entity, IPlayerState state, bool showCrosshair)
        {
            hud.SetActive(true);

            hasControl = entity.HasControl;

            activeEntity = entity;
            activeState = state;

            RemoveCallbacks();
            AddCallbacks();
            ApplyLoadout();
            ApplyEquip();
            ApplyActiveHealthBar();
            ApplyBriefHealthBar();
            ApplyAmmoBar();
            ApplyInteractBar();
            ToggleScope();
            SetInteractText(string.Empty);

            healTime.SetActive(false);
            crosshair.enabled = showCrosshair;
            scope.enabled = activeState.ActiveGadget.IsAiming;

            hitmarker.CrossFadeAlpha(0.0f, 0.0f, true);
            blood.CrossFadeAlpha(0.0f, 0.0f, true);
            errorText.CrossFadeAlpha(0.0f, 0.0f, true);
        }

        public void Detach()
        {
            if (hasControl)
            {
                return;
            }

            if (activeState == null)
            {
                return;
            }

            RemoveCallbacks();

            activeState = null;
            abilityBar.fillAmount = 1.0f;
            ability.SetActive(false);
            hud.SetActive(false);
            healTime.SetActive(false);

            for (int i = 0; i < loadout.Length; i++)
            {
                loadout[i].gameObject.SetActive(false);
            }
        }

        private void AddCallbacks()
        {
            activeState.AddCallback("OwnedGadgets[].ID", ApplyLoadout);
            activeState.AddCallback("ActiveGadget.Slot", ApplyEquip);
            activeState.AddCallback("ActiveGadget.HealTimer", ApplyHealthTime);
            activeState.AddCallback("ActiveHealth", ApplyActiveHealthBar);
            activeState.AddCallback("BriefHealth", ApplyBriefHealthBar);
            activeState.AddCallback("ActiveGadget.Slot", ApplyAmmoBar);
            activeState.AddCallback(BoltNetwork.IsServer ? "OwnedGadgets[].ActiveAmmo" : "OwnedGadgets[].PredictedAmmo", ApplyAmmoBar);
            activeState.AddCallback("ActiveAbility.AbilityTimer", ApplyAbilityBar);
            activeState.AddCallback("InteractTimer", ApplyInteractBar);
            activeState.AddCallback("ActiveGadget.IsAiming", ToggleScope);
        }

        private void RemoveCallbacks()
        {
            activeState.RemoveCallback("OwnedGadgets[].ID", ApplyLoadout);
            activeState.RemoveCallback("ActiveGadget.Slot", ApplyEquip);
            activeState.RemoveCallback("ActiveGadget.HealTimer", ApplyHealthTime);
            activeState.RemoveCallback("ActiveHealth", ApplyActiveHealthBar);
            activeState.RemoveCallback("BriefHealth", ApplyBriefHealthBar);
            activeState.RemoveCallback("ActiveGadget.Slot", ApplyAmmoBar);
            activeState.RemoveCallback(BoltNetwork.IsServer ? "OwnedGadgets[].ActiveAmmo" : "OwnedGadgets[].PredictedAmmo", ApplyAmmoBar);
            activeState.RemoveCallback("ActiveAbility.AbilityTimer", ApplyAbilityBar);
            activeState.RemoveCallback("InteractTimer", ApplyInteractBar);
            activeState.RemoveCallback("ActiveGadget.IsAiming", ToggleScope);
        }

        private void ApplyLoadout()
        {
            if (!activeState.CanInteract)
            {
                for (int i = 0; i < loadout.Length; i++)
                {
                    loadout[i].gameObject.SetActive(false);
                }

                return;
            }

            for (int i = 0; i < loadout.Length; i++)
            {
                if (!string.IsNullOrEmpty(activeState.OwnedGadgets[i].ID))
                {
                    loadout[i].gameObject.SetActive(true);
                    loadout[i].ApplyName(activeState.OwnedGadgets[i].Display);
                }
                else
                {
                    loadout[i].gameObject.SetActive(false);
                    loadout[i].ApplyName(string.Empty);
                }
            }
        }

        private void ApplyEquip()
        {
            if (!activeState.CanInteract)
            {
                return;
            }

            for (int i = 0; i < loadout.Length; i++)
            {
                if (activeState.ActiveGadget.Slot == i)
                {
                    loadout[i].ToggleEquip(equippedColor);
                }
                else
                {
                    loadout[i].ToggleEquip(dequippedColor);
                }
            }
        }

        private void ApplyHealthTime()
        {
            if (activeState.ActiveGadget.HealTimer <= 0.0f)
            {
                healTime.SetActive(false);
            }
            else
            {
                healTime.SetActive(true);
            }

            healTimeFill.fillAmount = activeState.ActiveGadget.HealTimer / activeState.ActiveGadget.MaxHealTime;
        }

        private void ApplyActiveHealthBar()
        {
            healthText.text = activeState.ActiveHealth.ToString();
            activeHealthBar.fillAmount = (float)activeState.ActiveHealth / activeState.MaxHealth;
        }

        void ResetHealthBars(){
            healthText.text = "0";
            activeHealthBar.fillAmount = 0.0f;
            briefHealthBar.fillAmount = 0.0f;
        }

        private void ApplyBriefHealthBar()
        {
            healthText.text = (activeState.BriefHealth + activeState.ActiveHealth).ToString();

            if (activeState.BriefHealth > 0)
            {
                briefHealthBar.fillAmount = (float)(activeState.BriefHealth + activeState.ActiveHealth) / activeState.MaxHealth;
            }
            else
            {
                briefHealthBar.fillAmount = 0.0f;
            }
        }

        private void ApplyAmmoBar()
        {
            if (activeState.ActiveGadget.Slot < 0)
            {
                ammo.SetActive(false);

                return;
            }

            var activeGadget = activeState.OwnedGadgets[activeState.ActiveGadget.Slot];

            ammo.SetActive(activeGadget.ActiveAmmo > -1);
            ammoText.text = BoltNetwork.IsServer ? activeGadget.ActiveAmmo.ToString() : activeGadget.PredictedAmmo.ToString();
            ammoBar.fillAmount = (BoltNetwork.IsServer ? activeGadget.ActiveAmmo : activeGadget.PredictedAmmo) / activeGadget.MaxAmmo;
        }

        private void ApplyAbilityBar()
        {
            abilityBar.fillAmount = activeState.ActiveAbility.AbilityTimer / activeState.ActiveAbility.AbilityRate;
        }

        private void ApplyInteractBar()
        {
            interactBar.fillAmount = activeState.InteractTimer;
        }

        public void SetInteractText(string text)
        {
            interactText.text = text;
        }

        public void DisplayHit()
        {
            hitmarker.CrossFadeAlpha(1.0f, 0.0f, true);
            hitmarker.CrossFadeAlpha(0.0f, 0.5f, false);
        }

        public void DisplayDamage()
        {
            blood.CrossFadeAlpha(1.0f, 0.0f, true);
            blood.CrossFadeAlpha(0.0f, 0.5f, false);
        }

        private void ToggleScope()
        {
            scope.enabled = activeState.ActiveGadget.IsAiming;
        }
    }
}