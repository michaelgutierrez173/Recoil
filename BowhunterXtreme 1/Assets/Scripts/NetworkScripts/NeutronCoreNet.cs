using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NeutronCoreNet : NetworkBehaviour {

	private PlayerHealthNet player;
	public Vector3 playerOffset;
	public float addedMass;
	public float normalMass;

	public Vector3 iniSpawn;

	private bool isActivated = true;
	private bool outOfBounds = false;

    private AudioSource audioSource;

	[SyncVar]
	public bool shouldRespawn = false;
	[SyncVar(hook = "ChangeOwner")]
	public int curPlayer = -1;

    void Start()
    {
        InvokeRepeating("IncreaseScore", 0, 1.0f);
        audioSource = GetComponent<AudioSource>();
    }

	void ChangeOwner(int curPlayer){
		if (curPlayer != -1) {
			player = FindPlayerNumber (curPlayer);
			if (!player) {
				Debug.LogError ("Couldn't find player in scene with number: " + curPlayer);
			}
			Debug.Log ("Added Mass to player");
			player.GetComponent<Rigidbody2D> ().mass = addedMass;
            player.GetComponent<ShootJetpackNet>().jumpJetsActive = false;
			//player = Network.
			this.transform.position = player.transform.position + playerOffset;
			this.transform.parent = player.transform;
			GetComponent<BoxCollider2D> ().enabled = false;
			isActivated = false;
			//player = 1;
		} else if(player) {
			Debug.Log ("Removed Mass from player");
			player.GetComponent<Rigidbody2D> ().mass = normalMass;
            player.GetComponent<ShootJetpackNet>().jumpJetsActive = true;
			this.transform.parent = null; // not sure if this works
			player = null;
			if (shouldRespawn) {
				transform.position = iniSpawn;
			}
			GetComponent<BoxCollider2D> ().enabled = true;
			isActivated = true;
		}
	}

	PlayerHealthNet FindPlayerNumber(int num){
		ShootJetpackNet[] go = GameObject.FindObjectsOfType<ShootJetpackNet> ();
		foreach (ShootJetpackNet s in go) {
			if (s.playerNumber == num) {
				return s.gameObject.GetComponent<PlayerHealthNet>();
			}
		}
		return null;
	}

	void FixedUpdate(){
		if (!isServer)
			return;
		
		if (player) {
			if (player.isDead) {
				Debug.Log ("Trigger neutron core change owner");
				curPlayer = -1; // will trigger sync var hook
				if(shouldRespawn) shouldRespawn = false;
			}
		}

		/*if (isActivated && outOfBounds) {
			// reset position
			Debug.Log("Resetting core position");
			transform.position = new Vector3(-2.019231f, -2.75f, 0f); // reset neutron core position
		}*/
	}
	
    private void IncreaseScore()
    {
		if (!isServer)
			return;

		if (curPlayer == 0)
        {
			GameManagerNet.GetInstance().IncreasePlayer1Score(1);
        }
		if (curPlayer == 1)
        {
			GameManagerNet.GetInstance().IncreasePlayer2Score(1);
        }
    }

	void OnTriggerEnter2D(Collider2D col){

        if(audioSource && !audioSource.isPlaying) audioSource.PlayOneShot(audioSource.clip);

		if (!isServer)
			return;
		
		//Debug.Log ("Colliding with player");
		if (col.tag == "Player") {
			curPlayer = col.gameObject.GetComponent<ShootJetpackNet> ().playerNumber;
			//player = col.gameObject.GetComponent<PlayerHealthNet> ();
		}

		if (isActivated && col.tag == "DeathBound") {
			// don't reset position
			outOfBounds = false;
			Debug.Log("Not resetting core position");
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (!isServer)
			return;
		
		//outOfBounds = true;
	}

}
