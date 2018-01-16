using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShotGunNet : GunNet {

	public int numShots;
	public float maxSpread;

	public override bool Fire(bool canShoot){
		if (readyToFire) {
			currentPlayer.ImpulsePlayer (forceOfBullet);
			readyToFire = false;     // Reloading
			timer = roundsPerMinute; // Set reload time
			Instantiate (muzzleFlash, 
				this.transform.position + (this.transform.right.normalized * offset), // this offset might change
				this.transform.rotation);
			if(canShoot) CmdFireShotgun ();
			return true;
		}
		return false;
	}

	[Command]
	public void CmdFireShotgun(){ // shot gun firing
		Debug.Log("Firing shotgun");
		hasFired = !hasFired;
		for (int i = 0; i < numShots; i++) {
			float degChange = maxSpread / (float)numShots;
			Quaternion rotation = Quaternion.Euler(this.transform.rotation.eulerAngles + new Vector3(0,0,((i+1)*degChange)+(-(maxSpread/2))));

			GameObject round = Instantiate (roundType, 
				                  this.transform.position + (this.transform.right.normalized * offset), rotation);
			NetworkServer.Spawn (round);
		}
	}
}
