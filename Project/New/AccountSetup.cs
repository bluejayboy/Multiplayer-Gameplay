using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;

public class AccountSetup : MonoBehaviour
{
    public TMP_InputField penName;

    public void ConfirmName()
    {
        if (penName.text.Length >= 1 && penName.text.Length <= 20)
        {
            ObscuredPrefs.SetString("PenName", penName.text);
            GetComponent<Scram.MenuChanger>().OpenMenu("Home Menu");
        }
    }
}