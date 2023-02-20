using UnityEngine;
using Scram;
using Photon.Bolt;

public class Pusher : MonoBehaviour
{
    [SerializeField] private float force = 100.0f;

    private void OnTriggerStay(Collider other)
    {
        var playerEntity = other.GetComponent<BoltEntity>();

        if (playerEntity == null || !playerEntity.IsAttached || !playerEntity.IsControllerOrOwner)
        {
            return;
        }

        var frigidbody = other.GetComponent<Frigidbody>();

        if (frigidbody == null)
        {
            return;
        }

        frigidbody.AddForce(transform.forward * force);
    }
}
