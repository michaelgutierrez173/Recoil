using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlackHoleSpeedNet : NetworkBehaviour {
    
	public float accelForce;
	public float maxVelocity;
	public float lifetime;
	public NetworkInstanceId id; // id of player who fired missile
	public GameObject BlackHolePrefab;

	Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		rb.AddForce (this.transform.right.normalized * accelForce, ForceMode2D.Impulse);
		if (!isServer)
			return;
		StartCoroutine (bulletLifetime());
	}

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
				BlowUp ();
			}
		} else if(col.tag == "Background" || col.tag == "Barrel"){
			BlowUp ();
		}
	}


	void BlowUp(){
		GameObject go = Instantiate (BlackHolePrefab, this.transform.position, Quaternion.identity);
		NetworkServer.Spawn (go);
		NetworkServer.Destroy (this.gameObject);
	}


}
