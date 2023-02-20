using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class CameraLook : MonoBehaviour
    {
        private Transform cachedTransform = null;
        private Transform mainCamera = null;

        private void Awake()
        {
            cachedTransform = transform;
            mainCamera = Camera.main.transform;
        }

        private void Update()
        {
            LookAtCamera();
        }

        private void LookAtCamera()
        {
            if (mainCamera == null || !mainCamera.gameObject.activeInHierarchy)
            {
                var cam = Camera.main;

                if (cam != null)
                {
                    mainCamera = cam.transform;
                }
            }

            if (cachedTransform != null && mainCamera != null)
            {
                cachedTransform.rotation = Quaternion.LookRotation(cachedTransform.position - mainCamera.position);
            }
        }
    }
}