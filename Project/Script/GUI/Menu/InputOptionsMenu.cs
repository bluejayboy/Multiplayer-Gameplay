using CodeStage.AntiCheat.ObscuredTypes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class InputOptionsMenu : MenuParent
    {
        [SerializeField] private Toggle invertMouse = null;
        [SerializeField] private Slider mouseSensitivity = null;
        [SerializeField] private Slider fieldOfView = null;
        [SerializeField] private TextMeshProUGUI left = null;
        [SerializeField] private TextMeshProUGUI right = null;
        [SerializeField] private TextMeshProUGUI backward = null;
        [SerializeField] private TextMeshProUGUI forward = null;
        [SerializeField] private TextMeshProUGUI jump = null;
        [SerializeField] private TextMeshProUGUI crouch = null;
        [SerializeField] private TextMeshProUGUI primaryFire = null;
        [SerializeField] private TextMeshProUGUI secondaryFire = null;
        [SerializeField] private TextMeshProUGUI interact = null;
        [SerializeField] private TextMeshProUGUI throwText = null;
        [SerializeField] private TextMeshProUGUI chat = null;
        [SerializeField] private TextMeshProUGUI teamChat = null;
        [SerializeField] private TextMeshProUGUI scoreboard = null;
        [SerializeField] private TextMeshProUGUI pause = null;
        [SerializeField] private TextMeshProUGUI confirm = null;
        [SerializeField] private TextMeshProUGUI voice = null;
        [SerializeField] private TextMeshProUGUI[] inputs = null;

        [SerializeField] private float defaultMouseSensitivity = 5.0f;
        [SerializeField] private float defaultFieldOfView = 70.0f;
        [SerializeField] private string defaultLeft = "A";
        [SerializeField] private string defaultRight = "D";
        [SerializeField] private string defaultBackward = "S";
        [SerializeField] private string defaultForward = "W";
        [SerializeField] private string defaultJump = "Space";
        [SerializeField] private string defaultCrouch = "LeftControl";
        [SerializeField] private string defaultPrimaryFire = "Mouse0";
        [SerializeField] private string defaultSecondaryFire = "Mouse1";
        [SerializeField] private string defaultInteract = "E";
        [SerializeField] private string defaultThrow = "G";
        [SerializeField] private string defaultChat = "Y";
        [SerializeField] private string defaultTeamChat = "U";
        [SerializeField] private string defaultScoreboard = "Tab";
        [SerializeField] private string defaultPause = "Escape";
        [SerializeField] private string defaultConfirm = "Return";
        [SerializeField] private string defaultVoice = "V";

        private TextMeshProUGUI activeText = null;

        private void Awake()
        {
            InitializePrefs();
            InputCode.ApplyInput();
        }

        private void Update()
        {
            if (activeText == null)
            {
                return;
            }

            var keycodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

            for (var i = 0; i < keycodes.Length; i++)
            {
                if (Input.GetKeyDown(keycodes[i]))
                {
                    if (keycodes[i] == KeyCode.Mouse0)
                    {
                        break;
                    }

                    for (var j = 0; j < inputs.Length; j++)
                    {
                        if (inputs[j].text == keycodes[i].ToString())
                        {
                            inputs[j].text = string.Empty;
                        }
                    }

                    activeText.text = keycodes[i].ToString();
                    activeText = null;

                    break;
                }
            }
        }

        public override void Activate()
        {
            if (activeText != null)
            {
                activeText.text = string.Empty;
                activeText = null;
            }

            invertMouse.isOn = ObscuredPrefs.GetBool("Invert Mouse");
            mouseSensitivity.value = ObscuredPrefs.GetFloat("Mouse Sensitivity");
            fieldOfView.value = ObscuredPrefs.GetFloat("Field of View");

            left.text = ObscuredPrefs.GetString("Left");
            right.text = ObscuredPrefs.GetString("Right");
            backward.text = ObscuredPrefs.GetString("Backward");
            forward.text = ObscuredPrefs.GetString("Forward");
            jump.text = ObscuredPrefs.GetString("Jump");
            crouch.text = ObscuredPrefs.GetString("Crouch");
            primaryFire.text = ObscuredPrefs.GetString("Primary Fire");
            secondaryFire.text = ObscuredPrefs.GetString("Secondary Fire");
            interact.text = ObscuredPrefs.GetString("Interact");
            throwText.text = ObscuredPrefs.GetString("Throw");
            chat.text = ObscuredPrefs.GetString("Chat");
            teamChat.text = ObscuredPrefs.GetString("Team Chat");
            scoreboard.text = ObscuredPrefs.GetString("Scoreboard");
            pause.text = ObscuredPrefs.GetString("Pause");
            confirm.text = ObscuredPrefs.GetString("Confirm");
            voice.text = ObscuredPrefs.GetString("Voice");
        }

        public void SelectKey(InputSnippet snippet)
        {
            if (activeText == snippet.Text)
            {
                for (var j = 0; j < inputs.Length; j++)
                {
                    if (inputs[j].text == KeyCode.Mouse0.ToString())
                    {
                        inputs[j].text = string.Empty;
                    }
                }

                activeText.text = KeyCode.Mouse0.ToString();
                activeText = null;
            }
            else
            {
                if (activeText != null)
                {
                    activeText.text = string.Empty;
                }

                activeText = snippet.Text;
                activeText.text = "...";
            }
        }

        public void Apply()
        {
            if (activeText != null)
            {
                activeText.text = string.Empty;
                activeText = null;
            }

            ApplyPrefs();
            InputCode.ApplyInput();
        }

        public void Default()
        {
            if (activeText != null)
            {
                activeText.text = string.Empty;
                activeText = null;
            }

            invertMouse.isOn = false;
            mouseSensitivity.value = defaultMouseSensitivity;
            fieldOfView.value = defaultFieldOfView;

            left.text = defaultLeft;
            right.text = defaultRight;
            backward.text = defaultBackward;
            forward.text = defaultForward;
            jump.text = defaultJump;
            crouch.text = defaultCrouch;
            primaryFire.text = defaultPrimaryFire;
            secondaryFire.text = defaultSecondaryFire;
            interact.text = defaultInteract;
            throwText.text = defaultThrow;
            chat.text = defaultChat;
            teamChat.text = defaultTeamChat;
            scoreboard.text = defaultScoreboard;
            pause.text = defaultPause;
            confirm.text = defaultConfirm;
            voice.text = defaultVoice;
        }

        private void InitializePrefs()
        {
            if (!ObscuredPrefs.HasKey("Invert Mouse"))
            {
                ObscuredPrefs.SetBool("Invert Mouse", false);
            }

            if (!ObscuredPrefs.HasKey("Mouse Sensitivity"))
            {
                ObscuredPrefs.SetFloat("Mouse Sensitivity", defaultMouseSensitivity);
            }

            if (!ObscuredPrefs.HasKey("Field of View"))
            {
                ObscuredPrefs.SetFloat("Field of View", defaultFieldOfView);
            }

            if (!ObscuredPrefs.HasKey("Left"))
            {
                ObscuredPrefs.SetString("Left", defaultLeft);
            }

            if (!ObscuredPrefs.HasKey("Right"))
            {
                ObscuredPrefs.SetString("Right", defaultRight);
            }

            if (!ObscuredPrefs.HasKey("Backward"))
            {
                ObscuredPrefs.SetString("Backward", defaultBackward);
            }

            if (!ObscuredPrefs.HasKey("Forward"))
            {
                ObscuredPrefs.SetString("Forward", defaultForward);
            }

            if (!ObscuredPrefs.HasKey("Jump"))
            {
                ObscuredPrefs.SetString("Jump", defaultJump);
            }

            if (!ObscuredPrefs.HasKey("Crouch"))
            {
                ObscuredPrefs.SetString("Crouch", defaultCrouch);
            }

            if (!ObscuredPrefs.HasKey("Primary Fire"))
            {
                ObscuredPrefs.SetString("Primary Fire", defaultPrimaryFire);
            }

            if (!ObscuredPrefs.HasKey("Secondary Fire"))
            {
                ObscuredPrefs.SetString("Secondary Fire", defaultSecondaryFire);
            }

            if (!ObscuredPrefs.HasKey("Interact"))
            {
                ObscuredPrefs.SetString("Interact", defaultInteract);
            }

            if (!ObscuredPrefs.HasKey("Throw"))
            {
                ObscuredPrefs.SetString("Throw", defaultThrow);
            }

            if (!ObscuredPrefs.HasKey("Chat"))
            {
                ObscuredPrefs.SetString("Chat", defaultChat);
            }

            if (!ObscuredPrefs.HasKey("Team Chat"))
            {
                ObscuredPrefs.SetString("Team Chat", defaultTeamChat);
            }

            if (!ObscuredPrefs.HasKey("Scoreboard"))
            {
                ObscuredPrefs.SetString("Scoreboard", defaultScoreboard);
            }

            if (!ObscuredPrefs.HasKey("Pause"))
            {
                ObscuredPrefs.SetString("Pause", defaultPause);
            }

            if (!ObscuredPrefs.HasKey("Confirm"))
            {
                ObscuredPrefs.SetString("Confirm", defaultConfirm);
            }

            if(!ObscuredPrefs.HasKey("Voice")){
                ObscuredPrefs.SetString("Voice", defaultVoice);
            }
        }

        private void ApplyPrefs()
        {
            ObscuredPrefs.SetBool("Invert Mouse", invertMouse.isOn);
            ObscuredPrefs.SetFloat("Mouse Sensitivity", mouseSensitivity.value);
            ObscuredPrefs.SetFloat("Field of View", fieldOfView.value);

            ObscuredPrefs.SetString("Left", left.text);
            ObscuredPrefs.SetString("Right", right.text);
            ObscuredPrefs.SetString("Backward", backward.text);
            ObscuredPrefs.SetString("Forward", forward.text);
            ObscuredPrefs.SetString("Jump", jump.text);
            ObscuredPrefs.SetString("Crouch", crouch.text);
            ObscuredPrefs.SetString("Primary Fire", primaryFire.text);
            ObscuredPrefs.SetString("Secondary Fire", secondaryFire.text);
            ObscuredPrefs.SetString("Interact", interact.text);
            ObscuredPrefs.SetString("Throw", throwText.text);
            ObscuredPrefs.SetString("Chat", chat.text);
            ObscuredPrefs.SetString("Team Chat", teamChat.text);
            ObscuredPrefs.SetString("Scoreboard", scoreboard.text);
            ObscuredPrefs.SetString("Pause", pause.text);
            ObscuredPrefs.SetString("Confirm", confirm.text);
            ObscuredPrefs.SetString("Voice", voice.text);

            if (GameOptionsMenu.Instance != null && GameOptionsMenu.Instance.OnSettingsChanged != null)
            {
                GameOptionsMenu.Instance.OnSettingsChanged.Invoke();
            }
        }
    }
}