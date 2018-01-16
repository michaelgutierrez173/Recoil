using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunNet : NetworkBehaviour {

	public ShootJetpackNet currentPlayer;
	public GameObject roundType;
	public GameObject muzzleFlash;
	public int roundsPerMinute;
	public float forceOfBullet;
	public bool fullyAuto;
	public float accuracy; // NOT IMPLEMENTED closer to 0 is more accurate
	public float gunOffset = 1;
	public float offset;
    public AudioClip gunLoadClip;

	protected bool readyToFire = true;
	protected int timer;
	protected AudioSource audioSource;

	[SyncVar]
	public NetworkInstanceId parentNetId;
	[SyncVar(hook = "HasFired")]
	public bool hasFired = false;

	public override void OnStartClient()
	{
		Debug.Log ("Spawned gun on client start: "+this.netId);
		// When we are spawned on the client,
		// find the parent object using its ID,
		// and set it to be our transform's parent.
		GameObject parentObject = ClientScene.FindLocalObject (parentNetId);
		if (parentObject) {
			parentObject.GetComponent<ShootJetpackNet> ().currentGun = this;
			currentPlayer = parentObject.GetComponent<ShootJetpackNet> ();
			//transform.SetParent (parentObject.transform);
		}
	}


	// Use this for initialization
	void Start () {

		//audioSource = this.GetComponent<AudioSource> ();

		timer = roundsPerMinute;

		if (currentPlayer) {
			GetComponent<BoxCollider2D> ().enabled = false;
		}

        audioSource = GetComponent<AudioSource>();
	}


	// Update is called once per frame
	void FixedUpdate () { // also make this tracked by server

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

	void HasFired(bool hasFired){
		if (!currentPlayer.isLocalPlayer) {
			Instantiate (muzzleFlash, 
				this.transform.position + (this.transform.right.normalized * offset), // this offset might change
				this.transform.rotation);
		}
	}

	public virtual bool Fire(bool canShoot){
		if (readyToFire) {
			currentPlayer.ImpulsePlayer (forceOfBullet);
			readyToFire = false;     // Reloading
			timer = roundsPerMinute; // Set reload time
			Instantiate (muzzleFlash, 
				this.transform.position + (this.transform.right.normalized * offset), // this offset might change
				this.transform.rotation);
			if(canShoot) CmdFireBullet ();
			return true;
		}
		return false;
	}

	[Command]
	public void CmdFireBullet(){
		hasFired = !hasFired;
		GameObject round = Instantiate (roundType, 
			this.transform.position + (this.transform.right.normalized * offset), 
			this.transform.rotation);
		NetworkServer.Spawn (round);

		/*GameObject flash = Instantiate (muzzleFlash, 
			this.transform.position + (this.transform.right.normalized * offset), // this offset might change
			this.transform.rotation);
		NetworkServer.Spawn (flash);*/
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			
			parentNetId = col.GetComponent<NetworkIdentity> ().netId;
			currentPlayer = col.GetComponent<ShootJetpackNet>();

			// Not sure if this code works over the network
			//Destroy(currentPlayer.currentGun.gameObject); // delete player's current gun
			//currentPlayer.currentGun = this; // set player gun to this
			if (!ClientScene.prefabs.TryGetValue (GetComponent<NetworkIdentity> ().assetId, out currentPlayer.gunToEquip)) {
				Debug.LogError ("Gun not registered in NetworkManager");
				return;
			}
			GameManagerNet.GetInstance ().GetComponent<GunSpawnerNet> ().RemoveGun (this.netId);
			currentPlayer.CmdEquipWeapon ();
			//this.GetComponent<BoxCollider2D>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            this.GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine("DestroyGun");
			//this.gameObject.SetActive(false);
		}
	}

    IEnumerator DestroyGun(){
        if(!audioSource.isPlaying) audioSource.PlayOneShot(gunLoadClip);
        yield return new WaitForSeconds(2);
        NetworkServer.Destroy(this.gameObject);
    }
}
