using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlackHoleDamageNet : NetworkBehaviour {

	public bool dmgBarrels = false;
    private List<PlayerHealthNet> players; //mine
    public int damagePerTick;

    void Start () {
		players = new List<PlayerHealthNet> ();
        InvokeRepeating("DamagePlayer", 0, .5f); // mine
    }

    /*
     * My func
     */
    void DamagePlayer()
    {
        if (!isServer) 
            return;
        
        Debug.Log("black hole damaging" + players.Count);
        foreach (PlayerHealthNet player in players)
        {
            if (player && !player.isDead)
            {
                player.TakeDamage(damagePerTick);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col){
		if (!isServer) {
			return;
		}

		if (col.tag == "Player") {
            PlayerHealthNet curPlayer = col.GetComponent<PlayerHealthNet>();
            if(curPlayer && !players.Contains(curPlayer)){
                players.Add(curPlayer);
            }
		} else if (col.tag == "Barrel") {
			if (!dmgBarrels)
				return;
			BarrelHealth bh = col.GetComponent<BarrelHealth> ();
			bh.TakeDamage (bh.startingHealth); // just blow it up
		}
	}

    void OnTriggerExit2D(Collider2D col)
    {
        if (!isServer)
        {
            return;
        }

        if (col.tag == "Player") {
            PlayerHealthNet curPlayer = col.GetComponent<PlayerHealthNet>();
            if (curPlayer && players.Contains(curPlayer))
            {
                players.Remove(curPlayer);
            }
        }
    }

}
