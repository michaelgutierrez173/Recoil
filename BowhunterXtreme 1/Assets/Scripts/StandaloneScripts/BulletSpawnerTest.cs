using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletSpawnerTest : NetworkBehaviour {

	public GameObject bullet;
	public float rateOfFire;

	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnBullet",0,rateOfFire);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpawnBullet(){
		GameObject go = Instantiate(bullet);
		go.transform.position = this.transform.position;
		go.transform.rotation = this.transform.rotation;
		NetworkServer.Spawn (go);
	}
}
