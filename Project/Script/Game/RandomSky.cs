using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RandomSky : MonoBehaviour
    {
        [SerializeField] private Material[] skies;

        private void Awake()
        {
            InvokeRepeating("SpawnItems", 0.0f, 60.0f);
        }

        private void SpawnItems()
        {
            var sky = skies[Random.Range(0, skies.Length)];

            RenderSettings.skybox = sky;
        }
    }
}