using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class BarrelHealth : NetworkBehaviour
{
    public GameObject explosion;
    private int timedead = 0;

    public int startingHealth = 50;                            // The amount of health the player starts the game with.
   
	[SyncVar]
	public int currentHealth;                                   // The current health the player has.
    public Slider healthSlider;                                 // Reference to the UI's health bar.
    public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public AudioClip deathClip;                                 // The audio clip to play when the player dies.
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
                                                                //PlayerMovement playerMovement;                              // Reference to the player's movement.
                                                                //PlayerShooting playerShooting;                              // Reference to the PlayerShooting script.
    public bool isDead;                                                // Whether the player is dead.
    bool damaged;                                               // True when the player gets damaged.


    void Start()
    {
        //playerMovement = GetComponent <PlayerMovement> ();
        //playerShooting = GetComponentInChildren <PlayerShooting> ();

		if (!isServer)
			return;
		
        // Set the initial health of the player.
        currentHealth = startingHealth;
    }


    void Update()
    {
        // If the player has just been damaged...
        if (damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            healthSlider.value = currentHealth;
            // ... transition the colour back to clear.
            //damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        if (isDead && timedead == 0)
        {
            PointEffector2D effector = explosion.GetComponent<PointEffector2D>();
            effector.enabled = false; 
        }
        // Reset the damaged flag.
        damaged = false;
    }


    public void TakeDamage(int amount)
    {

		if (!isServer)
			return;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        healthSlider.value = currentHealth;

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if (currentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death();
        }
    }


    void Death()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;



        // Turn off any remaining shooting effects.
        //playerShooting.DisableEffects ();

        // Tell the animator that the player is dead.
        //anim.SetTrigger ("Die");
		GameObject go = Instantiate(explosion, transform.position, Quaternion.identity);
		NetworkServer.Spawn (go);
        //PointEffector2D effector = explosion.GetComponent<PointEffector2D>();
        //effector.enabled = true;
        //transform.position = new Vector3(0, -50, 0);
        //timedead = 30;

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        //playerAudio.clip = deathClip;
        //playerAudio.Play ();

        // Turn off the movement and shooting scripts.
        //playerMovement.enabled = false;
        //playerShooting.enabled = false;

		GameManagerNet.GetInstance ().GetComponent<BarrelSpawnerNet> ().Remove (netId);

		NetworkServer.Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
		if (!isServer) {
			return;
		}
        //Debug.Log ("Colliding with: " + col.collider.tag);
        if (col.collider.tag == "Bullet")
        {
			BulletSpeedNet bullet = col.gameObject.GetComponent<BulletSpeedNet>();
            TakeDamage(bullet.damage);
            //Debug.Log ("Health at " + currentHealth);
			NetworkServer.Destroy(bullet.gameObject);
        }
    }
}
