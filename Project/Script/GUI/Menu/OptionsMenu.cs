using UnityEngine;

namespace Scram
{
	[DisallowMultipleComponent]
	public sealed class OptionsMenu : MonoBehaviour
	{
        [SerializeField] private GameObject main = null, input = null, game = null, data = null;

        private void OnEnable()
        {
            ActivateMain();
        }

        public void ActivateGame()
        {
            main.SetActive(false);
            input.SetActive(false);
            if(data != null)
                data.SetActive(false);
            game.SetActive(true);
        }

        public void ActivateInput()
        {
            main.SetActive(false);
            game.SetActive(false);
            if(data != null)
                data.SetActive(false);
            input.SetActive(true);
        }

        public void ActivateMain()
        {
            game.SetActive(false);
            input.SetActive(false);
            if(data != null)
                data.SetActive(false);
            main.SetActive(true);
        }

        public void ActiveData(){
            game.SetActive(false);
            input.SetActive(false);
            if(data != null)
                data.SetActive(true);
            main.SetActive(false);
        }
    }
}