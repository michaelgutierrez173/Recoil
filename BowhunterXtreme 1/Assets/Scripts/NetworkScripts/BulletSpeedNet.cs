using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletSpeedNet : NetworkBehaviour {

	public float speed;
	public float minSpeed;
	public float lifetime;
	public float accuracy;
	public int damage;
	Rigidbody2D rb;

	private int timer;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		//float accu = (Random.value - 0.5f) * accuracy;
		rb.AddForce (this.transform.right.normalized * speed, ForceMode2D.Impulse);
		StartCoroutine (bulletLifetime());
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (!isServer)
			return;
		
		if (this.gameObject && rb.velocity.magnitude < minSpeed) {
			//Debug.Log ("Bullet speed too low");
			NetworkServer.Destroy(this.gameObject);
		}
	}

	IEnumerator bulletLifetime(){
		yield return new WaitForSecondsRealtime(lifetime);
		NetworkServer.Destroy(this.gameObject);
	}


}
