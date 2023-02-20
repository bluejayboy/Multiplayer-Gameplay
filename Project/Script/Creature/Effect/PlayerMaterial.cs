using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerMaterial : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer avatar = null;
        [SerializeField] private Material invisibleCutoutMaterial = null;
        [SerializeField] private Material invisibleFadeMaterial = null;

        private PlayerLoadout loadout = null;

        private Material[] originalMaterials = null;
        private Material[] activeMaterials = null;

        private void Awake()
        {
#if ISDEDICATED
            return;
#endif

            loadout = GetComponent<PlayerLoadout>();

            originalMaterials = avatar.materials;
            activeMaterials = avatar.materials;
        }

        public void SetHandsInvisible(bool willSetInvisible, bool visibleLeftHand)
        {
#if ISDEDICATED
            return;
#endif

            activeMaterials[0] = (willSetInvisible) ? invisibleCutoutMaterial : originalMaterials[0];
            activeMaterials[1] = (willSetInvisible && !visibleLeftHand) ? invisibleCutoutMaterial : originalMaterials[1];

            avatar.materials = activeMaterials;
        }

        public void SetBodyInvisible(bool willSetInvisible)
        {
#if ISDEDICATED
            return;
#endif

            activeMaterials[2] = (willSetInvisible) ? invisibleFadeMaterial : originalMaterials[2];
            activeMaterials[3] = originalMaterials[3];
            activeMaterials[4] = (willSetInvisible) ? invisibleFadeMaterial : originalMaterials[4];
            activeMaterials[5] = originalMaterials[5];
            activeMaterials[6] = (willSetInvisible) ? invisibleFadeMaterial : originalMaterials[6];

            if (loadout.ActiveGadget != null)
            {
                if (loadout.ActiveGadget.Data.VisibleLeftHand)
                {
                    activeMaterials[1] = originalMaterials[1];
                }
            }
            else
            {
                activeMaterials[0] = originalMaterials[0];
                activeMaterials[1] = originalMaterials[1];
            }

            avatar.materials = activeMaterials;
        }

        public void ApplyFirstPersonMaterial(Material material, bool hideShadow)
        {
#if ISDEDICATED
            return;
#endif

            activeMaterials[3] = material;
            activeMaterials[5] = material;

            if (hideShadow)
            {
                activeMaterials[2] = invisibleCutoutMaterial;
                activeMaterials[4] = invisibleCutoutMaterial;
                activeMaterials[6] = invisibleCutoutMaterial;
            }

            if (loadout.ActiveGadget != null)
            {
                if (loadout.ActiveGadget.Data.VisibleLeftHand)
                {
                    activeMaterials[1] = material;
                }
            }
            else
            {
                activeMaterials[0] = material;
                activeMaterials[1] = material;
            }

            avatar.materials = activeMaterials;
        }

        public void ApplyThirdPersonMaterial(Material material)
        {
#if ISDEDICATED
            return;
#endif

            activeMaterials[3] = material;
            activeMaterials[4] = material;
            activeMaterials[5] = material;
            activeMaterials[6] = material;

            if (loadout.ActiveGadget != null)
            {
                if (loadout.ActiveGadget.Data.VisibleLeftHand)
                {
                    activeMaterials[1] = material;
                }
            }
            else
            {
                activeMaterials[0] = material;
                activeMaterials[1] = material;
            }

            avatar.materials = activeMaterials;
        }
    }
}