using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerHealthNet : NetworkBehaviour
{
	public static int startingHealth = 100;                     // The amount of health the player starts the game with.

	[SyncVar(hook = "OnChangeHealth")]
	public int currentHealth = startingHealth;                  // The current health the player has.
	[SyncVar]
	public bool isDead = false;

	public Slider healthSlider;                                 // Reference to the UI's health bar.
	public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
	public AudioClip deathClip;                                 // The audio clip to play when the player dies.
	public AudioClip hurtClip;                                  // The audio clip to play when the player gets shot.
	public GameObject bloodSplatter;
	public float bloodOffset;
	public float spawnTime;
	public GameObject spawnGun;
	public AudioSource outsideMapSound;
	public Text outOfMapWarning;
	public float minAcceptableSpawnDist;

    public int killScoreAmt;


	Animator anim;                                              // Reference to the Animator component.
	AudioSource playerAudio;                                    // Reference to the AudioSource component.

	private NetworkStartPosition[] spawnPoints;

	void Awake ()
	{
		// Setting up the references.
		anim = GetComponent <Animator> ();
		playerAudio = GetComponent <AudioSource> ();

		// Set the initial health of the player.
		currentHealth = startingHealth;
	}

	void Start(){
		if (isLocalPlayer)
		{
			NetworkStartPosition[] points = FindObjectsOfType<NetworkStartPosition> ();
			List<NetworkStartPosition> spnPoints = new List<NetworkStartPosition>();
			foreach (NetworkStartPosition st in points) {
				if (st.tag == "PlayerSpawn") {
					spnPoints.Add (st);
				}
			}
			spawnPoints = spnPoints.ToArray ();
		}
	}

	public void Heal(int amount){

		if (!isServer) {
			return;
		}

		currentHealth += amount;

		if (currentHealth > startingHealth) {
			currentHealth = startingHealth;
		}
	}

	// called when currentHealth value changes
	void OnChangeHealth (int currentHealth)
	{
		//healthBar.sizeDelta = new Vector2(health, currentHealth.sizeDelta.y);

		// Set the health bar's value to the current health.
		healthSlider.value = currentHealth;
	}


	public void TakeDamage (int amount)
	{

		if (!isServer) {
			return;
		}

		// Reduce the current health by the damage amount.
		currentHealth -= amount;

		// If the player has lost all it's health and the death flag hasn't been set yet...
		if(currentHealth <= 0)
		{
            // ... it should die.
            if(GameManagerNet.GetInstance().currentGameMode == GameManagerNet.GameMode.Deathmatch){
                if(GetComponent<ShootJetpackNet>().playerNumber == 0){
                    GameManagerNet.GetInstance().IncreasePlayer2Score(killScoreAmt);
                }
                else if (GetComponent<ShootJetpackNet>().playerNumber == 1)
                {
                    GameManagerNet.GetInstance().IncreasePlayer1Score(killScoreAmt);
                }
            }

			isDead = true;
			StartCoroutine("Death");

		}
	}


	IEnumerator Death ()
	{
		RpcDie ();

		yield return new WaitForSeconds(spawnTime);

		isDead = false;
		currentHealth = startingHealth;
		RpcRespawn ();

		// Turn off the movement and shooting scripts.
		//playerMovement.enabled = false;
		//playerShooting.enabled = false;
	}  

	[ClientRpc]
	void RpcDie(){

		// Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
		playerAudio.volume = 1.0f;
		playerAudio.pitch = 1.0f;
		playerAudio.clip = deathClip;
		playerAudio.Play ();

		// Turn off any remaining shooting effects and colliders
		GetComponent<ShootJetpackNet>().enabled = false;
		GetComponents<CircleCollider2D>()[0].enabled = false;
		GetComponents<CircleCollider2D>()[1].enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;

		// Tell the animator that the player is dead.
		//anim.SetTrigger ("Die");

		transform.rotation = Quaternion.Euler(new Vector3 (transform.rotation.x , transform.rotation.y, 90));
	}

	[ClientRpc]
	public void RpcRespawn(){
		Vector3 spawnPoint = Vector3.zero;

		// If there is a spawn point array and the array is not empty, pick one at random that is not close to players
		if (spawnPoints != null && spawnPoints.Length > 0)
		{
			float minDist = 1000; // some large number here
			float dist = 1000;
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			int iterationCount = 0;
			do{
				spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;

				foreach (GameObject pl in players) {
					dist = Vector3.Distance (spawnPoint, pl.transform.position);
					if (dist < minDist) {
						minDist = dist; 
					}
				}
				iterationCount++;
			}
			while(minDist < minAcceptableSpawnDist && iterationCount < 20); // 20 should hit most spawn points
		}

		// Set the player’s position to the chosen spawn point
		transform.position = spawnPoint;

		playerAudio.clip = hurtClip;

		GetComponent<ShootJetpackNet> ().gunToEquip = spawnGun;
		GetComponent<ShootJetpackNet> ().CmdEquipWeapon ();
		GetComponent<ShootJetpackNet> ().enabled = true;
		GetComponents<CircleCollider2D>()[0].enabled = true;
		GetComponents<CircleCollider2D>()[1].enabled = true;
		GetComponent<BoxCollider2D>().enabled = true;

		transform.rotation = Quaternion.Euler(new Vector3 (transform.rotation.x , transform.rotation.y, 0));
	}

	

	void OnCollisionEnter2D(Collision2D col){
		//Debug.Log ("Something hit: " + col.collider.tag);
		//Debug.Log ("Colliding with: " + col.collider.tag);
		if (col.collider.tag == "Bullet") {
			BulletSpeedNet bullet = col.gameObject.GetComponent<BulletSpeedNet> ();

			// Play the hurt sound effect.
			playerAudio.volume = 0.6f;
			playerAudio.pitch = 0.9f + Random.value * 0.25f;
			playerAudio.Play ();

		    //Debug.Log ("Bullet hit!");

			//Debug.Log ("Collider velo: " + col.relativeVelocity);
			GameObject go = Instantiate (bloodSplatter, this.transform);
			go.transform.right = -(Vector3)col.relativeVelocity;
			go.transform.position = col.contacts [0].rigidbody.gameObject.transform.position;


			TakeDamage (bullet.damage);
			//Debug.Log ("Health at " + currentHealth);
			NetworkServer.Destroy(bullet.gameObject);
		}
	}
} 