using UnityEngine.SceneManagement;
using Photon.Bolt;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class ClientDisconnect : GlobalEventListener
{
    public override void Disconnected(BoltConnection connection)
    {
        SceneManager.LoadScene("Home");
    }
}