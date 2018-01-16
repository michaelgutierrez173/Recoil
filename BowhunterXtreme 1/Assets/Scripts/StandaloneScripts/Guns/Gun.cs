using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	public ShootJetpack currentPlayer;
	public GameObject roundType;
	public int roundsPerMinute;
	public float forceOfBullet;
	public bool fullyAuto;
	public float accuracy; // NOT IMPLEMENTED closer to 0 is more accurate
	public float offset;

	private AmmoPool ammoClip;
	private bool readyToFire = true;
	private int timer;

	private AudioSource audioSource;



	// Use this for initialization
	void Start () {

		audioSource = this.GetComponent<AudioSource> ();

		timer = roundsPerMinute;

		if (!ammoClip)
			ammoClip = GameObject.FindWithTag ("AmmoPoolManager")
				.GetComponent<AmmoPoolManager>().getAmmoPool(roundType);

		if (!ammoClip) {
			Debug.LogError ("Ammo clip is null");
			return;
		}
		ammoClip.prefab = roundType;
	}


	// Update is called once per frame
	void FixedUpdate () {

		if (!currentPlayer) // if player is null return
			return;

		if (!readyToFire) { // If reloading, countdown timer until ready to fire
			if (timer > 0) { 
				timer--; 
			} else {
				readyToFire = true;
			}
		}
	}

	public void Fire(){

		if (readyToFire) {
			ammoClip.Instanciate (this.transform.position + (this.transform.right.normalized * offset), this.transform.rotation);
			if(audioSource) audioSource.PlayOneShot (audioSource.clip);
			currentPlayer.ImpulsePlayer (forceOfBullet);
			readyToFire = false;     // Reloading
			timer = roundsPerMinute; // Set reload time
		} 
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			if(col.GetComponent<ShootJetpack> ().currentGun != this){
				currentPlayer = col.GetComponent<ShootJetpack>();
				Destroy(currentPlayer.currentGun.gameObject); // delete player's current gun
				currentPlayer.currentGun = this; // set player gun to this

			}

		}
	}
}
