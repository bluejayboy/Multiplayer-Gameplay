using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scram;
using Photon.Bolt;

public class KillZone1 : MonoBehaviour {

	void OnCollisionEnter(Collision other)
	{
		if (BoltNetwork.IsClient || !other.gameObject.CompareTag(ScramConstant.PlayerTag))
		{
			return;
		}

		var player = other.gameObject.GetComponent<Player>();

		if (player != null)
		{
			player.Die(Vector3.up, "the world");
		}
	}
}
