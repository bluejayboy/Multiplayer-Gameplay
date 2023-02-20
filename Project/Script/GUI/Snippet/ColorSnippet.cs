using UnityEngine;
using UnityEngine.UI;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ColorSnippet : MonoBehaviour
    {
        public void SelectColor()
        {
            ColorPage.Instance.SelectColor(GetComponent<Button>().colors.normalColor);
        }
    }
}