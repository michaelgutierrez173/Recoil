using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RPGNet : GunNet {

	private bool shoot;

	public override bool Fire(bool canShoot){
		if (readyToFire) {
			currentPlayer.ImpulsePlayer (forceOfBullet);
			readyToFire = false;     // Reloading
			timer = roundsPerMinute; // Set reload time
			Instantiate (muzzleFlash, 
				this.transform.position + (this.transform.right.normalized * offset), // this offset might change
				this.transform.rotation);
				shoot = canShoot;
				CmdFireRocket ();
			return true;
		}
		return false;
	}

	[Command]
	public void CmdFireRocket(){
		hasFired = !hasFired;
		if (shoot) {
			GameObject round = Instantiate (roundType, 
				                   this.transform.position + (this.transform.right.normalized * offset), 
				                   this.transform.rotation);
			round.GetComponent<MissileSpeedNet> ().id = currentPlayer.netId;
			NetworkServer.Spawn (round);
		} else {
			GameObject round = Instantiate (roundType, 
				this.transform.position + (this.transform.right.normalized), 
				this.transform.rotation);
			round.GetComponent<MissileSpeedNet> ().id = currentPlayer.netId;
			NetworkServer.Spawn (round);
		}


		/*GameObject flash = Instantiate (muzzleFlash, 
			this.transform.position + (this.transform.right.normalized * offset), // this offset might change
			this.transform.rotation);
		NetworkServer.Spawn (flash);*/
	}
}
