using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class DeleteOnGunOwned : MonoBehaviour {

	// Use this for initialization
    void Start () {

        if(GetComponentInParent<GunNet>().currentPlayer){
            Destroy(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
