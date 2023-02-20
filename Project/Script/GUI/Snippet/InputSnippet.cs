using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class InputSnippet : MonoBehaviour
    {
        [SerializeField] private Button button = null;
        [SerializeField] private TextMeshProUGUI text = null;

        public Button Button { get { return button; } }
        public TextMeshProUGUI Text { get { return text; } }
    }
}