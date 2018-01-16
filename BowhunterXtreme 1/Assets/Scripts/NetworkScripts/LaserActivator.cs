using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserActivator : NetworkBehaviour {

    private LaserBeam lb;
    private SpriteRenderer sprite;
    public int switchtime;
    private int timer;
    //private int damage;
    private AudioSource audioS;
	private BoxCollider2D collide;

	[SyncVar]
	private bool laserEnabled = true;

	void Start () {
        lb = GetComponent<LaserBeam>();
        sprite = GetComponentInChildren<SpriteRenderer>();
		collide = GetComponent<BoxCollider2D> ();
        //damage = lb.damageTaken;
        timer = switchtime;
		audioS = GetComponent<AudioSource>();
		audioS.Play();
	}
	
	void Update () {

		collide.enabled = laserEnabled;
		sprite.enabled = laserEnabled;

		if(!laserEnabled && audioS.isPlaying){
			audioS.Stop();
            //Debug.Log("laser Audio stopping");
		}
		else if(laserEnabled && !audioS.isPlaying){
			audioS.Play();
            //Debug.Log("laser Audio playing");
		}
			
	}

	void FixedUpdate(){
		if (!isServer)
			return;

		if(timer == 0)
		{
			laserEnabled = !laserEnabled;
			timer = switchtime + (int)(Random.value * switchtime);
		}
		if(timer > -1) timer--; // should stop at -1
	}
}
