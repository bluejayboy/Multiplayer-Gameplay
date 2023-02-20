using UnityEngine;
using TMPro;

namespace Scram
{
	[DisallowMultipleComponent]
	public sealed class LoadingTipMenu : MonoBehaviour
	{
        [SerializeField] private TextMeshProUGUI loading = null;
        [SerializeField] private string[] tips = null;

        private void OnEnable()
        {
            loading.text = tips[Random.Range(0, tips.Length)];
        }
    }
}