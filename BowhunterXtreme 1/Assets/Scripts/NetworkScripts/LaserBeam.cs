using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserBeam : NetworkBehaviour
{

    public int damageTaken;
    public float forcePush;
    public AudioClip burnSound;

    private PlayerHealthNet player;
    private AudioSource audioS;

    void Start(){
		audioS = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
			Debug.Log("Player hit laser");
			player = col.gameObject.GetComponent<PlayerHealthNet>();
			audioS.PlayOneShot(burnSound);
            Vector2 forceDir = this.transform.position - player.transform.position;
			player.GetComponent<Rigidbody2D>().AddForce((forceDir.normalized * -forcePush), ForceMode2D.Impulse);
        }
        else if(col.gameObject.tag == "Bullet"){
			if (!isServer)
				return;
            NetworkServer.Destroy(col.gameObject);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (!isServer)
            return;
        
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player left laser");
            player = null;
        }
    }

}



