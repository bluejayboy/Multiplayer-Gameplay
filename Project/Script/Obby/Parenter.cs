using UnityEngine;
using Photon.Bolt;

public class Parenter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var playerEntity = other.GetComponent<BoltEntity>();

        if (playerEntity == null || !playerEntity.IsAttached || !playerEntity.IsControllerOrOwner)
        {
            return;
        }

        other.transform.SetParent(transform.parent, true);
    }

    private void OnTriggerExit(Collider other)
    {
        var playerEntity = other.GetComponent<BoltEntity>();

        if (playerEntity == null || !playerEntity.IsAttached || !playerEntity.IsControllerOrOwner)
        {
            return;
        }

        other.transform.SetParent(null, true);
    }
}