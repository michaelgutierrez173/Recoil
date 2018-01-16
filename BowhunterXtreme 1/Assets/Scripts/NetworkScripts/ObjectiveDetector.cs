using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectiveDetector : NetworkBehaviour {

	public GameObject arrowPrefab;
	public GameObject arrow;
	public float arrowOffset;
	public float distToCore;

	private Transform coreTrans;
    private bool notInit = false;

	// Use this for initialization
	public void Initialize () {
		if (!isLocalPlayer) {
			return;
		}
        if (!arrow)
        {
            arrow = Instantiate(arrowPrefab);
            //arrow.SetActive (true);
        }

        if(GameManagerNet.GetInstance().currentGameMode == GameManagerNet.GameMode.Oddball){
            coreTrans = GameManagerNet.GetInstance().nc.gameObject.transform;
        }
        else if(GameManagerNet.GetInstance().currentGameMode == GameManagerNet.GameMode.Capture){
            coreTrans = GameManagerNet.GetInstance().ncCap.gameObject.transform;
        }
        else if (GameManagerNet.GetInstance().currentGameMode == GameManagerNet.GameMode.Deathmatch)
        {

            if (GameManagerNet.GetInstance().players[0] && GameManagerNet.GetInstance().players[1])
            {
                if (GetComponent<ShootJetpackNet>().playerNumber == 0)
                {
                    coreTrans = GameManagerNet.GetInstance().players[1].transform;
                }
                else if (GetComponent<ShootJetpackNet>().playerNumber == 1)
                {
                    coreTrans = GameManagerNet.GetInstance().players[0].transform;
                }
            }
            else{
                notInit = true;
            }

        }
        else{
            Debug.Log("Gamemode does not require neutron core");
            return;
        }
		
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!isLocalPlayer || !arrow)
			return;

        if(notInit){
            Debug.Log("Objective arrow not init");
            if (GameManagerNet.GetInstance().players[0] && GameManagerNet.GetInstance().players[1])
            {
                if (GetComponent<ShootJetpackNet>().playerNumber == 0)
                {
                    coreTrans = GameManagerNet.GetInstance().players[1].transform;
                }
                else if (GetComponent<ShootJetpackNet>().playerNumber == 1)
                {
                    coreTrans = GameManagerNet.GetInstance().players[0].transform;
                }
                notInit = false;
            }
            else{
                return;
            }
        }

		if (Vector3.Distance (this.transform.position, coreTrans.position) > distToCore) {
			arrow.SetActive (true);
			AimPointer ();
		} else {
			arrow.SetActive (false);
		}
	}

	private void AimPointer(){

		// Calulate direction to core
		Vector2 forceDir = coreTrans.position - (this.transform.position);


		Vector2 arrowDir = forceDir.normalized * arrowOffset;
		Vector2 pos = (Vector2)this.transform.position;

		arrow.transform.position = this.transform.position + (Vector3)(arrowDir);
		arrow.transform.up = (Vector3)(arrowDir);
	}
}
