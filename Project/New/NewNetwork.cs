using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using UdpKit.Platform.Photon;
using UdpKit;
using Scram;
using CodeStage.AntiCheat.ObscuredTypes;
using Michsky.UI.ModernUIPack;
using System;
using TMPro;
using UnityEngine.UI;

public class NewNetwork : GlobalEventListener
{
    public string version;

    public AudioSource audioSource;
    public AudioClip buttonSound;

    public MenuChanger menuChanger;
    public GameObject joinMenu;
    public GameObject hostMenu;
    public Transform list;
    public GameObject room;

    public TMP_InputField roomName;
    public CustomDropdown officialMapSelection;
    public CustomDropdown modMapSelection;
    public Slider maxPlayerSlider;
    public TextMeshProUGUI maxPlayerText;

    public GameObject official;
    public GameObject mod;

    public GameObject refreshIcon;

    private bool isMod = false;

    private List<GameObject> buttons = new List<GameObject>(500);

    private void Awake()
    {
        UpdateMaxPlayerText();
    }

    private ConnectToken GetConnectToken()
    {
        var connectToken = new ConnectToken
        {
            PenName = ObscuredPrefs.GetString("PenName"),
            HeadColor = ObscuredPrefs.GetColor("Head Color"),
            TorsoColor = ObscuredPrefs.GetColor("Torso Color"),
            LeftHandColor = ObscuredPrefs.GetColor("Left Hand Color"),
            RightHandColor = ObscuredPrefs.GetColor("Right Hand Color"),
            LeftFootColor = ObscuredPrefs.GetColor("Left Foot Color"),
            RightFootColor = ObscuredPrefs.GetColor("Right Foot Color"),
            Version = version
        };

        if (ObscuredPrefs.GetBool("DebugTest"))
        {
            connectToken.HeadGarb = ObscuredPrefs.GetString("Head Garb");
        }

        return connectToken;
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer && !SteamLAN.IsLan)
        {
            var photonToken = new PhotonRoomProperties();
            string map = "";

            if (isMod)
            {
                map = modMapSelection.selectedText.text;
            }
            else
            {
                map = officialMapSelection.selectedText.text;
            }

            photonToken.AddRoomProperty("Name", roomName.text);
            photonToken.AddRoomProperty("Map", map);
            photonToken.AddRoomProperty("MaxPlayers", (int)maxPlayerSlider.value);

            BoltMatchmaking.CreateSession(roomName.text, photonToken, map, GetConnectToken());
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        //refreshIcon.SetActive(false);
        GetRoomList();
    }

    public void OpenMap(bool isOfficial)
    {
        if (isOfficial)
        {
            isMod = false;
            official.SetActive(true);
            mod.SetActive(false);
        }
        else
        {
            isMod = true;
            official.SetActive(false);
            mod.SetActive(true);
        }
    }

    public void UpdateMaxPlayerText()
    {
        maxPlayerText.text = "Max Players: " + maxPlayerSlider.value;
    }

    public void Host()
    {
        if (string.IsNullOrEmpty(roomName.text))
        {
            return;
        }

        menuChanger.OpenMenu("Loading Menu");
        BoltLauncher.Shutdown();
        BoltLauncher.StartServer();
    }

    public void GetRoomList()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i]);
        }

        buttons.Clear();

        foreach (var session in BoltNetwork.SessionList)
        {
            var photonSession = session.Value as PhotonSession;

            if (photonSession.Source != UdpSessionSource.Photon)
            {
                continue;
            }

            GameObject button = Instantiate(room, list);
            buttons.Add(button);

            var roomSnippet = button.GetComponent<RoomSnippet>();
            var sessionToken = photonSession.Properties;

            roomSnippet.roomName.text = sessionToken["Name"].ToString();
            roomSnippet.map.text = sessionToken["Map"].ToString();
            roomSnippet.playerCount.text = photonSession.ConnectionsCurrent + "/" + sessionToken["MaxPlayers"];

            var buttonManager = button.GetComponent<ButtonManagerBasic>();

            buttonManager.clickEvent.AddListener(delegate { BoltMatchmaking.JoinSession(photonSession, GetConnectToken()); });
            buttonManager.clickEvent.AddListener(delegate { MenuChanger.Instance.OpenMenu("Loading Menu"); });
            buttonManager.clickEvent.AddListener(delegate { audioSource.PlayOneShot(buttonSound); });
        }
    }

    public void OpenJoin()
    {
        //refreshIcon.SetActive(true);
        BoltLauncher.StartClient();
        joinMenu.SetActive(true);
        hostMenu.SetActive(false);
    }

    public void OpenHost()
    {
        joinMenu.SetActive(false);
        hostMenu.SetActive(true);
    }
}