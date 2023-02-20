using UnityEngine;
using Scram;
using Photon.Bolt;

public class LevelSwitcher : MonoBehaviour
{
    [SerializeField] private int level;

    private void OnTriggerEnter(Collider other)
    {
        if (BoltNetwork.IsClient || !other.CompareTag(ScramConstant.PlayerTag))
        {
            return;
        }

        var player = other.GetComponent<Player>();

        if (player != null)
        {
            ObbyGameMode.ChildInstance.GoToLevel(level, player.entity.Controller.GetPlayerConnection());
        }
    }
}
