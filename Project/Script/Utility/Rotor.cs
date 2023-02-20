using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Rotor : MonoBehaviour
    {
        [SerializeField] private float rotorSpeed = -360.0f;
        [SerializeField] private bool menu;

        private Transform cachedTransform;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void FixedUpdate()
        {
            if (menu)
            {
                var speed = rotorSpeed * Time.smoothDeltaTime;

                cachedTransform.Rotate(new Vector3(speed * Vector3.up.x, speed * Vector3.up.y, speed * Vector3.up.z));

                return;
            }

            if (BoltNetwork.IsServer || BoltNetwork.IsConnected)
            {
                transform.rotation = Quaternion.Euler(0, BoltNetwork.ServerTime * rotorSpeed, 0);
            }
        }
    }
}