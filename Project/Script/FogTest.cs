using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class FogTest : MonoBehaviour
{
    public static FogTest Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (!ObscuredPrefs.GetBool("Anisotropic"))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}