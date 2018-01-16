using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPoolManager : MonoBehaviour {

	public AmmoPool[] pools;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public AmmoPool getAmmoPool(GameObject ammoType){
		foreach(AmmoPool pool in pools){
			if (pool.prefab.Equals (ammoType)) { // see if this pool ammo matches gun's ammo type
				return pool;
			}
		}
		return null;
	}
}
