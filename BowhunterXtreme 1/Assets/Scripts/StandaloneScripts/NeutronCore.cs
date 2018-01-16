using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutronCore : MonoBehaviour {

    private int player;

    void Start()
    {
        InvokeRepeating("IncreaseScore", 0, 1.0f);
    }
	
    private void IncreaseScore()
    {
        GameManager.GetInstance().DecreaseTime();
        if (player == 0)
        {
            //Nobody has the core
        }
        if (player == 1)
        {
            GameManager.GetInstance().IncreasePlayer1Score();
        }
        if (player == 2)
        {
            GameManager.GetInstance().IncreasePlayer2Score();
        }
    }

	void OnTriggerEnter2D(Collider2D col){
		//Debug.Log ("Colliding with player");
		if (col.tag == "Player") {
			this.transform.parent = col.gameObject.transform;
			player = 1;
		}
	}

}
