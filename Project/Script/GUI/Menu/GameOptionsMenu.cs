using CodeStage.AntiCheat.ObscuredTypes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GameOptionsMenu : MenuParent
    {
        public static GameOptionsMenu Instance { get; private set; }

        public Action OnSettingsChanged { get; set; }

        [SerializeField] private bool applyOnAwake = false;
        [SerializeField] private Slider mainVolume = null;
        [SerializeField] private Slider musicVolume = null;
        [SerializeField] private Slider VoIPVolume = null;

        [SerializeField] private TMP_Dropdown resolution = null;
        [SerializeField] private TMP_Dropdown shadows = null;
        [SerializeField] private TMP_Dropdown ambientOcclusion = null;
        [SerializeField] private TMP_Dropdown antiAliasing = null;

        [SerializeField] private Toggle lighting = null;
        [SerializeField] private Toggle anisotropic = null;
        [SerializeField] private Toggle fullscreen = null;
        [SerializeField] private Toggle vsync = null;

        [SerializeField] private float defaultMainVolume = 1.0f;
        [SerializeField] private float defaultMusicVolume = 1.0f;
        [SerializeField] private float defaultVoIPVolume = 1.0f;
        [SerializeField] private int defaultResolution = 0;
        [SerializeField] private int defaultShadows = 4;
        [SerializeField] private int defaultAmbientOcclusion = 4;
        [SerializeField] private int defaultAntiAliasing = 3;

        private void Awake()
        {
            Instance = this;

            if (!applyOnAwake)
            {
                return;
            }

            InitializePrefs();
            ApplySettings();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void Activate()
        {
            mainVolume.value = ObscuredPrefs.GetFloat("Main Volume");
            musicVolume.value = ObscuredPrefs.GetFloat("Music Volume");
            VoIPVolume.value = ObscuredPrefs.GetFloat("VoIP Volume");

            resolution.value = -1;
            string[] resolutionSplits = ObscuredPrefs.GetString("Resolution").Split('x');

            if (!string.IsNullOrEmpty(ObscuredPrefs.GetString("Resolution")))
                resolution.captionText.text = int.Parse(resolutionSplits[0]) + "x" + int.Parse(resolutionSplits[1]);

            shadows.value = ObscuredPrefs.GetInt("Shadows");
            ambientOcclusion.value = ObscuredPrefs.GetInt("Ambient Occlusion");
            antiAliasing.value = ObscuredPrefs.GetInt("Anti Aliasing");

            lighting.isOn = ObscuredPrefs.GetBool("Lighting");
            anisotropic.isOn = ObscuredPrefs.GetBool("Anisotropic");
            fullscreen.isOn = ObscuredPrefs.GetBool("Fullscreen");
            vsync.isOn = ObscuredPrefs.GetBool("Vsync");
        }

        public void Apply()
        {
            ApplyPrefs();
            ApplySettings();
        }

        public void Default()
        {
            mainVolume.value = defaultMainVolume;
            musicVolume.value = defaultMusicVolume;
            VoIPVolume.value = defaultVoIPVolume;
            resolution.value = defaultResolution;
            shadows.value = defaultShadows;
            ambientOcclusion.value = defaultAmbientOcclusion;
            antiAliasing.value = defaultAntiAliasing;
            lighting.isOn = true;
            anisotropic.isOn = true;
            fullscreen.isOn = true;
            vsync.isOn = true;
        }

        public void Low()
        {
            shadows.value = 0;
            ambientOcclusion.value = 0;
            antiAliasing.value = 0;
            lighting.isOn = false;
            anisotropic.isOn = false;
            vsync.isOn = false;
        }

        public void Medium()
        {
            shadows.value = 1;
            ambientOcclusion.value = 1;
            antiAliasing.value = 1;
            lighting.isOn = true;
            anisotropic.isOn = false;
            vsync.isOn = false;
        }

        public void High()
        {
            shadows.value = 4;
            ambientOcclusion.value = 4;
            antiAliasing.value = 3;
            lighting.isOn = true;
            anisotropic.isOn = true;
            vsync.isOn = true;
        }

        private void InitializePrefs()
        {
            if (!ObscuredPrefs.HasKey("Main Volume"))
            {
                ObscuredPrefs.SetFloat("Main Volume", defaultMainVolume);
            }

            if (!ObscuredPrefs.HasKey("Music Volume"))
            {
                ObscuredPrefs.SetFloat("Music Volume", defaultMusicVolume);
            }

            if (!ObscuredPrefs.HasKey("VoIP Volume"))
            {
                ObscuredPrefs.SetFloat("VoIP Volume", defaultVoIPVolume);
            }

            if (!ObscuredPrefs.HasKey("Resolution") || string.IsNullOrEmpty(ObscuredPrefs.GetString("Resolution")))
            {
                Resolution maxResolution = Screen.resolutions[Screen.resolutions.Length - 1];

                ObscuredPrefs.SetString("Resolution", maxResolution.width + "x" + maxResolution.height);
            }

            if (!ObscuredPrefs.HasKey("Shadows"))
            {
                ObscuredPrefs.SetInt("Shadows", defaultShadows);
            }

            if (!ObscuredPrefs.HasKey("Ambient Occlusion"))
            {
                ObscuredPrefs.SetInt("Ambient Occlusion", defaultAmbientOcclusion);
            }

            if (!ObscuredPrefs.HasKey("Anti Aliasing"))
            {
                ObscuredPrefs.SetInt("Anti Aliasing", defaultAntiAliasing);
            }

            if (!ObscuredPrefs.HasKey("Lighting"))
            {
                ObscuredPrefs.SetBool("Lighting", true);
            }

            if (!ObscuredPrefs.HasKey("Anisotropic"))
            {
                ObscuredPrefs.SetBool("Anisotropic", true);
            }

            if (!ObscuredPrefs.HasKey("Fullscreen"))
            {
                ObscuredPrefs.SetBool("Fullscreen", true);
            }

            if (!ObscuredPrefs.HasKey("Vsync"))
            {
                ObscuredPrefs.SetBool("Vsync", true);
            }

            if (!ObscuredPrefs.HasKey("HasDiscord"))
            {
                ObscuredPrefs.SetBool("HasDiscord", false);
            }
        }

        private void ApplyPrefs()
        {
            if (resolution.value == 0)
            {
                Resolution maxResolution = Screen.resolutions[Screen.resolutions.Length - 1];
                resolution.captionText.text = maxResolution.width + "x" + maxResolution.height;
            }

            ObscuredPrefs.SetFloat("Main Volume", mainVolume.value);
            ObscuredPrefs.SetFloat("Music Volume", musicVolume.value);
            ObscuredPrefs.SetFloat("VoIP Volume", VoIPVolume.value);
            ObscuredPrefs.SetString("Resolution", resolution.captionText.text);
            ObscuredPrefs.SetInt("Shadows", shadows.value);
            ObscuredPrefs.SetInt("Ambient Occlusion", ambientOcclusion.value);
            ObscuredPrefs.SetInt("Anti Aliasing", antiAliasing.value);
            ObscuredPrefs.SetBool("Lighting", lighting.isOn);
            ObscuredPrefs.SetBool("Anisotropic", anisotropic.isOn);
            ObscuredPrefs.SetBool("Fullscreen", fullscreen.isOn);
            ObscuredPrefs.SetBool("Vsync", vsync.isOn);
        }

        private void ApplySettings()
        {
            AudioListener.volume = ObscuredPrefs.GetFloat("Main Volume");

            string[] resolutionSplits = ObscuredPrefs.GetString("Resolution").Split('x');

            if (!string.IsNullOrEmpty(ObscuredPrefs.GetString("Resolution")))
                Screen.SetResolution(int.Parse(resolutionSplits[0]), int.Parse(resolutionSplits[1]), false);

            if (ObscuredPrefs.GetInt("Shadows") <= 0)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
            }
            else
            {
                QualitySettings.shadows = ShadowQuality.All;

                switch (ObscuredPrefs.GetInt("Shadows"))
                {
                    case 1:
                        {
                            QualitySettings.shadowResolution = ShadowResolution.Low;

                            break;
                        }
                    case 2:
                        {
                            QualitySettings.shadowResolution = ShadowResolution.Medium;

                            break;
                        }
                    case 3:
                        {
                            QualitySettings.shadowResolution = ShadowResolution.High;

                            break;
                        }
                    case 4:
                        {
                            QualitySettings.shadowResolution = ShadowResolution.VeryHigh;

                            break;
                        }
                }
            }

            switch (ObscuredPrefs.GetInt("Anti Aliasing"))
            {
                case 0:
                    {
                        QualitySettings.antiAliasing = 0;

                        break;
                    }
                case 1:
                    {
                        QualitySettings.antiAliasing = 2;

                        break;
                    }
                case 2:
                    {
                        QualitySettings.antiAliasing = 4;

                        break;
                    }
                case 3:
                    {
                        QualitySettings.antiAliasing = 8;

                        break;
                    }
            }

            Screen.fullScreen = ObscuredPrefs.GetBool("Fullscreen");
            QualitySettings.vSyncCount = (ObscuredPrefs.GetBool("Vsync")) ? 1 : 0;

            if (FogTest.Instance != null)
            {
                if (ObscuredPrefs.GetBool("Anisotropic"))
                {
                    FogTest.Instance.gameObject.SetActive(true);
                }
                else
                {
                    FogTest.Instance.gameObject.SetActive(false);
                }
            }

            if (OnSettingsChanged != null)
            {
                OnSettingsChanged.Invoke();
            }
        }
    }
}