using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
	public class DisableRendererOnOwn : MonoBehaviour {

		[SerializeField] private bool teamA;
		[SerializeField] private Renderer rend;
		public static Player playerInstance;

		void Update(){
			#if ISDEDICATED
			return;
			#endif
			if(playerInstance != null){
				rend.enabled = (playerInstance.state.TeamLayer == (teamA ? "Team B" : "Team A"));
			}
		}
	}
}
