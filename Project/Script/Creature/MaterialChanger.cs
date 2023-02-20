using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class MaterialChanger : MonoBehaviour
    {
        private Material[] originalMaterials = null;
        private Renderer render = null;

        private void Awake()
        {
#if ISDEDICATED
            return;
#endif

            render = GetComponent<Renderer>();
            originalMaterials = render.materials;
        }

        public void ApplyMaterial(Material material)
        {
#if ISDEDICATED
            return;
#endif

            if (render == null)
            {
                return;
            }

            var materials = render.materials;

            for (var i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }

            render.materials = materials;
        }

        public void RemoveMaterial()
        {
#if ISDEDICATED
            return;
#endif

            if (render == null)
            {
                return;
            }

            render.materials = originalMaterials;
        }
    }
}