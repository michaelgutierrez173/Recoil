using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FireBound : NetworkBehaviour {

	public int damagePerSecond;

	private PlayerHealthNet player;
	private Renderer filter;
	public float finalAlpha = 0.5f;
	public float rate = 0.01f;
    //private float curAlpha = 0;

	// Use this for initialization
	void Start () {
		//filter = GameObject.Find ("DamageFilter").GetComponent<Renderer> ();
		InvokeRepeating ("DamagePlayer",0, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (player && player.isDead) {
			player.outsideMapSound.Stop ();
			//player.outOfMapWarning.enabled = false;
			player = null;
		}
	}

	void DamagePlayer(){
		if (player && !player.isDead) {
			//Debug.Log ("Player took damage");
			player.TakeDamage (damagePerSecond);
		} else {
			//StopCoroutine ("fadeRed");
			//StartCoroutine ("fadeBlack");
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			Debug.Log ("Player entered death bound");
			player = col.GetComponent<PlayerHealthNet> ();
            //player.gameObject.GetComponent<ShootJetpackNet>().fireParticles.SetActive(true);
            ParticleSystem.EmissionModule m = player.gameObject.GetComponent<ShootJetpackNet>().fireParticles.GetComponent<ParticleSystem>().emission;
            m.enabled = true;
			player.outsideMapSound.Play ();
			//player.outOfMapWarning.enabled = true;
			//StopCoroutine ("fadeBlack");
			//StartCoroutine ("fadeRed");
		}
		
		if (col.tag == "NeutronCore") {
			Debug.Log ("Resetting core position");
			col.transform.position = new Vector3(-2.019231f, -2.75f, 0f); // reset neutron core position
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.tag == "Player") {
			if (player) {
				player.outsideMapSound.Stop ();
                ParticleSystem.EmissionModule m = player.gameObject.GetComponent<ShootJetpackNet>().fireParticles.GetComponent<ParticleSystem>().emission;
                m.enabled = false;
                      //player.outOfMapWarning.enabled = false;
				//StopCoroutine ("fadeRed");
				//StartCoroutine ("fadeBlack");
			}
			Debug.Log ("Player left death bound");
			player = null;
		}

		if (col.tag == "NeutronCore") {
			Debug.Log ("Core in boundary");
		}
	}

	IEnumerator fadeRed(){
		//Debug.Log ("fading to red");
		while (filter.material.color.a < finalAlpha) {
			Color color = filter.material.color;
			color = new Color(color.r, color.g, color.b, color.a + rate);
			filter.material.color = color;
			Debug.Log ("filter.a: " + filter.material.color.a);
			yield return new WaitForSeconds (rate);
		}
	}

	IEnumerator fadeBlack(){
		//Debug.Log ("fading to black");
		while (filter.material.color.a > 0.0f) {
			Color color = filter.material.color;
			color = new Color(color.r, color.g, color.b, color.a - (rate*2));
			filter.material.color = color;
			Debug.Log ("filter.a: " + filter.material.color.a);
			yield return new WaitForSeconds (rate);
		}
	}
}
