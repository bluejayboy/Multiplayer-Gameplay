using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using BeautifyEffect;
using AmplifyOcclusion;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class CameraOptions : MonoBehaviour
    {
        [SerializeField] private bool allowFieldOfView = false;

        private Camera cachedCamera = null;
        private AmplifyOcclusionEffect ampOcc = null;
        private Beautify beautify = null;

        private void Awake()
        {
            cachedCamera = GetComponent<Camera>();
            ampOcc = GetComponent<AmplifyOcclusionEffect>();
            beautify = GetComponent<Beautify>();

            ApplySettings();
        }

        private void Start()
        {
            GameOptionsMenu.Instance.OnSettingsChanged += ApplySettings;
        }

        private void OnDestroy()
        {
            if (GameOptionsMenu.Instance != null && GameOptionsMenu.Instance.OnSettingsChanged != null)
            {
                GameOptionsMenu.Instance.OnSettingsChanged -= ApplySettings;
            }
        }

        private void ApplySettings()
        {
            if (allowFieldOfView)
            {
                cachedCamera.fieldOfView = ObscuredPrefs.GetFloat("Field of View");
            }

            if (beautify != null)
            {
                beautify.enabled = ObscuredPrefs.GetBool("Lighting");
            }

            if (ampOcc == null)
            {
                return;
            }

            var ambientOcclusion = ObscuredPrefs.GetInt("Ambient Occlusion");

            if (ambientOcclusion <= 0)
            {
                ampOcc.enabled = false;
            }
            else
            {
                ampOcc.enabled = true;

                switch (ambientOcclusion)
                {
                    case 1:
                        {
                            ampOcc.SampleCount = SampleCountLevel.Low;

                            break;
                        }
                    case 2:
                        {
                            ampOcc.SampleCount = SampleCountLevel.Medium;

                            break;
                        }
                    case 3:
                        {
                            ampOcc.SampleCount = SampleCountLevel.High;

                            break;
                        }
                    case 4:
                        {
                            ampOcc.SampleCount = SampleCountLevel.VeryHigh;

                            break;
                        }
                }
            }
        }
    }
}