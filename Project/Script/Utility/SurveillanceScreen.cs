using MEC;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class SurveillanceScreen : MonoBehaviour
    {
        [SerializeField] private Camera screenCamera = null;
        [SerializeField] private float disableTime = 1.0f;

        private void Awake()
        {
            DisableCamera();
        }

        private IEnumerator<float> DisableCamera()
        {
            yield return Timing.WaitForSeconds(disableTime);

            screenCamera.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(ScramConstant.PlayerTag))
            {
                return;
            }

            var playerEntity = other.GetComponent<BoltEntity>();

            if (playerEntity != null && playerEntity.IsAttached && playerEntity.HasControl)
            {
                screenCamera.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(ScramConstant.PlayerTag))
            {
                return;
            }

            var playerEntity = other.GetComponent<BoltEntity>();

            if (playerEntity != null && playerEntity.IsAttached && playerEntity.HasControl)
            {
                screenCamera.enabled = false;
            }
        }
    }
}