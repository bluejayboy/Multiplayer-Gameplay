using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public static class InputCode
    {
        public static KeyCode Left { get; private set; }
        public static KeyCode Right { get; private set; }
        public static KeyCode Backward { get; private set; }
        public static KeyCode Forward { get; private set; }
        public static KeyCode Jump { get; private set; }
        public static KeyCode Crouch { get; private set; }
        public static KeyCode PrimaryFire { get; private set; }
        public static KeyCode SecondaryFire { get; private set; }
        public static KeyCode Interact { get; private set; }
        public static KeyCode Throw { get; private set; }
        public static KeyCode Chat { get; private set; }
        public static KeyCode TeamChat { get; private set; }
        public static KeyCode Scoreboard { get; private set; }
        public static KeyCode Pause { get; private set; }
        public static KeyCode Confirm { get; private set; }
        public static KeyCode VoiceChat { get; private set; }

        public static float MouseSensitivity { get; private set; }
        public static float FieldOfView { get; private set; }

        private static bool invertMouse = false;

        public static float MouseX
        {
            get
            {
                return Input.GetAxisRaw("Mouse X");
            }
        }

        public static float MouseY
        {
            get
            {
                return (invertMouse) ? -Input.GetAxisRaw("Mouse Y") : Input.GetAxisRaw("Mouse Y");
            }
        }

        public static void ApplyInput()
        {
            invertMouse = ObscuredPrefs.GetBool("Invert Mouse");
            MouseSensitivity = ObscuredPrefs.GetFloat("Mouse Sensitivity");
            FieldOfView = ObscuredPrefs.GetFloat("Field of View");

            Left = Parse("Left");
            Right = Parse("Right");
            Backward = Parse("Backward");
            Forward = Parse("Forward");
            Jump = Parse("Jump");
            Crouch = Parse("Crouch");
            PrimaryFire = Parse("Primary Fire");
            SecondaryFire = Parse("Secondary Fire");
            Interact = Parse("Interact");
            Throw = Parse("Throw");
            Chat = Parse("Chat");
            TeamChat = Parse("Team Chat");
            Scoreboard = Parse("Scoreboard");
            Pause = Parse("Pause");
            Confirm = Parse("Confirm");
            VoiceChat = Parse("Voice");
        }

        private static KeyCode Parse(string prefs)
        {
            if (Enum.IsDefined(typeof(KeyCode), ObscuredPrefs.GetString(prefs)))
            {
                return (KeyCode)Enum.Parse(typeof(KeyCode), ObscuredPrefs.GetString(prefs));
            }
            else
            {
                return KeyCode.None;
            }
        }
    }
}