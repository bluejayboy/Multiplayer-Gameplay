using UnityEngine;
using UnityEngine.UI;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class FpsMeasurer : MonoBehaviour
    {
        private const float measurePeriod = 0.5f;
        private const string display = "{0} FPS";

        private float nextPeriod = 0.0f;
        private int accumulator = 0;
        private int activeFps = 0;
        private Text fps = null;

        private void Awake()
        {
            nextPeriod = Time.realtimeSinceStartup + measurePeriod;
            fps = GetComponent<Text>();
        }

        private void Update()
        {
            accumulator++;

            if (Time.realtimeSinceStartup > nextPeriod)
            {
                activeFps = (int)(accumulator / measurePeriod);
                accumulator = 0;
                nextPeriod += measurePeriod;
                fps.text = string.Format(display, activeFps);
            }
        }
    }
}