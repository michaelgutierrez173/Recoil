using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExplosionDamageNet : NetworkBehaviour {

	public float maxDamage;
	public float distDmgScaling;
	public bool dmgBarrels = false;
	private List<int> playerHit;

	// Use this for initialization
	void Start () {
		playerHit = new List<int> ();
		Debug.Log("Explosion Spawned " + Time.realtimeSinceStartup);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D col){
		if (!isServer) {
			return;
		}

		if (col.tag == "Player") {
			if (playerHit.Contains (col.GetComponent<ShootJetpackNet> ().playerNumber)) {
				return;
			}
			playerHit.Add (col.GetComponent<ShootJetpackNet> ().playerNumber);
			float dist = Vector3.Distance (col.transform.position, transform.position);
			Debug.Log ("Explosion function: " + ((1 / (dist * dist)) + distDmgScaling));
			float dmgScalar = Mathf.Clamp ((1 / (dist * dist) + distDmgScaling), 0, 1);
			col.GetComponent<PlayerHealthNet> ().TakeDamage ((int)(maxDamage * dmgScalar));
			Debug.Log ("Player hit by explosion with dmg: " + (int)(maxDamage * dmgScalar));
		} else if (col.tag == "Bullet") {
			NetworkServer.Destroy (col.gameObject);
		} else if (col.tag == "Barrel") {
			if (!dmgBarrels)
				return;
			BarrelHealth bh = col.GetComponent<BarrelHealth> ();
			bh.TakeDamage (bh.startingHealth); // just blow it up
		}
	}

	void OnDestroy(){
		Debug.Log("Explosion Destroyed " + Time.realtimeSinceStartup);
	}
}
