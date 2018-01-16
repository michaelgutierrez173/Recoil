using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpeed : MonoBehaviour {

	public float speed;
	public float minSpeed;
	public float lifetime;
	public float accuracy;
	public int damage;
	public AmmoPool ammoPool; // this could be handled better
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
		if (this.gameObject && rb.velocity.magnitude < minSpeed) {
			//Debug.Log ("Bullet speed too low");
			ammoPool.Recycle (this.gameObject);
		}
	}

	IEnumerator bulletLifetime(){
		yield return new WaitForSecondsRealtime(lifetime);
		ammoPool.Recycle (this.gameObject);
	}
}
