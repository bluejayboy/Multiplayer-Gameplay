using UnityEngine;
using LlockhamIndustries.Decals;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Footprint : MonoBehaviour
    {
        private ProjectionRenderer projectionRenderer = null;

        private void Awake()
        {
            projectionRenderer = GetComponentInChildren<ProjectionRenderer>(true);
        }

        public void SetFootprint(Color32 color)
        {
            #if ISDEDICATED
            return;
            #endif
            projectionRenderer.SetColor(0, color);
        }
    }
}