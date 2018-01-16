using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MissileSpeedNet : NetworkBehaviour {

	public float accelForce;
	public float maxVelocity;
	public float lifetime;
	public NetworkInstanceId id; // id of player who fired missile
	public GameObject explosionPrefab;

	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		//float accu = (Random.value - 0.5f) * accuracy;
		rb.AddForce (this.transform.right.normalized * accelForce, ForceMode2D.Impulse);
		if (!isServer)
			return;
		StartCoroutine (bulletLifetime());
	}

	// Update is called once per frame
	void FixedUpdate () {

		rb.AddForce (this.transform.right.normalized * accelForce, ForceMode2D.Impulse);

		if (rb.velocity.magnitude > maxVelocity) {
			rb.velocity = rb.velocity.normalized * maxVelocity;
		}
	}

	IEnumerator bulletLifetime(){
		yield return new WaitForSecondsRealtime(lifetime);
		BlowUp ();
		NetworkServer.Destroy(this.gameObject);
	}

	void OnTriggerEnter2D(Collider2D col){
		if (!isServer)
			return;
		if (col.tag == "Player") {
			// check if is local player and don't blow up
			if (id != col.GetComponent<NetworkIdentity> ().netId) {
				Debug.Log("Blowing up missile");
				BlowUp ();
			}
		} else if(col.tag == "Background" || col.tag == "Barrel" || col.tag == "Bullet"){
			Debug.Log("Blowing up missile");
			BlowUp ();
		}
	}


	void BlowUp(){
		GameObject go = Instantiate (explosionPrefab, this.transform.position, Quaternion.identity);
		NetworkServer.Spawn (go);
		NetworkServer.Destroy (this.gameObject);
	}


}
