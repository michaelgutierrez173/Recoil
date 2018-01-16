using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MuzzleFlash : NetworkBehaviour {

	public float totalLifetime;
	public float rendLifetime;

	// Use this for initialization
	void Start () {
		StartCoroutine (renderLifetime());
		StartCoroutine (flashLifetime());
	}

	IEnumerator flashLifetime(){
		yield return new WaitForSecondsRealtime(totalLifetime);
		NetworkServer.Destroy(this.gameObject);
	}

	IEnumerator renderLifetime(){
		yield return new WaitForSecondsRealtime(rendLifetime);
		GetComponent<SpriteRenderer> ().enabled = false;
	}
}
